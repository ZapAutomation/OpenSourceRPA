using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;

namespace Zappy.Decode.Aggregator
{
    internal class ConvertImplicitHover : ActionFilter
    {
        public ConvertImplicitHover() : base("ConvertImplicitHover", ZappyTaskActionFilterType.Binary, false, "MiscellaneousAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                ZappyTaskAction recordedAction = actions.Peek(1);
                if (MouseAction.IsExplicitHover(actions.Peek()) && MouseAction.IsImplicitHover(recordedAction))
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            for (int i = 1; i <= actions.Count - 1; i++)
            {
                if (!MouseAction.IsImplicitHover(actions.Peek(i)))
                {
                    break;
                }
                MouseAction action = actions.Peek(i) as MouseAction;
                action.ContinueOnError = true;
                action.ImplicitHover = false;
            }
            return false;
        }
    }
}

