using System;
using System.Xml;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class DatedVehicleJourneyRef
    {
        public long? Id { get; set; }
        public DateTime? OperatingDayDate { get; set; }
        public long? Gid { get; set; }

        public static DatedVehicleJourneyRef ReadFromXml(XElement xml, bool required = false)
        {
            if (xml == null && !required)
                return null;

            var datedVehicleJourneyRef = new DatedVehicleJourneyRef();

            if (long.TryParse(xml.Attribute("Id")?.Value, out var id))
                datedVehicleJourneyRef.Id = id;
            if (DateTime.TryParse(xml.Attribute("OperatingDayDate")?.Value, out var operatingDayDate))
                datedVehicleJourneyRef.OperatingDayDate = operatingDayDate;
            if (long.TryParse(xml.Attribute("Gid")?.Value, out var gid))
                datedVehicleJourneyRef.Gid = gid;

            return datedVehicleJourneyRef;
        }        

        public void WriteXmlAttributes(XmlWriter xmlWriter)
        {
            if (Id.HasValue)
                xmlWriter.WriteAttributeString("Id", XmlConvert.ToString(Id.Value));
            if (OperatingDayDate.HasValue)
                xmlWriter.WriteAttributeString("OperatingDayDate", XmlConvert.ToString(OperatingDayDate.Value, "yyyy-MM-dd"));
            if (Gid.HasValue)
                xmlWriter.WriteAttributeString("Gid", Gid.ToString());
        }
    }
}
