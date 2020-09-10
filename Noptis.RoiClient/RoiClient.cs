using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using App.Metrics;
using App.Metrics.Meter;
using App.Metrics.Timer;

using Microsoft.Extensions.Logging;

namespace Noptis.RoiClient
{
    public class RoiClient
    {
        private readonly ILogger<RoiClient> logger;
        private readonly IMetrics metrics;
        private readonly TimerOptions readerTimer = new TimerOptions
        {
            Name = "Inbound Reader Timer",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private readonly TimerOptions processTimer = new TimerOptions
        {
            Name = "Inbound Process Timer",
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private readonly MeterOptions eventTypeMeter = new MeterOptions
        {
            Name = "Inbound Events",
            MeasurementUnit = Unit.Events
        };
        
        private string server;
        private int port;
        private string peerId;
        private CancellationToken cancellationToken;
        private SemaphoreSlim sendSemaphore = new SemaphoreSlim(0, 1);
        
        private Socket socket;
        private NetworkStream stream;
        private Encoding encoding = Encoding.GetEncoding("iso-8859-1");
        private long messageId = 0;
        private ConcurrentDictionary<string, Func<MessageBase>> typeMap = new ConcurrentDictionary<string, Func<MessageBase>>();
        private BlockingCollection<MessageBase> incomming = new BlockingCollection<MessageBase>();
        private long lastProcessedMessageId;

        private readonly TimeSpan maxRetryDelay = TimeSpan.FromMinutes(15);
        private readonly XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
        {
            ConformanceLevel = ConformanceLevel.Fragment
        };

        public long? SubscriptionId { get; private set; } = null;

        public DateTime SynchronisedUptoUtcDateTime { get; private set; } = DateTime.UtcNow.AddHours(-3);

        public RoiClient(ILogger<RoiClient> logger, IMetrics metrics)
        {
            this.logger = logger;
            this.metrics = metrics;

            // Register basic types for handling ROI stream ack.
            RegisterType<FromPubTrans.SubscriptionResumeResponse>();
            RegisterType<FromPubTrans.SubscriptionErrorReport>();
            RegisterType<FromPubTrans.SynchronisationReport>();
            RegisterType<FromPubTrans.LastProcessedMessageRequest>();            
        }

        private Socket ConnectSocket(string server, int port)
        {
            var addresses = Dns.GetHostAddresses(server);
            IPAddress address = addresses[0];
            IPEndPoint ipe = new IPEndPoint(address, port);
            Socket socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            logger.LogInformation($"Connecting to {address} port {port}");
            socket.Connect(ipe);

            if (socket.Connected)
                return socket;

            return null;
        }

        public async Task Send(MessageBase msg)
        {
            var n = Interlocked.Increment(ref messageId);
            msg.MessageId = n;
            await Send(msg.ToXmlString());
        }

        private async Task Send(string msg)
        {
            var coconut = await sendSemaphore.WaitAsync(5000, cancellationToken);
            if (coconut)
            {
                try
                {
                    await SendInternal(msg);
                }
                finally
                {
                    sendSemaphore.Release();
                }
            }
        }

        private async Task SendInternal(string msg)
        {
            logger.LogTrace($"OUT: {msg}");
            var data = encoding.GetBytes(msg);
            await stream.WriteAsync(data);
            await stream.FlushAsync();
        }

        private async void Idle()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(30000, cancellationToken);
                    if (!cancellationToken.IsCancellationRequested)
                        await Send(new ToPubTrans.Idle());
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Exception en idle loop.");
                }
            }
        }

        private async Task TryCloseConnection()
        {
            /* Try to gently send termination emssage */            
            try
            {
                var coconut = await sendSemaphore.WaitAsync(5000, cancellationToken);
                if (coconut)
                    await SendInternal(@"</ROI:ToPubTrlansMessages>");
                stream?.Close();
                socket?.Close();
            }
            catch (Exception) { } // Swallow exception as we really don't care if we suceeed to send the termination signal.
        }

        private async void MessageExchangeLoop()
        {
            int retryAttempt = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    socket = ConnectSocket(server, port);
                    stream = new NetworkStream(socket, true);

                    await SendInternal(@"<?xml version=""1.0"" encoding=""iso-8859-1"" ?>");
                    await SendInternal($"<ROI:ToPubTransMessages xmlns:ROI=\"http://www.pubtrans.com/ROI/3.0\" DocumentLayoutVersion=\"3.0.7\" PeerId=\"{peerId}\" MaxMessageInterval=\"PT90S\">");
                    sendSemaphore.Release(); /* Allows transmission to begin */

                    await Send(new ToPubTrans.SubscriptionResumeRequest { StartUtcDateTime = DateTime.UtcNow, SynchronisedUptoUtcDateTime = SynchronisedUptoUtcDateTime });

                    logger.LogInformation("Beginning processing of incomming messages.");
                    var reader = new StreamReader(stream, encoding);
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        string msg = null;

                        using (metrics.Measure.Timer.Time(readerTimer))
                        {
                            try
                            {
                                msg = await reader.ReadLineAsync();
                            }
                            catch (Exception ex)
                            {
                                /* This is likely to happen when we shut down, in that case do not log */
                                if (!cancellationToken.IsCancellationRequested)
                                    logger.LogWarning(ex, "Error reading lines from network stream.");

                                break;
                            }
                        }

                        if (msg != null && msg.Length > 0)
                        {
                            logger.LogTrace($"IN: {msg}");

                            if (msg.Equals("</ROI:FromPubTransMessages>"))
                            {
                                logger.LogInformation("Received termination signal. Rebooting.");
                                break;
                            }

                            using (metrics.Measure.Timer.Time(processTimer))
                            using (XmlReader xmlReader = XmlReader.Create(new StringReader(msg), xmlReaderSettings))
                            {
                                while (xmlReader.NodeType != XmlNodeType.Element)
                                    xmlReader.Read();

                                var eventType = xmlReader.LocalName;
                                metrics.Measure.Meter.Mark(eventTypeMeter, eventType);

                                if (typeMap.TryGetValue(eventType, out var factory))
                                {
                                    var document = (XElement)XNode.ReadFrom(xmlReader);
                                    var entity = factory();
                                    entity.ReadXml(document);

                                    await HandleIncommingMessage(entity);
                                    lastProcessedMessageId = entity.MessageId.Value;
                                }

                                /* We have sucessfullt processed a message - reset retry attempt */
                                retryAttempt = 0;
                            }
                        }
                    }

                    await TryCloseConnection();
                }
                catch (Exception ex)
                {                    
                    retryAttempt++;
                    var retryDelay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    if (retryDelay > maxRetryDelay)
                        retryDelay = maxRetryDelay;
                    logger.LogError(ex, $"Exception en message exchange loop. Will attempt retry {retryAttempt} in {retryDelay}");
                    await TryCloseConnection();
                    await Task.Delay(retryDelay);
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
                FromPubTrans.SubscriptionErrorReport r => Task.Run(() => logger.LogWarning($"Subscription Error: {r.Text}")),
                FromPubTrans.SubscriptionResumeResponse r => Task.Run(() =>
                {
                    this.SubscriptionId = r.SubscriptionId;
                    logger.LogInformation($"Resuming subscription {r.SubscriptionId}");
                }),
                FromPubTrans.SynchronisationReport r => Task.Run(() =>
                {
                    this.SynchronisedUptoUtcDateTime = r.SynchronisedUptoUtcDateTime;
                    logger.LogInformation($"Synchronised upto {r.SynchronisedUptoUtcDateTime} (HasCompletedRecoveryPhase: {r.HasCompletedRecoveryPhase})");
                }),        
                _ => Task.Run(() => incomming.Add(m))
            };
        }
        
        public void RegisterType<T>() where T : MessageBase, new() => typeMap.TryAdd(typeof(T).Name, () => new T());

        public bool TryTake(out MessageBase msg) => incomming.TryTake(out msg, 1000);

        public int QueueCount { get => incomming.Count; }

        public void Start(string server, int port, string peerId, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            this.server = server;
            this.port = port;
            this.peerId = peerId;

            Task.Factory.StartNew(MessageExchangeLoop, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(Idle, TaskCreationOptions.LongRunning);           
        }
    }
}
