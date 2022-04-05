using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfStatusBarPropertyProvider : WpfControlPropertyProvider
    {
        public WpfStatusBarPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfStatusBar.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfStatusBar.PropertyNames.Panels, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.GetChildren();
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfStatusBar);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfStatusBar.PropertyNames.Panels, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.StatusBar;
    }
}

