using System.ComponentModel;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Sets Properties TO Excel")]
    public class ExcelSetProperty : TemplateAction, IExcelAction
    {
                public ExcelSetProperty()
        {
                    }
                
        [Category("Input")]
        [Description("Sets ExcelProperty Type")]
        public PropertyNames.PropertyNameEnum ExcelPropertyType { get; set; }

        [Category("Input")]
        [Description("Set PropertyValue to Excel")]
        public DynamicProperty<string> PropertyValue { get; set; }

        [Category("Input")]
        [Description("RowIndex of ExcelSheet")]
        public DynamicProperty<int> RowIndex { get; set; }

        [Category("Input")]
        [Description("Column Index of ExcelSheet")]
        public DynamicProperty<int> ColumnIndex { get; set; }

        [Category("Input")]
        [Description("SheetName of Excel")]
        public DynamicProperty<string> SheetName { get; set; }


        [Category("Input")]
        [Description("WorkbookName")]
        public DynamicProperty<string> WorkbookName { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(RowIndex, ColumnIndex, new ExcelWorksheetInfo(SheetName, WorkbookName));
            ExcelCommunicator.Instance.SetCellProperty(excelCellInfo, ExcelPropertyType.ToString(), PropertyValue);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkbookName + " SheetName:" + this.SheetName + " PropertyValue:" + this.PropertyValue;
        }
    }
}
