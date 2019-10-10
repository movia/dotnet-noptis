using System;
using System.Xml;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class Arrival : MessageBase
    {
        public long Id { get; set; }
        
        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset TimetabledLatestDateTime { get; set; }

        public DateTimeOffset TargetDateTime { get; set; }

        public DateTimeOffset? EstimatedDateTime { get; set; }

        public DateTimeOffset? ObservedDateTime { get; set; }

        public string State { get; set; }

        public string Type { get; set; } = "STOP_IF_ALIGHTING";

        public int JourneyPatternSequenceNumber { get; set; }

        public int VisitCountNumber { get; set; } = 1;

        public DatedVehicleJourneyRef DatedVehicleJourneyRef { get; set; }

        public long? MonitoredVehicleJourneyId { get; set; }

        public long TimetabledJourneyPatternPointGid { get; set; }

        public long? TargetJourneyPatternPointGid { get; set; }

        public override void ReadXmlAttribute(XAttribute attr)
        {
            switch (attr.Name.LocalName)
            {
                case "Id":
                    Id = long.Parse(attr.Value);
                    break;
                case "Timestamp":
                    Timestamp = XmlConvert.ToDateTimeOffset(attr.Value);
                    break;
                case "TimetabledLatestDateTime":
                    TimetabledLatestDateTime = XmlConvert.ToDateTimeOffset(attr.Value);
                    break;
                case "TargetDateTime":
                    TargetDateTime = XmlConvert.ToDateTimeOffset(attr.Value);
                    break;
                case "EstimatedDateTime":
                    EstimatedDateTime = XmlConvert.ToDateTimeOffset(attr.Value);
                    break;
                case "ObservedDateTime":
                    ObservedDateTime = XmlConvert.ToDateTimeOffset(attr.Value);
                    break;
                case "State": 
                    State = attr.Value;
                    break;
                case "Type":
                    Type = attr.Value;
                    break;
                case "JourneyPatternSequenceNumber":
                    JourneyPatternSequenceNumber = int.Parse(attr.Value);
                    break;
                case "VisitCountNumber":
                    VisitCountNumber = int.Parse(attr.Value);
                    break;
            }
        }

        public override void ReadXmlElement(XElement el)
        {
            switch (el.Name.LocalName)
            {
                case "DatedVehicleJourneyRef":
                    DatedVehicleJourneyRef = DatedVehicleJourneyRef.ReadFromXml(el);
                    break;
                case "MonitoredVehicleJourneyRef":
                    MonitoredVehicleJourneyId = long.Parse(el.Attribute("Id").Value);
                    break;
                case "TargetJourneyPatternPointRef":
                    TargetJourneyPatternPointGid = long.Parse(el.Attribute("Gid").Value);
                    break;
                case "TimetabledJourneyPatternPointRef":
                    TimetabledJourneyPatternPointGid = long.Parse(el.Attribute("Gid").Value);
                    break;
            }
        }
    }
}
