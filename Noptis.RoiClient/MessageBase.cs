using System;
using System.IO;
using System.Xml;

namespace Noptis.RoiClient
{
    public class MessageBase
    {
        public MessageBase() { }

        public long? MessageId { get; set; }

        public long? OnMessageId { get; set; }

        public string ToXmlString()
        {
            var xmlWriterSettings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                
            };
            var stringWriter = new StringWriter();
            var xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings);
            WriteXml(xmlWriter);
            xmlWriter.Flush();
            return stringWriter.ToString();
        }

        public virtual void ReadXml(XmlReader xmlReader)
        {
            if (long.TryParse(xmlReader.GetAttribute("MessageId"), out long mesageId))
                MessageId = mesageId;
            ReadXmlAttributes(xmlReader);
            xmlReader.Read();
            ReadXmlElements(xmlReader);
        }

        public virtual void ReadXmlAttributes(XmlReader xmlReader) { }

        public virtual void ReadXmlElements(XmlReader xmlReader) { }

        public virtual void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(this.GetType().Name);
            if (MessageId != null)
                xmlWriter.WriteAttributeString("MessageId", MessageId.ToString());
            if (OnMessageId != null)
                xmlWriter.WriteAttributeString("OnMessageId", OnMessageId.ToString());
            WriteXmlAttributes(xmlWriter);
            WriteXmlElements(xmlWriter);
            xmlWriter.WriteEndElement();
        }

        public virtual void WriteXmlAttributes(XmlWriter xmlWriter) { }

        public virtual void WriteXmlElements(XmlWriter xmlWriter) { }
    }
}