using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.Mssa;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbActivitiesOnWPFDataGrid : ActionFilter
    {
        public AbsorbActivitiesOnWPFDataGrid() : base("AbsorbActivitiesOnWPFDataGrid", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count <= 1)
            {
                return false;
            }
            ZappyTaskAction action = actions.Peek();
            ZappyTaskAction action2 = actions.Peek(1);
            return action is SetValueAction && action2 is InputAction && AggregatorUtilities.IsActionOnControlType(action, ControlType.Cell) && action.ActivityElement != null && UITechnologyManager.AreEqual("UIA", action.ActivityElement.Framework);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            SetValueAction valueAction = actions.Peek() as SetValueAction;
            bool firstInputAction = true;
            ZappyTaskAction source = null;
            while (actions.Count > 1 && actions.Peek(1).ActivityElement != null)
            {
                bool flag2 = false;
                ZappyTaskAction inputAction = actions.Peek(1) as ZappyTaskAction;
                if (!inputAction.NeedFiltering || string.Equals(inputAction.AdditionalInfo, "DoNotAggregate", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                if (inputAction is InputAction || source != null && inputAction is SetValueAction)
                {
                    flag2 = AbsorbInputActivitiesBeforeSetValueOnDataGrid.ShouldEatInputAction(inputAction, valueAction);
                }
                if (!flag2)
                {
                    if (inputAction.ActivityElement.Equals(valueAction.ActivityElement) || actions.Count <= 2 || !(inputAction is MouseAction))
                    {
                        break;
                    }
                    ZappyTaskAction action4 = actions.Peek(2) as ZappyTaskAction;
                    if (action4 == null || !action4.ActivityElement.Equals(valueAction.ActivityElement) && !RecorderUtilities.AncestorMatchedForCombo(action4, valueAction))
                    {
                        break;
                    }
                    actions.Pop();
                    actions.Pop();
                    actions.Push(valueAction);
                    source = inputAction;
                }
                else
                {
                    SendKeysAction action5 = inputAction as SendKeysAction;
                    if (action5 != null)
                    {
                        string valueAsString = action5.ValueAsString;
                        action5.ValueAsString = string.Empty;
                        if (!string.IsNullOrEmpty(valueAsString))
                        {
                            if (valueAsString.EndsWith("{Enter}", StringKeys.Comparison))
                            {
                                action5.ValueAsString = "{Enter}";
                                source = action5;
                            }
                            else if (valueAsString.EndsWith("{Tab}", StringKeys.Comparison))
                            {
                                action5.ValueAsString = "{Tab}";
                                source = action5;
                            }
                        }
                    }
                    AbsorbInputActivitiesBeforeSetValueOnDataGrid.EatPreviousAction(actions, valueAction, inputAction, ref firstInputAction);
                }
            }
            if (source != null)
            {
                ZappyTaskAction element = Activator.CreateInstance(source.GetType()) as ZappyTaskAction;
                element.ShallowCopy(source as ZappyTaskAction, true);
                actions.Push(element);
            }
            return false;
        }
    }
}

