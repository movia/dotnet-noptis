using System;
using System.Xml;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class UpdatedArrival : MessageBase
    {
        public long Id { get; set; }
        
        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset TargetDateTime { get; set; }

        public DateTimeOffset? EstimatedDateTime { get; set; }

        public DateTimeOffset? ObservedDateTime { get; set; }

        public string State { get; set; }

        public long? MonitoredVehicleJourneyId { get; set; }

        public long? TargetJourneyPatternPointGid { get; set; }

        public long? TimetabledJourneyPatternPointGid { get; set; }

        public override void ReadXmlAttribute(XAttribute attr)
        {
            switch (attr.Name.LocalName)
            {
                case "Id": 
                    if (long.TryParse(attr.Value, out var id))
                        Id = id;
                    break;
                case "Timestamp":
                    if (DateTimeOffset.TryParse(attr.Value, out var timestamp))
                        Timestamp = timestamp;
                    break;
                case "TargetDateTime":
                    if (DateTimeOffset.TryParse(attr.Value, out var targetDateTime))
                        TargetDateTime = targetDateTime;
                    break;
                case "EstimatedDateTime":
                    if (DateTimeOffset.TryParse(attr.Value, out var estimatedDateTime))
                        EstimatedDateTime = estimatedDateTime;
                    break;
                case "ObservedDateTime":
                    if (DateTimeOffset.TryParse(attr.Value, out var observedDateTime))
                        ObservedDateTime = observedDateTime;
                    break;
                case "State": State = attr.Value;
                    break;
            }
        }

        public override void ReadXmlElement(XElement el)
        {
            switch (el.Name.LocalName)
            {
                case "MonitoredVehicleJourneyRef":
                    if (long.TryParse(el.Attribute("Id")?.Value, out var monitoredVehicleJourneyId))
                        MonitoredVehicleJourneyId = monitoredVehicleJourneyId;
                    break;
                case "TargetJourneyPatternPointRef":
                    if (long.TryParse(el.Attribute("Gid")?.Value, out var targetJourneyPatternPointGid))
                        TargetJourneyPatternPointGid = targetJourneyPatternPointGid;
                    break;
                case "TimetabledJourneyPatternPointRef":
                    if (long.TryParse(el.Attribute("Gid")?.Value, out var timetabledJourneyPatternPointGid))
                        TimetabledJourneyPatternPointGid = timetabledJourneyPatternPointGid;
                    break;
            }
        }
    }
}