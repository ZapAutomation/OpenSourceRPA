using System;
using System.Diagnostics;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.LogManager;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class TreeWalkerHelper
    {
        private const string CurrentlyRecordingName = "CurrentlyRecordingCaption{BFA0AEFD-65DB-4a94-B64B-8CDAAF444FB6}";
        internal static readonly TreeWalker defaultWalker = TreeWalker.ControlViewWalker;

        private static AutomationElement FindNearestSiblingWithSameControlType(AutomationElement startElement, NavigateInvoker navigate, ControlType controlTypeToMatch)
        {
            AutomationElement element = startElement;
            while (element != null)
            {
                ControlType currentPropertyValue = element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                if (UiaUtility.MatchUiaControlType(controlTypeToMatch, currentPropertyValue))
                {
                    return element;
                }
                element = NavigateHelper(navigate, element);
            }
            return element;
        }

        private static AutomationElement FindNearestSiblingWithSameControlTypeByExpandCollapse(AutomationElement container, AutomationElement startElement, NavigateInvoker navigate, ControlType itemControlType)
        {
            AutomationElement element = null;
            ExpandCollapsePattern expandCollapsePattern = PatternHelper.GetExpandCollapsePattern(container, true);
            bool flag = false;
            if (expandCollapsePattern != null && expandCollapsePattern.Current.ExpandCollapseState == ExpandCollapseState.Collapsed)
            {
                flag = true;
                expandCollapsePattern.Expand();
            }
            element = FindNearestSiblingWithSameControlType(startElement, navigate, itemControlType);
            if (flag && expandCollapsePattern.Current.ExpandCollapseState == ExpandCollapseState.Expanded)
            {
                expandCollapsePattern.Collapse();
            }
            return element;
        }

        internal static AutomationElement FindPreviousItemSibling(AutomationElement element, ItemContainerPattern pattern)
        {
            int[] runtimeId = element.GetRuntimeId();
            string name = element.Current.Name;
            ControlType currentPropertyValue = element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
            Stopwatch stopwatch = null;
            int technologyManagerProperty = -1;
            technologyManagerProperty = UITechnologyManager.GetTechnologyManagerProperty<int>(UiaTechnologyManager.Instance, UITechnologyManagerProperty.NavigationTimeout);
            if (technologyManagerProperty > 0)
            {
                stopwatch = Stopwatch.StartNew();
            }
            AutomationElement element2 = null;
            AutomationElement element3 = null;
            for (AutomationElement element4 = pattern.FindItemByProperty(null, null, 0); null != element4; element4 = pattern.FindItemByProperty(element4, null, 0))
            {
                if (technologyManagerProperty > 0 && (int)stopwatch.ElapsedMilliseconds > technologyManagerProperty)
                {
                    stopwatch.Stop();
                    object[] args = { technologyManagerProperty };
                    CrapyLogger.log.ErrorFormat("Navigation to PreviousSibling took more than {0} ms.", args);
                    throw new TimeoutException();
                }
                if (UiaUtility.MatchAutomationElement(element, runtimeId, name, currentPropertyValue, element4))
                {
                    element3 = element2;
                    break;
                }
                element2 = element4;
            }
            if (stopwatch != null)
            {
                stopwatch.Stop();
            }
            return element3;
        }

        internal static AutomationElement GetFirstChild(AutomationElement element) =>
            GetFirstChild(element, defaultWalker, VirtualizationContext.Unknown);

        internal static AutomationElement GetFirstChild(AutomationElement element, VirtualizationContext context) =>
            GetFirstChild(element, defaultWalker, context);

        internal static AutomationElement GetFirstChild(AutomationElement element, TreeWalker walker) =>
            GetFirstChild(element, walker, VirtualizationContext.Unknown);

        internal static AutomationElement GetFirstChild(AutomationElement element, TreeWalker walker, VirtualizationContext context)
        {
            if (context == VirtualizationContext.Unknown || context == VirtualizationContext.Enable)
            {
                BasePattern virtualizedItemPattern = PatternHelper.GetVirtualizedItemPattern(element);
                if (virtualizedItemPattern != null || PatternHelper.GetItemContainerPattern(element) != null)
                {
                    return GetVirtualFirstChild(element, walker, virtualizedItemPattern != null);
                }
            }
            AutomationElement firstChild = null;
            if (GroupTreeWalkerHelper.TryGetFirstChildOfGroup(element, walker, GroupTreeWalkerHelper.GroupType.Datagrid, ref firstChild))
            {
                return firstChild;
            }
            firstChild = null;
            if (GroupTreeWalkerHelper.TryGetFirstChildOfGroup(element, walker, GroupTreeWalkerHelper.GroupType.List, ref firstChild))
            {
                return firstChild;
            }
            return NavigateHelper(walker.GetFirstChild, element);
        }

        internal static AutomationElement GetLastChild(AutomationElement element) =>
            GetLastChild(element, defaultWalker, VirtualizationContext.Unknown);

        internal static AutomationElement GetLastChild(AutomationElement element, TreeWalker walker, VirtualizationContext context)
        {
            if (context == VirtualizationContext.Unknown || context == VirtualizationContext.Enable)
            {
                BasePattern virtualizedItemPattern = PatternHelper.GetVirtualizedItemPattern(element);
                if (virtualizedItemPattern != null || PatternHelper.GetItemContainerPattern(element) != null)
                {
                    return GetVirtualLastChild(element, walker, virtualizedItemPattern != null);
                }
            }
            AutomationElement lastChild = null;
            if (GroupTreeWalkerHelper.TryGetLastChildOfGroup(element, walker, GroupTreeWalkerHelper.GroupType.Datagrid, ref lastChild))
            {
                return lastChild;
            }
            lastChild = null;
            if (GroupTreeWalkerHelper.TryGetLastChildOfGroup(element, walker, GroupTreeWalkerHelper.GroupType.List, ref lastChild))
            {
                return lastChild;
            }
            return NavigateHelper(walker.GetLastChild, element);
        }

        internal static AutomationElement GetNextSibling(AutomationElement element)
        {
            VirtualizationContext unknown = VirtualizationContext.Unknown;
            return GetNextSibling(element, defaultWalker, ref unknown);
        }

        internal static AutomationElement GetNextSibling(AutomationElement element, ref VirtualizationContext context) =>
            GetNextSibling(element, defaultWalker, ref context);

        internal static AutomationElement GetNextSibling(AutomationElement element, TreeWalker walker, ref VirtualizationContext context)
        {
            if (context == VirtualizationContext.Unknown)
            {
                if (UiaUtility.IsVirtualElement(element))
                {
                    context = VirtualizationContext.Enable;
                    return GetVirtualNextSibling(element, walker);
                }
                context = VirtualizationContext.Disable;
            }
            else if (context == VirtualizationContext.Enable)
            {
                return GetVirtualNextSibling(element, walker);
            }
            return IgnoreCurrentlyRecordingByNavigation(walker.GetNextSibling, NavigateHelper(walker.GetNextSibling, element));
        }

        internal static AutomationElement GetParent(AutomationElement element) =>
            GetParent(element, defaultWalker);

        internal static AutomationElement GetParent(AutomationElement element, TreeWalker walker)
        {
            try
            {
                return NavigateHelper(walker.GetParent, element);
            }
            catch (ElementNotAvailableException)
            {
                if (PatternHelper.GetVirtualizedItemPattern(element) == null)
                {
                    throw;
                }
                UiaUtility.RealizeElement(element);
                return NavigateHelper(walker.GetParent, element);
            }
        }

        internal static AutomationElement GetPreviousSibling(AutomationElement element)
        {
            VirtualizationContext unknown = VirtualizationContext.Unknown;
            return GetPreviousSibling(element, ref unknown);
        }

        internal static AutomationElement GetPreviousSibling(AutomationElement element, ref VirtualizationContext context) =>
            GetPreviousSibling(element, defaultWalker, ref context);

        internal static AutomationElement GetPreviousSibling(AutomationElement element, TreeWalker walker, ref VirtualizationContext context)
        {
            if (context == VirtualizationContext.Unknown)
            {
                if (UiaUtility.IsVirtualElement(element))
                {
                    context = VirtualizationContext.Enable;
                    return GetVirtualPreviousSibling(element, walker);
                }
                context = VirtualizationContext.Disable;
            }
            else if (context == VirtualizationContext.Enable)
            {
                return GetVirtualPreviousSibling(element, walker);
            }
            return IgnoreCurrentlyRecordingByNavigation(walker.GetPreviousSibling, NavigateHelper(walker.GetPreviousSibling, element));
        }

        private static AutomationElement GetVirtualFirstChild(AutomationElement element, TreeWalker walker, bool isVirtualizedElement)
        {
            AutomationElement elementToMatch = null;
            if (isVirtualizedElement)
            {
                try
                {
                    elementToMatch = NavigateHelper(walker.GetFirstChild, element);
                }
                catch (ElementNotAvailableException)
                {
                    UiaUtility.RealizeElement(element);
                    elementToMatch = NavigateHelper(walker.GetFirstChild, element);
                }
            }
            AutomationElement navigatedElement = elementToMatch ?? NavigateHelper(walker.GetFirstChild, element);
            if (GridGroupTreeWalker.IsGroupedGrid(element, walker, navigatedElement, GroupTreeWalkerHelper.ElementRequest.FirstChild))
            {
                return navigatedElement;
            }
            if (ListGroupTreeWalker.IsGroupedList(element, walker, navigatedElement, GroupTreeWalkerHelper.ElementRequest.FirstChild))
            {
                return navigatedElement;
            }
            ItemContainerPattern itemContainerPattern = null;
            ControlType controlTypeToMatch = null;
            itemContainerPattern = PatternHelper.GetItemContainerPattern(element) as ItemContainerPattern;
            if (itemContainerPattern != null)
            {
                elementToMatch = itemContainerPattern.FindItemByProperty(null, null, 0);
                if (elementToMatch != null)
                {
                    controlTypeToMatch = elementToMatch.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                }
                AutomationElement element4 = NavigateHelper(walker.GetFirstChild, element);
                if (elementToMatch == null)
                {
                    return element4;
                }
                if (element4 == null || UiaUtility.MatchAutomationElement(element4, elementToMatch))
                {
                    return elementToMatch;
                }
                if (IsFirstVisualChildTypeOfExpandableContainer(element, element4))
                {
                    return element4;
                }
                AutomationElement siblingElementToMatch = FindNearestSiblingWithSameControlType(element4, walker.GetNextSibling, controlTypeToMatch);
                if (siblingElementToMatch == null)
                {
                    siblingElementToMatch = FindNearestSiblingWithSameControlTypeByExpandCollapse(element, element4, walker.GetNextSibling, controlTypeToMatch);
                }
                if (siblingElementToMatch != null && IsAutomationPeerTrailingSibling(elementToMatch, siblingElementToMatch, itemContainerPattern))
                {
                    elementToMatch = element4;
                }
            }
            return elementToMatch;
        }

        private static AutomationElement GetVirtualLastChild(AutomationElement element, TreeWalker walker, bool isVirtualizedElement)
        {
            AutomationElement elementToMatch = null;
            if (isVirtualizedElement)
            {
                try
                {
                    elementToMatch = NavigateHelper(walker.GetLastChild, element);
                }
                catch (ElementNotAvailableException)
                {
                    UiaUtility.RealizeElement(element);
                    elementToMatch = NavigateHelper(walker.GetLastChild, element);
                }
            }
            AutomationElement navigatedElement = elementToMatch ?? NavigateHelper(walker.GetLastChild, element);
            if (GridGroupTreeWalker.IsGroupedGrid(element, walker, navigatedElement, GroupTreeWalkerHelper.ElementRequest.LastChild))
            {
                return navigatedElement;
            }
            if (ListGroupTreeWalker.IsGroupedList(element, walker, navigatedElement, GroupTreeWalkerHelper.ElementRequest.LastChild))
            {
                return navigatedElement;
            }
            ItemContainerPattern itemContainerPattern = null;
            ControlType controlTypeToMatch = null;
            itemContainerPattern = PatternHelper.GetItemContainerPattern(element) as ItemContainerPattern;
            if (itemContainerPattern != null)
            {
                for (AutomationElement element5 = itemContainerPattern.FindItemByProperty(null, null, 0); null != element5; element5 = itemContainerPattern.FindItemByProperty(element5, null, 0))
                {
                    elementToMatch = element5;
                }
                if (elementToMatch != null)
                {
                    controlTypeToMatch = elementToMatch.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                }
                AutomationElement element4 = NavigateHelper(walker.GetLastChild, element);
                if (elementToMatch == null)
                {
                    return element4;
                }
                if (element4 == null || UiaUtility.MatchAutomationElement(element4, elementToMatch))
                {
                    return elementToMatch;
                }
                ControlType currentPropertyValue = element4.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                if (UiaUtility.MatchUiaControlType(currentPropertyValue, controlTypeToMatch))
                {
                    return elementToMatch;
                }
                AutomationElement startElement = FindNearestSiblingWithSameControlType(element4, walker.GetPreviousSibling, controlTypeToMatch);
                if (startElement == null)
                {
                    startElement = FindNearestSiblingWithSameControlTypeByExpandCollapse(element, element4, walker.GetPreviousSibling, controlTypeToMatch);
                }
                if (startElement != null && IsAutomationPeerTrailingSibling(startElement, elementToMatch, itemContainerPattern))
                {
                    elementToMatch = element4;
                }
            }
            return elementToMatch;
        }

        private static AutomationElement GetVirtualNextSibling(AutomationElement element, TreeWalker walker)
        {
            AutomationElement nextSibling = null;
            try
            {
                nextSibling = NavigateHelper(walker.GetNextSibling, element);
            }
            catch (ElementNotAvailableException)
            {
                UiaUtility.RealizeElement(element);
                nextSibling = NavigateHelper(walker.GetNextSibling, element);
            }
            if (GridGroupTreeWalker.IsGroupedGrid(element, walker, null, GroupTreeWalkerHelper.ElementRequest.NextSibling))
            {
                return nextSibling ?? NavigateHelper(walker.GetNextSibling, element);
            }
            if (!GroupTreeWalkerHelper.TryGetSiblingInGroup(element, walker, GroupTreeWalkerHelper.GroupType.Datagrid, ref nextSibling, GroupTreeWalkerHelper.ElementRequest.NextSibling))
            {
                if (ListGroupTreeWalker.IsGroupedList(element, walker, null, GroupTreeWalkerHelper.ElementRequest.NextSibling))
                {
                    return nextSibling ?? NavigateHelper(walker.GetNextSibling, element);
                }
                if (GroupTreeWalkerHelper.TryGetSiblingInGroup(element, walker, GroupTreeWalkerHelper.GroupType.List, ref nextSibling, GroupTreeWalkerHelper.ElementRequest.NextSibling))
                {
                    return nextSibling;
                }
                AutomationElement element3 = NavigateHelper(walker.GetParent, element);
                ItemContainerPattern itemContainerPattern = null;
                ControlType controlTypeToMatch = null;
                ControlType currentPropertyValue = element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                AutomationElement element4 = null;
                if (null != element3)
                {
                    itemContainerPattern = PatternHelper.GetItemContainerPattern(element3) as ItemContainerPattern;
                    if (itemContainerPattern != null)
                    {
                        element4 = itemContainerPattern.FindItemByProperty(null, null, 0);
                        if (element4 != null)
                        {
                            controlTypeToMatch = element4.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                        }
                    }
                }
                if (itemContainerPattern != null)
                {
                    if (!UiaUtility.MatchUiaControlType(currentPropertyValue, controlTypeToMatch))
                    {
                        ControlType controlType = nextSibling != null ? nextSibling.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType : null;
                        if (UiaUtility.MatchUiaControlType(controlType, controlTypeToMatch))
                        {
                            nextSibling = itemContainerPattern.FindItemByProperty(null, null, 0);
                        }
                        else if (controlType == null && element4 != null)
                        {
                            VirtualizedItemPattern virtualizedItemPattern = PatternHelper.GetVirtualizedItemPattern(element4) as VirtualizedItemPattern;
                            if (virtualizedItemPattern != null)
                            {
                                ExpandCollapsePattern expandCollapsePattern = PatternHelper.GetExpandCollapsePattern(element3, true);
                                bool flag = false;
                                if (expandCollapsePattern != null && expandCollapsePattern.Current.ExpandCollapseState == ExpandCollapseState.Collapsed)
                                {
                                    flag = true;
                                }
                                virtualizedItemPattern.Realize();
                                nextSibling = NavigateHelper(walker.GetNextSibling, element);
                                if (nextSibling != null && !UiaUtility.MatchAutomationElement(nextSibling, element4))
                                {
                                    nextSibling = null;
                                }
                                if (flag && expandCollapsePattern.Current.ExpandCollapseState == ExpandCollapseState.Expanded)
                                {
                                    expandCollapsePattern.Collapse();
                                }
                            }
                        }
                    }
                    else
                    {
                        AutomationElement element5 = itemContainerPattern.FindItemByProperty(element, null, 0);
                        if (element5 != null)
                        {
                            nextSibling = element5;
                        }
                    }
                }
                if (nextSibling != null && element4 != null && UiaUtility.MatchUiaControlType(controlTypeToMatch, ControlType.Custom) && UiaUtility.MatchUiaControlType(currentPropertyValue, ControlType.Custom))
                {
                    currentPropertyValue = nextSibling.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                    if (UiaUtility.MatchUiaControlType(currentPropertyValue, ControlType.Custom) && UiaUtility.MatchAutomationElement(element4, nextSibling))
                    {
                        return null;
                    }
                }
            }
            return nextSibling;
        }

        private static AutomationElement GetVirtualPreviousSibling(AutomationElement element, TreeWalker walker)
        {
            AutomationElement nextSibling = null;
            try
            {
                nextSibling = NavigateHelper(walker.GetPreviousSibling, element);
            }
            catch (ElementNotAvailableException)
            {
                UiaUtility.RealizeElement(element);
                nextSibling = NavigateHelper(walker.GetPreviousSibling, element);
            }
            if (GridGroupTreeWalker.IsGroupedGrid(element, walker, null, GroupTreeWalkerHelper.ElementRequest.PreviousSibling))
            {
                return nextSibling ?? NavigateHelper(walker.GetPreviousSibling, element);
            }
            if (!GroupTreeWalkerHelper.TryGetSiblingInGroup(element, walker, GroupTreeWalkerHelper.GroupType.Datagrid, ref nextSibling, GroupTreeWalkerHelper.ElementRequest.PreviousSibling))
            {
                if (ListGroupTreeWalker.IsGroupedList(element, walker, null, GroupTreeWalkerHelper.ElementRequest.PreviousSibling))
                {
                    return nextSibling ?? NavigateHelper(walker.GetPreviousSibling, element);
                }
                if (GroupTreeWalkerHelper.TryGetSiblingInGroup(element, walker, GroupTreeWalkerHelper.GroupType.List, ref nextSibling, GroupTreeWalkerHelper.ElementRequest.PreviousSibling))
                {
                    return nextSibling;
                }
                AutomationElement element3 = NavigateHelper(walker.GetParent, element);
                ControlType controlTypeToMatch = null;
                ControlType currentPropertyValue = element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                ItemContainerPattern itemContainerPattern = null;
                AutomationElement element4 = null;
                if (null != element3)
                {
                    itemContainerPattern = PatternHelper.GetItemContainerPattern(element3) as ItemContainerPattern;
                    if (itemContainerPattern != null)
                    {
                        element4 = itemContainerPattern.FindItemByProperty(null, null, 0);
                        if (element4 != null)
                        {
                            controlTypeToMatch = element4.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                        }
                    }
                }
                if (itemContainerPattern == null)
                {
                    return nextSibling;
                }
                if (!UiaUtility.MatchUiaControlType(currentPropertyValue, controlTypeToMatch))
                {
                    ControlType controlType = nextSibling != null ? nextSibling.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType : null;
                    if (UiaUtility.MatchUiaControlType(controlType, controlTypeToMatch))
                    {
                        for (AutomationElement element5 = nextSibling; element5 != null; element5 = itemContainerPattern.FindItemByProperty(element5, null, 0))
                        {
                            nextSibling = element5;
                        }
                    }
                    return nextSibling;
                }
                AutomationElement element6 = FindPreviousItemSibling(element, itemContainerPattern);
                if (element6 != null)
                {
                    nextSibling = element6;
                }
            }
            return nextSibling;
        }

        private static AutomationElement IgnoreCurrentlyRecordingByNavigation(NavigateInvoker navigate, AutomationElement element)
        {
            if (element != null && string.Equals(element.Current.AutomationId, "CurrentlyRecordingCaption{BFA0AEFD-65DB-4a94-B64B-8CDAAF444FB6}", StringComparison.Ordinal))
            {
                element = navigate(element);
            }
            return element;
        }

        private static bool IsAutomationPeerTrailingSibling(AutomationElement startElement, AutomationElement siblingElementToMatch, ItemContainerPattern pattern)
        {
                        {
                int[] runtimeId = siblingElementToMatch.GetRuntimeId();
                string name = siblingElementToMatch.Current.Name;
                ControlType currentPropertyValue = siblingElementToMatch.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                for (AutomationElement element = startElement; element != null; element = pattern.FindItemByProperty(element, AutomationElement.NameProperty, name))
                {
                    if (UiaUtility.MatchAutomationElement(siblingElementToMatch, runtimeId, name, currentPropertyValue, element))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private static bool IsFirstVisualChildTypeOfExpandableContainer(AutomationElement containerElement, AutomationElement child)
        {
            if (containerElement == null || child == null)
            {
                return false;
            }
            ControlType currentPropertyValue = child.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
            ControlType controlTypeToMatch = containerElement.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
            if (UiaUtility.MatchUiaControlType(ControlType.Tree, controlTypeToMatch) || UiaUtility.MatchUiaControlType(ControlType.TreeItem, controlTypeToMatch))
            {
                return UiaUtility.MatchUiaControlType(ControlType.Button, currentPropertyValue);
            }
            return UiaUtility.MatchUiaControlType(ControlType.ComboBox, controlTypeToMatch) && UiaUtility.MatchUiaControlType(ControlType.Edit, currentPropertyValue);
        }

        internal static AutomationElement NavigateHelper(NavigateInvoker navigate, AutomationElement element)
        {
            AutomationElement element3;
            if (element == null)
            {
                return null;
            }
            try
            {
                AutomationElement element2 = navigate(element);
                if (element2 != null && Automation.Compare(element2, element) && !string.Equals(element.Current.ClassName, "WebView"))
                {
                    object[] args = { element };
                    
                    return null;
                }
                element3 = element2;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return element3;
        }

        internal delegate AutomationElement NavigateInvoker(AutomationElement element);
    }
}

