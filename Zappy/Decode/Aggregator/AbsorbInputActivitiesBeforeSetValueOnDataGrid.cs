using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbInputActivitiesBeforeSetValueOnDataGrid : ActionFilter
    {
        public AbsorbInputActivitiesBeforeSetValueOnDataGrid() : base("AbsorbInputActivitiesBeforeSetValueOnDataGrid", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }

        internal static void EatPreviousAction(ZappyTaskActionStack actions, SetValueAction valueAction, ZappyTaskAction inputAction, ref bool firstInputAction)
        {
            actions.Pop(1);
            if (firstInputAction)
            {
                valueAction.AdditionalInfo = "Aggregated";
                AggregatorUtilities.AddTagToAction(valueAction, "SetValueCausedBy", inputAction);
                firstInputAction = false;
            }
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count <= 1)
            {
                return false;
            }
            ZappyTaskAction action = actions.Peek();
            ZappyTaskAction action2 = actions.Peek(1);
            return action is SetValueAction && (action2 is InputAction || action2 is DragDropAction) && (AggregatorUtilities.IsActionOnControlType(action, ControlType.ComboBox) || AggregatorUtilities.IsActionOnControlType(action, ControlType.DateTimePicker) || AggregatorUtilities.IsActionOnControlType(action, ControlType.Calendar)) && AggregatorUtilities.MatchesWinformsEditingControl(action.ActivityElement.Name);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            SetValueAction valueAction = actions.Peek() as SetValueAction;
            bool firstInputAction = true;
            while (actions.Count > 1 && actions.Peek(1).ActivityElement != null)
            {
                bool flag2 = false;
                ZappyTaskAction inputAction = actions.Peek(1) as ZappyTaskAction;
                if (!inputAction.NeedFiltering || string.Equals(inputAction.AdditionalInfo, "DoNotAggregate", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                if (inputAction is InputAction || inputAction is DragDropAction)
                {
                    if (ShouldEatInputAction(inputAction, valueAction))
                    {
                        flag2 = true;
                    }
                    else if (AggregatorUtilities.IsElementUnderWin32MonthCalendar(inputAction.ActivityElement) && (ControlType.DateTimePicker.NameEquals(valueAction.ActivityElement.ControlTypeName) || ControlType.Calendar.NameEquals(valueAction.ActivityElement.ControlTypeName)))
                    {
                        flag2 = true;
                    }
                }
                if (!flag2)
                {
                    break;
                }
                EatPreviousAction(actions, valueAction, inputAction, ref firstInputAction);
            }
            return false;
        }

        internal static bool ShouldEatInputAction(ZappyTaskAction inputAction, SetValueAction valueAction)
        {
            MouseAction action = inputAction as MouseAction;
            bool flag = false;
            if (action != null && action.ActionType == MouseActionType.Hover)
            {
                return false;
            }
            if (inputAction.ActivityElement.Equals(valueAction.ActivityElement))
            {
                return true;
            }
            if (inputAction.ActivityElement.Equals(valueAction.SourceElement))
            {
                return true;
            }
            if (RecorderUtilities.AncestorMatchedForCombo(inputAction, valueAction))
            {
                flag = true;
            }
            return flag;
        }
    }
}

