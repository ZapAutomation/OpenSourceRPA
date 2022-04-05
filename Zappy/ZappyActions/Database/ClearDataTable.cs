using System.Activities;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Database
{
            
    [Description("Clear DataTable")]
    public class ClearDataTable : TemplateAction
    {
        [Description("Clear data from a DataTable")]
        [Category("Input"), RequiredArgument]
        public DynamicProperty<System.Data.DataTable> DataTable { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            this.DataTable.Value.Clear();
        }

    }
}

