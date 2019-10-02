using System.Xml;

namespace Noptis.RoiClient.FromPubTrans
{
    public class SubscriptionResumeResponse : MessageBase
    {
        public long SubscriptionId { get; set; }

        public override void ReadXmlAttributes(XmlReader xmlReader)
        {
            if (long.TryParse(xmlReader.GetAttribute("SubscriptionId"), out long subscriptionId))
                SubscriptionId = subscriptionId;
        }
    }
}
