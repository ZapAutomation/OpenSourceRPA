using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                [Description("Create New file")]
    public class FileCreate : TemplateAction
    {
                                [Category("Input")]
        [Description("Enter file path")]
        public DynamicProperty<string> FilePath { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            File.CreateText(FilePath).Close();
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath;
        }
    }
}
