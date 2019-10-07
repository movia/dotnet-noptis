using System;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Noptis.RoiClient.FromPubTrans
{
    public class VehicleJourneyAssignment : MessageBase
    {
        public long Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string State { get; set; }

        public DateTimeOffset ValidFromDateTime { get; set; }

        public DateTimeOffset? InvalidFromDateTime { get; set; }

        public DatedVehicleJourneyRef DatedVehicleJourneyRef { get; set; }

        public VehicleRef AssignedVehicleRef { get; set; }

        public override void ReadXml(XElement xml)
        {
            Id = long.Parse(xml.Attribute("Id").Value);
            Timestamp = DateTimeOffset.Parse(xml.Attribute("Timestamp").Value);
            State = xml.Attribute("State").Value;
            ValidFromDateTime = DateTimeOffset.Parse(xml.Attribute("ValidFromDateTime").Value);
            if (xml.Attribute("InvalidFromDateTime") != null)
                InvalidFromDateTime = DateTimeOffset.Parse(xml.Attribute("InvalidFromDateTime").Value);

            DatedVehicleJourneyRef = DatedVehicleJourneyRef.ReadFromXml(xml.Element("DatedVehicleJourneyRef"));
            AssignedVehicleRef = VehicleRef.ReadFromXml(xml.Element("AssignedVehicleRef"));
        }

        public override void WriteXmlAttributes(XmlWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("Id", XmlConvert.ToString(Id));
            xmlWriter.WriteAttributeString("Timestamp", XmlConvert.ToString(Timestamp));
            xmlWriter.WriteAttributeString("State", State);
            xmlWriter.WriteAttributeString("ValidFromDateTime", XmlConvert.ToString(ValidFromDateTime));
            if (InvalidFromDateTime.HasValue)
                xmlWriter.WriteAttributeString("InvalidFromDateTime", XmlConvert.ToString(InvalidFromDateTime.Value));

            if (DatedVehicleJourneyRef != null)
            {
                xmlWriter.WriteStartElement("DatedVehicleJourneyRef");
                DatedVehicleJourneyRef.WriteXmlAttributes(xmlWriter);
                xmlWriter.WriteEndElement();
            }

            if (AssignedVehicleRef != null)
            {
                xmlWriter.WriteStartElement("AssignedVehicleRef");
                AssignedVehicleRef.WriteXmlAttributes(xmlWriter);
                xmlWriter.WriteEndElement();
            }
        }
    }
}
