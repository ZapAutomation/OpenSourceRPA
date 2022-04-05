using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.Plugins.Excel
{
    public class ExcelCell : ZappyTaskControl
    {
        public ExcelCell()
            : this(null)
        {

        }

        public ExcelCell(ZappyTaskControl parent)
            : base(parent)
        {
            this.TechnologyName = "Excel";
            this.SearchProperties.Add(PropertyNames.ControlType, ControlType.Cell.Name);
        }

        public string Value
        {
            get
            {
                return GetProperty("Value") as string;
            }
            set
            {
                SetProperty("Value", value);
            }
        }

        public new class PropertyNames : ZappyTaskControl.PropertyNames
        {
            public static string RowIndex = "RowIndex";
            public static string ColumnIndex = "ColumnIndex";
        }
    }
}
