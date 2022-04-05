using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal static class ZappyTaskSerializationHelper
    {
        public static object Deserialize(XmlSerializer xmlSerializer, Stream reader)
        {
            using (XmlTextReader reader2 = new XmlTextReader(reader))
            {
                reader2.DtdProcessing = DtdProcessing.Prohibit;
                return xmlSerializer.Deserialize(reader2);
            }
        }

                                                                                                                                    }
}