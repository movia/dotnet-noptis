using System;
using System.Xml;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class SynchronisationReport : MessageBase
    {
        public long OnSubscriptionId { get; set; }
        public DateTime SynchronisedUptoUtcDateTime { get; set; }
        public string HasCompletedRecoveryPhase { get; set; }

        public override void ReadXml(XElement xml)
        {
            base.ReadXml(xml);

            if (long.TryParse(xml.Attribute("OnSubscriptionId")?.Value, out long onSubscriptionId))
                OnSubscriptionId = onSubscriptionId;
            if (DateTime.TryParse(xml.Attribute("SynchronisedUptoUtcDateTime")?.Value, out DateTime synchronisedUptoUtcDateTime))
                SynchronisedUptoUtcDateTime = DateTime.SpecifyKind(synchronisedUptoUtcDateTime, DateTimeKind.Utc);
            HasCompletedRecoveryPhase = xml.Attribute("HasCompletedRecoveryPhase")?.Value;
        }
    }
}
