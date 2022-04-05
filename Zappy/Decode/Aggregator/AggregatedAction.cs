using System;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.Decode.Aggregator
{
    [Serializable]
    public abstract class AggregatedAction : ZappyTaskAction
    {
        protected AggregatedAction()
        {
        }

        protected AggregatedAction(TaskActivityElement uiElement) : base(uiElement)
        {
        }
    }
}