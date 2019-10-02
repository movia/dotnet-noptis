using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Noptis.RoiClient.ToPubTrans
{
    public class SubscriptionResumeRequest : MessageBase
    {
        public long? SubscriptionId { get; set; }

        public DateTime? StartUtcDateTime { get; set; }

        public DateTime? SynchronisedUptoUtcDateTime { get; set; }

        public override void WriteXmlAttributes(XmlWriter xmlWriter)
        {
            if (SubscriptionId != null)
                xmlWriter.WriteAttributeString(nameof(SubscriptionId), SubscriptionId.ToString());
            if (StartUtcDateTime != null)
                xmlWriter.WriteAttributeString(nameof(StartUtcDateTime), StartUtcDateTime.Value.ToString("o"));
            if (SynchronisedUptoUtcDateTime != null)
                xmlWriter.WriteAttributeString(nameof(SynchronisedUptoUtcDateTime), SynchronisedUptoUtcDateTime.Value.ToString("o"));
        }
    }
}
