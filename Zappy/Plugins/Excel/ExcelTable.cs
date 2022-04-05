using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.Plugins.Excel
{
    public class ExcelTable : ZappyTaskControl
    {
        public ExcelTable()
            : this(null)
        {

        }

        public ExcelTable(ZappyTaskControl parent)
            : base(parent)
        {
            this.TechnologyName = "Excel";
            this.SearchProperties.Add(PropertyNames.ControlType, ControlType.Table.Name);
        }

        public new class PropertyNames : ZappyTaskControl.PropertyNames
        {
        }
    }
}
