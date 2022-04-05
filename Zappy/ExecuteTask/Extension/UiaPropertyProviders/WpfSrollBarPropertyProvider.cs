using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfSrollBarPropertyProvider : WpfControlPropertyProvider
    {
        public WpfSrollBarPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfScrollBar.PropertyNames);

        public override string GetPropertyForAction(ZappyTaskAction action) =>
            WpfScrollBar.PropertyNames.Position;

        public override object GetPropertyValue(ZappyTaskControl UIControl, string propertyName)
        {
            int digits = 2;
            if (string.Equals(propertyName, WpfScrollBar.PropertyNames.Orientation, StringComparison.OrdinalIgnoreCase))
            {
                AutomationElement nativeElement = UIControl.TechnologyElement.NativeElement as AutomationElement;
                return nativeElement.Current.Orientation;
            }
            RangeValuePattern pattern = GetAutomationPattern<RangeValuePattern>(UIControl, RangeValuePattern.Pattern, AutomationElement.IsRangeValuePatternAvailableProperty);
            if (string.Equals(propertyName, WpfScrollBar.PropertyNames.Position, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.Value, digits);
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.Value, digits).ToString(CultureInfo.InvariantCulture);
            }
            if (string.Equals(propertyName, WpfScrollBar.PropertyNames.MaximumPosition, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.Maximum, digits);
            }
            if (string.Equals(propertyName, WpfScrollBar.PropertyNames.MinimumPosition, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(pattern.Current.Minimum, digits);
            }
            return base.GetPropertyValue(UIControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfScrollBar);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfScrollBar.PropertyNames.Position, new ZappyTaskPropertyDescriptor(typeof(double), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfScrollBar.PropertyNames.MaximumPosition, new ZappyTaskPropertyDescriptor(typeof(double)));
            m_ControlSpecificProperties.Add(WpfScrollBar.PropertyNames.MinimumPosition, new ZappyTaskPropertyDescriptor(typeof(double)));
            m_ControlSpecificProperties.Add(WpfScrollBar.PropertyNames.Orientation, new ZappyTaskPropertyDescriptor(typeof(OrientationType)));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override void SetPropertyValue(ZappyTaskControl UIControl, string propertyName, object value)
        {
            ZappyTaskElementKind horizontalThumb = ZappyTaskElementKind.HorizontalThumb;
            if (((OrientationType)this.GetPropertyValue(UIControl, WpfScrollBar.PropertyNames.Orientation)) == OrientationType.Vertical)
            {
                horizontalThumb = ZappyTaskElementKind.VerticalThumb;
            }
            if (string.Equals(propertyName, WpfScrollBar.PropertyNames.Position, StringComparison.OrdinalIgnoreCase))
            {
                double num = ZappyTaskUtilities.ConvertToType<double>(value);
                UIControl.ScreenElement.SetValueAsScrollBar(num.ToString(CultureInfo.InvariantCulture), horizontalThumb);
            }
            else if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                double num2;
                if (!ALUtility.ConvertStringToDouble(ZappyTaskUtilities.ConvertToType<string>(value), out num2))
                {
                    object[] args = new object[] { value, propertyName, UIControl.ControlType };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidParameterValue, args));
                }
                UIControl.ScreenElement.SetValueAsScrollBar(num2.ToString(CultureInfo.CurrentCulture), horizontalThumb);
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.ScrollBar;
    }
}

