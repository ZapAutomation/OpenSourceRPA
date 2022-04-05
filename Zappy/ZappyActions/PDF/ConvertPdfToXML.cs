extern alias itextsharp;

using itextsharp::iTextSharp.text.pdf;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.PDF
{
    public class ConvertPdfToXML : TemplateAction
    {
        [Category("Input")]
        [Description("Input PDF file path")]
        public DynamicProperty<string> PDFFilePath { get; set; }
        [Category("Output")]
        [Description("XML view of PDF")]
        public string OutputString { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            OutputString = ReadData(PDFFilePath);
        }

        public string ReadData(string src)
        {
            PdfReader reader = new PdfReader(src);
            XfaForm xfa = new XfaForm(reader);
            XmlNode node = xfa.DatasetsNode;
            XmlNodeList list = node.ChildNodes;
            for (int i = 0; i < list.Count; i++)
            {
                if ("data".Equals(list.Item(i).LocalName))
                {
                    node = list.Item(i);
                    break;
                }
            }
                                                                                                                        reader.Close();

            var sb = new StringBuilder(4000);
            var Xsettings = new XmlWriterSettings() { Indent = true };

            
            
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(node.OuterXml);
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }

        }
    }
}
