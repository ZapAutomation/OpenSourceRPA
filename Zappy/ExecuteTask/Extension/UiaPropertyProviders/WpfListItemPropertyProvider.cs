using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfListItemPropertyProvider : WpfControlPropertyProvider
    {
        public WpfListItemPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfListItem.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfListItem.PropertyNames.Selected, StringComparison.OrdinalIgnoreCase))
            {
                return GetAutomationPattern<SelectionItemPattern>(uiTaskControl, SelectionItemPattern.Pattern, AutomationElement.IsSelectionItemPatternAvailableProperty).Current.IsSelected;
            }
            if (!string.Equals(propertyName, WpfListItem.PropertyNames.DisplayText, StringComparison.OrdinalIgnoreCase) && !string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(uiTaskControl, propertyName);
            }
            return uiTaskControl.TechnologyElement.Name;
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfListItem);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfImage.PropertyNames.Alt, new ZappyTaskPropertyDescriptor(typeof(string)));
            m_ControlSpecificProperties.Add(WpfListItem.PropertyNames.DisplayText, new ZappyTaskPropertyDescriptor(typeof(string)));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName)
        {
            if (!string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) && !string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return base.IsCommonReadableProperty(propertyName);
            }
            return true;
        }

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override ControlType SupportedControlType =>
            ControlType.ListItem;
    }
}

