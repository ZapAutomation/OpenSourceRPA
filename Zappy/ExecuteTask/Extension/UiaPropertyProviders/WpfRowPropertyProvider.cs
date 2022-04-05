using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfRowPropertyProvider : WpfControlPropertyProvider
    {
        public WpfRowPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfRow.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(WpfRow.PropertyNames.Cells, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return ALUtility.GetDescendantsByControlType(uiTaskControl, uiTaskControl.TechnologyName, ControlType.Cell, 1);
            }
            if (string.Equals(WpfRow.PropertyNames.Selected, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                SelectionItemPattern pattern = GetAutomationPattern<SelectionItemPattern>(uiTaskControl, SelectionItemPattern.Pattern, AutomationElement.IsSelectionItemPatternAvailableProperty);
                return pattern?.Current.IsSelected;
            }
            if (string.Equals(WpfRow.PropertyNames.Header, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls = ALUtility.GetDescendantsByControlType(uiTaskControl, uiTaskControl.TechnologyName, ControlType.RowHeader, 1);
                if (controls != null)
                {
                    return controls[0];
                }
                return null;
            }
            if (WpfRow.PropertyNames.CanSelectMultiple.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                SelectionPattern pattern2 = GetAutomationPattern<SelectionPattern>(uiTaskControl, SelectionPattern.Pattern, AutomationElement.IsSelectionPatternAvailableProperty);
                return pattern2?.Current.CanSelectMultiple;
            }
            if (!WpfRow.PropertyNames.RowIndex.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(uiTaskControl, propertyName);
            }
            ZappyTaskControl uiControl = new ZappyTaskControl(uiTaskControl)
            {
                SearchProperties = { [ZappyTaskControl.PropertyNames.ControlType] = ControlType.Cell.Name },
                MaxDepth = 1
            };
            try
            {
                uiControl.Find();
            }
            catch (ZappyTaskException)
            {
                uiControl = null;
            }
            if (uiControl != null)
            {
                try
                {
                    TableItemPattern pattern3 = GetAutomationPattern<TableItemPattern>(uiControl, TableItemPattern.Pattern, AutomationElement.IsTableItemPatternAvailableProperty);
                    if (pattern3 != null)
                    {
                        return pattern3.Current.Row;
                    }
                }
                catch (ElementNotAvailableException)
                {
                }
            }
            ALUtility.ThrowNotSupportedException(true);
            return 0;
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfRow);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfRow.PropertyNames.Cells, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfRow.PropertyNames.Header, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfRow.PropertyNames.CanSelectMultiple, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfRow.PropertyNames.Selected, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfRow.PropertyNames.RowIndex, new ZappyTaskPropertyDescriptor(typeof(int)));
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.Row;
    }
}

