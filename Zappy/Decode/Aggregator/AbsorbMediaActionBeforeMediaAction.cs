using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Helper.Enums;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbMediaActionBeforeMediaAction : ActionFilter
    {
        public AbsorbMediaActionBeforeMediaAction() : base("AbsorbMediaActionBeforeMediaAction", ZappyTaskActionFilterType.Binary, true, "SetValueAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                MediaAction firstAction = actions.Peek() as MediaAction;
                MediaAction secondAction = actions.Peek(1) as MediaAction;
                if (AggregatorUtilities.AreActivitiesOnSameElement(firstAction, secondAction))
                {
                    if (firstAction.ActionType == secondAction.ActionType && firstAction.ActionType != MediaActionType.Mute)
                    {
                        return true;
                    }
                    if (secondAction.ActionType == MediaActionType.Mute && !secondAction.Mute && firstAction.ActionType == MediaActionType.VolumeChange)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            MediaAction action = actions.Pop(1) as MediaAction;
            MediaAction action2 = actions.Peek() as MediaAction;
            if (AggregatorUtilities.GetTagFromAction<bool>(action, "ShouldEatClickAction"))
            {
                AggregatorUtilities.AddTagToAction(action2, "ShouldEatClickAction", true);
            }
            return false;
        }
    }
}

