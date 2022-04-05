using System.ComponentModel;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Gets Excel Cell Properties such as Value, Formula, etc.")]
    public class ExcelGetProperty : TemplateAction, IExcelAction
    {
                public ExcelGetProperty()
        {
                    }

                
        [Category("Input")]
        [Description("Type of the Excel property")]
        public PropertyNames.PropertyNameEnum ExcelPropertyType { get; set; }

        [Category("Input")]
        [Description("RowIndex of Excel")]
        public DynamicProperty<int> RowIndex { get; set; }

        [Category("Input")]
        [Description("ColumnIndex of Excel")]
        public DynamicProperty<int> ColumnIndex { get; set; }

        [Category("Input")]
        [Description("SheetName of Workbook")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        public DynamicProperty<string> WorkbookName { get; set; }

        [Category("Output")]
        [Description("Gets PropertyValue of Excel")]
        public object PropertyValue { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(RowIndex, ColumnIndex, new ExcelWorksheetInfo(SheetName, WorkbookName));
            PropertyValue = ExcelCommunicator.Instance.GetCellProperty(excelCellInfo, ExcelPropertyType.ToString());
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkbookName + " SheetName:" + this.SheetName + " PropertyValue:" + this.PropertyValue;
        }
    }
}