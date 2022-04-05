using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfTitleBarPropertyProvider : WpfControlPropertyProvider
    {
        public WpfTitleBarPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfTitleBar.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl UIControl, string propertyName)
        {
            object[] args = new object[] { propertyName };
                        WinPropertyProvider provider = new WinPropertyProvider();
            return provider.GetPropertyValueWrapper(UIControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfTitleBar);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfTitleBar.PropertyNames.DisplayText, new ZappyTaskPropertyDescriptor(typeof(string)));
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.TitleBar;
    }
}

