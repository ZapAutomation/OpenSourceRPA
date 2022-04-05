using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Mssa;
using Zappy.Properties;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;
using DataGridUtility = Zappy.Plugins.Uia.Uia.Utilities.DataGridUtility;
using PropertyCondition = Zappy.ActionMap.HelperClasses.PropertyCondition;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class QueryIdHelper
    {
        private const string ContextMenuClassName = "Uia.ContextMenu";
        private const int DefaultLevelsForItemContainer = 2;
        private const string DocumentViewerClassName = "Uia.DocumentViewer";
        private const string ExpanderClassName = "Uia.Expander";
        private const string FrameClassName = "Uia.Frame";
        private const string GroupBoxClassName = "Uia.GroupBox";
        private const string HubClassName = "Hub";
        private const string HubSectionClassName = "HubSection";
        private static int? maxLevelsForItemContainer;
        private const int MaxNamelessElementNavigation = 30;
        private const int MaxParentNavigation = 15;

        private static void AddClassNameCondition(UiaElement element, AndConditionBuilder andConditionBuilder)
        {
            string className = element.ClassName;
            PropertyConditionOperator equalTo = PropertyConditionOperator.EqualTo;
            if (!string.IsNullOrEmpty(className) && className.StartsWith("Uia.", StringComparison.OrdinalIgnoreCase))
            {
                className = className.Substring("Uia.".Length);
                className = "Uia." + ZappyTaskUtilities.NormalizeDynamicClassName(className);
                if (!string.Equals(element.ClassName, className, StringComparison.OrdinalIgnoreCase))
                {
                    equalTo = PropertyConditionOperator.Contains;
                }
            }
            andConditionBuilder.Append(new PropertyCondition("ClassName", className, equalTo));
        }

        internal static bool ElementHasGoodQueryId(UiaElement element)
        {
            if (!element.HasValidAutomationId())
            {
                return GetPropertyConditionFromFirstValidPropertyValue(element) != null;
            }
            return true;
        }

        private static bool ForceAppendParentQueryId(UiaElement element)
        {
            if (!ControlType.TreeItem.NameEquals(element.ControlTypeName) && !ControlType.MenuItem.NameEquals(element.ControlTypeName) && !ControlType.ListItem.NameEquals(element.ControlTypeName) && !ControlType.Cell.NameEquals(element.ControlTypeName) && (!ControlType.ColumnHeader.NameEquals(element.ControlTypeName) && !ControlType.Row.NameEquals(element.ControlTypeName)) && !ControlType.RowHeader.NameEquals(element.ControlTypeName) && !ControlType.TabPage.NameEquals(element.ControlTypeName) && (element.Parent == null || !ControlType.Cell.NameEquals(element.Parent.ControlTypeName)) && !ControlType.ScrollBar.NameEquals(element.ControlTypeName) && (element.Parent == null || !ControlType.ScrollBar.NameEquals(element.Parent.ControlTypeName)))
            {
                return ControlType.Menu.NameEquals(element.ControlTypeName) && !ElementHasGoodQueryId(element) && !string.Equals(element.ClassName, "Uia.ContextMenu");
            }
            return true;
        }

        private static int GetInstanceInAncestor(UiaElement ancestor, UiaElement element, IQueryCondition PropertyCondition, bool ancestorIsParent)
        {
            bool matched = false;
            int num = 0;
            if (ancestorIsParent)
            {
                if (ancestor.ControlTypeName == ControlType.Calendar && (!string.IsNullOrEmpty(element.Name) || !string.IsNullOrEmpty(element.AutomationId)))
                {
                    return num;
                }
                num = GetInstanceInParent(ancestor, element, PropertyCondition, out matched);
                if (!matched)
                {
                    
                }
            }
            return num;
        }

        private static int GetInstanceInParent(UiaElement probableParent, UiaElement element, IQueryCondition PropertyCondition, out bool matched)
        {
            matched = false;
            int num = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            ConditionTreeWalker walker = new ConditionTreeWalker(probableParent.InnerElement, PropertyCondition, false);
            for (AutomationElement element2 = walker.GetFirstChild(probableParent.InnerElement, 500); element2 != null; element2 = walker.GetNextSibling(element2, 500))
            {
                num++;
                if (Automation.Compare(element.InnerElement, element2))
                {
                    matched = true;
                    break;
                }
                if (stopwatch.ElapsedMilliseconds > 500L)
                {
                    
                    break;
                }
            }
            if (!matched)
            {
                num = 0;
            }
            stopwatch.Stop();
            return num;
        }

        public static IQueryCondition GetPropertyConditionFromFirstValidPropertyValue(UiaElement element)
        {
            foreach (string str in PropertyNames.SingleQueryConditonProperties)
            {
                object propertyValue = element.GetPropertyValue(str);
                if (propertyValue != null)
                {
                    string str2 = propertyValue as string;
                    if (str2 == null || !string.IsNullOrEmpty(str2))
                    {
                        if (string.Equals(str, "Name", StringComparison.OrdinalIgnoreCase) && DataBoundControlIssue.HasNameIssue(element.InnerElement))
                        {
                            if (UiaTechnologyManager.Instance.IsRecordingSession && element.ControlTypeName == ControlType.ListItem)
                            {
                                object[] args = { ControlType.ListItem.FriendlyName };
                                throw new ZappyTaskControlNotAvailableException(string.Format(CultureInfo.CurrentCulture, Resources.DataBoundNameIssueError, args));
                            }
                        }
                        else
                        {
                            return new PropertyCondition(str, propertyValue);
                        }
                    }
                }
            }
            return null;
        }

        public static IQueryElement GetQueryId(UiaElement element)
        {
                        {
                return GetQueryId(element, true);
            }
        }

        private static IQueryElement GetQueryId(UiaElement element, bool appendParent)
        {
            object[] args = { appendParent, element };
            
            IQueryElement queryId = new QueryElement();
            List<string> list = new List<string>();
            bool flag = false;
            if (UiaUtility.IsBoundaryElement(element) || UiaUtility.IsPopUpWithTitleBar(element.NativeWindowHandle))
            {
                GetWpfWindowQueryCondition(element, ref queryId);
                return queryId;
            }
            if (element.IsCeilingElement)
            {
                return new QueryElement { Condition = GetSingleQueryCondition(null, element, false, -1) };
            }
            bool flag3 = ForceAppendParentQueryId(element);
            appendParent |= flag3;
            UiaElement ancestor = null;
            bool ancestorIsParent = false;
            UiaElement nextToElement = null;
            int elementInstance = 1;
            bool flag5 = false;
            bool flag6 = false;
            if (!appendParent)
            {
                flag5 = (ancestor = GetSpecialIntermediate(element.Parent)) != null;
                if (ancestor != null && Automation.Compare(element.Parent.InnerElement, ancestor.InnerElement))
                {
                    ancestorIsParent = true;
                }
                else
                {
                    ancestorIsParent = false;
                }
            }
            else
            {
                UiaElement parent = element.Parent;
                if (element.ControlTypeName == ControlType.MenuItem && UiaTechnologyManager.Instance != null)
                {
                    flag = UiaTechnologyManager.Instance.DoMenuItemStitch(element, ref parent);
                }
                bool flag7 = DataGridUtility.IsElementPartOfWpfDataGrid(element);
                if (parent != null && !UiaUtility.IsWindowOfSupportedTechnology(parent) && !flag7 && !flag3 && PatternHelper.GetItemContainerPattern(parent.InnerElement) == null && !ElementHasGoodQueryId(element))
                {
                    UiaElement element7 = TryGetNextToElementForPoorQueryIdElement(element, ref nextToElement, ref elementInstance);
                    if (element7 != null)
                    {
                        parent = element7;
                    }
                }
                ancestor = parent;
                if (!flag)
                {
                    while (parent != null && !UiaUtility.IsBoundaryElement(parent))
                    {
                        if (UiaUtility.IsElementOfItemType(parent.InnerElement) || PatternHelper.GetItemContainerPattern(parent.InnerElement) != null || DataGridUtility.IsElementPartOfWpfDataGrid(parent))
                        {
                            ancestor = parent;
                            break;
                        }
                        if (ElementHasGoodQueryId(parent) && !flag3)
                        {
                            ancestor = parent;
                            break;
                        }
                        parent = parent.Parent;
                    }
                }
                else
                {
                    ancestorIsParent = true;
                    goto Label_01BD;
                }
                ancestorIsParent = ancestor == element.Parent;
            }
        Label_01BD:
            queryId.Condition = GetSingleQueryCondition(ancestor, element, ancestorIsParent, elementInstance);
            if (ancestor != null && UiaUtility.IsWindowOfSupportedTechnology(ancestor))
            {
                ancestor = null;
                ancestorIsParent = false;
            }
            if ((ancestor != null) & flag5)
            {
                ancestor.QueryIdInternal = new QueryElement();
                ancestor.QueryIdInternal.Condition = GetSingleQueryCondition(null, ancestor, false, 1);
                queryId.Ancestor = ancestor;
            }
            else if (ancestor != null && !ControlType.Window.NameEquals(element.ControlTypeName))
            {
                if (!ancestor.IsCeilingElement)
                {
                    AutomationElement innerElement = ancestor.InnerElement;
                    for (int i = 0; i < MaxLevelsForItemContainer; i++)
                    {
                        innerElement = TreeWalkerHelper.GetParent(innerElement);
                        if (innerElement == null)
                        {
                            break;
                        }
                        if (IsItemContainer(innerElement))
                        {
                            flag6 = true;
                            break;
                        }
                    }
                    ancestor.CeilingElement = element.CeilingElement;
                    ancestor.QueryIdInternal = GetQueryId(ancestor, flag6);
                }
                if (nextToElement != null)
                {
                    nextToElement.CeilingElement = element.CeilingElement;
                    nextToElement.QueryIdInternal = GetQueryId(nextToElement, false);
                    nextToElement.QueryIdInternal.Ancestor = ancestor;
                    list.Add(SearchConfiguration.NextSibling);
                    queryId.Ancestor = nextToElement;
                }
                else
                {
                    queryId.Ancestor = ancestor;
                }
            }
            if (list.Count > 0)
            {
                queryId.SearchConfigurations = list.ToArray();
            }
            return queryId;
        }

        private static IQueryCondition GetSingleQueryCondition(UiaElement ancestor, UiaElement element, bool ancestorIsParent, int nextToInstance)
        {
            object[] args = { ancestor, element, ancestorIsParent, nextToInstance };
            
            if (DataGridUtility.IsElementPartOfWpfDataGrid(element))
            {
                if (element.ControlTypeName == ControlType.Cell.Name)
                {
                    return DataGridUtility.GetSingleQueryIdForCell(element);
                }
                if (element.ControlTypeName == ControlType.Row.Name)
                {
                    return DataGridUtility.GetSingleQueryIdForRow(element);
                }
            }
            AndConditionBuilder andConditionBuilder = new AndConditionBuilder();
            string frameworkId = element.FrameworkId;
            if (!string.IsNullOrEmpty(frameworkId))
            {
                andConditionBuilder.Append("FrameworkId", frameworkId);
            }
            andConditionBuilder.Append("ControlType", element.ControlTypeName);
            if (ControlType.Custom.NameEquals(element.ControlTypeName) || ControlType.Pane.NameEquals(element.ControlTypeName) || string.Equals(element.ClassName, "Uia.ContextMenu", StringComparison.OrdinalIgnoreCase))
            {
                AddClassNameCondition(element, andConditionBuilder);
            }
            if (element.HasValidAutomationId() && !UiaUtility.isTitleBarButton(ancestor))
            {
                andConditionBuilder.Append("AutomationId", element.AutomationId);
            }
            else
            {
                IQueryCondition propertyConditionFromFirstValidPropertyValue = GetPropertyConditionFromFirstValidPropertyValue(element);
                if (propertyConditionFromFirstValidPropertyValue != null)
                {
                    andConditionBuilder.Append(propertyConditionFromFirstValidPropertyValue);
                }
            }
            int propertyValue = -1;
            if (ancestor == null)
            {
                goto Label_015E;
            }
            if (nextToInstance == 1)
            {
                                {
                    propertyValue = GetInstanceInAncestor(ancestor, element, andConditionBuilder.Build(), ancestorIsParent);
                    goto Label_0149;
                }
            }
            propertyValue = nextToInstance;
        Label_0149:
            if (propertyValue > 1)
            {
                andConditionBuilder.Append("Instance", propertyValue);
            }
        Label_015E:
            return andConditionBuilder.Build();
        }

        private static UiaElement GetSpecialIntermediate(UiaElement element)
        {
            UiaElement element2 = null;
                        {
                for (int i = 0; i < 15 && element != null; i++)
                {
                    if (ZappyTaskUtilities.IsPhone && UiaUtility.GetAutomationPropertyValue<System.Windows.Automation.ControlType>(element.InnerElement, AutomationElement.ControlTypeProperty) == System.Windows.Automation.ControlType.Custom && element.Parent == null)
                    {
                        return element2;
                    }
                    if (IsSpecialIntermediate(element))
                    {
                        return element;
                    }
                    element = element.Parent;
                }
            }
            return element2;
        }

        private static void GetWpfWindowQueryCondition(UiaElement element, ref IQueryElement queryId)
        {
            AndConditionBuilder builder = new AndConditionBuilder();
            builder.Append(new PropertyCondition("ControlType", element.ControlTypeName));
            string name = element.Name;
            if (!string.IsNullOrEmpty(name))
            {
                builder.Append(new PropertyCondition("Name", name));
            }
            builder.Append("FrameworkId", element.FrameworkId);
            if (UiaUtility.IsChildOfDesktop(element))
            {
                string className = NativeMethods.GetClassName(element.WindowHandle);
                if (ZappyTaskUtilities.IsImmersiveWindowClassName(className) || UiaUtility.CheckIfCharmsBar(className, element.WindowHandle))
                {
                    builder.Append(new PropertyCondition("ClassName", className));
                }
                else
                {
                    builder.Append(new PropertyCondition("ClassName", "HwndWrapper", PropertyConditionOperator.Contains));
                    int orderOfInvocation = OrderOfInvoke.GetOrderOfInvocation(element, builder.Build());
                    if (orderOfInvocation > 1)
                    {
                        PropertyCondition condition = new PropertyCondition("OrderOfInvocation", orderOfInvocation);
                        IQueryCondition[] conditions = { condition };
                        builder.Append(new FilterCondition(conditions));
                    }
                }
            }
            queryId.Condition = builder.Build();
        }

        private static bool IsItemContainer(AutomationElement element) =>
            PatternHelper.GetItemContainerPattern(element) != null;

        private static bool IsSpecialIntermediate(UiaElement element)
        {
            System.Windows.Automation.ControlType automationPropertyValue = UiaUtility.GetAutomationPropertyValue<System.Windows.Automation.ControlType>(element.InnerElement, AutomationElement.ControlTypeProperty);
            if (automationPropertyValue != System.Windows.Automation.ControlType.Tab && automationPropertyValue != System.Windows.Automation.ControlType.DataItem && automationPropertyValue != System.Windows.Automation.ControlType.Table && automationPropertyValue != System.Windows.Automation.ControlType.DataGrid && automationPropertyValue != System.Windows.Automation.ControlType.Custom && automationPropertyValue != System.Windows.Automation.ControlType.TreeItem && automationPropertyValue != System.Windows.Automation.ControlType.MenuItem && (automationPropertyValue != System.Windows.Automation.ControlType.Group || !string.Equals(element.ClassName, "Uia.Expander", StringComparison.OrdinalIgnoreCase)) && (automationPropertyValue != System.Windows.Automation.ControlType.Group || !string.Equals(element.ClassName, "Uia.GroupBox", StringComparison.OrdinalIgnoreCase)) && (automationPropertyValue != System.Windows.Automation.ControlType.Pane || !string.Equals(element.ClassName, "Uia.Frame", StringComparison.OrdinalIgnoreCase)))
            {
                return automationPropertyValue == System.Windows.Automation.ControlType.Document && string.Equals(element.ClassName, "Uia.DocumentViewer", StringComparison.OrdinalIgnoreCase);
            }
            return true;
        }

        internal static UiaElement TryGetNextToElementForPoorQueryIdElement(UiaElement element, ref UiaElement nextToElement, ref int elementInstance)
        {
            UiaElement parentPrevious = null;
            UiaElement element3 = null;
            string controlTypeName = element.ControlTypeName;
            string className = element.ClassName;
            for (int i = 0; i < 30; i++)
            {
                bool flag;
                UiaElement element4 = UiaUtility.GetPreviousSiblingFlattened(element, out parentPrevious, out flag);
                if (flag)
                {
                    element3 = parentPrevious;
                }
                if (element4 != null)
                {
                    if (!ElementHasGoodQueryId(element4))
                    {
                        if (ControlType.NameComparer.Equals(element4.ControlTypeName, controlTypeName) && (!ControlType.Custom.NameEquals(controlTypeName) || string.Equals(className, element4.ClassName, StringComparison.OrdinalIgnoreCase)))
                        {
                            elementInstance++;
                        }
                        goto Label_0076;
                    }
                    nextToElement = element4;
                }
                break;
            Label_0076:
                element = element4;
            }
            if (element3 != null)
            {
                parentPrevious = element3;
            }
            if (nextToElement == null && elementInstance == 1)
            {
                
                parentPrevious = null;
            }
            else if (elementInstance == 1)
            {
                elementInstance = -1;
            }
            if (parentPrevious == null)
            {
                nextToElement = null;
                elementInstance = 1;
            }
            return parentPrevious;
        }

        private static int MaxLevelsForItemContainer
        {
            get
            {
                if (!maxLevelsForItemContainer.HasValue)
                {
                    string str;
                    int result = 2;
                    if (ZappyTaskUtilities.GetAppSettings().TryGetValue("MaxLevelsForItemContainer", out str) && !int.TryParse(str, out result))
                    {
                        result = 2;
                    }
                    maxLevelsForItemContainer = result;
                }
                return maxLevelsForItemContainer.Value;
            }
        }
    }
}

