using System.ComponentModel;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
                [Description("Change the column width to automatically fit the contents (AutoFit)")]
    public class ExcelSheetAutoFitColumns : TemplateAction
    {
        public ExcelSheetAutoFitColumns()
        {
        }

        [Category("Input")]
        [Description("Enter SheetName")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description("Enter WorkbookName")]
        public DynamicProperty<string> WorkbookName { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(1, 1, new ExcelWorksheetInfo(SheetName, WorkbookName));
            ExcelCommunicator.Instance.RunExcelCustomAction(excelCellInfo, ExcelCustomPropertyNames.FitExcelWorkSheet.ToString(), "");
        }


        public override string AuditInfo()
        {
            return base.AuditInfo() + "Workbook Name: " + WorkbookName + "SheetName" + SheetName;
        }
    }
}