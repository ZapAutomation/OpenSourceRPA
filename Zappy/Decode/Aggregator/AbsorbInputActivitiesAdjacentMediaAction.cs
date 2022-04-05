using System.Windows.Forms;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbInputActivitiesAdjacentMediaAction : ActionFilter
    {
        public AbsorbInputActivitiesAdjacentMediaAction() : base("AbsorbInputActivitiesAdjacentMediaAction", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                ZappyTaskAction firstAction = actions.Peek();
                ZappyTaskAction secondAction = actions.Peek(1);
                if (AggregatorUtilities.AreActivitiesOnSameElement(firstAction, secondAction))
                {
                    if (firstAction is MediaAction && secondAction is InputAction)
                    {
                        return true;
                    }
                    if (firstAction is InputAction && secondAction is MediaAction)
                    {
                        MouseAction action3 = firstAction as MouseAction;
                        if (action3 != null)
                        {
                            return action3.MouseButton == MouseButtons.Left;
                        }
                    }
                }
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            MediaAction actionToTag = actions.Peek() as MediaAction;
            if (actionToTag != null)
            {
                MouseAction mouseInputAction = actions.Pop(1) as MouseAction;
                AggregatorUtilities.TagActionWithEatClick(mouseInputAction, actionToTag);
                return false;
            }
            actionToTag = actions.Peek(1) as MediaAction;
            MouseAction action3 = null;
            if (AggregatorUtilities.GetTagFromAction<bool>(actionToTag, "ShouldEatClickAction"))
            {
                action3 = actions.Pop() as MouseAction;
            }
            if (action3 != null && (action3.ActionType == MouseActionType.Click || action3.ActionType == MouseActionType.WheelRotate))
            {
                AggregatorUtilities.AddTagToAction(actionToTag, "ShouldEatClickAction", false);
            }
            return true;
        }
    }
}

