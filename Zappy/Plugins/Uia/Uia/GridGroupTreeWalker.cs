using System.Windows.Automation;
using Zappy.Plugins.Uia.Uia.Utilities;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class GridGroupTreeWalker
    {
        internal static bool IsGroupedGrid(UiaElement uiaElement)
        {
            AutomationElement nativeElement = uiaElement.NativeElement as AutomationElement;
            if (DataGridUtility.IsDataGridTable(nativeElement))
            {
                ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(nativeElement) as ItemContainerPattern;
                if (itemContainerPattern != null)
                {
                    AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(nativeElement);
                    if (UiaUtility.MatchUiaControlType(ControlType.Header, firstChild))
                    {
                        firstChild = TreeWalkerHelper.GetNextSibling(firstChild);
                    }
                    if (UiaUtility.MatchUiaControlType(ControlType.Group, firstChild))
                    {
                        return GroupTreeWalkerHelper.ValidGroupItem(itemContainerPattern, nativeElement, firstChild);
                    }
                }
            }
            return false;
        }

        internal static bool IsGroupedGrid(AutomationElement elementForNavigation, TreeWalker walker, AutomationElement navigatedElement, GroupTreeWalkerHelper.ElementRequest request)
        {
            if (elementForNavigation != null)
            {
                if (request == GroupTreeWalkerHelper.ElementRequest.NextSibling || request == GroupTreeWalkerHelper.ElementRequest.PreviousSibling)
                {
                    ControlType currentPropertyValue = elementForNavigation.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                    if (UiaUtility.MatchUiaControlType(ControlType.Group, currentPropertyValue) || UiaUtility.MatchUiaControlType(ControlType.Header, currentPropertyValue))
                    {
                        return DataGridUtility.IsDataGridTable(TreeWalkerHelper.GetParent(elementForNavigation, walker));
                    }
                }
                else if ((request == GroupTreeWalkerHelper.ElementRequest.FirstChild || request == GroupTreeWalkerHelper.ElementRequest.LastChild) && navigatedElement != null && DataGridUtility.IsDataGridTable(elementForNavigation))
                {
                    ControlType controlTypeToMatch = navigatedElement.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                    if (UiaUtility.MatchUiaControlType(ControlType.Group, controlTypeToMatch))
                    {
                        return true;
                    }
                    if (UiaUtility.MatchUiaControlType(ControlType.Header, controlTypeToMatch))
                    {
                        if (request == GroupTreeWalkerHelper.ElementRequest.LastChild)
                        {
                            return true;
                        }
                        AutomationElement elementToMatch = TreeWalkerHelper.NavigateHelper(walker.GetNextSibling, navigatedElement);
                        if (elementToMatch == null || UiaUtility.MatchUiaControlType(ControlType.Group, elementToMatch))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}

