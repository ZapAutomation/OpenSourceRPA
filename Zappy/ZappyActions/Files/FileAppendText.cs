using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;


namespace Zappy.ZappyActions.Files
{
                [Description("Append Text To File")]
    public class FileAppendText : TemplateAction
    {
                                [Category("Input")]
        [Description("Enter file path")]
        public DynamicProperty<string> FilePath { get; set; }

                                [Category("Input")]
        [Description("Text to append into File")]
        public DynamicProperty<string> Text { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            File.AppendAllText(FilePath, Text);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath + " Text:" + this.Text;
        }
    }
}
