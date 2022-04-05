using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                [Description("Open File")]
    public class FileOpen : TemplateAction
    {
                                [Category("Input")]
        [Description("FilePath to open the file")]
        public DynamicProperty<string> FilePath { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            System.Diagnostics.Process.Start(FilePath);
                    }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath;
        }
    }
}