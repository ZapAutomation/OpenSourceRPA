using System.IO;
using System.Text;
using System.Xml.Linq;
using ZappyMessages.Util.Files;

namespace ZappyMessages.Robot
{
    public class ZappyFileHelper : GenFileHelper
    {
        public ZappyFileHelper(string filePath, Encoding encoding = null) : base(filePath, encoding)
        {
        }

        public string GetGUID()
        {
            //Extract uid from zappy file
            XElement task = XElement.Load(filePath);
            var fguid = task.Attribute("Id").Value;
            return fguid;
        }

        public ZappyFileInfo BuildInfo()
        {
            //return info
            return new ZappyFileInfo()
            {
                Guid = GetGUID(),
                Description = "",
                FilePath = filePath,
                Name = Path.GetFileName(filePath)
            };
        }
    }
}
