using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Gets ColumnHeading of Excel")]
    public class ExcelGetColumnHeading : TemplateAction
    {
        public ExcelGetColumnHeading()
        {
            ColumnHeadingRow = 1;
        }
        [Category("Input")]
        [Description("SheetName of opened workbook")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description(" Workbook name to get heading of set column")]
        public DynamicProperty<string> WorkbookName { get; set; }

        [Category("Optional")]
        [DefaultValue(1)]
        [Description("Column Heading Row")]
        public DynamicProperty<int> ColumnHeadingRow { get; set; }

        [Category("Input")]
        [Description("Column number to output the heading of the column")]
        public DynamicProperty<int> ColumnToGetHeading { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Gets column heading of excel")]
        public string ColumnHeading { get; set; }

                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            ExcelCellInfo excelCellInfoProp =
                new ExcelCellInfo(ColumnHeadingRow, ColumnToGetHeading,
                    new ExcelWorksheetInfo(SheetName, WorkbookName));
            ColumnHeading =
                ExcelCommunicator.Instance.GetCellProperty(excelCellInfoProp, PropertyNames.Value) as string;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkbookName + " SheetName:" + this.SheetName + " ColumnHeading:" + this.ColumnHeading;
        }
    }
}