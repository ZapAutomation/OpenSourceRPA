

using System.Windows.Automation;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class ListGroupTreeWalker
    {
        internal static bool IsGroupedList(UiaElement uiaElement)
        {
            AutomationElement nativeElement = uiaElement.NativeElement as AutomationElement;
            if (UiaUtility.IsList(nativeElement))
            {
                ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(nativeElement) as ItemContainerPattern;
                if (itemContainerPattern != null)
                {
                    AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(nativeElement);
                    if (!UiaUtility.MatchUiaControlType(ControlType.Group, firstChild))
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

        internal static bool IsGroupedList(AutomationElement elementForNavigation, TreeWalker walker, AutomationElement navigatedElement, GroupTreeWalkerHelper.ElementRequest request)
        {
            if (elementForNavigation != null)
            {
                if (request == GroupTreeWalkerHelper.ElementRequest.NextSibling || request == GroupTreeWalkerHelper.ElementRequest.PreviousSibling)
                {
                    ControlType currentPropertyValue = elementForNavigation.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                    if (!UiaUtility.MatchUiaControlType(ControlType.ListItem, currentPropertyValue))
                    {
                        return UiaUtility.IsList(TreeWalkerHelper.GetParent(elementForNavigation, walker));
                    }
                }
                else if ((request == GroupTreeWalkerHelper.ElementRequest.FirstChild || request == GroupTreeWalkerHelper.ElementRequest.LastChild) && navigatedElement != null && UiaUtility.IsList(elementForNavigation))
                {
                    return !UiaUtility.MatchUiaControlType(ControlType.ListItem, navigatedElement);
                }
            }
            return false;
        }
    }
}

