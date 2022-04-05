using System;
using System.Collections.Generic;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfHostingPanePropertyProvider : WpfControlPropertyProvider
    {
        public WpfHostingPanePropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override int GetControlSupportLevel(ZappyTaskControl control)
        {
            string a = null;
            if (control.SearchProperties.Contains(WpfControl.PropertyNames.FrameworkId))
            {
                a = control.SearchProperties[WpfControl.PropertyNames.FrameworkId];
            }
            else if (control.TechnologyElement != null)
            {
                a = control.TechnologyElement.ClassName;
            }
            if (string.Equals(a, "Winform", StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }
            return 0;
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfPane.PropertyNames);

        public override Type GetSpecializedClass() =>
            typeof(ZappyTaskControl);

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

