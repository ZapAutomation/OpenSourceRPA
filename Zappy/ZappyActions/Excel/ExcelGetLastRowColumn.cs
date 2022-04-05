using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Gets Last Used Row And Column Of Excel")]
    public class ExcelGetLastRowColumn : TemplateAction
    {
        [Category("Input")]
        [Description(" SheetName ")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description("WorkbookName")]
        public DynamicProperty<string> WorkbookName { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Last used column from the Excel")]
        public int lastUsedColumn { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Last used row from the excel")]
        public int lastUsedRow { get; set; }

                                                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(0, 0, new ExcelWorksheetInfo(SheetName, WorkbookName));
            ExcelCellInfo LastCell =
                ExcelCommunicator.Instance.RunExcelCustomAction(excelCellInfo, ExcelCustomPropertyNames.FindLastCellRow.ToString(), "") as ExcelCellInfo;
            if (LastCell != null)
            {
                lastUsedColumn = LastCell.ColumnIndex;
                lastUsedRow = LastCell.RowIndex;
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkbookName + " SheetName:" + this.SheetName + " lastUsedRow:" + this.lastUsedRow + " lastUsedColumn:" + this.lastUsedColumn;
        }
    }
}