using System.Xml;

namespace Noptis.RoiClient.FromPubTrans
{
    public class SubscriptionErrorReport : MessageBase
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }

        public override void ReadXmlAttributes(XmlReader xmlReader)
        {
            Type = xmlReader.GetAttribute("Type");
            Text = xmlReader.GetAttribute("Text");
            Code = xmlReader.GetAttribute("Code");
        }
    }
}

