using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbActivitiesBeforeLaunchApplication : ActionFilter
    {
        private const string DeskTopClassName = "#32769";
        private const string JumpListWindowClassName = "DV2ControlHost";
        private const string MSTasksClassName = "MSTaskSwWClass";
        private const string ProgramManagerClassName = "Progman";
        private const string ProgramsMenuClassName = "ToolbarWindow32";
        private static string startMenuListItemClassName;
        private const string SysListView32 = "SysListView32";

        public AbsorbActivitiesBeforeLaunchApplication() : base("AbsorbActivitiesBeforeLaunchApplication", ZappyTaskActionFilterType.Binary, false, "LaunchApplicationAggregators")
        {
        }

        private static bool IfDesktopShortcutThenAggregate(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = actions.Peek(1);
            if (!IsAcceptableActionOnDesktopShortCutToAggregate(action) && !IsAcceptableActionOnListToAggregate(action) || !IsActionOnChildOfDesktop(action))
            {
                return false;
            }
            actions.Pop(1);
            while (actions.Count > 1)
            {
                action = actions.Peek(1);
                if (!AggregatorUtilities.IsActionOnControlType(action, ControlType.ListItem) && !AggregatorUtilities.IsActionOnControlType(action, ControlType.CheckBox) || !AggregatorUtilities.IsLeftClick(action) || !IsActionOnChildOfDesktop(action))
                {
                    break;
                }
                actions.Pop(1);
            }
            return true;
        }

        private static bool IfQuickLaunchThenAggregate(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = actions.Peek(1);
            if (AggregatorUtilities.IsLeftClick(action) || AggregatorUtilities.DoesSendKeysEndWithEnter(action))
            {
                if (IsActionOnChildOfQuickLaunch(action))
                {
                    actions.Pop(1);
                    return true;
                }
            }
            else if (AggregatorUtilities.IsWindows7OrHigher && (AggregatorUtilities.IsMiddleClick(action) || AggregatorUtilities.IsLeftClick(action)) && RecorderUtilities.IsWinSevenTaskBarButton(action))
            {
                actions.Pop(1);
                return true;
            }
            return false;
        }

        private static void IfStartButtonThenAggregate(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = actions.Peek(1);
            bool flag = false;
            if (AggregatorUtilities.IsLeftClick(action) && AggregatorUtilities.IsActionOnControlType(action, ControlType.Button))
            {
                try
                {
                    if (action is ZappyTaskAction)
                    {
                        string propertyValue = (action as ZappyTaskAction).ActivityElement.GetPropertyValue("AccessKey") as string;
                        if (!string.IsNullOrEmpty(propertyValue))
                        {
                            flag = string.Equals(propertyValue.Replace(" ", string.Empty), "Ctrl+Esc", StringComparison.OrdinalIgnoreCase);
                        }
                    }

                }
                catch (ZappyTaskException)
                {
                }
            }
            else
            {
                SendKeysAction action2 = action as SendKeysAction;
                if (action2 != null)
                {
                    if (action2.ModifierKeys == ModifierKeys.None && (StringKeys.Comparer.Equals("{RWin}", action2.Text) || StringKeys.Comparer.Equals("{LWin}", action2.Text)))
                    {
                        flag = true;
                    }
                    else if (action2.ModifierKeys == ModifierKeys.Control && StringKeys.Comparer.Equals("{Escape}", action2.Text))
                    {
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                actions.Pop(1);
            }
        }

        internal static bool IfStartMenuSearchThenAggregate(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = AggregatorUtilities.PeekHelper(actions, 1) as ZappyTaskAction;

            if ((!IsActionOnChildOfStartMenu(action) || !IsActionOnListItemOnStartMenu(action, true) && (!IsActionOnSearchBoxOnStartMenu(action) || !AggregatorUtilities.DoesSendKeysEndWithEnter(action))) && (action == null || !string.Equals(action.AdditionalInfo, "AggregateIfLaunched", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            do
            {
                actions.Pop(1);
                action = AggregatorUtilities.PeekHelper(actions, 1) as ZappyTaskAction;
            }
            while (action != null && string.Equals(action.AdditionalInfo, "AggregateIfLaunched", StringComparison.OrdinalIgnoreCase) || IsActionOnChildOfStartMenu(action) && (IsActionOnSearchItemsOnStartMenu(action) || IsActionOnSearchBoxOnStartMenu(action)));
            IfStartButtonThenAggregate(actions);
            return true;
        }

        private static bool IfStartMenuThenAggregate(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = actions.Peek(1);
            if ((AggregatorUtilities.IsLeftClick(action) || AggregatorUtilities.DoesSendKeysEndWithEnter(action)) && AggregatorUtilities.IsActionOnControlType(action, ControlType.MenuItem) && IsActionOnChildOfStartMenu(action))
            {
                do
                {
                    action = actions.Pop(1);
                }
                while (actions.Count > 0 && IsActionOnChildOfStartMenu(actions.Peek(1)));
                IfStartButtonThenAggregate(actions);
                return true;
            }
            return false;
        }

        private bool IfWin7TaskbarThenAggregate(ZappyTaskActionStack actions)
        {
            if (AggregatorUtilities.IsWindows7OrHigher && actions.Count > 2)
            {
                ZappyTaskAction action = actions.Peek(1) as ZappyTaskAction;
                ZappyTaskAction action2 = actions.Peek(2) as ZappyTaskAction;
                if (AggregatorUtilities.IsLeftClick(action) && AggregatorUtilities.IsRightClick(action2) && IsActionOnJumperListItem(action) && RecorderUtilities.IsWinSevenTaskBarButton(action2))
                {
                    actions.Pop(1);
                    actions.Pop(1);
                    return true;
                }
                if (actions.Count > 3)
                {
                    ZappyTaskAction action3 = actions.Peek(3);
                    if (AggregatorUtilities.IsLeftClick(action) && IsActionOnJumperListItem(action) && AggregatorUtilities.IsSendKeysOfUp(action2) && IsActionOnJumperListMenu(action2) && RecorderUtilities.IsWinSevenTaskBarButton(action3) && AggregatorUtilities.IsRightClick(action3))
                    {
                        actions.Pop(1);
                        actions.Pop(1);
                        actions.Pop(1);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsAcceptableActionOnDesktopShortCutToAggregate(ZappyTaskAction action)
        {
            if (!AggregatorUtilities.IsLeftClickOrDoubleClick(action) && !AggregatorUtilities.DoesSendKeysEndWithEnter(action))
            {
                return false;
            }
            if (!AggregatorUtilities.IsActionOnControlType(action, ControlType.ListItem))
            {
                return AggregatorUtilities.IsActionOnControlType(action, ControlType.CheckBox);
            }
            return true;
        }

        private static bool IsAcceptableActionOnListToAggregate(ZappyTaskAction action) =>
            action is SetValueAction && AggregatorUtilities.IsActionOnControlType(action, ControlType.List);

        private static bool IsActionOnChildOfDesktop(ZappyTaskAction zaction)
        {
            ZappyTaskAction action = zaction as ZappyTaskAction;
            if (action.ActivityElement == null)
            {
                return false;
            }
            TaskActivityElement element = FrameworkUtilities.TopLevelElement(action.ActivityElement);
            if (element == null)
            {
                return false;
            }
            if (!string.Equals(element.ClassName, "Progman", StringComparison.Ordinal))
            {
                return string.Equals(element.ClassName, "#32769", StringComparison.Ordinal);
            }
            return true;
        }

        private static bool IsActionOnChildOfQuickLaunch(ZappyTaskAction zaction)
        {
            if (zaction is ZappyTaskAction)
            {
                ZappyTaskAction action = zaction as ZappyTaskAction;
                TaskActivityElement parent = null;
                if (AggregatorUtilities.IsActionOnControlType(action, ControlType.Button))
                {
                    parent = RecorderUtilities.GetParent(action.ActivityElement);
                }
                else if (AggregatorUtilities.IsActionOnControlType(action, ControlType.ToolBar))
                {
                    parent = action.ActivityElement;
                }
                if (parent != null && ControlType.ToolBar.NameEquals(parent.ControlTypeName))
                {
                    string[] anyTargets = { LocalizedSystemStrings.Instance.WindowsQuickLaunchText, LocalizedSystemStrings.Instance.WindowsRunningApplicationsText };
                    if (AggregatorUtilities.MatchesAnyOfGivenStrings(parent.Name, anyTargets))
                    {
                        TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(action.ActivityElement);
                        if (element2 != null)
                        {
                            string[] textArray2 = { "ToolbarWindow32", "MSTaskSwWClass" };
                            return AggregatorUtilities.MatchesAnyOfGivenStrings(element2.ClassName, textArray2);
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        internal static bool IsActionOnChildOfStartMenu(ZappyTaskAction zaction)
        {
            ZappyTaskAction action = zaction as ZappyTaskAction;
            if (action != null && action.ActivityElement != null)
            {
                TaskActivityElement element = FrameworkUtilities.TopLevelElement(action.ActivityElement);
                if (element != null && !IsActionOnJumperListItem(action) && ControlType.Window.NameEquals(element.ControlTypeName))
                {
                    if (!string.Equals(element.ClassName, "DV2ControlHost", StringComparison.Ordinal))
                    {
                        return string.Equals(element.ClassName, "ToolbarWindow32", StringComparison.Ordinal);
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool IsActionOnJumperListItem(ZappyTaskAction zaction)
        {
            ZappyTaskAction action = zaction as ZappyTaskAction;

            IQueryElement queryId = action.ActivityElement.QueryId;
            if (queryId == null)
            {
                return false;
            }
            ITaskActivityElement ancestor = queryId.Ancestor;
            ITaskActivityElement element3 = FrameworkUtilities.TopLevelElement(action.ActivityElement);
            return ancestor != null && element3 != null && AggregatorUtilities.IsActionOnControlType(action, ControlType.MenuItem) && string.Equals(action.ActivityElement.ClassName, "SysListView32", StringComparison.Ordinal) && ControlType.Client.NameEquals(ancestor.ControlTypeName) && string.Equals(element3.ClassName, "DV2ControlHost", StringComparison.Ordinal);
        }

        private static bool IsActionOnJumperListMenu(ZappyTaskAction action)
        {
            TaskActivityElement element = FrameworkUtilities.TopLevelElement(action.ActivityElement);
            return element != null && AggregatorUtilities.IsActionOnControlType(action, ControlType.Menu) && string.Equals(action.ActivityElement.ClassName, "SysListView32", StringComparison.Ordinal) && string.Equals(element.ClassName, "DV2ControlHost", StringComparison.Ordinal);
        }

        internal static bool IsActionOnListItemOnStartMenu(ZappyTaskAction zaction, bool isLastAction)
        {
            ZappyTaskAction action = zaction as ZappyTaskAction;
            TaskActivityElement uIElement = action.ActivityElement;
            if (uIElement == null)
            {
                return false;
            }
            if (isLastAction)
            {
                SendKeysAction action2 = action as SendKeysAction;
                if (!AggregatorUtilities.IsLeftClick(action) && !AggregatorUtilities.DoesSendKeysEndWithEnter(action))
                {
                    return false;
                }
                if (AggregatorUtilities.IsActionOnControlType(action, ControlType.MenuItem) && string.Equals(uIElement.ClassName, "SysListView32", StringComparison.OrdinalIgnoreCase))
                {
                    uIElement = RecorderUtilities.GetParent(uIElement);
                    TaskActivityElement element = null;
                    if (uIElement != null)
                    {
                        element = RecorderUtilities.GetParent(uIElement);
                        if (element != null)
                        {
                            element = RecorderUtilities.GetParent(element);
                        }
                    }
                    return element != null && uIElement != null && ControlType.Menu.NameEquals(uIElement.ControlTypeName) && string.Equals(uIElement.ClassName, "SysListView32", StringComparison.OrdinalIgnoreCase) && string.Equals(element.ClassName, "Desktop top match", StringComparison.Ordinal);
                }
                if (AggregatorUtilities.IsLeftClick(action) && AggregatorUtilities.IsActionOnControlType(action, ControlType.Edit) && string.Equals(uIElement.ClassName, StartMenuListItemClassName, StringComparison.OrdinalIgnoreCase))
                {
                    uIElement = RecorderUtilities.GetParent(uIElement);
                    if (uIElement == null || !ControlType.ListItem.NameEquals(uIElement.ControlTypeName))
                    {
                        return false;
                    }
                }
            }
            return string.Equals(StartMenuListItemClassName, uIElement.ClassName);
        }

        internal static bool IsActionOnSearchBoxOnStartMenu(ZappyTaskAction action)
        {
            if (!(action is ZappyTaskAction) || !IsActionOnChildOfStartMenu(action) || !AggregatorUtilities.IsActionOnControlType(action, ControlType.Edit))
            {
                return false;
            }

            TaskActivityElement parent = RecorderUtilities.GetParent(action.ActivityElement);
            return parent != null && !ControlType.ListItem.NameEquals(parent.ControlTypeName);
        }

        internal static bool IsActionOnSearchItemsOnStartMenu(ZappyTaskAction action)
        {
            TaskActivityElement uIElement = action.ActivityElement;
            return IsActionOnListItemOnStartMenu(action, false) || AggregatorUtilities.IsActionOnControlType(action, ControlType.MenuItem) && string.Equals("SysListView32", uIElement.ClassName, StringComparison.Ordinal);
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                ZappyTaskAction action = actions.Peek();
                if (action is LaunchApplicationAction)                {
                    return (actions.Peek(1) as ZappyTaskAction).ActivityElement != null;
                }
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            bool flag = IfWin7TaskbarThenAggregate(actions);
            if (!flag)
            {
                flag = IfQuickLaunchThenAggregate(actions);
            }
            if (!flag)
            {
                flag = IfDesktopShortcutThenAggregate(actions);
            }
            if (!flag)
            {
                flag = IfStartMenuThenAggregate(actions);
            }
            if (!flag)
            {
                flag = IfStartMenuSearchThenAggregate(actions);
            }
            if (flag)
            {
                if ((actions.Peek() is ZappyTaskAction))
                    (actions.Peek() as ZappyTaskAction).AdditionalInfo = "Aggregated";
            }
            return false;
        }

        private static string StartMenuListItemClassName
        {
            get
            {
                if (string.IsNullOrEmpty(startMenuListItemClassName))
                {
                    if (AggregatorUtilities.IsWindows7OrHigher)
                    {
                        startMenuListItemClassName = "DirectUIHWND";
                    }
                    else
                    {
                        startMenuListItemClassName = "SysListView32";
                    }
                }
                return startMenuListItemClassName;
            }
        }
    }
}

