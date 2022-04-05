using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Find Text From ExcelFile")]
    public class ExcelFindText : TemplateAction
    {
        public ExcelFindText()
        {
            ColumnHeadingRow = 1;
        }

                        [Category("Input")]
        [Description("Excel SheetName for finding Text")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description("WorkbookName")]
        public DynamicProperty<string> WorkbookName { get; set; }

        [Category("Input")]
        [Description("Search Text from Workbook")]
        public DynamicProperty<string> TextToSearch { get; set; }

        [Category("Input")]
        [Description("")]
        public DynamicProperty<bool> GetColumnHeadingIfTextFound { get; set; }

        [DefaultValue(1)]
        [Category("Input")]
        [Description("Column Heading Row")]
        public DynamicProperty<int> ColumnHeadingRow { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("First Occurance Cell Info ")]
        public ExcelCellInfo OccuranceCell { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Row Index ")]
        public int RowIndexFoundText { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Column Index ")]
        public int ColumnIndexFoundText { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Gets true if Text is found otherwise false")]
        public bool TextFound { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("ColumnHeading")]
        public string ColumnHeading { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(1, 1, new ExcelWorksheetInfo(SheetName, WorkbookName));
            OccuranceCell =
                ExcelCommunicator.Instance.RunExcelCustomAction(excelCellInfo, ExcelCustomPropertyNames.FindTextInSheet.ToString(), TextToSearch) as ExcelCellInfo;

            if (OccuranceCell != null)
            {
                TextFound = true;

                if (GetColumnHeadingIfTextFound)
                {
                    ExcelCellInfo excelCellInfoProp =
                        new ExcelCellInfo(ColumnHeadingRow, OccuranceCell.ColumnIndex, new ExcelWorksheetInfo(SheetName, WorkbookName));
                    ColumnHeading = ExcelCommunicator.Instance.GetCellProperty(excelCellInfoProp, PropertyNames.Value) as string;
                }
            }
            RowIndexFoundText = OccuranceCell.RowIndex;
            ColumnIndexFoundText = OccuranceCell.ColumnIndex;                                                                                 
                                                                                    
                                                                                    
                                                                        
                        
                                }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkbookName + " SheetName:" + this.SheetName + " TextToSearch:" + this.TextToSearch + " ColumnHeading:" + this.ColumnHeading;
        }
    }
}