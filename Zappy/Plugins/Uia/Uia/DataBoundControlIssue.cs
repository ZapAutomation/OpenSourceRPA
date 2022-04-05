using System;
using System.Windows.Automation;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class DataBoundControlIssue
    {
        private const int MaxChildrenToCheck = 3;
        private const string NewRowName = "{NewItemPlaceholder}";

        private static AutomationElement GetNamedChild(AutomationElement element, bool headerItemsControl)
        {
            AutomationElement element2 = null;
            int num = 0;
            VirtualizationContext unknown = VirtualizationContext.Unknown;
            AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(element, unknown);
            while (firstChild != null && num < 3)
            {
                if (firstChild.Current.ControlType == ControlType.Text)
                {
                    if (element2 != null)
                    {
                        element2 = null;
                        break;
                    }
                    element2 = firstChild;
                }
                firstChild = TreeWalkerHelper.GetNextSibling(firstChild, ref unknown);
                num++;
            }
            if (!headerItemsControl && num == 3)
            {
                element2 = null;
            }
            return element2;
        }

        public static bool HasNameIssue(AutomationElement element)
        {
            bool flag3;
            try
            {
                bool flag = false;
                string name = element.Current.Name;
                if (!string.IsNullOrEmpty(name))
                {
                                        {
                        int index = name.IndexOf('.');
                        if (index > 0 && index != name.Length - 1)
                        {
                            bool headerItemsControl = IsHeaderItemsControl(element);
                            if (HasNeighbourOfSameTypeAndName(element, name, headerItemsControl))
                            {
                                if (ReferenceEquals(element.Current.ControlType, ControlType.DataItem))
                                {
                                    flag = true;
                                }
                                else
                                {
                                    AutomationElement namedChild = GetNamedChild(element, headerItemsControl);
                                    if (namedChild != null)
                                    {
                                        string str2 = namedChild.Current.Name;
                                        if (!string.IsNullOrEmpty(str2))
                                        {
                                            flag = !string.Equals(name, str2, StringComparison.Ordinal);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                flag3 = flag;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return flag3;
        }

        private static bool HasNeighbourOfSameTypeAndName(AutomationElement element, string elementName, bool headerItemsControl)
        {
            if (element.Current.ControlType != ControlType.DataItem)
            {
                ControlType controlType = element.Current.ControlType;
                VirtualizationContext context = VirtualizationContext.Disable;
                if (IsOfSameTypeAndName(TreeWalkerHelper.GetNextSibling(element, ref context), controlType, elementName))
                {
                    return true;
                }
                if (IsOfSameTypeAndName(TreeWalkerHelper.GetPreviousSibling(element, ref context), controlType, elementName))
                {
                    return true;
                }
                if (!headerItemsControl)
                {
                    return false;
                }
                if (IsOfSameTypeAndName(TreeWalkerHelper.GetParent(element), controlType, elementName))
                {
                    return true;
                }
                AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(element);
                for (int k = 1; k < 3 && firstChild != null && !ReferenceEquals(firstChild.Current.ControlType, element.Current.ControlType); k++)
                {
                    firstChild = TreeWalkerHelper.GetNextSibling(firstChild, ref context);
                }
                return IsOfSameTypeAndName(firstChild, controlType, elementName);
            }
            AutomationElement previousSibling = element;
            VirtualizationContext disable = VirtualizationContext.Disable;
            for (int i = 0; i < 10 && previousSibling != null; i++)
            {
                previousSibling = TreeWalkerHelper.GetPreviousSibling(previousSibling, ref disable);
                if (IsDataItemAndDifferentName(previousSibling, elementName))
                {
                    return false;
                }
            }
            AutomationElement nextSibling = element;
            for (int j = 0; j < 10 && nextSibling != null; j++)
            {
                nextSibling = TreeWalkerHelper.GetNextSibling(nextSibling, ref disable);
                if (IsDataItemAndDifferentName(nextSibling, elementName))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsDataItemAndDifferentName(AutomationElement element, string name) =>
            element != null && element.Current.ControlType == ControlType.DataItem && !string.Equals(element.Current.Name, name, StringComparison.Ordinal) && !string.Equals(element.Current.Name, "{NewItemPlaceholder}", StringComparison.OrdinalIgnoreCase);

        private static bool IsHeaderItemsControl(AutomationElement element)
        {
            ControlType controlType = element.Current.ControlType;
            if (!ReferenceEquals(controlType, ControlType.ToolBar) && !ReferenceEquals(controlType, ControlType.MenuItem))
            {
                return controlType == ControlType.TreeItem;
            }
            return true;
        }

        private static bool IsOfSameTypeAndName(AutomationElement element, ControlType controlType, string controlName) =>
            element != null && element.Current.ControlType == controlType && string.Equals(element.Current.Name, controlName, StringComparison.Ordinal);
    }
}

