using System.ComponentModel;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
                [Description("Close Excel Workbook")]
    public class ExcelCloseWorkbook : TemplateAction
    {
        public ExcelCloseWorkbook()
        {
        }


        [Category("Input")]
        [Description("Enter WorkbookName")]
        public DynamicProperty<string> WorkbookName { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(1, 1, new ExcelWorksheetInfo(string.Empty,WorkbookName));
            ExcelCommunicator.Instance.RunExcelCustomAction(excelCellInfo, ExcelCustomPropertyNames.CloseWorkbook.ToString(), "");
        }


        public override string AuditInfo()
        {
            return base.AuditInfo() + "Workbook Name: " + WorkbookName;
        }
    }
}