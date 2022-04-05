using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Utilities
{
    [Description("Convert XML string or file into Json string")]
    public class XmlToJson : TemplateAction
    {
       
        [Category("Optional")]
        [Description("Enter XML File Path")]
        public DynamicProperty<string> XMLFilePath { get; set; }

        [Category("Input")]
        [Description("Enter XML Input String")]
        public DynamicProperty<string> InputString { get; set; }


        [Category("Output")]
        [Description("Converted Json Output")]
        public string Result { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            if (string.IsNullOrEmpty(InputString))
            {
                string xmldata = File.ReadAllText(XMLFilePath);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmldata);

                Result = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);

            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(InputString);

                Result = JsonConvert.SerializeXmlNode(doc);
            }

        }
           
       public override string AuditInfo()
       {
          return base.AuditInfo() + " XML File Path:" + this.XMLFilePath + " InputString:" + this.InputString + " JsonResult:" + this.Result;
       }

    }
}