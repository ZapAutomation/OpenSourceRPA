﻿using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfHyperlinkPropertyProvider : WpfControlPropertyProvider
    {
        public WpfHyperlinkPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfHyperlink.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfHyperlink.PropertyNames.Alt, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.Name;
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfHyperlink);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfHyperlink.PropertyNames.Alt, new ZappyTaskPropertyDescriptor(typeof(string)));
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.Hyperlink;
    }
}

