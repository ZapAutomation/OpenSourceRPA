using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Documents
{
            
    [Description("Reads Text File content")]
    public class ReadTextFile : TemplateAction
    {
        [Category("Optional"), DefaultValue((string)null)]
        [Description("Encoding String")]
        public DynamicProperty<string> Encoding { get; set; }

        [Category("Input"), RequiredArgument]
        [Description("Text file path ")]
        public DynamicProperty<string> FilePath { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Read Content from File")]
        public string Content { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string encoding = this.Encoding;
            string fileName = this.FilePath;
            Content = this.Execute(fileName, encoding);
        }


        protected string Execute(string fileName, string encodingStr)
        {
            if (!string.IsNullOrWhiteSpace(encodingStr))
            {
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(encodingStr.Trim());
                return File.ReadAllText(fileName, encoding);
            }
            return File.ReadAllText(fileName);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FileName:" + this.FilePath + " Content:" + this.Content;
        }
    }
}

