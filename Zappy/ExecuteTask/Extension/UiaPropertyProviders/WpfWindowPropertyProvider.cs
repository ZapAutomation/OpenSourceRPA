using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfWindowPropertyProvider : WpfControlPropertyProvider
    {
        public WpfWindowPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfWindow.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            object[] args = new object[] { propertyName };
                        WinPropertyProvider provider = new WinPropertyProvider();
            return provider.GetPropertyValueWrapper(uiTaskControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfWindow);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.Resizable, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.HasTitleBar, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.Popup, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.TabStop, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.Transparent, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.AlwaysOnTop, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.Maximized, new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Writable | ZappyTaskPropertyAttributes.Readable));
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.Minimized, new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Writable | ZappyTaskPropertyAttributes.Readable));
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.ShowInTaskbar, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfWindow.PropertyNames.Restored, new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Writable | ZappyTaskPropertyAttributes.Readable));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            object[] args = new object[] { propertyName, value };
                        new WinPropertyProvider().SetPropertyValueWrapper(uiTaskControl, propertyName, value);
        }

        public override ControlType SupportedControlType =>
            ControlType.Window;
    }
}

