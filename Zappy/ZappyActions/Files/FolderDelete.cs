using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
            
    [Description("Delete Folder")]
    public class FolderDelete : TemplateAction
    {
                                [Category("Input")]
        [Description("Enter folder path")]
        public DynamicProperty<string> FolderPath { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Directory.Delete(FolderPath, true);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FolderPath:" + this.FolderPath;
        }
    }
}
