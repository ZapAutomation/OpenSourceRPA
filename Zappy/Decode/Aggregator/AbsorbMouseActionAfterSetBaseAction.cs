using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Mouse;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbMouseActionAfterSetBaseAction : ActionFilter
    {
        public AbsorbMouseActionAfterSetBaseAction() : base("AbsorbMouseActionAfterSetBaseAction", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                MouseAction action = actions.Peek(0) as MouseAction;
                if (action != null && (action.ActionType == MouseActionType.Click || action.ActionType == MouseActionType.Drag))
                {
                    SetBaseAction action2 = actions.Peek(1) as SetBaseAction;
                    if (action2 != null)
                    {
                        return true;
                    }
                    MediaAction action3 = actions.Peek(1) as MediaAction;
                    return action3 != null;
                }
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            SetBaseAction action = actions.Peek(1) as SetBaseAction;
            MediaAction action2 = actions.Peek(1) as MediaAction;
            MouseAction action3 = actions.Peek() as MouseAction;
            if (action != null)
            {
                if (AggregatorUtilities.GetTagFromAction<bool>(action, "ShouldEatClickAction"))
                {
                    actions.Pop();
                    if (action3.ActionType == MouseActionType.Click)
                    {
                        AggregatorUtilities.AddTagToAction(action, "ShouldEatClickAction", false);
                        if (action is SetValueAction)
                        {
                            AggregatorUtilities.AddTagToAction(action, "SetValueCausedBy", action3);
                            return false;
                        }
                    }
                    return true;
                }
            }
            else if (AggregatorUtilities.GetTagFromAction<bool>(action2, "ShouldEatClickAction"))
            {
                actions.Pop();
                if (action3.ActionType == MouseActionType.Click)
                {
                    AggregatorUtilities.AddTagToAction(action2, "ShouldEatClickAction", false);
                }
                return true;
            }
            return false;
        }
    }
}

