using System;
using System.Xml;

namespace Noptis.RoiClient.FromPubTrans
{
    public class SynchronisationReport : MessageBase
    {
        public long OnSubscriptionId { get; set; }
        public DateTime SynchronisedUptoUtcDateTime { get; set; }
        public string HasCompletedRecoveryPhase { get; set; }

        public override void ReadXmlAttributes(XmlReader xmlReader)
        {
            if (long.TryParse(xmlReader.GetAttribute("OnSubscriptionId"), out long onSubscriptionId))
                OnSubscriptionId = onSubscriptionId;
            if (DateTime.TryParse(xmlReader.GetAttribute("SynchronisedUptoUtcDateTime"), out DateTime synchronisedUptoUtcDateTime))
                SynchronisedUptoUtcDateTime = DateTime.SpecifyKind(synchronisedUptoUtcDateTime, DateTimeKind.Utc);
            HasCompletedRecoveryPhase = xmlReader.GetAttribute("HasCompletedRecoveryPhase");
        }
    }
}
