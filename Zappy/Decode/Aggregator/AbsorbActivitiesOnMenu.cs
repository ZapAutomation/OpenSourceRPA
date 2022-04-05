using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Aggregator
{
    internal class AbsorbActivitiesOnMenu : ActionFilter
    {
        public AbsorbActivitiesOnMenu() : base("AbsorbActivitiesOnMenu", ZappyTaskActionFilterType.Binary, true, "MiscellaneousAggregators")
        {
        }

        private static ITaskActivityElement GetMenuContainer(ITaskActivityElement element)
        {
            ITaskActivityElement ancestor = element;
            while (ancestor != null && ancestor.QueryId != null)
            {
                if (IsMenuContainer(ancestor))
                {
                    break;
                }
                ancestor = ancestor.QueryId.Ancestor;
            }
            if (ancestor == null)
            {
                TaskActivityElement element3 = FrameworkUtilities.TopLevelElement(element);
                if (IsMenuContainer(element3))
                {
                    return element3;
                }
            }
            return ancestor;
        }

        private static bool HasCommonParent(TaskActivityElement lastElement, TaskActivityElement secondLastElement)
        {
            if (lastElement != null && lastElement.QueryId != null && IsMenuContainer(lastElement.QueryId.Ancestor))
            {
                return false;
            }
            ITaskActivityElement menuContainer = GetMenuContainer(lastElement);
            ITaskActivityElement objB = GetMenuContainer(secondLastElement);
            return Equals(menuContainer, objB);
        }

        protected override bool IsMatch(ZappyTaskActionStack actions) =>
            AggregatorUtilities.IsActionOnControlType(actions.Peek(), ControlType.MenuItem);

        private static bool IsMenuContainer(ITaskActivityElement element)
        {
            if (element == null)
            {
                return false;
            }
            if (!ControlType.MenuBar.NameEquals(element.ControlTypeName) && !ControlType.StatusBar.NameEquals(element.ControlTypeName) && !ControlType.ToolBar.NameEquals(element.ControlTypeName))
            {
                return ControlType.Menu.NameEquals(element.ControlTypeName);
            }
            return true;
        }

        private static bool IsParentInHierarchy(ITaskActivityElement parentElement, ITaskActivityElement element)
        {
            if (parentElement == null || element == null || element.Equals(parentElement) && element.QueryId != null && element.QueryId.Ancestor != null && ControlType.MenuItem.NameEquals(element.QueryId.Ancestor.ControlTypeName))
            {
                return false;
            }
            ITaskActivityElement ancestor = element;
            bool flag = false;
            while (ancestor != null && ancestor.QueryId != null)
            {
                if (!flag && ancestor.Equals(parentElement))
                {
                    break;
                }
                flag = SearchConfiguration.ConfigurationExists(ancestor.QueryId.SearchConfigurations, SearchConfiguration.NextSibling);
                ancestor = ancestor.QueryId.Ancestor;
            }
            if (ancestor != null)
            {
                return true;
            }
            ITaskActivityElement element3 = FrameworkUtilities.TopLevelElement(element);
            return element3 != null && element3.Equals(parentElement);
        }

        protected override bool ProcessOutputQuery(ZappyTaskActionStack actions)
        {
            ZappyTaskAction action = actions.Peek(0);
            if (actions.Count > 1)
            {
                ZappyTaskAction action3 = actions.Peek(1);
                MouseAction action4 = action3 as MouseAction;
                bool flag = IsParentInHierarchy(action3.ActivityElement, action.ActivityElement);
                if ((flag || action3 is SendKeysAction || action4 != null && action4.ActionType != MouseActionType.Click && action4.ActionType != MouseActionType.Hover) && AggregatorUtilities.IsActionOnControlType(action3, ControlType.MenuItem))
                {
                    TaskActivityElement uIElement = action.ActivityElement;
                    TaskActivityElement secondLastElement = action3.ActivityElement;
                    if ((flag || HasCommonParent(uIElement, secondLastElement)) && !(action is SetStateAction))
                    {
                        actions.Pop(1);
                    }
                }
            }
            SendKeysAction source = action as SendKeysAction;
            if (source != null && StringKeys.Comparer.Equals("{Enter}", source.ValueAsString))
            {
                actions.Pop();
                MouseAction element = new MouseAction(source.ActivityElement, MouseButtons.Left, MouseActionType.Click);
                element.ShallowCopy(source, false);
                actions.Push(element);
            }
            return false;
        }
    }
}

