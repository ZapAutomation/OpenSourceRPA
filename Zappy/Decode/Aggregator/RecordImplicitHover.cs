using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class RecordImplicitHover : ActionFilter
    {
        public RecordImplicitHover() : base("RecordImplicitHover", ZappyTaskActionFilterType.Binary, false, "RecordImplicitHoverAggregator")
        {
        }

        internal static bool ImplicitMouseHoverMatchCondition(ZappyTaskActionStack actions)
        {
            bool flag = false;
            if (actions.Count <= 1)
            {
                return flag;
            }
            ZappyTaskAction recordedAction = null;
            if (actions.Count > 2)
            {
                recordedAction = actions.Peek(2);
            }
            ZappyTaskAction action2 = actions.Peek(1);
            ZappyTaskAction action3 = actions.Peek();
            KeyboardAction action4 = action3 as KeyboardAction;
            if (!MouseAction.IsImplicitHover(action3) && !(action3 is SendKeysAction) && (action4 == null || action4.ActionType != KeyActionType.KeyUp) && MouseAction.IsImplicitHover(action2))
            {
                flag = true;
            }
            if (!MouseAction.IsImplicitHover(recordedAction) || !(action2 is SendKeysAction))
            {
                return flag;
            }
            if ((action4 == null || action4.ActionType != KeyActionType.KeyUp) && !(action3 is SetValueAction))
            {
                return flag;
            }
            return true;
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (!ImplicitMouseHoverMatchCondition(actions))
            {
                return RedundantImplicitHoverMatch(actions);
            }
            return true;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            if (RedundantImplicitHoverMatch(actions) && MouseAction.IsImplicitHover(actions.Peek(1)))
            {
                ZappyTaskAction action2 = actions.Pop(1);
            }
            if (ImplicitMouseHoverMatchCondition(actions))
            {
                int nth = 1;
                if (actions.Peek(1) is SendKeysAction)
                {
                    nth = 2;
                }
                ZappyTaskAction recordedAction = null;
                while (nth < actions.Count && (recordedAction = actions.Peek(nth)) != null && MouseAction.IsImplicitHover(recordedAction))
                {
                    recordedAction.ContinueOnError = true;
                    nth++;
                }
            }
            return false;
        }

        private static bool RedundantImplicitHoverMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                ZappyTaskAction recordedAction = actions.Peek(1);
                ZappyTaskAction secondAction = actions.Peek();
                if (MouseAction.IsImplicitHover(recordedAction))
                {
                    return AggregatorUtilities.AreActivitiesOnSameElement(recordedAction, secondAction);
                }
            }
            return false;
        }
    }
}