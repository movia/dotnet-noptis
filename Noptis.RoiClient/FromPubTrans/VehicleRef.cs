using System.Xml;
using System.Xml.Linq;

namespace Noptis.RoiClient.FromPubTrans
{
    public class VehicleRef
    {
        public long? Gid { get; set; }

        public static VehicleRef ReadFromXml(XElement xml, bool required = false)
        {
            if (xml == null && !required)
                return null;

            var vehicleRef = new VehicleRef();

            if (long.TryParse(xml.Attribute("Gid")?.Value, out var gid))
                vehicleRef.Gid = gid;

            return vehicleRef;
        }

        public void WriteXmlAttributes(XmlWriter xmlWriter)
        {
            if (Gid.HasValue)
                xmlWriter.WriteAttributeString("Gid", Gid.ToString());
        }
    }
}
