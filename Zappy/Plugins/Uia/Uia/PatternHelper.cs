using System;
using System.Windows.Automation;
using System.Windows.Automation.Text;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class PatternHelper
    {
        private static T GetAutomationPattern<T>(AutomationElement element, AutomationPattern pattern, AutomationProperty isPatternAvailableProperty)
        {
            T local;
            try
            {
                if ((element != null) && (isPatternAvailableProperty != null))
                {
                    object obj3;
                    object currentPropertyValue = element.GetCurrentPropertyValue(isPatternAvailableProperty);
                    if ((((currentPropertyValue != null) && (currentPropertyValue != AutomationElement.NotSupported)) && ((currentPropertyValue is bool) && ((bool)currentPropertyValue))) && element.TryGetCurrentPattern(pattern, out obj3))
                    {
                        return (T)obj3;
                    }
                }
                local = default(T);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return local;
        }

        public static ExpandCollapsePattern GetExpandCollapsePattern(AutomationElement element, bool realize)
        {
            ExpandCollapsePattern pattern = GetAutomationPattern<ExpandCollapsePattern>(element, ExpandCollapsePattern.Pattern, AutomationElement.IsExpandCollapsePatternAvailableProperty);
            if ((pattern != null) & realize)
            {
                UiaUtility.RealizeElement(element);
            }
            return pattern;
        }

        public static InvokePattern GetInvokePattern(AutomationElement element, bool realize)
        {
            InvokePattern pattern = GetAutomationPattern<InvokePattern>(element, InvokePattern.Pattern, AutomationElement.IsInvokePatternAvailableProperty);
            if ((pattern != null) & realize)
            {
                UiaUtility.RealizeElement(element);
            }
            return pattern;
        }

        public static BasePattern GetItemContainerPattern(AutomationElement element)
        {
            if ((element != null) && (element.Current.ControlType != ControlType.Calendar))
            {
                return GetAutomationPattern<ItemContainerPattern>(element, ItemContainerPattern.Pattern, AutomationElement.IsItemContainerPatternAvailableProperty);
            }
            return null;
        }

        public static LegacyIAccessiblePattern GetLegacyIAccessiblePattern(AutomationElement element) =>
            GetAutomationPattern<LegacyIAccessiblePattern>(element, LegacyIAccessiblePattern.Pattern, AutomationElement.IsLegacyIAccessiblePatternAvailableProperty);

        internal static dynamic GetPatternValue<T>(BasePattern pattern, AutomationProperty property)
        {
            dynamic isReadOnly = default(T);
            if (pattern is ValuePattern)
            {
                ValuePattern pattern2 = pattern as ValuePattern;
                if (property == ValuePatternIdentifiers.ValueProperty)
                {
                    return pattern2.Current.Value;
                }
                if (property == ValuePatternIdentifiers.IsReadOnlyProperty)
                {
                    isReadOnly = pattern2.Current.IsReadOnly;
                }
                return isReadOnly;
            }
            if (pattern is TogglePattern)
            {
                TogglePattern pattern3 = pattern as TogglePattern;
                if (property == TogglePatternIdentifiers.ToggleStateProperty)
                {
                    isReadOnly = pattern3.Current.ToggleState;
                }
                return isReadOnly;
            }
            if (pattern is ExpandCollapsePattern)
            {
                ExpandCollapsePattern pattern4 = pattern as ExpandCollapsePattern;
                if (property == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty)
                {
                    isReadOnly = pattern4.Current.ExpandCollapseState;
                }
                return isReadOnly;
            }
            if (pattern is RangeValuePattern)
            {
                RangeValuePattern pattern5 = pattern as RangeValuePattern;
                if (property == RangeValuePatternIdentifiers.ValueProperty)
                {
                    return pattern5.Current.Value;
                }
                if (property == RangeValuePatternIdentifiers.IsReadOnlyProperty)
                {
                    isReadOnly = pattern5.Current.IsReadOnly;
                }
            }
            return isReadOnly;
        }

        public static RangeValuePattern GetRangeValuePattern(AutomationElement element) =>
            GetAutomationPattern<RangeValuePattern>(element, RangeValuePattern.Pattern, AutomationElement.IsRangeValuePatternAvailableProperty);

        public static ScrollItemPattern GetScrollItemPattern(AutomationElement element, bool realize)
        {
            ScrollItemPattern pattern = GetAutomationPattern<ScrollItemPattern>(element, ScrollItemPattern.Pattern, AutomationElement.IsScrollItemPatternAvailableProperty);
            if ((pattern != null) & realize)
            {
                UiaUtility.RealizeElement(element);
            }
            return pattern;
        }

        public static ScrollPattern GetScrollPattern(AutomationElement element) =>
            GetAutomationPattern<ScrollPattern>(element, ScrollPattern.Pattern, AutomationElement.IsScrollPatternAvailableProperty);

        public static AutomationElement[] GetSelection(SelectionPattern selectionPattern)
        {
            AutomationElement[] selection = null;
            if (selectionPattern != null)
            {
                selection = selectionPattern.Current.GetSelection();
            }
            return selection;
        }

        public static SelectionItemPattern GetSelectionItemPattern(AutomationElement element, bool realize)
        {
            SelectionItemPattern pattern = GetAutomationPattern<SelectionItemPattern>(element, SelectionItemPattern.Pattern, AutomationElement.IsSelectionItemPatternAvailableProperty);
            if ((pattern != null) & realize)
            {
                UiaUtility.RealizeElement(element);
            }
            return pattern;
        }

        public static SelectionPattern GetSelectionPattern(AutomationElement element) =>
            GetAutomationPattern<SelectionPattern>(element, SelectionPattern.Pattern, AutomationElement.IsSelectionPatternAvailableProperty);

        public static TableItemPattern GetTableItemPattern(AutomationElement element) =>
            GetAutomationPattern<TableItemPattern>(element, TableItemPattern.Pattern, AutomationElement.IsTableItemPatternAvailableProperty);

        public static TablePattern GetTablePattern(AutomationElement element) =>
            GetAutomationPattern<TablePattern>(element, TablePattern.Pattern, AutomationElement.IsTablePatternAvailableProperty);

        public static string GetText(TextPattern textPattern)
        {
            string text = null;
            if (textPattern != null)
            {
                TextPatternRange documentRange = textPattern.DocumentRange;
                if (documentRange != null)
                {
                    text = documentRange.GetText(-1);
                }
            }
            return text;
        }

        public static TextPattern GetTextPattern(AutomationElement element) =>
            GetAutomationPattern<TextPattern>(element, TextPattern.Pattern, AutomationElement.IsTextPatternAvailableProperty);

        public static TogglePattern GetTogglePattern(AutomationElement element) =>
            GetAutomationPattern<TogglePattern>(element, TogglePattern.Pattern, AutomationElement.IsTogglePatternAvailableProperty);

        public static ValuePattern GetValuePattern(AutomationElement element) =>
            GetAutomationPattern<ValuePattern>(element, ValuePattern.Pattern, AutomationElement.IsValuePatternAvailableProperty);

        public static BasePattern GetVirtualizedItemPattern(AutomationElement element) =>
            GetAutomationPattern<VirtualizedItemPattern>(element, VirtualizedItemPattern.Pattern, AutomationElement.IsVirtualizedItemPatternAvailableProperty);
    }

}

