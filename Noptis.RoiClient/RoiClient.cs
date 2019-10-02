using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Noptis.RoiClient
{
    public class RoiClient
    {
        private CancellationToken cancellationToken;
        private Socket socket;
        private NetworkStream stream;
        private Encoding encoding = Encoding.GetEncoding("iso-8859-1");
        private long subscriptionId = 0;
        private long messageId = 0;
        private ConcurrentDictionary<string, Func<MessageBase>> typeMap = new ConcurrentDictionary<string, Func<MessageBase>>();
        private ConcurrentDictionary<string, int> incomming_stats = new ConcurrentDictionary<string, int>();
        private BlockingCollection<MessageBase> incomming = new BlockingCollection<MessageBase>();
        private long lastProcessedMessageId;
        private DateTime synchronisedUptoUtcDateTime;
        private readonly XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
        {
            ConformanceLevel = ConformanceLevel.Fragment
        };

    public RoiClient(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            RegisterType<FromPubTrans.SubscriptionResumeResponse>();
            RegisterType<FromPubTrans.SubscriptionErrorReport>();
            RegisterType<FromPubTrans.SynchronisationReport>();
            RegisterType<FromPubTrans.LastProcessedMessageRequest>();
        }

        private static Socket ConnectSocket(string server, int port)
        {
            var addresses = Dns.GetHostAddresses(server);
            IPAddress address = addresses[0];
            IPEndPoint ipe = new IPEndPoint(address, port);
            Socket socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine($"Connecting to {address} port {port}");
            socket.Connect(ipe);

            if (socket.Connected)
                return socket;

            return null;
        }

        private async Task Send(MessageBase msg)
        {
            var n = Interlocked.Increment(ref messageId);
            msg.MessageId = n;
            await Send(msg.ToXmlString());
        }

        private async Task Send(Func<long, string> msg)
        {
            var n = Interlocked.Increment(ref messageId);
            await Send(msg(n));
        }

        private async Task Send(string msg)
        {
            //Console.WriteLine(msg);
            var data = encoding.GetBytes(msg);
            await stream.WriteAsync(data);
            await stream.FlushAsync();
        }

        private async void Idle()
        {
            while (!cancellationToken.IsCancellationRequested)
            {

                await Send(msg_id => $"<Idle MessageId=\"{msg_id}\"/>");

                Console.WriteLine($"Queue stats: (in/out): {incomming.Count}");
                Console.WriteLine($"Message stats:");
                foreach ((string key, int value) in incomming_stats)
                {
                    Console.WriteLine($"- {key}: {value}");
                }
                await Task.Delay(30000);
            }
        }

        private async void ProcessIncomming()
        {
            var reader = new StreamReader(stream, encoding);
            Console.WriteLine("Beginning processing of incomming messages.");
            while (!cancellationToken.IsCancellationRequested)
            {
                var msg = await reader.ReadLineAsync();
                if (msg != null && msg.Length > 0)
                {
                    //Console.WriteLine(msg);
                    //try { 
                    using (XmlReader xmlReader = XmlReader.Create(new StringReader(msg), xmlReaderSettings))
                    {
                        // Read until we get to an element
                        while (xmlReader.NodeType != XmlNodeType.Element)
                            xmlReader.Read();

                        var eventType = xmlReader.LocalName;

                        MessageBase entity = null;

                        if (xmlReader.LocalName == "FromPubTransMessages")
                            continue;
                        if (typeMap.TryGetValue(eventType, out var factory))
                        {
                            entity = factory();
                            entity.ReadXml(xmlReader);
                            await HandleIncommingMessage(entity);
                            lastProcessedMessageId = entity.MessageId.Value;
                        }
                        else
                        {
                            if (!incomming_stats.ContainsKey(eventType))
                                Console.WriteLine(msg);
                        }

                        // Update stats
                        incomming_stats.AddOrUpdate(eventType, 1, (_, x) => x + 1);
                    }
                }
            }
        }

        private Task HandleIncommingMessage(MessageBase m) {
            return m switch
            {
                FromPubTrans.LastProcessedMessageRequest r =>
                    Send(new ToPubTrans.LastProcessedMessageResponse
                    {
                        OnMessageId = r.MessageId,
                        LastProcessedMessageId = lastProcessedMessageId
                    }),
                FromPubTrans.SubscriptionErrorReport r => LogInfo(r.Text),
                FromPubTrans.SubscriptionResumeResponse r => LogInfo($"Resuming subscription {r.SubscriptionId}"),
                FromPubTrans.SynchronisationReport r => SetSynchronisedUptoUtcDateTime(r),
                _ => Task.Run(() => incomming.Add(m))
            };
        }

        private Task SetSynchronisedUptoUtcDateTime(FromPubTrans.SynchronisationReport synchronisationReport)
        {
            this.synchronisedUptoUtcDateTime = synchronisationReport.SynchronisedUptoUtcDateTime;
            return LogInfo($"Synchronised upto {synchronisedUptoUtcDateTime} (HasCompletedRecoveryPhase: {synchronisationReport.HasCompletedRecoveryPhase})");
        }

        private Task LogInfo(string msg) => Task.Run(() => Console.WriteLine(msg));

        public void RegisterType<T>() where T : MessageBase, new() => typeMap.TryAdd(typeof(T).Name, () => new T());

        public bool TryTake(out MessageBase msg) => incomming.TryTake(out msg, 1000);

        public void Start()
        {
            string server = "ptvtdartapp03.ptvt.local";
            int port = 2345;

            socket = ConnectSocket(server, port);
            stream = new NetworkStream(socket, true);

            //writer = new StreamWriter(stream, encoding);            
            Task.Factory.StartNew(ProcessIncomming, TaskCreationOptions.LongRunning);
            Task.Run(async () =>
            {
                await Send(@"<?xml version=""1.0"" encoding=""iso-8859-1"" ?>");
                await Send(@"<ROI:ToPubTransMessages xmlns:ROI=""http://www.pubtrans.com/ROI/3.0"" DocumentLayoutVersion=""3.0.7"" PeerId=""BI-TEST"" MaxMessageInterval=""PT90S"">");
                //await Send(msg_id => $"<SubscriptionRequest MessageId=\"{msg_id}\"> <VehicleJourneyEventSelection LookAheadDuration=\"PT3H\" ExpandLineData=\"Y\" ExpandVehicleOperatorData=\"Y\"> <ScopeElements> <ScopeElement/> </ScopeElements> </VehicleJourneyEventSelection> <AssignmentEventSelection/> </SubscriptionRequest>");
                /*
                await Send(
                    new ToPubTrans.SubscriptionUpdateRequest
                    {
                        VehicleJourneyEventSelection = new ToPubTrans.VehicleJourneyEventSelection
                        {
                            LookAheadDuration = "PT3H",
                            ExpandLineData = true,
                            ExpandVehicleOperatorData = true,
                            ScopeElements = new ToPubTrans.ScopeElements
                            {
                                new ToPubTrans.ScopeElement()
                            }
                        },
                        AssignmentEventSelection = new ToPubTrans.AssignmentEventSelection(),
                    });
                */
                //ait Send(new ToPubTrans.SubscriptionResumeRequest { StartUtcDateTime = DateTime.UtcNow, SynchronisedUptoUtcDateTime = DateTime.UtcNow.Date.AddDays(-1) });
                await Send(new ToPubTrans.SubscriptionResumeRequest { StartUtcDateTime = DateTime.UtcNow, SynchronisedUptoUtcDateTime = DateTime.UtcNow.AddHours(-1) });
                //await Send(new ToPubTrans.SubscriptionResumeRequest { });
                await Task.Factory.StartNew(Idle, TaskCreationOptions.LongRunning);
            });

            cancellationToken.Register(async () =>
            {
                await Send(@"</ROI:ToPubTrlansMessages>");
                stream.Close();
                socket.Close();
            });
        }
    }
}
