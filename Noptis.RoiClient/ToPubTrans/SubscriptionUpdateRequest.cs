using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Noptis.RoiClient.ToPubTrans
{
    public class SubscriptionUpdateRequest : MessageBase
    {
        public long? SubscriptionId { get; set; }

        public AssignmentEventSelection AssignmentEventSelection { get; set; }

        public VehicleJourneyEventSelection VehicleJourneyEventSelection { get; set; }

        public SubscriptionUpdateRequest()
        {
            AssignmentEventSelection = new AssignmentEventSelection();
        }

        public override void WriteXmlAttributes(XmlWriter xmlWriter)
        {
            if (SubscriptionId != null)
                xmlWriter.WriteAttributeString(nameof(SubscriptionId), SubscriptionId.ToString());
        }

        public override void WriteXmlElements(XmlWriter xmlWriter)
        {
            // Order matters!
            VehicleJourneyEventSelection?.WriteXml(xmlWriter);
            AssignmentEventSelection?.WriteXml(xmlWriter);
            //xmlWriter.WriteString("<DeviationCaseEventSelection><ScopeElements><ScopeElement/></ScopeElements><PublicationScopeElements><PublicationScopeElement/></PublicationScopeElements></DeviationCaseEventSelection>")
            //xmlWriter.WriteString("<NetworkDeviationEventSelection/>");
        }
    }
}
