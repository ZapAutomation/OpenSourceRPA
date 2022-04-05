using System.Diagnostics;
using System.Windows.Automation;
using Zappy.ActionMap.Query;

namespace Zappy.Plugins.Uia.Uia
{
    internal class ConditionTreeWalker
    {
        private IQueryCondition condition;
        private VirtualizationContext context;
        private VirtualChildrenEnumerator virtualChildEnumerator;
        private bool walkVisibleOnly;

        public ConditionTreeWalker(AutomationElement element, IQueryCondition condition, bool visibleOnly)
        {
            this.condition = condition;
            walkVisibleOnly = visibleOnly;
            virtualChildEnumerator = null;
            if (element != null && !ReferenceEquals(element.Current.ControlType, ControlType.Menu) && PatternHelper.GetItemContainerPattern(element) is ItemContainerPattern)
            {
                UiaElement uiaElement = UiaElementFactory.GetUiaElement(element, true);
                virtualChildEnumerator = new VirtualChildrenEnumerator(uiaElement, condition);
            }
        }

        private bool ElementPropertyMatch(AutomationElement element)
        {
            if (walkVisibleOnly && UiaUtility.GetAutomationPropertyValue<bool>(element, AutomationElement.IsOffscreenProperty))
            {
                return false;
            }
            UiaElement uiaElement = UiaElementFactory.GetUiaElement(element, true);
            return condition.Match(uiaElement);
        }

        public AutomationElement GetFirstChild(AutomationElement element, int timeOut)
        {
            if (virtualChildEnumerator != null)
            {
                virtualChildEnumerator.Reset();
                virtualChildEnumerator.MoveNext();
                UiaElement current = virtualChildEnumerator.Current as UiaElement;
                return current?.InnerElement;
            }
            context = VirtualizationContext.Disable;
            AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(element, context);
            return GetNextChildWithProperty(firstChild, timeOut);
        }

        private AutomationElement GetNextChildWithProperty(AutomationElement element, int timeOut)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (element != null && !ElementPropertyMatch(element))
            {
                element = TreeWalkerHelper.GetNextSibling(element, ref context);
                if (timeOut < stopwatch.ElapsedMilliseconds)
                {
                    
                    return null;
                }
            }
            return element;
        }

        public AutomationElement GetNextSibling(AutomationElement element, int timeOut)
        {
            if (virtualChildEnumerator != null)
            {
                virtualChildEnumerator.MoveNext();
                UiaElement current = virtualChildEnumerator.Current as UiaElement;
                return current?.InnerElement;
            }
            AutomationElement nextSibling = TreeWalkerHelper.GetNextSibling(element, ref context);
            return GetNextChildWithProperty(nextSibling, timeOut);
        }
    }
}

