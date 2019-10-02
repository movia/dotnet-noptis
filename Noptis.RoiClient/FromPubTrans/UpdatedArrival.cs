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

        public override void ReadXmlAttributes(XmlReader xmlReader)
        {
            if (long.TryParse(xmlReader.GetAttribute("Id"), out var id))
                Id = id;
            if (DateTimeOffset.TryParse(xmlReader.GetAttribute("Timestamp"), out var timestamp))
                Timestamp = timestamp;
            if (DateTimeOffset.TryParse(xmlReader.GetAttribute("TargetDateTime"), out var targetDateTime))
                TargetDateTime = targetDateTime;
            if (DateTimeOffset.TryParse(xmlReader.GetAttribute("EstimatedDateTime"), out var estimatedDateTime))
                EstimatedDateTime = estimatedDateTime;
            if (DateTimeOffset.TryParse(xmlReader.GetAttribute("ObservedDateTime"), out var observedDateTime))
                ObservedDateTime = observedDateTime;

            State = xmlReader.GetAttribute("State");
            /*
            using (var xmlSubTreeReader = xmlReader.ReadSubtree())
            {
                var doc = XDocument.Load(xmlSubTreeReader);

                if (long.TryParse(doc.Root.Element("MonitoredVehicleJourneyRef")?.Attribute("Id")?.Value, out var monitoredVehicleJourneyId))
                    MonitoredVehicleJourneyId = monitoredVehicleJourneyId;

                if (long.TryParse(doc.Root.Element("TargetJourneyPatternPointRef")?.Attribute("Gid")?.Value, out var targetJourneyPatternPointGid))
                    TargetJourneyPatternPointGid = targetJourneyPatternPointGid;

                if (long.TryParse(doc.Root.Element("TimetabledJourneyPatternPointRef")?.Attribute("Gid")?.Value, out var timetabledJourneyPatternPointGid))
                    TimetabledJourneyPatternPointGid = timetabledJourneyPatternPointGid;
            }
            */
        }
    }
}