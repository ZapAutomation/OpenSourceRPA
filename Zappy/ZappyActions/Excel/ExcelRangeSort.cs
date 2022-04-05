using System;
using System.ComponentModel;
using System.Data;
using Zappy.Decode.LogManager;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    public class ExcelRangeSort : TemplateAction
    {
        public ExcelRangeSort()
        {
            ColumnToSortBy = 1;
        }

        [Category("Input")]
        [Description("Start row index of excel")]
        public DynamicProperty<int> StartRowIndex { get; set; }

        [Category("Input")]
        [Description("Start column index of excel")]
        public DynamicProperty<int> StartColumnIndex { get; set; }

        [Category("Input")]
        [Description("End row index of excel")]
        public DynamicProperty<int> EndRowIndex { get; set; }

        [Category("Input")]
        [Description("End row index of excel")]
        public DynamicProperty<int> EndColumnIndex { get; set; }

        [Category("Input")]
        [Description("ExSheet name of excel")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description("WorkbookName name")]
        public DynamicProperty<string> WorkbookName { get; set; }

        [Category("Input")]
        [Description("WorkbookName name")]
        public DynamicProperty<int> ColumnToSortBy { get; set; }

        [Category("Input")]
        [Description("SortOrder")]
        public ExcelSortOrder SortOrder { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(StartRowIndex, StartColumnIndex, new ExcelWorksheetInfo(SheetName, WorkbookName));
            excelCellInfo.ExcelRange = new ExcelRangeInfo();
            excelCellInfo.ExcelRange.UseRange = true;
            excelCellInfo.ExcelRange.RangeEndRowIndex = EndRowIndex;
            excelCellInfo.ExcelRange.RangeEndColumnIndex = EndColumnIndex;
            ExcelCommunicator.Instance.SortExcelRange(excelCellInfo, SortOrder.ToString(), ColumnToSortBy);

        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " SheetName:" + this.SheetName + " Excel File location:" + this.WorkbookName;
        }
    }
}
