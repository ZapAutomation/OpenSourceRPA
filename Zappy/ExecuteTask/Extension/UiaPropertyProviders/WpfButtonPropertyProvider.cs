using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfButtonPropertyProvider : WpfControlPropertyProvider
    {
        public WpfButtonPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfButton.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            AutomationElement nativeElement = uiTaskControl.TechnologyElement.NativeElement as AutomationElement;
            if (string.Equals(propertyName, WpfButton.PropertyNames.DisplayText, StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return nativeElement.Current.Name;
            }
            if (string.Equals(propertyName, WpfButton.PropertyNames.Shortcut, StringComparison.OrdinalIgnoreCase))
            {
                return nativeElement.Current.AccessKey;
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfButton);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfButton.PropertyNames.Shortcut, new ZappyTaskPropertyDescriptor(typeof(string)));
            m_ControlSpecificProperties.Add(WpfButton.PropertyNames.DisplayText, new ZappyTaskPropertyDescriptor(typeof(string)));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override ControlType SupportedControlType =>
        ControlType.Button;
    }
}

