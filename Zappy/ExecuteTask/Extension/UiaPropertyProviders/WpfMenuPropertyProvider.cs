using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfMenuPropertyProvider : WpfControlPropertyProvider
    {
        public WpfMenuPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfMenu.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (!string.Equals(propertyName, WpfMenu.PropertyNames.Items, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(uiTaskControl, propertyName);
            }
            ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
            foreach (ZappyTaskControl control in uiTaskControl.GetChildren())
            {
                if (((control.ControlType == ControlType.MenuItem) || (control.ControlType == ControlType.ComboBox)) || (control.ControlType == ControlType.Edit))
                {
                    controls.Add(control);
                }
            }
            return controls;
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfMenu);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfMenu.PropertyNames.Items, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.Menu;
    }
}

