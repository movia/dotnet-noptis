using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class DepartureCreateEvent : MessageBase
    {
        public List<Departure> Departures { get; set; } = new List<Departure>();

        public override void ReadXmlElement(XElement el)
        {
            switch (el.Name.LocalName)
            {
                case "Departure":
                    var departure = new Departure();
                    departure.ReadXml(el);
                    Departures.Add(departure);
                    break;
            }
        }
    }
}
