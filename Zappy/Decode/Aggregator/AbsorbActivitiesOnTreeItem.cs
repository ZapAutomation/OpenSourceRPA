using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbActivitiesOnTreeItem : ActionFilter
    {
        private string[] ignoreSendKeys;
        private static bool isIgnoreNewUiElement;
        private static bool isIgnoreSendKeys;
        private bool isLastActionMouseAction;
        private bool isLastActionSendKeys;
        private bool isLastActionSetState;
        private bool isSecondLastActionMouseAction;
        private bool isSecondLastActionSendKeys;
        private bool isSecondLastActionSetState;

        public AbsorbActivitiesOnTreeItem() : base("AbsorbActivitiesOnTreeItem", ZappyTaskActionFilterType.Binary, true, "MiscellaneousAggregators")
        {
            ignoreSendKeys = new[] { "{Right}", "{Left}", "{Enter}", "{+}", "-", "{Add}", "{Subtract}", "{Space}" };
        }

        private static ITaskActivityElement GetAncestor(ITaskActivityElement element)
        {
            if (element != null && element.QueryId != null)
            {
                element = element.QueryId.Ancestor;
            }
            return element;
        }

        private static TaskActivityElement GetTreeItem(ZappyTaskAction action)
        {
            if (action != null && action.ActivityElement != null)
            {
                TaskActivityElement uIElement = action.ActivityElement;
                if (ControlType.TreeItem.NameEquals(uIElement.ControlTypeName) || ControlType.CheckBoxTreeItem.NameEquals(uIElement.ControlTypeName))
                {
                    return uIElement;
                }
                uIElement = RecorderUtilities.GetParent(uIElement);
                if (uIElement != null && (ControlType.TreeItem.NameEquals(uIElement.ControlTypeName) || ControlType.CheckBoxTreeItem.NameEquals(uIElement.ControlTypeName)))
                {
                    return uIElement;
                }
            }
            return null;
        }

        protected override bool IsMatch(ZappyTaskActionStack actions)
        {
            if (actions.Count > 1)
            {
                ZappyTaskAction action = actions.Peek();
                if (AggregatorUtilities.IsActionOnControlType(action, ControlType.TreeItem) || AggregatorUtilities.IsActionOnControlType(action, ControlType.CheckBoxTreeItem) || AggregatorUtilities.IsActionOnControlType(action, ControlType.Button))
                {
                    ZappyTaskAction action2 = actions.Peek(1);
                    if (!AggregatorUtilities.IsActionOnControlType(action2, ControlType.TreeItem) && !AggregatorUtilities.IsActionOnControlType(action2, ControlType.CheckBoxTreeItem))
                    {
                        return AggregatorUtilities.IsActionOnControlType(action2, ControlType.Button);
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool IsParentInHierarchy(ITaskActivityElement parentElement, ITaskActivityElement element)
        {
            if (parentElement == null || element == null)
            {
                return false;
            }
            ITaskActivityElement ancestor = element;
            while (ancestor != null && ancestor.QueryId != null)
            {
                if (ancestor.Equals(parentElement))
                {
                    break;
                }
                ancestor = ancestor.QueryId.Ancestor;
            }
            return ancestor != null;
        }

        private static void ModifyLastActionName(ZappyTaskActionStack actions, ZappyTaskAction lastAction, ZappyTaskAction secondLastAction)
        {
            
            actions.Pop(0);
            lastAction.ActivityElement = secondLastAction.ActivityElement;
            ZappyTaskAction element = lastAction;
            actions.Push(element);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = actions.Peek(0);
            ZappyTaskAction action2 = actions.Peek(1);
            MouseAction action3 = action as MouseAction;
            MouseAction action4 = action2 as MouseAction;
            TaskActivityElement treeItem = GetTreeItem(action);
            TaskActivityElement objB = GetTreeItem(action2);
            isLastActionSetState = action is SetStateAction;
            isLastActionSendKeys = action is SendKeysAction;
            isLastActionMouseAction = action3 != null;
            isSecondLastActionSetState = action2 is SetStateAction;
            isSecondLastActionSendKeys = action2 is SendKeysAction;
            isSecondLastActionMouseAction = action4 != null;
            if (isLastActionMouseAction && isSecondLastActionMouseAction && !Equals(treeItem, objB) && action4.ActionType == MouseActionType.ButtonDown)
            {
                ModifyLastActionName(actions, action, action2);
                isIgnoreNewUiElement = true;
                return false;
            }
            if (isLastActionSetState && isSecondLastActionMouseAction && action4.MouseButton == MouseButtons.Left && (Equals(treeItem, objB) || isIgnoreNewUiElement))
            {
                if (isIgnoreNewUiElement)
                {
                    ModifyLastActionName(actions, action, action2);
                    isIgnoreNewUiElement = false;
                }
                if (action4.ActionType == MouseActionType.ButtonDown)
                {
                    isIgnoreSendKeys = true;
                    AggregatorUtilities.AddTagToAction(action, "ShouldEatClickAction", true);
                }
                AggregatorUtilities.AddTagToAction(action, "SetValueCausedBy", action2);
                action.AdditionalInfo = "Aggregated";
                actions.Pop(1);
                return false;
            }
            if (!ShouldAbsorbAction(treeItem, objB))
            {
                isIgnoreSendKeys = false;
            }
            else if (!isSecondLastActionSendKeys || isIgnoreSendKeys)
            {
                isIgnoreSendKeys = false;
                if (!ControlType.CheckBoxTreeItem.NameEquals(objB.ControlTypeName) && !AggregatorUtilities.GetTagFromAction<bool>(action2, "ShouldEatClickAction"))
                {
                    action.AdditionalInfo = "Aggregated";
                    actions.Pop(1);
                    return false;
                }
            }
            else
            {
                if (action2 is ZappyTaskAction)
                {
                    string valueAsString = (action2 as ZappyTaskAction).ValueAsString;
                    for (int i = 0; i < ignoreSendKeys.Length; i++)
                    {
                        if (valueAsString.EndsWith(ignoreSendKeys[i], StringKeys.Comparison))
                        {
                            (action2 as ZappyTaskAction).ValueAsString = valueAsString.Remove(valueAsString.Length - ignoreSendKeys[i].Length);
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty((action2 as ZappyTaskAction).ValueAsString))
                    {
                        return true;
                    }
                    AggregatorUtilities.AddTagToAction(action, "SetValueCausedBy", action2);
                    action.AdditionalInfo = "Aggregated";
                    actions.Pop(1);
                }
                return false;
            }
            return false;
        }

        private bool ShouldAbsorbAction(TaskActivityElement lastElement, TaskActivityElement secondLastElement)
        {
            if (lastElement != null && secondLastElement != null && lastElement.QueryId != null && lastElement.QueryId.Ancestor != null)
            {
                if (lastElement.QueryId.Equals(secondLastElement.QueryId) && isLastActionSetState && (isSecondLastActionSetState || isSecondLastActionSendKeys))
                {
                    return true;
                }
                ITaskActivityElement ancestor = GetAncestor(lastElement);
                if (IsParentInHierarchy(secondLastElement, ancestor) && isSecondLastActionSetState && (isLastActionSetState || isLastActionMouseAction || isLastActionSendKeys))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

