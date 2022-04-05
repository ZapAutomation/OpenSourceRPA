using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                [Description("Write Text To File ")]
    public class FileWriteText : TemplateAction
    {

        [Category("Optional"), DefaultValue((string)null)]
        [Description("Encoding String")]
        public DynamicProperty<string> Encoding { get; set; }
                                [Description("File path to write the content to file")]
        [Category("Input")]
        public DynamicProperty<string> FilePath { get; set; }

                                [Description("Write text to file")]
        [Category("Input")] public DynamicProperty<string> Text { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (!string.IsNullOrWhiteSpace(Encoding))
            {
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(Encoding.Value.Trim());
                File.WriteAllText(FilePath, Text, encoding);
            }
            else
            {
                File.WriteAllText(FilePath, Text);
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath + " Text:" + this.Text;
        }
    }
}
