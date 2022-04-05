using System;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using DragAction = Zappy.ZappyActions.AutomaticallyCreatedActions.DragAction;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbInputActivitiesBeforeSetValue : ActionFilter
    {
        public AbsorbInputActivitiesBeforeSetValue() : base("AbsorbInputActivitiesBeforeSetValue", ZappyTaskActionFilterType.Binary, false, "SetValueAggregators")
        {
        }

        private static bool AncestorMatched(ZappyTaskAction inputAction, SetValueAction valueAction)
        {
            if (AggregatorUtilities.IsActionOnControlType(valueAction, ControlType.List) || AggregatorUtilities.IsActionOnControlType(valueAction, ControlType.Slider))
            {
                TaskActivityElement parent = RecorderUtilities.GetParent(inputAction.ActivityElement);
                if (valueAction.ActivityElement.Equals(parent))
                {
                    return true;
                }
            }
            if (AggregatorUtilities.IsActionOnControlType(valueAction, ControlType.ComboBox))
            {
                return RecorderUtilities.AncestorMatchedForCombo(inputAction, valueAction);
            }
            if (!AggregatorUtilities.IsIE8AddressEditBox(valueAction.ActivityElement))
            {
                return false;
            }
            MouseAction action = inputAction as MouseAction;
            if (inputAction == null)
            {
                return false;
            }
            if (!AggregatorUtilities.IsIE8NavigationBar(inputAction.ActivityElement))
            {
                return AggregatorUtilities.IsIE8AddresBarButton(inputAction.ActivityElement);
            }
            return true;
        }

        private static bool AreActivitiesOnIEAddressDropDown(ZappyTaskAction inputAction, SetValueAction valueAction) =>
            valueAction != null && AggregatorUtilities.IsIEAddressBoxControl(valueAction.ActivityElement) && AggregatorUtilities.IsIEAddressDropDown(inputAction.ActivityElement);

        private static bool CompareUris(string inputString1, string inputString2)
        {
                                                                                                                        
                        return false;
        }

        private static bool IsActionNotIgnorableOnList(ZappyTaskAction uiTaskAction)
        {
            if (ZappyTaskUtilities.IsRemoteTestingEnabled)
            {
                return false;
            }
            MouseAction action = uiTaskAction as MouseAction;
            DragAction action2 = action == null ? uiTaskAction as DragAction : null;
            return action != null && (action.MouseButton == MouseButtons.Right || action.ActionType == MouseActionType.DoubleClick) || action2 != null && action2.MouseButton == MouseButtons.Right;
        }

        private static bool IsActionSendKeysOfEnter(ZappyTaskAction inputAction)
        {
            SendKeysAction action = inputAction as SendKeysAction;
            return action != null && action.Text != null && action.Text.Value.EndsWith("{Enter}", StringKeys.Comparison);
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                ZappyTaskAction action = actions.Peek();
                ZappyTaskAction uiTaskAction = actions.Peek(1);
                if (action is SetBaseAction && (uiTaskAction is InputAction || uiTaskAction is DragDropAction || uiTaskAction is InvokeAction))
                {
                    SetValueAction setValueAction = action as SetValueAction;
                    if (!AggregatorUtilities.IsActionOnControlType(action, ControlType.TreeItem) && !AggregatorUtilities.IsActionOnControlType(action, ControlType.CheckBoxTreeItem) && !IsNotIgnorableClickOnList(setValueAction, uiTaskAction))
                    {
                        return !AggregatorUtilities.IsActionOnControlType(uiTaskAction, ControlType.TreeItem) && !AggregatorUtilities.IsActionOnControlType(uiTaskAction, ControlType.CheckBoxTreeItem);
                    }
                }
            }
            return false;
        }

        private static bool IsModifierClickOnCheckBox(SetStateAction setStateAction, MouseAction mouseAction)
        {
            if (setStateAction != null && mouseAction != null)
            {
                TaskActivityElement element = setStateAction.SourceElement == null ? setStateAction.ActivityElement : setStateAction.SourceElement;
                if (mouseAction.ModifierKeys != ModifierKeys.None && AggregatorUtilities.IsLeftClickOrDoubleClick(mouseAction, true) && AggregatorUtilities.IsActionOnControlType(setStateAction, ControlType.CheckBox) && AggregatorUtilities.AreActivitiesOnSameElement(mouseAction, setStateAction))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsNavigationOnDataGrid(ZappyTaskAction zinputAction, SetBaseAction setBaseAction)
        {
            ZappyTaskAction inputAction = zinputAction as ZappyTaskAction;
            if (!(setBaseAction is SetValueAction) || !AggregatorUtilities.IsKeyNavigationAction(inputAction) || !ControlType.Cell.NameEquals(setBaseAction.ActivityElement.ControlTypeName) || ControlType.Cell.NameEquals(inputAction.ActivityElement.ControlTypeName) || ControlType.ListItem.NameEquals(inputAction.ActivityElement.ControlTypeName) || IsNavigationOnDatagridEditableComboBox(inputAction))
            {
                return false;
            }
            return !ControlType.ComboBox.NameEquals(inputAction.ActivityElement.ControlTypeName) || inputAction == null || inputAction.ValueAsString == null || !inputAction.ValueAsString.EndsWith("{Up}", StringComparison.OrdinalIgnoreCase) && !inputAction.ValueAsString.EndsWith("{Down}", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNavigationOnDatagridEditableComboBox(ZappyTaskAction inputAction)
        {
            if (inputAction == null || inputAction.ActivityElement == null || !inputAction.ValueAsString.EndsWith("{Up}", StringComparison.OrdinalIgnoreCase) && !inputAction.ValueAsString.EndsWith("{Down}", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return AggregatorUtilities.IsWinformsDataGridEditingComboboxCell(inputAction, true);
        }

        private static bool IsNavigationToLastDataGridRow(ZappyTaskAction inputAction, SetBaseAction setBaseAction)
        {
            TaskActivityElement uIElement = setBaseAction.ActivityElement;
            if (!AggregatorUtilities.IsKeyNavigationAction(inputAction) || !ControlType.Cell.NameEquals(setBaseAction.ActivityElement.ControlTypeName) || !ControlType.Cell.NameEquals(inputAction.ActivityElement.ControlTypeName) || uIElement.QueryId.Ancestor == null || !ControlType.Row.NameEquals(uIElement.QueryId.Ancestor.ControlTypeName) || uIElement.QueryId.Ancestor.QueryId.Ancestor == null || !ControlType.Table.NameEquals(uIElement.QueryId.Ancestor.QueryId.Ancestor.ControlTypeName))
            {
                return false;
            }
            object propertyValue = uIElement.QueryId.Ancestor.QueryId.Condition.GetPropertyValue("Value");
            object obj3 = uIElement.QueryId.Ancestor.QueryId.Ancestor.QueryId.Condition.GetPropertyValue("Name");
            return propertyValue != null && string.Equals(propertyValue.ToString(), "(Create New)", StringComparison.OrdinalIgnoreCase) && obj3 != null && !string.Equals(obj3.ToString(), "DataGridView", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNotIgnorableClickOnList(SetValueAction setValueAction, ZappyTaskAction uiTaskAction)
        {
            if (!AggregatorUtilities.IsActionOnControlType(setValueAction, ControlType.List) || !IsActionNotIgnorableOnList(uiTaskAction) || !AggregatorUtilities.IsSecondActionElementSameAsSourceOfFirst(setValueAction, uiTaskAction) && !AggregatorUtilities.AreActivitiesOnSameElement(uiTaskAction, setValueAction))
            {
                return false;
            }
            return true;
        }

        private static bool NeedToSwapEnter(SetValueAction previousSetValue, ZappyTaskAction inputAction, ZappyTaskAction valueAction, ref bool needAggregation)
        {
            if (!inputAction.Tags.ContainsKey(RecorderUtilities.ImeLanguageTag))
            {
                if (inputAction.ActivityElement.Framework != "UiaWidget" && NeedToSwapEnterOnEdit(previousSetValue, inputAction, valueAction))
                {
                    return true;
                }
                if (AggregatorUtilities.IsIEAddressTextBox(inputAction.ActivityElement) && AggregatorUtilities.IsIEAddressBoxControl(valueAction.ActivityElement))
                {
                    needAggregation = true;
                    return true;
                }
            }
            return false;
        }

        private static bool NeedToSwapEnterOnEdit(SetValueAction previousSetValue, ZappyTaskAction inputAction, ZappyTaskAction valueAction) =>
            AggregatorUtilities.IsActionOnControlType(valueAction, ControlType.Edit) && AggregatorUtilities.AreActivitiesOnSameElement(inputAction, valueAction) && AggregatorUtilities.AreActivitiesOnSameElement(previousSetValue, valueAction) && string.Equals(previousSetValue.ValueAsString, valueAction.ValueAsString, StringComparison.Ordinal);

        private static bool NeedToSwapEnterOnIEAddressBoxIme(SetValueAction previousSetValue, ZappyTaskAction inputAction, ZappyTaskAction valueAction) =>
            inputAction.Tags.ContainsKey(RecorderUtilities.ImeLanguageTag) && AggregatorUtilities.IsIEAddressTextBox(inputAction.ActivityElement) && AggregatorUtilities.IsIEAddressBoxControl(valueAction.ActivityElement) && AggregatorUtilities.AreActivitiesOnSameElement(previousSetValue, valueAction) && CompareUris(previousSetValue.ValueAsString, valueAction.ValueAsString);

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            bool flag = false;
            bool needAggregation = false;
            SetValueAction secondAction = actions.Peek() as SetValueAction;
            SetStateAction setStateAction = actions.Peek() as SetStateAction;
            SetBaseAction setBaseAction = actions.Peek() as SetBaseAction;
            MouseAction mouseAction = actions.Peek(1) as MouseAction;
            if (IsModifierClickOnCheckBox(setStateAction, mouseAction))
            {
                mouseAction.NeedFiltering = false;
                return false;
            }
            ZappyTaskAction firstAction = actions.Peek(1);
            if (!AggregatorUtilities.IsWindows7OrHigher && AggregatorUtilities.AreActivitiesOnSameElement(firstAction, secondAction) && AbsorbActivitiesBeforeLaunchApplication.IsActionOnSearchBoxOnStartMenu(firstAction))
            {
                SendKeysAction source = firstAction as SendKeysAction;
                if (source != null && !string.IsNullOrEmpty(source.Text) && source.Text.Value.EndsWith("{Enter}", StringComparison.OrdinalIgnoreCase))
                {
                    source.Text = "{Enter}";
                    source.NeedFiltering = false;
                    setBaseAction.AdditionalInfo = "Aggregated";
                    actions.Pop();
                    actions.Pop();
                    SetValueAction action7 = new SetValueAction(secondAction.ActivityElement, secondAction.ValueAsString);
                    SendKeysAction action8 = new SendKeysAction();
                    action8.ShallowCopy(source, true);
                    actions.Push(action7);
                    actions.Push(action8);
                    return true;
                }
            }
            bool flag3 = true;
            while (actions.Count > 1 && actions.Peek(1).ActivityElement != null)
            {
                bool flag4 = false;
                ZappyTaskAction inputAction = actions.Peek(1) as ZappyTaskAction;
                if (!inputAction.NeedFiltering || string.Equals(inputAction.AdditionalInfo, "DoNotAggregate", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                MouseAction action10 = inputAction as MouseAction;
                if (flag3 && AggregatorUtilities.IsActionOnControlType(secondAction, ControlType.ComboBox) && secondAction.SourceElement != null && ControlType.Edit.NameEquals(secondAction.SourceElement.ControlTypeName) && (Equals(inputAction.ActivityElement, secondAction.SourceElement) || RecorderUtilities.AncestorMatchedForCombo(inputAction, secondAction)) && !AggregatorUtilities.IsVerticalKeyNavigationAction(inputAction) && (action10 == null || action10.ActionType != MouseActionType.WheelRotate))
                {
                    secondAction.PreferEdit = true;
                }
                if (AggregatorUtilities.IsActionOnControlType(secondAction, ControlType.Cell) && inputAction is SendKeysAction && AggregatorUtilities.IsActionOnControlType(inputAction, ControlType.Edit) && AggregatorUtilities.MatchesWinformsEditingControl(inputAction.ActivityElement.Name))
                {
                    ITaskActivityElement grandParentUsingQid = AggregatorUtilities.GetGrandParentUsingQid(inputAction);
                    if (grandParentUsingQid != null && ControlType.ComboBox.NameEquals(grandParentUsingQid.ControlTypeName))
                    {
                        secondAction.PreferEdit = true;
                    }
                }
                SetValueAction previousSetValue = null;
                if (actions.Count > 2)
                {
                    previousSetValue = actions.Peek(2) as SetValueAction;
                }
                bool flag5 = false;
                if (inputAction is InputAction || inputAction is InvokeAction || inputAction is DragDropAction)
                {
                    MouseAction action12 = inputAction as MouseAction;
                    if (action12 != null && action12.ActionType == MouseActionType.Hover)
                    {
                        flag4 = false;
                    }
                    else if (action12 != null && action12.ActionType == MouseActionType.DoubleClick && AggregatorUtilities.IsActionOnControlType(inputAction, ControlType.ListItem))
                    {
                        flag4 = false;
                    }
                    else if (flag3 && IsActionSendKeysOfEnter(inputAction) && (NeedToSwapEnter(previousSetValue, inputAction, secondAction, ref needAggregation) || (needAggregation = NeedToSwapEnterOnIEAddressBoxIme(previousSetValue, inputAction, secondAction))))
                    {
                        flag4 = true;
                        flag = true;
                    }
                    else if (AreActivitiesOnIEAddressDropDown(inputAction, secondAction))
                    {
                        flag4 = true;
                    }
                    else if (ShouldAbsorbAction(inputAction, setBaseAction))
                    {
                        if (IsNavigationOnDataGrid(inputAction, setBaseAction))
                        {
                            inputAction.AdditionalInfo = "DoNotAggregate";
                        }
                        else if (AggregatorUtilities.DoesSendKeysStartWithEnter(inputAction) && AggregatorUtilities.IsActionOnControlType(inputAction, ControlType.Edit) && RecorderUtilities.AncestorMatchedForCombo(inputAction, secondAction))
                        {
                            inputAction.AdditionalInfo = "DoNotAggregate";
                            ((SendKeysAction)inputAction).Text = "{Enter}";
                            secondAction.AdditionalInfo = "Aggregated";
                        }
                        else
                        {
                            flag4 = true;
                            if (actions.Count > 2)
                            {
                                flag5 = ShouldRemoveListExpandActionInDatagridCell(actions.Peek(2) as ZappyTaskAction, inputAction, setBaseAction);
                            }
                        }
                    }
                    else if (setStateAction == null && AncestorMatched(inputAction, secondAction))
                    {
                        flag4 = true;
                    }
                }
                if (!flag4)
                {
                    break;
                }
                actions.Pop(1);
                if (flag5)
                {
                    actions.Pop(1);
                }
                if (flag3)
                {
                    setBaseAction.AdditionalInfo = "Aggregated";
                    AggregatorUtilities.AddTagToAction(setBaseAction, "SetValueCausedBy", inputAction);
                    AggregatorUtilities.TagActionWithEatClick(mouseAction, setBaseAction);
                    flag3 = false;
                }
            }
            if (flag)
            {
                SetValueAction action14 = null;
                if (actions.Count > 1)
                {
                    action14 = actions.Peek(1) as SetValueAction;
                }
                if (action14 != null && action14.ActivityElement.QueryId == secondAction.ActivityElement.QueryId)
                {
                    actions.Pop(1);
                }
                SendKeysAction element = new SendKeysAction(secondAction.ActivityElement)
                {
                    Text = "{Enter}",
                    NeedFiltering = needAggregation
                };
                actions.Push(element);
                secondAction.NeedFiltering = needAggregation;
            }
            return false;
        }

        private static bool ShouldAbsorbAction(ZappyTaskAction inputAction, SetBaseAction setBaseAction)
        {
            TaskActivityElement uIElement = inputAction.ActivityElement;
            if (!IsNavigationToLastDataGridRow(inputAction, setBaseAction) && (!uIElement.Equals(setBaseAction.ActivityElement) && !uIElement.Equals(setBaseAction.SourceElement) && !AggregatorUtilities.IsSourceInputAction(inputAction, setBaseAction)) && (!ControlType.ComboBox.NameEquals(setBaseAction.ActivityElement.ControlTypeName) || !AggregatorUtilities.IsActionOnControlType(inputAction, ControlType.ListItem) || !RecorderUtilities.AncestorMatchedForCombo(inputAction, setBaseAction as SetValueAction)) && (!AggregatorUtilities.IsElementUnderWin32MonthCalendar(uIElement) || !ControlType.DateTimePicker.NameEquals(setBaseAction.ActivityElement.ControlTypeName) && !ControlType.Calendar.NameEquals(setBaseAction.ActivityElement.ControlTypeName)))
            {
                return ShouldAbsorbActionForWpfDatePickerCalendar(inputAction, setBaseAction);
            }
            return true;
        }

        private static bool ShouldAbsorbActionForWpfDatePickerCalendar(ZappyTaskAction inputAction, SetBaseAction setBaseAction)
        {
            TaskActivityElement uIElement = inputAction.ActivityElement;
            if ((!ControlType.Calendar.NameEquals(setBaseAction.ActivityElement.ControlTypeName) || uIElement.QueryId.Ancestor == null || !setBaseAction.ActivityElement.Equals(uIElement.QueryId.Ancestor)) && (!ControlType.DatePicker.NameEquals(setBaseAction.ActivityElement.ControlTypeName) || uIElement.QueryId.Ancestor == null || uIElement.QueryId.Ancestor.ControlTypeName != ControlType.Calendar && uIElement.QueryId.Ancestor.ControlTypeName != ControlType.DatePicker))
            {
                return false;
            }
            return true;
        }

        private static bool ShouldRemoveListExpandActionInDatagridCell(ZappyTaskAction previousInputAction, ZappyTaskAction inputAction, SetBaseAction setBaseAction)
        {
            if (previousInputAction != null && inputAction != null && setBaseAction != null && previousInputAction.ActivityElement != null && inputAction.ActivityElement != null && setBaseAction.ActivityElement != null && ControlType.Button.NameEquals(previousInputAction.ActivityElement.ControlTypeName) && ControlType.ListItem.NameEquals(inputAction.ActivityElement.ControlTypeName) && ControlType.Cell.NameEquals(setBaseAction.ActivityElement.ControlTypeName))
            {
                TaskActivityElement parent = RecorderUtilities.GetParent(previousInputAction.ActivityElement);
                TaskActivityElement uIElement = inputAction.ActivityElement;
                for (int i = 0; i < 3; i++)
                {
                    uIElement = RecorderUtilities.GetParent(uIElement);
                    if (uIElement == null)
                    {
                        break;
                    }
                    if (uIElement.Equals(parent))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

