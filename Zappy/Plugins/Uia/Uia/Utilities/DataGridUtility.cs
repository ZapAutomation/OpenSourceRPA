using System;
using System.Globalization;
using System.Windows.Automation;
using Zappy.ActionMap.Query;
using Zappy.Decode.Helper;
using Zappy.Decode.Mssa;
using Zappy.Properties;
using PropertyCondition = Zappy.ActionMap.HelperClasses.PropertyCondition;

namespace Zappy.Plugins.Uia.Uia.Utilities
{
    internal static class DataGridUtility
    {
        internal static string GetColumnHeaderString(UiaElement cellElement, out AutomationElement headerItem)
        {
            headerItem = null;
            if (cellElement == null || cellElement.ControlTypeName != ActionMap.HelperClasses.ControlType.Cell.Name)
            {
                return null;
            }
            if (UiaTechnologyManager.Instance != null && !UiaTechnologyManager.Instance.IsRecordingSession)
            {
                UiaUtility.RealizeElement(cellElement.InnerElement);
            }
            string name = string.Empty;
            try
            {
                TableItemPattern tableItemPattern = PatternHelper.GetTableItemPattern(cellElement.InnerElement);
                AutomationElement[] columnHeaderItems = null;
                if (tableItemPattern != null)
                {
                    columnHeaderItems = tableItemPattern.Current.GetColumnHeaderItems();
                }
                if (columnHeaderItems != null && columnHeaderItems.Length != 0 && columnHeaderItems[0] != null)
                {
                    headerItem = columnHeaderItems[0];
                    name = headerItem.Current.Name;
                }
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, cellElement, false);
                throw;
            }
            return name;
        }

        internal static string GetRowValue(UiaElement rowElement, out bool isPartialValue)
        {
            isPartialValue = false;
            if (rowElement == null || rowElement.ControlTypeName != ActionMap.HelperClasses.ControlType.Row.Name)
            {
                return null;
            }
            CommaListBuilder builder = new CommaListBuilder();
            try
            {
                VirtualizationContext unknown = VirtualizationContext.Unknown;
                AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(rowElement.InnerElement);
                int num = 0;
                while (firstChild != null && num++ < 20)
                {
                    UiaUtility.RealizeElement(firstChild);
                    if (IsDataGridCell(firstChild, firstChild.Current.ClassName, firstChild.Current.ControlType))
                    {
                        ValuePattern valuePattern = PatternHelper.GetValuePattern(firstChild);
                        if (valuePattern != null)
                        {
                            builder.AddValue(valuePattern.Current.Value);
                        }
                    }
                    firstChild = TreeWalkerHelper.GetNextSibling(firstChild, ref unknown);
                }
                if (firstChild != null)
                {
                    isPartialValue = true;
                }
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, rowElement, false);
                throw;
            }
            return builder.ToString();
        }

        internal static IQueryCondition GetSingleQueryIdForCell(UiaElement element)
        {
            AutomationElement element2;
            object[] args = { element };
            
            AndConditionBuilder builder = new AndConditionBuilder();
            bool flag = true;
            string columnHeaderString = GetColumnHeaderString(element, out element2);
            builder.Append("ColumnHeader", columnHeaderString);
            if (string.IsNullOrEmpty(columnHeaderString) || !IsColumnHeaderUniqueUnderTable(element, element2))
            {
                bool flag2;
                int propertyValue = TryGetColumnNumber(element, out flag2);
                if (flag2)
                {
                    flag = false;
                    builder.Append("ColumnIndex", propertyValue);
                }
            }
            if (element.HasValidAutomationId())
            {
                flag = false;
                builder.Append("AutomationId", element.AutomationId);
            }
            if (string.IsNullOrEmpty(columnHeaderString) & flag)
            {
                string str2 = element.Value;
                if (!string.IsNullOrEmpty(str2))
                {
                    builder.Append("Value", str2);
                }
            }
            builder.Append("ControlType", element.ControlTypeName);
            if (UiaUtility.IsWpfWindow(element.WindowHandle))
            {
                builder.Append("FrameworkId", element.FrameworkId);
            }
            return builder.Build();
        }

        internal static IQueryCondition GetSingleQueryIdForRow(UiaElement element)
        {
            object[] args = { element };
            
            if (element == null)
            {
                return null;
            }
            AndConditionBuilder builder = new AndConditionBuilder();
            builder.Append("ControlType", element.ControlTypeName);
            if (element.HasValidAutomationId())
            {
                builder.Append("AutomationId", element.AutomationId);
            }
            else if (!DataBoundControlIssue.HasNameIssue(element.InnerElement))
            {
                builder.Append("Name", element.Name);
            }
            else if (UiaTechnologyManager.Instance != null && UiaTechnologyManager.Instance.IsRecordingSession)
            {
                object[] objArray2 = { element.ControlTypeName };
                throw new ZappyTaskControlNotAvailableException(string.Format(CultureInfo.CurrentCulture, Resources.DataBoundNameIssueError, objArray2));
            }
            builder.Append("FrameworkId", element.FrameworkId);
            return builder.Build();
        }

        private static AutomationElement GetTableForCellOrRow(UiaElement element)
        {
            if (element == null || element.ControlTypeName != ActionMap.HelperClasses.ControlType.Cell && element.ControlTypeName != ActionMap.HelperClasses.ControlType.Row)
            {
                return null;
            }
            AutomationElement parent = TreeWalkerHelper.GetParent(element.InnerElement);
            for (int i = 0; i < 3 && parent != null && parent.Current.ControlType != ControlType.DataGrid; i++)
            {
                parent = TreeWalkerHelper.GetParent(parent);
            }
            if (parent != null && parent.Current.ControlType != ControlType.DataGrid)
            {
                parent = null;
            }
            return parent;
        }

        internal static bool IsCheckBoxPartOfDataGridCell(AutomationElement element)
        {
            AutomationElement parent = TreeWalkerHelper.GetParent(element);
            if (element == null || parent == null || element.Current.ControlType != ControlType.CheckBox || !IsDataGridCell(parent, parent.Current.ClassName, parent.Current.ControlType))
            {
                return false;
            }
            element = TreeWalkerHelper.GetFirstChild(parent);
            int num = 1;
            VirtualizationContext unknown = VirtualizationContext.Unknown;
            while (element != null)
            {
                if (num > 1)
                {
                    return false;
                }
                element = TreeWalkerHelper.GetNextSibling(element, ref unknown);
                num++;
            }
            return true;
        }

        private static bool IsColumnHeaderUniqueAmongPreviousSiblings(AutomationElement headerElement)
        {
            AutomationElement parent = TreeWalkerHelper.GetParent(headerElement);
            if (parent == null)
            {
                return false;
            }
            IQueryCondition condition = new PropertyCondition("Name", headerElement.Current.Name);
            condition.Conditions = new[] { condition };
            AutomationElement firstChild = new ConditionTreeWalker(parent, condition, false).GetFirstChild(parent, 100);
            return firstChild != null && Automation.Compare(headerElement, firstChild);
        }

        private static bool IsColumnHeaderUniqueUnderTable(UiaElement cellElement, AutomationElement headerElement)
        {
            if (headerElement != null && PatternHelper.GetVirtualizedItemPattern(cellElement.InnerElement) == null)
            {
                AutomationElement tableForCellOrRow = GetTableForCellOrRow(cellElement);
                TablePattern pattern = null;
                if (tableForCellOrRow == null || (pattern = PatternHelper.GetTablePattern(tableForCellOrRow)) == null)
                {
                    return IsColumnHeaderUniqueAmongPreviousSiblings(headerElement);
                }
                AutomationElement[] columnHeaders = pattern.Current.GetColumnHeaders();
                if (columnHeaders != null && columnHeaders.Length != 0)
                {
                    foreach (AutomationElement element2 in columnHeaders)
                    {
                        if (element2 != null && string.Equals(element2.Current.Name, headerElement.Current.Name, StringComparison.Ordinal))
                        {
                            return Automation.Compare(element2, headerElement);
                        }
                    }
                }
            }
            return false;
        }

        private static bool IsCustomDataGridCell(AutomationElement element)
        {
            TableItemPattern tableItemPattern = PatternHelper.GetTableItemPattern(element);
            return tableItemPattern?.Current.ContainingGrid != null && IsDataGridTable(tableItemPattern?.Current.ContainingGrid);
        }

        private static bool IsCustomDataGridRow(AutomationElement element, string className) =>
            !string.IsNullOrEmpty(className) && element.Current.ControlType == ControlType.DataItem && className.IndexOf("Row", StringComparison.OrdinalIgnoreCase) != -1;

        internal static bool IsDataGridCell(AutomationElement element, string uiaClassName, ControlType uiaControlType) =>
            !string.IsNullOrEmpty(uiaClassName) && uiaControlType == ControlType.DataItem && uiaClassName.IndexOf("Cell", StringComparison.OrdinalIgnoreCase) != -1 || uiaControlType == ControlType.Custom && (string.Equals(uiaClassName, "DataGridCell", StringComparison.Ordinal) || IsVirtualizedDataGridCell(element)) || IsCustomDataGridCell(element);

        internal static bool IsDataGridRow(AutomationElement element, string uiaClassName, ControlType uiaControlType) =>
            uiaControlType == ControlType.DataItem && (string.Equals(uiaClassName, "DataGridRow", StringComparison.Ordinal) || IsVirtualizedDataGridRow(element)) || IsCustomDataGridRow(element, uiaClassName);

        internal static bool IsDataGridTable(AutomationElement automationElement)
        {
            if (automationElement == null || automationElement.Current.ControlType != ControlType.DataGrid && automationElement.Current.ControlType != ControlType.Table)
            {
                return false;
            }
            return PatternHelper.GetTablePattern(automationElement) != null;
        }

        internal static bool IsElementNotTemplateContentOfCell(AutomationElement element)
        {
            AutomationElement parent = TreeWalkerHelper.GetParent(element);
            if (element == null || parent == null || element.Current.ControlType != ControlType.Text && element.Current.ControlType != ControlType.ComboBox && element.Current.ControlType != ControlType.Edit || !IsDataGridCell(parent, parent.Current.ClassName, parent.Current.ControlType))
            {
                return false;
            }
            element = TreeWalkerHelper.GetFirstChild(parent);
            int num = 1;
            VirtualizationContext unknown = VirtualizationContext.Unknown;
            while (element != null)
            {
                if (num > 1)
                {
                    return false;
                }
                element = TreeWalkerHelper.GetNextSibling(element, ref unknown);
                num++;
            }
            return true;
        }

        internal static bool IsElementPartOfWpfDataGrid(UiaElement element)
        {
            if (element == null || element.ControlTypeName != ActionMap.HelperClasses.ControlType.Cell && element.ControlTypeName != ActionMap.HelperClasses.ControlType.Row && element.ControlTypeName != ActionMap.HelperClasses.ControlType.Table)
            {
                return false;
            }
            return UiaUtility.IsWpfFrameWorkId(element.InnerElement);
        }

        private static bool IsVirtualizedDataGridCell(AutomationElement dataGridCel)
        {
            if (PatternHelper.GetVirtualizedItemPattern(dataGridCel) == null || dataGridCel.Current.ControlType != ControlType.Custom)
            {
                return false;
            }
            AutomationElement parent = TreeWalkerHelper.GetParent(dataGridCel);
            return parent != null && parent.Current.ControlType == ControlType.DataItem;
        }

        private static bool IsVirtualizedDataGridRow(AutomationElement dataGridRow)
        {
            if (PatternHelper.GetVirtualizedItemPattern(dataGridRow) == null || dataGridRow.Current.ControlType != ControlType.DataItem)
            {
                return false;
            }
            AutomationElement parent = TreeWalkerHelper.GetParent(dataGridRow);
            return parent != null && parent.Current.ControlType == ControlType.DataGrid;
        }

        internal static int TryGetColumnNumber(UiaElement cellElement, out bool hasPattern)
        {
            hasPattern = false;
            if (cellElement == null || cellElement.ControlTypeName != ActionMap.HelperClasses.ControlType.Cell.Name)
            {
                return 0;
            }
            if (UiaTechnologyManager.Instance != null && !UiaTechnologyManager.Instance.IsRecordingSession)
            {
                UiaUtility.RealizeElement(cellElement.InnerElement);
            }
            try
            {
                TableItemPattern tableItemPattern = PatternHelper.GetTableItemPattern(cellElement.InnerElement);
                if (tableItemPattern != null)
                {
                    hasPattern = true;
                    return tableItemPattern.Current.Column;
                }
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, cellElement, false);
                throw;
            }
            return 0x7fffffff;
        }
    }
}

