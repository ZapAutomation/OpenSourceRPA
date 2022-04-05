using System.IO;
using System.Text;

namespace ZappyMessages.Util.Files
{
    public class GenFileHelper
    {
        protected string filePath;
        protected readonly Encoding DefaultEncoding = Encoding.UTF8;

        public GenFileHelper(string filePath, Encoding encoding = null)
        {
            this.filePath = filePath;

            if (encoding != null)
            {
                DefaultEncoding = encoding;
            }
        }

        public string ReadContent()
        {
            var fstrm = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(fstrm, DefaultEncoding);

            try
            {
                var content = reader.ReadToEnd();
                return content;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public void WriteToFile(string content)
        {
            var fstrm = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter writer = new StreamWriter(fstrm, DefaultEncoding);

            try
            {
                writer.Write(content);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        public byte[] ReadBytes()
        {
            var content = ReadContent();
            return DefaultEncoding.GetBytes(content);
        }

        public void WriteBytes(byte[] content)
        {
            WriteToFile(DefaultEncoding.GetString(content));
        }
    }
}
