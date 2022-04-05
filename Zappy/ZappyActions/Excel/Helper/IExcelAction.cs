using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Excel.Helper
{
    public interface IExcelAction
    {
        DynamicProperty<int> RowIndex { get; set; }
        DynamicProperty<int> ColumnIndex { get; set; }
        DynamicProperty<string> SheetName { get; set; }
        DynamicProperty<string> WorkbookName { get; set; }
    }
}