using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfPanePropertyProvider : WpfControlPropertyProvider
    {
        public WpfPanePropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfPane.PropertyNames);

        public override Type GetSpecializedClass() =>
            typeof(WpfPane);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfPane.PropertyNames.VerticalScrollBar, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfPane.PropertyNames.HorizontalScrollBar, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), s_ReadNonAssertPermissions));
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.Pane;
    }
}

