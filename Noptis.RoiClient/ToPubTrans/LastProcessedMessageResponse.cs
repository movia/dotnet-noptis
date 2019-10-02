using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Noptis.RoiClient.ToPubTrans
{
    class LastProcessedMessageResponse : MessageBase
    {
        public long LastProcessedMessageId { get; set; }

        public override void WriteXmlAttributes(XmlWriter xmlWriter) => 
            xmlWriter.WriteAttributeString("LastProcessedMessageId", LastProcessedMessageId.ToString());
    }
}
