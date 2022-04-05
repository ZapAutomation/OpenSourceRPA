using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                [Description("Create New Folder")]
    public class FolderCreate : TemplateAction
    {
                                [Category("Input")]
        [Description("Enter folder path")]
        public DynamicProperty<string> FolderPath { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            Directory.CreateDirectory(FolderPath);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FolderPath:" + this.FolderPath;
        }
    }
}
