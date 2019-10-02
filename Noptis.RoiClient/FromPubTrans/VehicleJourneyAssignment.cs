using System;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class VehicleJourneyAssignment : MessageBase
    {
        public long Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string State { get; set; }

        public DateTimeOffset ValidFromDateTime { get; set; }

        public DateTimeOffset? InvalidFromDateTime { get; set; }

        public override void ReadXml(XElement xml)
        {
            Id = long.Parse(xml.Attribute("Id").Value);
            Timestamp = DateTimeOffset.Parse(xml.Attribute("Timestamp").Value);
            State = xml.Attribute("State").Value;
            ValidFromDateTime = DateTimeOffset.Parse(xml.Attribute("ValidFromDateTime").Value);
            if (xml.Attribute("InvalidFromDateTime") != null)
                InvalidFromDateTime = DateTimeOffset.Parse(xml.Attribute("InvalidFromDateTime").Value);
        }
    }
}
