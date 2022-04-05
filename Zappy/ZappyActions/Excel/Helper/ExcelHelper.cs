using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel.Helper
{
    public static class ExcelHelper
    {
        public static ExcelCellInfo CreateExcelCellInfo(int RowIndex, int ColumnIndex, string SheetName, string WorkbookName)
        {
            return new ExcelCellInfo(RowIndex, ColumnIndex, new ExcelWorksheetInfo(SheetName, WorkbookName));

        }
    }
}