using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Core
{
    [Description("End workflow")]
    public sealed class EndNodeAction : TemplateAction
    {
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
        }
    }
}
