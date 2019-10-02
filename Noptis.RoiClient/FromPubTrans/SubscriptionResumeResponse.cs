using System.Xml;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class SubscriptionResumeResponse : MessageBase
    {
        public long SubscriptionId { get; set; }

        public override void ReadXmlAttribute(XAttribute attr)
        {
            switch (attr.Name.LocalName)
            {
                case "SubscriptionId":
                    if (long.TryParse(attr.Value, out long subscriptionId))
                        SubscriptionId = subscriptionId;
                    break;
            }

        }
    }
}
