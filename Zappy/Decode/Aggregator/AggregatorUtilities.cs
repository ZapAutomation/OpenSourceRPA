using System;
using System.Collections.Generic;
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
    internal static class AggregatorUtilities
    {
        internal const string AggregateIfLaunched = "AggregateIfLaunched";
        private const string AutoSuggestDropDown = "Auto-Suggest Dropdown";
        private const string AutoSuggestList = "SysListView32";
        private const string ContextMenuClassName = "#32768";
        internal const string DatagridCreateNewRowValue = "(Create New)";
        internal const string DatagridViewTableName = "DataGridView";
        private const string DeskTopClassName = "#32769";
        private const string DragActionName = "DragAction";
        private const string DTPMonthCalendarClassName = "SysMonthCal32";
        private const string ie8AddressBarButtonClassName = "ToolbarWindow32";
        private const string ie8AddressBarEditGrandParentClassName = "Address Band Root";
        private const string ie8DropDownGrandParentClassName = "DirectUIHWND";
        private const string IEFrameClassname = "IEFrame";
        private static bool? isWindows7;
        internal const string SetValueCausedBy = "SetValueCausedBy";
        internal const string ShouldEatClickAction = "ShouldEatClickAction";

        public static void AddTagToAction(ZappyTaskAction zaction, string key, object value)
        {
            if (zaction is ZappyTaskAction)
            {
                ZappyTaskAction action = zaction as ZappyTaskAction;

                if (action.Tags.ContainsKey(key))
                {
                    action.Tags.Remove(key);
                }
                action.Tags.Add(key, value);
            }

        }

        public static bool AreActivitiesOnSameElement(ZappyTaskAction zfirstAction, ZappyTaskAction zsecondAction)
        {
            if (zfirstAction is ZappyTaskAction && zsecondAction is ZappyTaskAction)
            {
                ZappyTaskAction firstAction = zfirstAction as ZappyTaskAction;

                ZappyTaskAction secondAction = zsecondAction as ZappyTaskAction;

                return IsSourceInputAction(firstAction, secondAction as SetBaseAction) || IsSourceInputAction(secondAction, firstAction as SetBaseAction) || firstAction != null && secondAction != null && firstAction.ActivityElement != null && secondAction.ActivityElement != null && firstAction.ActivityElement.Equals(secondAction.ActivityElement);
            }
            return false;
        }

        public static bool DoesSendKeysEndWithEnter(ZappyTaskAction action)
        {
            SendKeysAction action2 = action as SendKeysAction;
            return action2 != null && action2.ModifierKeys == ModifierKeys.None && action2.Text != null && action2.Text.Value.EndsWith("{Enter}", StringKeys.Comparison);
        }

        public static bool DoesSendKeysStartWithEnter(ZappyTaskAction action)
        {
            SendKeysAction action2 = action as SendKeysAction;
            return action2 != null && action2.ModifierKeys == ModifierKeys.None && action2.Text != null && action2.Text.Value.StartsWith("{Enter}", StringKeys.Comparison);
        }

        public static ITaskActivityElement GetGrandParentUsingQid(ZappyTaskAction zaction)
        {
            if (zaction is ZappyTaskAction)
            {
                ZappyTaskAction action = zaction as ZappyTaskAction;

                ITaskActivityElement ancestor = null;
                ITaskActivityElement element2 = null;
                if (action.ActivityElement != null && action.ActivityElement.QueryId != null)
                {
                    ancestor = action.ActivityElement.QueryId.Ancestor;
                }
                if (ancestor != null && ancestor.QueryId != null)
                {
                    element2 = ancestor.QueryId.Ancestor;
                }
                return element2;
            }
            return null;
        }

        public static string GetNativeWindowText(ITaskActivityElement element)
        {
            try
            {
                object propertyValue = element.GetPropertyValue("WindowText");
                if (propertyValue != null)
                {
                    return propertyValue.ToString();
                }
            }
            catch (NotSupportedException)
            {
            }
            catch (NotImplementedException)
            {
            }
            return NativeMethods.GetWindowText(element.WindowHandle);
        }

        public static T GetTagFromAction<T>(ZappyTaskAction zaction, string key)
        {
            if (zaction is ZappyTaskAction)
            {
                ZappyTaskAction action = zaction as ZappyTaskAction;

                if (action.Tags.ContainsKey(key))
                {
                    return (T)action.Tags[key];
                }
            }

            return default(T);
        }

        public static bool IsActionOnControlType(ZappyTaskAction zaction, ControlType controlType)
        {
            if (zaction is ZappyTaskAction)
            {
                ZappyTaskAction action = zaction as ZappyTaskAction;
                return action != null && action.ActivityElement != null && controlType.NameEquals(action.ActivityElement.ControlTypeName);

            }
            else
                return false;

        }

        public static bool IsActionOnIEAddressBar(ZappyTaskAction action) =>
            action != null && action.ActivityElement != null && LocalizedSystemStrings.Instance.MatchIEAddressBoxNameText(action.ActivityElement.Name);

        public static bool IsElementUnderContextMenu(TaskActivityElement element)
        {
            if (element != null && !string.IsNullOrEmpty(element.ClassName))
            {
                TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(element);
                if (element2 != null && !string.IsNullOrEmpty(element2.ClassName) && element2.ClassName.StartsWith("#32768", StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsElementUnderWin32MonthCalendar(TaskActivityElement element)
        {
            if (element != null && !string.IsNullOrEmpty(element.ClassName))
            {
                TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(element);
                if (element2 != null && !string.IsNullOrEmpty(element2.ClassName) && (element.ClassName.StartsWith("SysMonthCal32", StringComparison.Ordinal) || element2.ClassName.StartsWith("#32768", StringComparison.Ordinal) || element2.ClassName.StartsWith("SysMonthCal32", StringComparison.Ordinal)))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsIE8AddresBarButton(TaskActivityElement element) =>
            string.Equals(element.ClassName, "ToolbarWindow32") && ControlType.MenuItem.NameEquals(element.ControlTypeName) && string.Equals(element.Name, LocalizedSystemStrings.Instance.IE8AddressBarButtonName, StringComparison.Ordinal);

        private static bool IsIE8AddressDropDownParent(TaskActivityElement parent)
        {
            TaskActivityElement element = RecorderUtilities.GetParent(parent);
            return element != null && string.Equals(element.ClassName, "DirectUIHWND", StringComparison.Ordinal);
        }

        internal static bool IsIE8AddressEditBox(TaskActivityElement element)
        {
            if (element == null || !ControlType.Edit.NameEquals(element.ControlTypeName))
            {
                return false;
            }
            TaskActivityElement parent = RecorderUtilities.GetParent(RecorderUtilities.GetParent(element));
            return parent != null && string.Equals(parent.ClassName, "Address Band Root", StringComparison.Ordinal);
        }

        internal static bool IsIE8NavigationBar(TaskActivityElement element) =>
            element != null && string.Equals(element.ClassName, "AddressDisplay Control") && ControlType.Client.NameEquals(element.ControlTypeName);

        public static bool IsIEAddressBoxControl(TaskActivityElement element)
        {
            if (element == null || !LocalizedSystemStrings.Instance.MatchIEAddressBoxNameText(element.Name) || !ControlType.ComboBox.NameEquals(element.ControlTypeName) && !IsIE8AddressEditBox(element))
            {
                return false;
            }
            return IsTopLevelElementAnIEWindow(element);
        }

        public static bool IsIEAddressComboBoxButton(TaskActivityElement element)
        {
            if ((element == null || !ControlType.Button.NameEquals(element.ControlTypeName) || !LocalizedSystemStrings.Instance.MatchIEAddressBoxNameText(element.Name)) && !IsIE8AddresBarButton(element))
            {
                return false;
            }
            return IsTopLevelElementAnIEWindow(element);
        }

        public static bool IsIEAddressDropDown(TaskActivityElement element)
        {
            if (element != null && ControlType.ListItem.NameEquals(element.ControlTypeName))
            {
                TaskActivityElement parent = RecorderUtilities.GetParent(element);
                if (parent != null && ControlType.List.NameEquals(parent.ControlTypeName))
                {
                    if (LocalizedSystemStrings.Instance.MatchIEAddressBoxNameText(parent.Name))
                    {
                        return IsTopLeveElementDesktop(element);
                    }
                    if (string.Equals(parent.ClassName, "SysListView32", StringComparison.Ordinal))
                    {
                        TaskActivityElement element3 = FrameworkUtilities.TopLevelElement(element);
                        return element3 != null && string.Equals(element3.ClassName, "Auto-Suggest Dropdown", StringComparison.Ordinal);
                    }
                    if (IsIE8AddressDropDownParent(parent))
                    {
                        if (!IsTopLeveElementDesktop(element))
                        {
                            return IsTopLeveElementIE9DropDownTopLevel(element);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsIEAddressTextBox(TaskActivityElement element) =>
            element != null && ControlType.Edit.NameEquals(element.ControlTypeName) && LocalizedSystemStrings.Instance.MatchIEAddressBoxNameText(element.Name) && IsTopLevelElementAnIEWindow(element);

        public static bool IsKeyNavigationAction(ZappyTaskAction inputAction)
        {
            SendKeysAction action = inputAction as SendKeysAction;
            if (action == null || action.ValueAsString == null)
            {
                return false;
            }
            if (!action.ValueAsString.EndsWith("{Tab}", StringKeys.Comparison) && !action.ValueAsString.EndsWith("{Right}", StringKeys.Comparison) && !action.ValueAsString.EndsWith("{Left}", StringKeys.Comparison) && !action.ValueAsString.EndsWith("{Down}", StringKeys.Comparison) && !action.ValueAsString.EndsWith("{Up}", StringKeys.Comparison) && !action.ValueAsString.EndsWith("{PageDown}", StringKeys.Comparison) && !action.ValueAsString.EndsWith("{PageUp}", StringKeys.Comparison))
            {
                return action.ValueAsString.EndsWith("{Enter}", StringKeys.Comparison);
            }
            return true;
        }

        public static bool IsLeftClick(ZappyTaskAction action)
        {
            MouseAction action2 = action as MouseAction;
            return action2 != null && action2.ModifierKeys == ModifierKeys.None && action2.MouseButton == MouseButtons.Left && action2.ActionType == MouseActionType.Click;
        }

        public static bool IsLeftClickOrDoubleClick(ZappyTaskAction action) =>
            IsLeftClickOrDoubleClick(action, false);

        public static bool IsLeftClickOrDoubleClick(ZappyTaskAction action, bool ignoreModifierKeys)
        {
            MouseAction action2 = action as MouseAction;
            if (action2 != null && (ignoreModifierKeys || !ignoreModifierKeys && action2.ModifierKeys == ModifierKeys.None) && action2.MouseButton == MouseButtons.Left)
            {
                if (action2.ActionType != MouseActionType.Click)
                {
                    return action2.ActionType == MouseActionType.DoubleClick;
                }
                return true;
            }
            return false;
        }

        public static bool IsLeftDoubleClick(ZappyTaskAction action)
        {
            MouseAction action2 = action as MouseAction;
            return action2 != null && action2.ModifierKeys == ModifierKeys.None && action2.MouseButton == MouseButtons.Left && action2.ActionType == MouseActionType.DoubleClick;
        }

        public static bool IsMiddleClick(ZappyTaskAction action)
        {
            MouseAction action2 = action as MouseAction;
            return action2 != null && action2.MouseButton == MouseButtons.Middle && action2.ActionType == MouseActionType.Click;
        }

        public static bool IsRightClick(ZappyTaskAction action)
        {
            MouseAction action2 = action as MouseAction;
            return action2 != null && action2.ModifierKeys == ModifierKeys.None && action2.MouseButton == MouseButtons.Right && action2.ActionType == MouseActionType.Click;
        }

        public static bool IsSecondActionElementSameAsSourceOfFirst(SetValueAction firstAction, ZappyTaskAction zsecondAction)
        {
            if (zsecondAction is ZappyTaskAction)
            {
                ZappyTaskAction secondAction = zsecondAction as ZappyTaskAction;
                return
firstAction != null && secondAction != null && firstAction.SourceElement != null && secondAction.ActivityElement != null && firstAction.SourceElement.Equals(secondAction.ActivityElement);

            }
            return false;
        }

        public static bool IsSendKeysOfEnter(ZappyTaskAction action)
        {
            SendKeysAction action2 = action as SendKeysAction;
            return action2 != null && action2.ModifierKeys == ModifierKeys.None && StringKeys.Comparer.Equals("{Enter}", action2.Text);
        }

        internal static bool IsSendKeysOfUp(ZappyTaskAction action)
        {
            SendKeysAction action2 = action as SendKeysAction;
            return action2 != null && action2.ModifierKeys == ModifierKeys.None && StringKeys.Comparer.Equals("{Up}", action2.Text);
        }

        internal static bool IsSourceInputAction(ZappyTaskAction zaction, SetBaseAction setbaseAction)
        {
            if (zaction is ZappyTaskAction)
            {
                ZappyTaskAction inputAction = zaction as ZappyTaskAction;
                if ((inputAction is InputAction || inputAction is InvokeAction) && (inputAction != null && setbaseAction != null && setbaseAction.ActivityElement != null) && inputAction.ActivityElement != null && setbaseAction.SourceElements != null)
                {
                    List<TaskActivityElement> sourceElements = setbaseAction.SourceElements;
                    if (setbaseAction.ActivityElement.Equals(inputAction.ActivityElement) || sourceElements.Contains(inputAction.ActivityElement))
                    {
                        return true;
                    }
                    InvokeAction action = inputAction as InvokeAction;
                    if (action != null && (setbaseAction.ActivityElement.Equals(action.SourceElement) || sourceElements.Contains(action.SourceElement)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsTopLeveElementDesktop(TaskActivityElement element)
        {
            TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(element);
            return element2 != null && string.Equals(element2.ClassName, "#32769", StringComparison.Ordinal);
        }

        private static bool IsTopLeveElementIE9DropDownTopLevel(TaskActivityElement element)
        {
            TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(element);
            return element2 != null && string.Equals(element2.ClassName, "DirectUIHWND");
        }

        public static bool IsTopLevelElementAnIEWindow(TaskActivityElement element)
        {
            if (element == null)
            {
                return false;
            }
            TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(element);
            return element2 != null && string.Equals(element2.ClassName, "IEFrame", StringComparison.Ordinal);
        }

        public static bool IsVerticalKeyNavigationAction(ZappyTaskAction inputAction)
        {
            SendKeysAction action = inputAction as SendKeysAction;
            if (action == null || action.ValueAsString == null)
            {
                return false;
            }
            if (!action.ValueAsString.EndsWith("{Down}", StringKeys.Comparison) && !action.ValueAsString.EndsWith("{Up}", StringKeys.Comparison) && !action.ValueAsString.EndsWith("{PageDown}", StringKeys.Comparison))
            {
                return action.ValueAsString.EndsWith("{PageUp}", StringKeys.Comparison);
            }
            return true;
        }

        public static bool IsWinformsDataGridEditingComboboxCell(ZappyTaskAction zaction, bool checkForExpandedState)
        {
            if (zaction is ZappyTaskAction)
            {
                ZappyTaskAction inputAction = zaction as ZappyTaskAction;
                if (!MatchesWinformsEditingControl(inputAction.ActivityElement.Name) || inputAction.ActivityElement.ControlTypeName != ControlType.Edit)
                {
                    return false;
                }
                ITaskActivityElement grandParentUsingQid = GetGrandParentUsingQid(inputAction);
                if (grandParentUsingQid == null || grandParentUsingQid.ControlTypeName != ControlType.ComboBox)
                {
                    return false;
                }
                return !checkForExpandedState || TaskActivityElement.IsState(grandParentUsingQid, AccessibleStates.Expanded);
            }
            return false;
        }

        internal static bool MatchesAnyOfGivenStrings(string source, params string[] anyTargets)
        {
            foreach (string str in anyTargets)
            {
                if (string.Equals(source, str, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool MatchesWinformsEditingControl(string name)
        {
            if (!string.Equals(name, LocalizedSystemStrings.Instance.WinFormsEditableControlNameENU, StringComparison.Ordinal))
            {
                return string.Equals(name, LocalizedSystemStrings.Instance.WinFormsEditableControlNameLocalized, StringComparison.Ordinal);
            }
            return true;
        }

        internal static ZappyTaskAction PeekHelper(ZappyTaskActionStack actions, int peekFrom)
        {
            if (actions.Count - 1 - peekFrom >= 0)
            {
                return actions.Peek(peekFrom);
            }
            return null;
        }

        internal static void TagActionWithEatClick(InputAction mouseInputAction, ZappyTaskAction actionToTag)
        {
            if (mouseInputAction != null && actionToTag != null)
            {
                if (mouseInputAction is DragAction && string.Equals(mouseInputAction.ActionName, "DragAction", StringComparison.OrdinalIgnoreCase))
                {
                    AddTagToAction(actionToTag, "ShouldEatClickAction", false);
                }
                else if (!GetTagFromAction<bool>(actionToTag, "ShouldEatClickAction"))
                {
                    if (mouseInputAction is DragAction)
                    {
                        AddTagToAction(actionToTag, "ShouldEatClickAction", true);
                    }
                    else
                    {
                        MouseAction action = mouseInputAction as MouseAction;
                        if (action != null && action.ActionType != MouseActionType.Click && action.ActionType != MouseActionType.WheelRotate)
                        {
                            AddTagToAction(actionToTag, "ShouldEatClickAction", true);
                        }
                    }
                }
            }
        }

        public static int TimeDifference(ZappyTaskAction firstAction, ZappyTaskAction secondAction) =>
            (int)(firstAction.StartTimestamp - secondAction.EndTimestamp);

        internal static void UntagActionWithEatClick(ZappyTaskAction inputAction, ZappyTaskAction actiontoTag)
        {
            if (GetTagFromAction<bool>(actiontoTag, "ShouldEatClickAction"))
            {
                MouseAction action = inputAction as MouseAction;
                if (action != null && (action.ActionType == MouseActionType.Click || action.ActionType == MouseActionType.WheelRotate))
                {
                    AddTagToAction(actiontoTag, "ShouldEatClickAction", false);
                }
            }
        }

        public static bool IsWindows7OrHigher
        {
            get
            {
                if (!isWindows7.HasValue)
                {
                    isWindows7 = ZappyTaskUtilities.IsWin7;
                }
                return isWindows7.Value;
            }
            set
            {
                isWindows7 = value;
            }
        }
    }
}

