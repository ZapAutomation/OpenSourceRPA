using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbClicksOnTaskBar : ActionFilter
    {
        private const string BaseBarClassName = "BaseBar";
        private const string TaskBarClassName = "MSTaskSwWClass";
        private const string ToolbarClassName = "ToolbarWindow32";

        public AbsorbClicksOnTaskBar() : base("AbsorbClicksOnTaskBar", ZappyTaskActionFilterType.Binary, false, "AbsorbWindowResizeAndSetFocusAggregators")
        {
        }

        private static bool IsClickOnTaskBarButton(ZappyTaskAction secondLastAction)
        {
            if (RecorderUtilities.IsWinSevenTaskBarButton(secondLastAction))
            {
                MouseAction action = secondLastAction as MouseAction;
                if (action != null)
                {
                    return action.MouseButton != MouseButtons.Middle;
                }
                return true;
            }
            TaskActivityElement uIElement = secondLastAction.ActivityElement;
            TaskActivityElement parent = RecorderUtilities.GetParent(uIElement);
            TaskActivityElement element3 = FrameworkUtilities.TopLevelElement(uIElement);
            return parent != null && element3 != null && ControlType.Button.NameEquals(uIElement.ControlTypeName) && ControlType.ToolBar.NameEquals(parent.ControlTypeName) && string.Equals(uIElement.ClassName, "ToolbarWindow32", StringComparison.Ordinal) && string.Equals(parent.ClassName, "ToolbarWindow32", StringComparison.Ordinal) && string.Equals(element3.ClassName, "MSTaskSwWClass", StringComparison.Ordinal);
        }

        private static bool IsClickOnTaskBarGroupedButton(ZappyTaskAction lastAction)
        {
            if (RecorderUtilities.IsWinSevenGroupedTaskBarButton(lastAction))
            {
                return true;
            }
            TaskActivityElement uIElement = lastAction.ActivityElement;
            if (uIElement != null)
            {
                TaskActivityElement element2 = FrameworkUtilities.TopLevelElement(uIElement);
                TaskActivityElement parent = RecorderUtilities.GetParent(uIElement);
                if (element2 != null && parent != null && ControlType.MenuItem.NameEquals(uIElement.ControlTypeName) && string.Equals(uIElement.ClassName, "ToolbarWindow32", StringComparison.Ordinal) && ControlType.Menu.NameEquals(parent.ControlTypeName) && ControlType.Window.NameEquals(element2.ControlTypeName) && string.Equals(element2.ClassName, "ToolbarWindow32", StringComparison.Ordinal))
                {
                    NativeMethods.OSVERSIONINFO lpVersionInfo = new NativeMethods.OSVERSIONINFO
                    {
                        dwOSVersionInfoSize = (uint)Marshal.SizeOf(typeof(NativeMethods.OSVERSIONINFO))
                    };
                    if (NativeMethods.GetVersionEx(ref lpVersionInfo) == 0 || lpVersionInfo.dwMajorVersion != 5 || lpVersionInfo.dwMinorVersion != 2 || lpVersionInfo.wProductType == 1)
                    {
                        return string.Equals(parent.ClassName, "ToolbarWindow32", StringComparison.Ordinal);
                    }
                    if (!string.Equals(parent.ClassName, "ToolbarWindow32", StringComparison.Ordinal))
                    {
                        return string.Equals(parent.ClassName, "BaseBar", StringComparison.Ordinal);
                    }
                    return true;
                }
            }
            return false;
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                MouseAction action = actions.Peek(1) as MouseAction;
                if (action != null && action.ActivityElement != null && (AggregatorUtilities.IsWindows7OrHigher || action.MouseButton != MouseButtons.Right))
                {
                    MouseAction action2 = actions.Peek() as MouseAction;
                    return action2 == null || action2.ActionType != MouseActionType.ButtonDown && action2.ActionType != MouseActionType.Drag;
                }
            }
            return false;
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            ZappyTaskAction secondLastAction = actions.Peek(1);
            MouseAction action2 = secondLastAction as MouseAction;
            MouseAction lastAction = actions.Peek() as MouseAction;
            if (action2 != null && action2.MouseButton != MouseButtons.Right && IsClickOnTaskBarButton(secondLastAction))
            {
                if (lastAction != null && lastAction.ActivityElement != null && IsClickOnTaskBarGroupedButton(lastAction))
                {
                    if (!AggregatorUtilities.IsWindows7OrHigher && lastAction.MouseButton == MouseButtons.Right)
                    {
                        return false;
                    }
                    actions.Pop();
                    actions.Pop();
                    return true;
                }
                actions.Pop(1);
            }
            else
            {
                bool flag = false;
                if (RecorderUtilities.IsWinSevenGroupedTaskBarButton(lastAction))
                {
                    actions.Pop();
                    flag = true;
                }
                if (RecorderUtilities.IsWinSevenGroupedTaskBarButton(secondLastAction))
                {
                    if (flag)
                    {
                        actions.Pop();
                    }
                    else
                    {
                        actions.Pop(1);
                    }
                }
            }
            return false;
        }
    }
}

