using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfCustomPropertyProvider : WpfControlPropertyProvider
    {
        public WpfCustomPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfControl.PropertyNames);

        public override Type GetSpecializedClass() =>
            typeof(WpfCustom);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.Custom;
    }
}

