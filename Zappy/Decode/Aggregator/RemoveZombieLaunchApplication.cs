using System;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class RemoveZombieLaunchApplication : ActionFilter
    {
        public RemoveZombieLaunchApplication() : base("RemoveZombieLaunchApplication", ZappyTaskActionFilterType.Unary, false, "SystemAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = actions.Peek();
            if (string.Equals(action.AdditionalInfo, "Aggregated", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return action is LaunchApplicationAction;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            if (AbsorbActivitiesBeforeLaunchApplication.IfStartMenuSearchThenAggregate(actions))
            {
                actions.Peek().AdditionalInfo = "Aggregated";
                return false;
            }
            actions.Pop();
            return true;
        }
    }
}

