using System.Collections.Generic;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class AssignmentEvent : MessageBase
    {
        public List<VehicleJourneyAssignment> VehicleJourneyAssignments { get; set; } = new List<VehicleJourneyAssignment>();

        public override void ReadXmlElement(XElement el) {
            switch (el.Name.LocalName)
            {
                case "VehicleJourneyAssignment": ReadVehicleJourneyAssignmentXml(el); break;
            }
        }

        private void ReadVehicleJourneyAssignmentXml(XElement el) {
            var vehicleJourneyAssignment = new VehicleJourneyAssignment();
            vehicleJourneyAssignment.ReadXml(el);
            VehicleJourneyAssignments.Add(vehicleJourneyAssignment);
        }
    }
}
