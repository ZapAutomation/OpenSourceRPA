using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfToolBarPropertyProvider : WpfControlPropertyProvider
    {
        public WpfToolBarPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfToolBar.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
            if (string.Equals(propertyName, WpfToolBar.PropertyNames.Items, StringComparison.OrdinalIgnoreCase))
            {
                foreach (ZappyTaskControl control in uiTaskControl.GetChildren())
                {
                    if ((control.ControlType != ControlType.Grip) && (control.ControlType != ControlType.Separator))
                    {
                        controls.Add(control);
                    }
                }
                return controls;
            }
            if (string.Equals(propertyName, WpfToolBar.PropertyNames.Header, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.Name;
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfToolBar);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfToolBar.PropertyNames.Items, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfToolBar.PropertyNames.Header, new ZappyTaskPropertyDescriptor(typeof(string)));
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.ToolBar;
    }
}

