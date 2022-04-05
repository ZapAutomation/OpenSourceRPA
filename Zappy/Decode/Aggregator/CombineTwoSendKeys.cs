using System;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;

namespace Zappy.Decode.Aggregator
{
    internal class CombineTwoSendKeys : ActionFilter
    {
        public CombineTwoSendKeys() : base("CombineTwoSendKeys", ZappyTaskActionFilterType.Binary, true, "MiscellaneousAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                SendKeysAction action = actions.Peek() as SendKeysAction;
                SendKeysAction action2 = actions.Peek(1) as SendKeysAction;
                if (action != null && action2 != null)
                {
                    TaskActivityElement uIElement = action.ActivityElement;
                    TaskActivityElement element = action2.ActivityElement;
                    return uIElement != null && element != null && !string.Equals("DoNotAggregate", action.AdditionalInfo, StringComparison.OrdinalIgnoreCase) && !string.Equals("DoNotAggregate", action2.AdditionalInfo, StringComparison.OrdinalIgnoreCase) && (!string.Equals("ModifierKey", action.AdditionalInfo, StringComparison.OrdinalIgnoreCase) && !string.Equals("ModifierKey", action.AdditionalInfo, StringComparison.OrdinalIgnoreCase)) && action.ModifierKeys == ModifierKeys.None && action2.ModifierKeys == ModifierKeys.None && uIElement.Equals(element);
                }
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                SendKeysAction action = actions.Pop() as SendKeysAction;
                SendKeysAction action2 = actions.Peek() as SendKeysAction;
                if (action != null && action2 != null)
                {
                    action2.ValueAsString = action2.ValueAsString + action.ValueAsString;
                }
            }
            return false;
        }
    }
}

