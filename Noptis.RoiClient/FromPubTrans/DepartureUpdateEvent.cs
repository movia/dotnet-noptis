using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class DepartureUpdateEvent : MessageBase
    {
        public List<UpdatedDeparture> Departures { get; set; } = new List<UpdatedDeparture>();

        public override void ReadXmlElement(XElement el)
        {
            switch (el.Name.LocalName)
            {
                case "Departure":
                    var departure = new UpdatedDeparture();
                    departure.ReadXml(el);
                    Departures.Add(departure);
                    break;
            }
        }
    }
}
