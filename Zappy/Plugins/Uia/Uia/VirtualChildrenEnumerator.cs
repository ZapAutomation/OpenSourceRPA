using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.Query;
using Zappy.ExecuteTask.Helpers.Interface;
using PropertyCondition = Zappy.ActionMap.HelperClasses.PropertyCondition;

namespace Zappy.Plugins.Uia.Uia
{
    [Serializable]
    internal class VirtualChildrenEnumerator : IEnumerator, IDisposable
    {
        private List<AncestorExpandState> ancestorExpandStates;
        private IQueryCondition condition;
        private ITaskActivityElement current;
        [NonSerialized]
        private AutomationElement currentChild;
        private static List<AutomationProperty> defaultPriorityList;
        private bool disposed;
        [NonSerialized]
        private PropertyCondition highestPrioritySearchCondition;
        [NonSerialized]
        private ItemContainerPattern itemContainerPattern;
        [NonSerialized]
        private ControlType itemControlType;
        private UiaElement parent;
        private static Dictionary<ControlType, List<AutomationProperty>> priorityMap;
        private List<PropertyCondition> searchConditions;

        public VirtualChildrenEnumerator(UiaElement element, object parsedQueryIdCookie)
        {
            parent = element;
            condition = parsedQueryIdCookie as IQueryCondition;
            if (priorityMap == null)
            {
                InitializePriorityMap();
            }
            if (condition != null)
            {
                SetSearchPriority();
            }
            itemContainerPattern = parent.InnerElement.GetCurrentPattern(ItemContainerPattern.Pattern) as ItemContainerPattern;
            AutomationElement element2 = itemContainerPattern.FindItemByProperty(null, null, 0);
            if (element2 != null)
            {
                itemControlType = element2.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
            }
            ancestorExpandStates = new List<AncestorExpandState>();
            for (AutomationElement element3 = parent.InnerElement; element3 != null; element3 = TreeWalkerHelper.GetParent(element3))
            {
                ExpandCollapsePattern expandCollapsePattern = PatternHelper.GetExpandCollapsePattern(element3, true);
                if (expandCollapsePattern == null)
                {
                    break;
                }
                ancestorExpandStates.Add(new AncestorExpandState(expandCollapsePattern));
            }
            Reset();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        private AutomationElement GetFirstChildInNavigationTree()
        {
            VirtualizationContext enable = VirtualizationContext.Enable;
            AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(parent.InnerElement);
            while (firstChild != null)
            {
                ControlType currentPropertyValue = firstChild.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                if (UiaUtility.MatchUiaControlType(currentPropertyValue, itemControlType))
                {
                    return null;
                }
                if (MatchEnumeratorSearchCondition(firstChild, true))
                {
                    return firstChild;
                }
                firstChild = TreeWalkerHelper.GetNextSibling(firstChild, ref enable);
            }
            return firstChild;
        }

        internal static void InitializePriorityMap()
        {
            if (defaultPriorityList == null)
            {
                List<AutomationProperty> list1 = new List<AutomationProperty> {
                    AutomationElement.NameProperty,
                    AutomationElement.AutomationIdProperty,
                    AutomationElement.ControlTypeProperty,
                    SelectionItemPattern.IsSelectedProperty
                };
                defaultPriorityList = list1;
                priorityMap = new Dictionary<ControlType, List<AutomationProperty>>();
                List<AutomationProperty> list = new List<AutomationProperty> {
                    AutomationElement.NameProperty,
                    AutomationElement.AutomationIdProperty,
                    SelectionItemPattern.IsSelectedProperty
                };
                priorityMap.Add(ControlType.Tab, list);
            }
        }

        private bool MatchEnumeratorSearchCondition(AutomationElement element, bool matchHighestPriorityCondition)
        {
            bool flag = false;
            UiaElement uiaElement = UiaElementFactory.GetUiaElement(element, false);
            if (uiaElement != null && (!matchHighestPriorityCondition || highestPrioritySearchCondition == null || highestPrioritySearchCondition.Match(uiaElement)))
            {
                flag = true;
                if (searchConditions == null)
                {
                    return flag;
                }
                foreach (PropertyCondition condition in searchConditions)
                {
                    if (!condition.Match(uiaElement))
                    {
                        return false;
                    }
                }
            }
            return flag;
        }

        public bool MoveNext()
        {
            Search();
            UiaElement uiaElement = UiaElementFactory.GetUiaElement(currentChild, false);
            if (uiaElement != null)
            {
                uiaElement.Parent = parent;
            }
            current = uiaElement;
            if (current == null)
            {
                foreach (AncestorExpandState state in ancestorExpandStates)
                {
                    if (state.Pattern.Current.ExpandCollapseState != state.State)
                    {
                        if (state.State == ExpandCollapseState.Expanded)
                        {
                            state.Pattern.Expand();
                        }
                        else
                        {
                            state.Pattern.Collapse();
                        }
                    }
                }
            }
            return current != null;
        }

        public void Reset()
        {
            currentChild = null;
        }

        private void Search()
        {
            if (this.currentChild == null)
            {
                this.currentChild = GetFirstChildInNavigationTree();
                if (this.currentChild != null)
                {
                    return;
                }
            }
            AutomationProperty property = highestPrioritySearchCondition != null ? PropertyNames.nameToAutomationProperty[highestPrioritySearchCondition.PropertyName] : null;
            object automationControlTypeFromName = highestPrioritySearchCondition != null ? highestPrioritySearchCondition.Value : 0;
            if (property != null && property.Equals(AutomationElement.ControlTypeProperty))
            {
                automationControlTypeFromName = PropertyNames.GetAutomationControlTypeFromName(automationControlTypeFromName as string);
            }
            AutomationElement currentChild = this.currentChild;
            this.currentChild = null;
            ControlType controlType = null;
            AutomationElement element2 = null;
            bool flag = false;
            bool flag2 = false;
            while (!flag2)
            {
                VirtualizationContext enable = VirtualizationContext.Enable;
                if (currentChild != null)
                {
                    controlType = currentChild.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                    if (!UiaUtility.MatchUiaControlType(controlType, itemControlType))
                    {
                        flag = true;
                        currentChild = TreeWalkerHelper.GetNextSibling(currentChild, ref enable);
                        if (currentChild != null)
                        {
                            controlType = currentChild.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
                            if (UiaUtility.MatchUiaControlType(controlType, itemControlType))
                            {
                                goto Label_011D;
                            }
                            if (MatchEnumeratorSearchCondition(currentChild, true))
                            {
                                this.currentChild = currentChild;
                                flag2 = true;
                            }
                            continue;
                        }
                        this.currentChild = null;
                        return;
                    }
                }
            Label_011D:
                if (flag)
                {
                    element2 = currentChild;
                    flag = false;
                    currentChild = itemContainerPattern.FindItemByProperty(null, property, automationControlTypeFromName);
                }
                else
                {
                    currentChild = itemContainerPattern.FindItemByProperty(currentChild, property, automationControlTypeFromName);
                }
                if (currentChild != null)
                {
                    if (MatchEnumeratorSearchCondition(currentChild, false))
                    {
                        this.currentChild = currentChild;
                        return;
                    }
                }
                else
                {
                    this.currentChild = null;
                    return;
                }
            }
        }

        private void SetSearchPriority()
        {
            AutomationProperty property;
            List<AutomationProperty> defaultPriorityList;
            searchConditions = new List<PropertyCondition>();
            highestPrioritySearchCondition = null;
            ControlType currentPropertyValue = parent.InnerElement.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
            if (!priorityMap.TryGetValue(currentPropertyValue, out defaultPriorityList) || defaultPriorityList == null)
            {
                defaultPriorityList = VirtualChildrenEnumerator.defaultPriorityList;
            }
            foreach (AutomationProperty property2 in defaultPriorityList)
            {
                foreach (PropertyCondition condition in this.condition.Conditions)
                {
                    if (condition.PropertyOperator != PropertyConditionOperator.Contains && PropertyNames.nameToAutomationProperty.TryGetValue(condition.PropertyName, out property) && property2.Equals(property))
                    {
                        highestPrioritySearchCondition = condition;
                        break;
                    }
                }
                if (highestPrioritySearchCondition != null)
                {
                    break;
                }
            }
            List<PropertyCondition> collection = new List<PropertyCondition>();
            foreach (PropertyCondition condition2 in this.condition.Conditions)
            {
                if (PropertyNames.nameToAutomationProperty.TryGetValue(condition2.PropertyName, out property))
                {
                    if (defaultPriorityList.Contains(property))
                    {
                        if (highestPrioritySearchCondition == null || !condition2.Equals(highestPrioritySearchCondition))
                        {
                            searchConditions.Add(condition2);
                        }
                    }
                    else
                    {
                        collection.Add(condition2);
                    }
                }
            }
            if (collection.Count > 0)
            {
                searchConditions.AddRange(collection);
            }
        }

        public object Current =>
            current;

        private class AncestorExpandState
        {
            private ExpandCollapsePattern expandCollapsePattern;
            private ExpandCollapseState expandCollapseState;

            public AncestorExpandState(ExpandCollapsePattern pattern)
            {
                expandCollapsePattern = pattern;
                expandCollapseState = pattern.Current.ExpandCollapseState;
            }

            public ExpandCollapsePattern Pattern =>
                expandCollapsePattern;

            public ExpandCollapseState State =>
                expandCollapseState;
        }
    }
}

