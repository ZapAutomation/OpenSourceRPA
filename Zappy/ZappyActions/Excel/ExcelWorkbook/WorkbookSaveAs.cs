using System.ComponentModel;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel.ExcelWorkbook
{
    [Description("Save Workbook in other directory")]
    public class WorkbookSaveAs : TemplateAction
    {
                                [Category("Input")]
        [Description("Enter Workbook Name")]
        public DynamicProperty<string> WorkbookName { get; set; }

                                [Category("Input")]
        [Description("Enter Path")]
        public DynamicProperty<string> SavePath { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

                                                            ExcelCellInfo excelCellInfo =
              new ExcelCellInfo(1, 1, new ExcelWorksheetInfo(SavePath, WorkbookName));
            ExcelCommunicator.Instance.SaveWorkbookAs(excelCellInfo);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkbookName + " WorkbookNameAs:" + this.SavePath;
        }
    }
}