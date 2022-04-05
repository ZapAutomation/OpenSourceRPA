using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfSliderPropertyProvider : WpfControlPropertyProvider
    {
        public WpfSliderPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfSlider.PropertyNames);

        public override string GetPropertyForAction(ZappyTaskAction action) =>
            WpfSlider.PropertyNames.PositionAsString;

        public override object GetPropertyValue(ZappyTaskControl UIControl, string propertyName)
        {
            int digits = 2;
            RangeValuePattern pattern = GetAutomationPattern<RangeValuePattern>(UIControl, RangeValuePattern.Pattern, AutomationElement.IsRangeValuePatternAvailableProperty);
            if (string.Equals(propertyName, WpfSlider.PropertyNames.Position, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.Value, digits);
            }
            if (string.Equals(propertyName, WpfSlider.PropertyNames.PositionAsString, StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.Value, digits).ToString(CultureInfo.InvariantCulture);
            }
            if (string.Equals(propertyName, WpfSlider.PropertyNames.MaximumPosition, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.Maximum, digits);
            }
            if (string.Equals(propertyName, WpfSlider.PropertyNames.MinimumPosition, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.Minimum, digits);
            }
            if (string.Equals(propertyName, WpfSlider.PropertyNames.LargeChange, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.LargeChange, digits);
            }
            if (string.Equals(propertyName, WpfSlider.PropertyNames.SmallChange, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.SmallChange, digits);
            }
            return base.GetPropertyValue(UIControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfSlider);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfSlider.PropertyNames.Position, new ZappyTaskPropertyDescriptor(typeof(double), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfSlider.PropertyNames.PositionAsString, new ZappyTaskPropertyDescriptor(typeof(string), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfSlider.PropertyNames.MaximumPosition, new ZappyTaskPropertyDescriptor(typeof(double)));
            m_ControlSpecificProperties.Add(WpfSlider.PropertyNames.MinimumPosition, new ZappyTaskPropertyDescriptor(typeof(double)));
            m_ControlSpecificProperties.Add(WpfSlider.PropertyNames.LargeChange, new ZappyTaskPropertyDescriptor(typeof(double)));
            m_ControlSpecificProperties.Add(WpfSlider.PropertyNames.SmallChange, new ZappyTaskPropertyDescriptor(typeof(double)));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if (string.Equals(propertyName, WpfSlider.PropertyNames.Position, StringComparison.OrdinalIgnoreCase))
            {
                double num = ZappyTaskUtilities.ConvertToType<double>(value);
                uiTaskControl.ScreenElement.SetValueAsSlider(num.ToString(CultureInfo.CurrentCulture));
            }
            else if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName, WpfSlider.PropertyNames.PositionAsString, StringComparison.OrdinalIgnoreCase))
            {
                double num2;
                if (!ALUtility.ConvertStringToDouble(ZappyTaskUtilities.ConvertToType<string>(value), out num2))
                {
                    object[] args = new object[] { value, propertyName, uiTaskControl.ControlType };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidParameterValue, args));
                }
                uiTaskControl.ScreenElement.SetValueAsSlider(num2.ToString(CultureInfo.CurrentCulture));
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.Slider;
    }
}

