using System;
using Zappy.ActionMap.Enums;
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
    internal class ConvertActionOnWindowTitleBar : ActionFilter
    {
        private const string MaximizeButtonName = "Maximize";
        private const string RestoreButtonName = "Restore";

        public ConvertActionOnWindowTitleBar() : base("ConvertActionOnWindowTitleBar", ZappyTaskActionFilterType.Binary, false, "MiscellaneousAggregators")
        {
        }

        private static bool AreSameWindow(TaskActivityElement firstElement, TaskActivityElement secondElement)
        {
            if (firstElement == null || secondElement == null || !ControlType.Window.NameEquals(firstElement.ControlTypeName) || !ControlType.Window.NameEquals(secondElement.ControlTypeName))
            {
                return false;
            }
            if (!Equals(firstElement, secondElement))
            {
                return firstElement.WindowHandle == secondElement.WindowHandle;
            }
            return true;
        }

        private static bool IsActionOnButtonOfName(ZappyTaskAction action, string buttonName, string localizedButtonName)
        {
            if (!AggregatorUtilities.IsActionOnControlType(action, ControlType.Button) || !string.Equals(action.ActivityElement.Name, buttonName, StringComparison.Ordinal) && !string.Equals(action.ActivityElement.Name, localizedButtonName, StringComparison.CurrentCulture))
            {
                return false;
            }
            TaskActivityElement parent = RecorderUtilities.GetParent(action.ActivityElement);
            return parent != null && ControlType.TitleBar.NameEquals(parent.ControlTypeName);
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count <= 1)
            {
                return false;
            }
            ZappyTaskAction action = actions.Peek(1);
            return action.ActivityElement != null && AggregatorUtilities.IsLeftClickOrDoubleClick(action);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            TaskActivityElement parent;
            MouseAction action = actions.Peek(1) as MouseAction;
            ControlStates maximized = ControlStates.None | ControlStates.Restored;
            if (action.ActionType == MouseActionType.Click)
            {
                parent = RecorderUtilities.GetParent(action.ActivityElement);
                if (parent != null)
                {
                    parent = RecorderUtilities.GetParent(parent);
                }
                if (!IsActionOnButtonOfName(action, "Maximize", LocalizedSystemStrings.Instance.WindowMaximizeButtonText) && !IsActionOnButtonOfName(action, "Restore", LocalizedSystemStrings.Instance.WindowRestoreButtonText))
                {
                    return false;
                }
            }
            else if (AggregatorUtilities.IsActionOnControlType(action, ControlType.TitleBar))
            {
                parent = RecorderUtilities.GetParent(action.ActivityElement);
            }
            else
            {
                return false;
            }
            if (!NativeMethods.IsZoomed(action.ActivityElement.WindowHandle))
            {
                maximized = ControlStates.None | ControlStates.Restored;
            }
            else
            {
                maximized = ControlStates.Maximized;
            }
            TaskActivityElement secondElement = FrameworkUtilities.TopLevelElement(action.ActivityElement);
            if (AreSameWindow(parent, secondElement))
            {
                SetStateAction element = new SetStateAction(secondElement, maximized);
                element.ShallowCopy(action, false);
                element.ActivityElement = secondElement;
                ZappyTaskAction action3 = actions.Pop();
                actions.Pop();
                actions.Push(element);
                element.AdditionalInfo = "Aggregated";
                actions.Push(action3);
            }
            return false;
        }
    }
}

