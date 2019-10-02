using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

namespace Noptis.RoiClient.ToPubTrans
{
    public class VehicleJourneyEventSelection : MessageBase
    {
        public string LookAheadDuration { get; set; } = "PT3H";

        public bool ExpandLineData { get; set; } = true;

        public bool ExpandVehicleOperatorData { get; set; } = true;

        public ScopeElements ScopeElements { get; set; } = new ScopeElements() { new ScopeElement() };

        public override void WriteXmlAttributes(XmlWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("LookAheadDuration", LookAheadDuration);
            xmlWriter.WriteAttributeString("ExpandLineData", ExpandLineData ? "Y" : "N");
            xmlWriter.WriteAttributeString("ExpandVehicleOperatorData", ExpandVehicleOperatorData ? "Y" : "N");            
        }

        public override void WriteXmlElements(XmlWriter xmlWriter) => ScopeElements.WriteXml(xmlWriter);
    }
}
