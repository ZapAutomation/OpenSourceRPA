using System.ComponentModel;
using System.IO;
using Trinet.Core.IO.Ntfs;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
                [Description("It will remove internet protection from file")]
    public class RemoveInternetFileProtection : TemplateAction
    {
                                [Description("Enter file path")]
        [Category("Input")]
        public DynamicProperty<string> FilePath { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        FileInfo fInfo = new FileInfo(FilePath);
                        fInfo.DeleteAlternateDataStream("Zone.Identifier");

        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath: " + this.FilePath;
        }
    }
}

