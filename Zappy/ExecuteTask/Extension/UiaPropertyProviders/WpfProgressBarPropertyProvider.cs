using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfProgressBarPropertyProvider : WpfControlPropertyProvider
    {
        public WpfProgressBarPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfProgressBar.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            object[] args = new object[] { propertyName };
                        RangeValuePattern pattern = GetAutomationPattern<RangeValuePattern>(uiTaskControl, RangeValuePattern.Pattern, AutomationElement.IsRangeValuePatternAvailableProperty);
            if (string.Equals(propertyName, WpfProgressBar.PropertyNames.MaximumValue, StringComparison.OrdinalIgnoreCase))
            {
                return pattern.Current.Maximum;
            }
            if (string.Equals(propertyName, WpfProgressBar.PropertyNames.MinimumValue, StringComparison.OrdinalIgnoreCase))
            {
                return pattern.Current.Minimum;
            }
            if (string.Equals(propertyName, WpfProgressBar.PropertyNames.Position, StringComparison.OrdinalIgnoreCase))
            {
                return pattern.Current.Value;
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return pattern.Current.Value.ToString(CultureInfo.InvariantCulture);
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfProgressBar);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfProgressBar.PropertyNames.MinimumValue, new ZappyTaskPropertyDescriptor(typeof(double)));
            m_ControlSpecificProperties.Add(WpfProgressBar.PropertyNames.MaximumValue, new ZappyTaskPropertyDescriptor(typeof(double)));
            m_ControlSpecificProperties.Add(WpfProgressBar.PropertyNames.Position, new ZappyTaskPropertyDescriptor(typeof(double)));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override ControlType SupportedControlType =>
            ControlType.ProgressBar;
    }
}

