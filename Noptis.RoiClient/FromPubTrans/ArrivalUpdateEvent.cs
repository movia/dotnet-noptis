using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Noptis.RoiClient.FromPubTrans
{
    public class ArrivalUpdateEvent : MessageBase
    {
        public List<UpdatedArrival> Arrivals { get; set; } = new List<UpdatedArrival>();

        public override void ReadXmlElements(XmlReader xmlReader)
        {
            while (xmlReader.LocalName == "Arrival") {
                var arrival = new UpdatedArrival();
                arrival.ReadXml(xmlReader);
                Arrivals.Add(arrival);
             }
        }
    }
}
