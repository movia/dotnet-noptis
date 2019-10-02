using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class ArrivalUpdateEvent : MessageBase
    {
        public List<UpdatedArrival> Arrivals { get; set; } = new List<UpdatedArrival>();

        public override void ReadXmlElement(XElement el)
        {
            switch (el.Name.LocalName)
            {
                case "Arrival":
                    var arrival = new UpdatedArrival();
                    arrival.ReadXml(el);
                    Arrivals.Add(arrival);
                    break;
            }
        }
    }
}
