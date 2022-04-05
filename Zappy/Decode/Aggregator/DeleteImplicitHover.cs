using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;

namespace Zappy.Decode.Aggregator
{
    internal class DeleteImplicitHover : ActionFilter
    {
        public DeleteImplicitHover() : base("DeleteImplicitHover", ZappyTaskActionFilterType.Binary, false, "DeleteImplicitHoverAggregator")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions) =>
            RecordImplicitHover.ImplicitMouseHoverMatchCondition(actions);

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            int nth = 1;
            if (actions.Peek(1) is SendKeysAction)
            {
                nth = 2;
            }
            while (nth < actions.Count && MouseAction.IsImplicitHover(actions.Peek(nth)))
            {
                ZappyTaskAction action = actions.Pop(nth);
            }
            return false;
        }
    }
}