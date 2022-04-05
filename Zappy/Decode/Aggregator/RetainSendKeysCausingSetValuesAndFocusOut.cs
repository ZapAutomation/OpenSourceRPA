using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Mssa;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{


    internal class RetainSendKeysCausingSetValuesAndFocusOut : ActionFilter
    {
        private const string MayCauseFocusOut = "MayCauseFocusOut";

        public RetainSendKeysCausingSetValuesAndFocusOut() : base("RetainSendKeysCausingSetValuesAndFocusOut", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count <= 1)
            {
                return false;
            }
            SendKeysAction action = actions.Peek(1) as SendKeysAction;
            return action != null && action.ModifierKeys == ModifierKeys.None && !string.IsNullOrEmpty(action.Text) && action.Text.Value.EndsWith("{Tab}", StringKeys.Comparison) && AggregatorUtilities.IsActionOnControlType(action, ControlType.Edit);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            ZappyTaskAction firstAction = actions.Peek();
            SetValueAction secondAction = firstAction as SetValueAction;
            SendKeysAction action3 = actions.Peek(1) as SendKeysAction;
            if (secondAction != null)
            {
                if (AggregatorUtilities.AreActivitiesOnSameElement(action3, secondAction))
                {
                    actions.Pop(1);
                    SendKeysAction element = new SendKeysAction(actions.Peek().ActivityElement)
                    {
                        Text = "{Tab}",
                        Tags = { {
                            "MayCauseFocusOut",
                            "MayCauseFocusOut"
                        } }
                    };
                    actions.Push(element);
                    if (actions.Count > 2)
                    {
                        SetValueAction action5 = actions.Peek(2) as SetValueAction;
                        if (action5 != null && AggregatorUtilities.AreActivitiesOnSameElement(action5, action3))
                        {
                            actions.Pop(2);
                        }
                    }
                }
            }
            else if (action3.Tags.ContainsKey("MayCauseFocusOut") && (!(firstAction is SendKeysAction) || AggregatorUtilities.AreActivitiesOnSameElement(firstAction, action3)))
            {
                actions.Pop(1);
            }
            return false;
        }
    }
}

