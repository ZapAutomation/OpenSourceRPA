using System.ComponentModel;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel.ExcelWorkbook
{
    [Description("Save Workbook")]
    public class WorkbookSave : TemplateAction
    {
        [Category("Input")]
        [Description("Enter the path and Workbook Name with extension where you want to save")]
        public DynamicProperty<string> WorkbookName { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(1, 1, new ExcelWorksheetInfo("", WorkbookName));
            ExcelCommunicator.Instance.SaveWorkbook(excelCellInfo);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkbookName;
        }
    }
}