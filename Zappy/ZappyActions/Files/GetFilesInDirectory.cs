using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                [Description("Get files from directory")]
    public class GetFilesInDirectory : TemplateAction
    {
                                [Description("Enter directory path")]
        [Category("Input")]

        public DynamicProperty<string> DirectoryPath { get; set; }

                                        [Description("Enter search pattern like *.xls,*.pdf, etc")]
        [Category("Input")]
        public DynamicProperty<string> FileSearchPattern { get; set; }

        [Description("Search Top Directory or ")]
        [Category("Optional")]
        public System.IO.SearchOption SearchOptions { get; set; }

                                [Description("get all files")]
        [Category("Output")]
        public string[] Files { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Files = System.IO.Directory.GetFiles(DirectoryPath, FileSearchPattern, SearchOptions);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo();
        }
    }
}