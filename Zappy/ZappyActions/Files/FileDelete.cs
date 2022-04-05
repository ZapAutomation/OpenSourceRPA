using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                    [Description("Delete The File")]
    public class FileDelete : TemplateAction
    {
                                [Category("Input")]
        [Description("Enter file path ")]
        public DynamicProperty<string> FilePath { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            File.Delete(FilePath);
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath;
        }
    }
}
