using System;
using System.Windows.Automation;
using Zappy.Plugins.Uia.Uia.Utilities;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class GroupTreeWalkerHelper
    {
        private static bool IsGroupItem(AutomationElement element, TreeWalker walker, GroupType groupType, ref AutomationElement group, ref ItemContainerPattern itemContainerPattern)
        {
            ControlType controlType = groupType == GroupType.Datagrid ? ControlType.DataItem : ControlType.ListItem;
            if (UiaUtility.MatchUiaControlType(controlType, element))
            {
                group = TreeWalkerHelper.GetParent(element, walker);
                if (UiaUtility.MatchUiaControlType(ControlType.Group, group))
                {
                    AutomationElement parent = TreeWalkerHelper.GetParent(group, walker);
                    ControlType type2 = groupType == GroupType.Datagrid ? ControlType.Table : ControlType.List;
                    if (UiaUtility.MatchUiaControlType(type2, parent))
                    {
                        itemContainerPattern = PatternHelper.GetItemContainerPattern(parent) as ItemContainerPattern;
                        if (itemContainerPattern != null)
                        {
                            AutomationElement element3 = itemContainerPattern.FindItemByProperty(null, null, 0);
                            if (element3 != null)
                            {
                                string automationPropertyValue = UiaUtility.GetAutomationPropertyValue<string>(element, AutomationElement.ClassNameProperty);
                                string b = UiaUtility.GetAutomationPropertyValue<string>(element3, AutomationElement.ClassNameProperty);
                                if (string.Equals(automationPropertyValue, b, StringComparison.Ordinal))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal static bool TryGetFirstChildOfGroup(AutomationElement containerElement, TreeWalker walker, GroupType groupType, ref AutomationElement firstChild)
        {
            bool flag;
            try
            {
                if (!UiaUtility.MatchUiaControlType(ControlType.Group, containerElement))
                {
                    return false;
                }
                AutomationElement parent = TreeWalkerHelper.GetParent(containerElement, walker);
                if (groupType == GroupType.Datagrid && !DataGridUtility.IsDataGridTable(parent))
                {
                    return false;
                }
                if (groupType == GroupType.List && !UiaUtility.IsList(parent))
                {
                    return false;
                }
                ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(parent) as ItemContainerPattern;
                if (itemContainerPattern == null)
                {
                    return false;
                }
                                {
                    AutomationElement element = TreeWalkerHelper.NavigateHelper(walker.GetFirstChild, containerElement);
                    if (element != null)
                    {
                        AutomationElement element4 = element;
                        do
                        {
                            UiaUtility.RealizeElement(element);
                            if (!Automation.Compare(TreeWalkerHelper.GetParent(element, walker), containerElement))
                            {
                                break;
                            }
                            element4 = element;
                            element = TreeWalkerHelper.FindPreviousItemSibling(element, itemContainerPattern);
                        }
                        while (element != null);
                        firstChild = element4;
                        return true;
                    }
                    VirtualizationContext disable = VirtualizationContext.Disable;
                    for (AutomationElement element3 = TreeWalkerHelper.GetPreviousSibling(containerElement, walker, ref disable); element3 != null; element3 = TreeWalkerHelper.GetPreviousSibling(element3, walker, ref disable))
                    {
                        for (element = TreeWalkerHelper.NavigateHelper(walker.GetLastChild, element3); element != null; element = itemContainerPattern.FindItemByProperty(element, null, 0))
                        {
                            UiaUtility.RealizeElement(element);
                            AutomationElement element6 = TreeWalkerHelper.GetParent(element, walker);
                            if (Automation.Compare(containerElement, element6))
                            {
                                firstChild = element;
                                return true;
                            }
                        }
                    }
                }
                firstChild = null;
                flag = true;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return flag;
        }

        internal static bool TryGetLastChildOfGroup(AutomationElement containerElement, TreeWalker walker, GroupType groupType, ref AutomationElement lastChild)
        {
            bool flag;
            try
            {
                if (!UiaUtility.MatchUiaControlType(ControlType.Group, containerElement))
                {
                    return false;
                }
                AutomationElement parent = TreeWalkerHelper.GetParent(containerElement, walker);
                if (groupType == GroupType.Datagrid && !DataGridUtility.IsDataGridTable(parent))
                {
                    return false;
                }
                if (groupType == GroupType.List && !UiaUtility.IsList(parent))
                {
                    return false;
                }
                ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(parent) as ItemContainerPattern;
                if (itemContainerPattern == null)
                {
                    flag = false;
                }
                else
                {
                                        {
                        AutomationElement element = null;
                        element = TreeWalkerHelper.NavigateHelper(walker.GetLastChild, containerElement);
                        if (element != null)
                        {
                            AutomationElement element5 = element;
                            do
                            {
                                UiaUtility.RealizeElement(element);
                                if (!Automation.Compare(TreeWalkerHelper.GetParent(element, walker), containerElement))
                                {
                                    break;
                                }
                                element5 = element;
                                element = itemContainerPattern.FindItemByProperty(element, null, 0);
                            }
                            while (element != null);
                            lastChild = element5;
                            return true;
                        }
                        VirtualizationContext disable = VirtualizationContext.Disable;
                        for (AutomationElement element3 = TreeWalkerHelper.GetPreviousSibling(containerElement, walker, ref disable); element3 != null; element3 = TreeWalkerHelper.GetPreviousSibling(element3, walker, ref disable))
                        {
                            element = TreeWalkerHelper.NavigateHelper(walker.GetLastChild, element3);
                            if (element != null)
                            {
                                break;
                            }
                        }
                        element = itemContainerPattern.FindItemByProperty(element, null, 0);
                        AutomationElement element4 = element;
                        while (element != null)
                        {
                            UiaUtility.RealizeElement(element);
                            AutomationElement element7 = TreeWalkerHelper.GetParent(element, walker);
                            if (!Automation.Compare(containerElement, element7))
                            {
                                break;
                            }
                            element4 = element;
                            element = itemContainerPattern.FindItemByProperty(element, null, 0);
                        }
                        lastChild = element4;
                        flag = true;
                    }
                }
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return flag;
        }

        internal static bool TryGetSiblingInGroup(AutomationElement elementForNavigation, TreeWalker walker, GroupType groupType, ref AutomationElement nextSibling, ElementRequest request)
        {
            AutomationElement group = null;
            ItemContainerPattern itemContainerPattern = null;
            if (IsGroupItem(elementForNavigation, walker, groupType, ref group, ref itemContainerPattern))
            {
                AutomationElement element = request == ElementRequest.NextSibling ? itemContainerPattern.FindItemByProperty(elementForNavigation, null, 0) : TreeWalkerHelper.FindPreviousItemSibling(elementForNavigation, itemContainerPattern);
                AutomationElement parent = TreeWalkerHelper.GetParent(element, walker);
                nextSibling = Automation.Compare(group, parent) ? element : null;
                return true;
            }
            return false;
        }

        internal static bool ValidGroupItem(ItemContainerPattern itemContainerPattern, AutomationElement listElement, AutomationElement groupElement)
        {
            if (TreeWalkerHelper.NavigateHelper(TreeWalkerHelper.defaultWalker.GetFirstChild, groupElement) != null)
            {
                AutomationElement element = itemContainerPattern.FindItemByProperty(null, null, 0);
                if (element != null)
                {
                    string automationPropertyValue = UiaUtility.GetAutomationPropertyValue<string>(listElement, AutomationElement.ClassNameProperty);
                    string b = UiaUtility.GetAutomationPropertyValue<string>(element, AutomationElement.ClassNameProperty);
                    if (!string.Equals(automationPropertyValue, b, StringComparison.Ordinal))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal enum ElementRequest
        {
            FirstChild,
            LastChild,
            NextSibling,
            PreviousSibling,
            Parent
        }

        internal enum GroupType
        {
            Datagrid,
            List
        }
    }
}

