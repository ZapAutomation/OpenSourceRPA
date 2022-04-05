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
    [Description("Convert Json string or file into Xml string")]
    public class JsonToXml : TemplateAction
    {

        [Category("Optional")]
        [Description("Enter Json File Path")]
        public DynamicProperty<string> JsonFilePath { get; set; }

        [Category("Input")]
        [Description("Enter Json Input String")]
        public DynamicProperty<string> InputString { get; set; }


        [Category("Output")]
        [Description("Converted XML Output")]
        public string Result { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            if (string.IsNullOrEmpty(InputString))
            {
                string jsondata = File.ReadAllText(JsonFilePath);
                XmlDocument node = JsonConvert.DeserializeXmlNode(jsondata);
                Result = node.InnerXml;
            }
            else
            {
                XmlDocument node = JsonConvert.DeserializeXmlNode(InputString);
                Result = node.InnerXml;
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " JsonFile Path:" + this.JsonFilePath + " InputString:" + this.InputString + " XMLResult:" + this.Result;
        }

    }
}