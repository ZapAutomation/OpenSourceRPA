using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;


namespace Zappy.ZappyActions.Core
{
    [Description("Start the workflow")]
                public sealed class StartNodeAction : TemplateAction
    {
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
        }
    }
}
