using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbSetBaseActionBeforeSetBaseAction : ActionFilter
    {
        public AbsorbSetBaseActionBeforeSetBaseAction() : base("AbsorbSetBaseActionBeforeSetBaseAction", ZappyTaskActionFilterType.Binary, true, "SetValueAggregators")
        {
        }

        private bool IsContextMenuAction(ref SetBaseAction action)
        {
            SetStateAction action2 = action as SetStateAction;
            if (action2 != null && action2.States == ControlStates.None)
            {
                action2.AdditionalInfo = "Aggregated";
                return true;
            }
            return false;
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count <= 1)
            {
                return false;
            }
            SetBaseAction firstAction = actions.Peek() as SetBaseAction;
            SetBaseAction secondAction = actions.Peek(1) as SetBaseAction;
            return AggregatorUtilities.AreActivitiesOnSameElement(firstAction, secondAction) && Equals(firstAction.GetType(), secondAction.GetType());
        }

        private static void MergeOrthogonalStates(SetBaseAction lastAction, SetBaseAction secondLastAction)
        {
            SetStateAction action = lastAction as SetStateAction;
            SetStateAction action2 = secondLastAction as SetStateAction;
            if (action != null && action2 != null)
            {
                ControlStates states = action.States;
                ControlStates states2 = action2.States;
                bool flag = (states & (ControlStates.Collapsed | ControlStates.Expanded)) > ControlStates.None;
                bool flag2 = (states2 & (ControlStates.Collapsed | ControlStates.Expanded)) > ControlStates.None;
                bool flag3 = (states & (ControlStates.Checked | ControlStates.Indeterminate | ControlStates.Normal | ControlStates.Pressed)) > ControlStates.None;
                bool flag4 = (states2 & (ControlStates.Checked | ControlStates.Indeterminate | ControlStates.Normal | ControlStates.Pressed)) > ControlStates.None;
                if (!flag & flag2)
                {
                    action.States = states | (states2 & (ControlStates.Collapsed | ControlStates.Expanded));
                }
                else if (!flag3 & flag4)
                {
                    action.States = states | (states2 & (ControlStates.Checked | ControlStates.Indeterminate | ControlStates.Normal | ControlStates.Pressed));
                }
            }
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            SetBaseAction action = actions.Peek() as SetBaseAction;
            SetBaseAction action2 = actions.Peek(1) as SetBaseAction;
            if (!IsContextMenuAction(ref action) && !IsContextMenuAction(ref action2))
            {
                actions.Pop(1);
                MergeOrthogonalStates(action, action2);
                bool flag = false;
                if (AggregatorUtilities.IsActionOnControlType(action, ControlType.Cell) && action.AdditionalInfo.Equals("Aggregated"))
                {
                    flag = true;
                }
                action.AdditionalInfo = "Aggregated";
                if (AggregatorUtilities.GetTagFromAction<ZappyTaskAction>(action, "SetValueCausedBy") == null)
                {
                    AggregatorUtilities.AddTagToAction(action, "SetValueCausedBy", AggregatorUtilities.GetTagFromAction<ZappyTaskAction>(action2, "SetValueCausedBy"));
                }
                if (!AggregatorUtilities.GetTagFromAction<bool>(action, "ShouldEatClickAction"))
                {
                    AggregatorUtilities.AddTagToAction(action, "ShouldEatClickAction", AggregatorUtilities.GetTagFromAction<bool>(action2, "ShouldEatClickAction"));
                }
                SetValueAction action3 = action2 as SetValueAction;
                if (action3 != null && (AggregatorUtilities.IsActionOnControlType(action2, ControlType.ComboBox) || AggregatorUtilities.IsActionOnControlType(action2, ControlType.Cell) && !flag))
                {
                    SetValueAction action4 = (SetValueAction)action;
                    action4.PreferEdit = action3.PreferEdit;
                }
            }
            return false;
        }
    }
}

