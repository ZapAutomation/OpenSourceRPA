using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfTablePropertyProvider : WpfControlPropertyProvider
    {
        public WpfTablePropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override int GetControlSupportLevel(ZappyTaskControl uiControl)
        {
            string name = uiControl.ControlType.Name;
            if (ControlType.Table.NameEquals(name) && string.Equals(uiControl.TechnologyName, "UIA", StringComparison.OrdinalIgnoreCase))
            {
                return 0xc7;
            }
            return 0;
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfTable.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiControl, string propertyName)
        {
            string technologyName = uiControl.TechnologyName;
            if (string.Equals(WpfTable.PropertyNames.ColumnCount, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                TablePattern pattern = GetAutomationPattern<TablePattern>(uiControl, TablePattern.Pattern, AutomationElement.IsTablePatternAvailableProperty);
                return pattern?.Current.ColumnCount;
            }
            if (string.Equals(WpfTable.PropertyNames.RowCount, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                TablePattern pattern2 = GetAutomationPattern<TablePattern>(uiControl, TablePattern.Pattern, AutomationElement.IsTablePatternAvailableProperty);
                return pattern2?.Current.RowCount;
            }
            if (string.Equals(WpfTable.PropertyNames.RowHeaders, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                TablePattern pattern3 = GetAutomationPattern<TablePattern>(uiControl, TablePattern.Pattern, AutomationElement.IsTablePatternAvailableProperty);
                if (pattern3 == null)
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                AutomationElement[] rowHeaders = pattern3.Current.GetRowHeaders();
                ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
                if (pattern3.Current.RowCount > rowHeaders.Length)
                {
                    ZappyTaskControlCollection controls2 = null;
                    if ((bool)uiControl.TechnologyElement.GetPropertyValue(WpfTable.PropertyNames.IsGroupedTable))
                    {
                        controls2 = ALUtility.GetDescendantsByControlType(uiControl, technologyName, ControlType.Row, 2);
                    }
                    else
                    {
                        controls2 = ALUtility.GetDescendantsByControlType(uiControl, technologyName, ControlType.Row, 1);
                    }
                    foreach (ZappyTaskControl control in controls2)
                    {
                        controls.Add(control.GetProperty(WpfRow.PropertyNames.Header) as ZappyTaskControl);
                    }
                    return controls;
                }
                foreach (AutomationElement element in rowHeaders)
                {
                    controls.Add(ZappyTaskControlFactory.FromNativeElement(element, technologyName));
                }
                return controls;
            }
            if (string.Equals(WpfTable.PropertyNames.ColumnHeaders, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                TablePattern pattern4 = GetAutomationPattern<TablePattern>(uiControl, TablePattern.Pattern, AutomationElement.IsTablePatternAvailableProperty);
                if (pattern4 == null)
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
                AutomationElement[] columnHeaders = pattern4.Current.GetColumnHeaders();
                ZappyTaskControlCollection controls3 = new ZappyTaskControlCollection();
                foreach (AutomationElement element2 in columnHeaders)
                {
                    controls3.Add(ZappyTaskControlFactory.FromNativeElement(element2, technologyName));
                }
                return controls3;
            }
            if (WpfTable.PropertyNames.CanSelectMultiple.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                SelectionPattern pattern5 = GetAutomationPattern<SelectionPattern>(uiControl, SelectionPattern.Pattern, AutomationElement.IsSelectionPatternAvailableProperty);
                return pattern5?.Current.CanSelectMultiple;
            }
            if (string.Equals(WpfTable.PropertyNames.Rows, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if ((bool)uiControl.TechnologyElement.GetPropertyValue(WpfTable.PropertyNames.IsGroupedTable))
                {
                    return ALUtility.GetDescendantsByControlType(uiControl, technologyName, ControlType.Row, 2);
                }
                return ALUtility.GetDescendantsByControlType(uiControl, technologyName, ControlType.Row, 1);
            }
            if (!string.Equals(WpfTable.PropertyNames.Cells, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(uiControl, propertyName);
            }
            ZappyTaskControlCollection controls4 = null;
            if ((bool)uiControl.TechnologyElement.GetPropertyValue(WpfTable.PropertyNames.IsGroupedTable))
            {
                controls4 = ALUtility.GetDescendantsByControlType(uiControl, technologyName, ControlType.Row, 2);
            }
            else
            {
                controls4 = ALUtility.GetDescendantsByControlType(uiControl, technologyName, ControlType.Row, 1);
            }
            ZappyTaskControlCollection controls5 = new ZappyTaskControlCollection();
            foreach (ZappyTaskControl control2 in controls4)
            {
                foreach (ZappyTaskControl control3 in ALUtility.GetDescendantsByControlType(control2, technologyName, ControlType.Cell, 1))
                {
                    controls5.Add(control3);
                }
            }
            return controls5;
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfTable);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfTable.PropertyNames.Rows, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfTable.PropertyNames.RowHeaders, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfTable.PropertyNames.ColumnHeaders, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfTable.PropertyNames.Cells, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfTable.PropertyNames.RowCount, new ZappyTaskPropertyDescriptor(typeof(int)));
            m_ControlSpecificProperties.Add(WpfTable.PropertyNames.ColumnCount, new ZappyTaskPropertyDescriptor(typeof(int)));
            m_ControlSpecificProperties.Add(WpfTable.PropertyNames.CanSelectMultiple, new ZappyTaskPropertyDescriptor(typeof(bool)));
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.Table;
    }
}

