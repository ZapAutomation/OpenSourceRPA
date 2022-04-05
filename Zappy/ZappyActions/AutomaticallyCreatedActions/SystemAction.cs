using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    public class SystemAction : ZappyTaskAction
    {
        public SystemAction()
        {
        }

        public SystemAction(ZappyTaskActionLogEntry actionLog)
        {
            ZappyTaskUtilities.CheckForNull(actionLog, "actionLog");
                    }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                    }

        public string ActionType { get; set; }
    }
}