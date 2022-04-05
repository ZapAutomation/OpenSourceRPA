using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfTextPropertyProvider : WpfControlPropertyProvider
    {
        public WpfTextPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfText.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl UIControl, string propertyName)
        {
            if (!string.Equals(propertyName, WpfText.PropertyNames.DisplayText, StringComparison.OrdinalIgnoreCase) && !string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(UIControl, propertyName);
            }
            return UIControl.TechnologyElement.Name;
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfText);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfText.PropertyNames.DisplayText, new ZappyTaskPropertyDescriptor(typeof(string)));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override ControlType SupportedControlType =>
            ControlType.Text;
    }
}

