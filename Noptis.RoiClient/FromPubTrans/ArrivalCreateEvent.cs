using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class ArrivalCreateEvent : MessageBase
    {
        public List<Arrival> Arrivals { get; set; } = new List<Arrival>();

        public override void ReadXmlElement(XElement el)
        {
            switch (el.Name.LocalName)
            {
                case "Arrival":
                    var arrival = new Arrival();
                    arrival.ReadXml(el);
                    Arrivals.Add(arrival);
                    break;
            }
        }
    }
}
