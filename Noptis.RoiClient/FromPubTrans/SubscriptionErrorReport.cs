using System.Xml;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class SubscriptionErrorReport : MessageBase
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }

        public override void ReadXml(XElement xml)
        {
            base.ReadXml(xml);
            Type = xml.Attribute("Type")?.Value;
            Text = xml.Attribute("Text")?.Value;
            Code = xml.Attribute("Code")?.Value;
        }
    }
}

