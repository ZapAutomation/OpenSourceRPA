using System;
using System.Drawing;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Properties;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using DragAction = Zappy.ZappyActions.AutomaticallyCreatedActions.DragAction;

namespace Zappy.Decode.Aggregator
{
    internal class MergeMouseDragActivities : ActionFilter
    {
        private static readonly ControlType[] convertDragsToClickOnControls = { ControlType.Button, ControlType.CheckBox, ControlType.Menu, ControlType.MenuItem, ControlType.RadioButton, ControlType.Separator, ControlType.StatusBar, ControlType.TabList };

        public MergeMouseDragActivities() : base("MergeMouseDragActivities", ZappyTaskActionFilterType.Binary, false, "SystemAggregators")
        {
        }

        private static bool ConvertToDragDrop(ZappyTaskActionStack actions)
        {
            MouseAction action = actions.Pop() as MouseAction;
            MouseAction source = actions.Pop() as MouseAction;
            DragDropAction element = new DragDropAction(source.ActivityElement, action.ActivityElement, action.MouseButton)
            {
                StartLocation = source.Location,
                StopLocation = action.Location
            };
            element.ShallowCopy(source, false);
            if (!SystemInformation.DragFullWindows)
            {
                element.Comment = Resources.DragFullWindowsError;
            }
            actions.Push(element);
            return false;
        }

        private static Point GetDisplacement(MouseAction lastAction, MouseAction secondLastAction)
        {
            int num = lastAction.Location.X - secondLastAction.Location.X;
            int num2 = lastAction.Location.Y - secondLastAction.Location.Y;
            int num3 = lastAction.AbsoluteLocation.X - secondLastAction.AbsoluteLocation.X;
            int num4 = lastAction.AbsoluteLocation.Y - secondLastAction.AbsoluteLocation.Y;
            if (Math.Abs(num) < Math.Abs(num3))
            {
                num = num3;
            }
            if (Math.Abs(num2) < Math.Abs(num4))
            {
                num2 = num4;
            }
            return new Point(num, num2);
        }

        private static bool IgnoreDragAndKeepClick(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = actions.Pop(1);
            return false;
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count < 2)
            {
                return false;
            }
            MouseAction secondAction = actions.Peek(1) as MouseAction;
            SetValueAction firstAction = actions.Peek() as SetValueAction;
            MediaAction action3 = actions.Peek() as MediaAction;
            return secondAction != null && secondAction.ActionType == MouseActionType.Drag && !AggregatorUtilities.AreActivitiesOnSameElement(firstAction, secondAction) && !AggregatorUtilities.IsSecondActionElementSameAsSourceOfFirst(firstAction, secondAction) && !AggregatorUtilities.AreActivitiesOnSameElement(action3, secondAction);
        }

        private static bool IsScrollBarElement(TaskActivityElement element)
        {
            if (ControlType.ScrollBar.NameEquals(element.ControlTypeName))
            {
                return true;
            }
            TaskActivityElement parent = RecorderUtilities.GetParent(element);
            return parent != null && ControlType.ScrollBar.NameEquals(parent.ControlTypeName);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            MouseAction secondAction = actions.Peek(1) as MouseAction;
            MouseAction recordedAction = actions.Peek() as MouseAction;
            if (MouseAction.IsImplicitHover(recordedAction))
            {
                actions.Pop();
                return false;
            }
            if (recordedAction == null || recordedAction.ActionType != MouseActionType.Click)
            {
                actions.Pop(1);
                return false;
            }
            if (IsScrollBarElement(secondAction.ActivityElement) && recordedAction.ActionType == MouseActionType.Click)
            {
                if (recordedAction.MouseButton == MouseButtons.Right)
                {
                    actions.Pop(1);
                    return false;
                }
                actions.Pop();
                actions.Pop();
                return true;
            }
            if (AggregatorUtilities.AreActivitiesOnSameElement(recordedAction, secondAction))
            {
                if (Array.IndexOf(convertDragsToClickOnControls, ControlType.GetControlType(recordedAction.ActivityElement.ControlTypeName)) != -1 || RecorderUtilities.PointsWithinRange(recordedAction.AbsoluteLocation, secondAction.AbsoluteLocation, SystemInformation.DragSize))
                {
                    return IgnoreDragAndKeepClick(actions);
                }
            }
            else
            {
                MouseButtons mouseButton = recordedAction.MouseButton;
                if ((mouseButton == MouseButtons.Left || mouseButton == MouseButtons.Right || mouseButton == MouseButtons.Middle) && recordedAction.ActivityElement != null && secondAction.ActivityElement != null && !IsScrollBarElement(recordedAction.ActivityElement))
                {
                    return ConvertToDragDrop(actions);
                }
            }
            actions.Pop();
            actions.Pop();
            DragAction element = new DragAction();
            element.ShallowCopy(secondAction, false);
            element.MouseButton = secondAction.MouseButton;
            element.StartLocation = secondAction.Location;
            element.MoveBy = GetDisplacement(recordedAction, secondAction);
            if (!SystemInformation.DragFullWindows)
            {
                element.Comment = Resources.DragFullWindowsError;
            }
            actions.Push(element);
            return false;
        }
    }
}