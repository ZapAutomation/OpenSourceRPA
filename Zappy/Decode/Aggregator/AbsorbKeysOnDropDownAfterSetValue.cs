using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbKeysOnDropDownAfterSetValue : ActionFilter
    {
        public AbsorbKeysOnDropDownAfterSetValue() : base("AbsorbKeysOnDropDownAfterSetValue", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                SendKeysAction action = actions.Peek() as SendKeysAction;
                SetValueAction action2 = actions.Peek(1) as SetValueAction;
                if (AggregatorUtilities.IsActionOnControlType(action2, ControlType.DateTimePicker) && AggregatorUtilities.IsActionOnControlType(action, ControlType.Calendar) || AggregatorUtilities.IsActionOnControlType(action2, ControlType.ComboBox) && AggregatorUtilities.IsActionOnControlType(action, ControlType.ListItem) || AggregatorUtilities.IsActionOnControlType(action2, ControlType.Cell) && AggregatorUtilities.IsActionOnControlType(action, ControlType.ListItem))
                {
                    if (!StringKeys.Comparer.Equals("{Space}", action.Text) && !StringKeys.Comparer.Equals("{Enter}", action.Text))
                    {
                        return StringKeys.Comparer.Equals("{Escape}", action.Text);
                    }
                    return true;
                }
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            bool flag = false;
            SetValueAction action = actions.Peek(1) as SetValueAction;
            SendKeysAction action2 = actions.Peek() as SendKeysAction;
            if (AggregatorUtilities.IsActionOnControlType(action2, ControlType.ListItem))
            {
                if (AggregatorUtilities.IsActionOnControlType(action, ControlType.ComboBox))
                {
                    flag = RecorderUtilities.AncestorMatchedForCombo(action2, action);
                }
                else if (AggregatorUtilities.IsActionOnControlType(action, ControlType.Cell))
                {
                    ITaskActivityElement grandParentUsingQid = AggregatorUtilities.GetGrandParentUsingQid(action);
                    if (ControlType.Table.NameEquals(grandParentUsingQid.ControlTypeName))
                    {
                        flag = true;
                    }
                }
            }
            else
            {
                ZappyTaskAction tagFromAction = AggregatorUtilities.GetTagFromAction<ZappyTaskAction>(action, "SetValueCausedBy");
                flag = AggregatorUtilities.AreActivitiesOnSameElement(action2, tagFromAction);
            }
            if (flag)
            {
                actions.Pop();
                AggregatorUtilities.AddTagToAction(action, "SetValueCausedBy", action2);
            }
            return false;
        }
    }
}

