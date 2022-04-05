using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfTreePropertyProvider : WpfControlPropertyProvider
    {
        public WpfTreePropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfTree.PropertyNames);

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfTree.PropertyNames.Nodes, StringComparison.OrdinalIgnoreCase))
            {
                return ALUtility.GetDescendantsByControlType(uiTaskControl, uiTaskControl.TechnologyName, ControlType.TreeItem, 1);
            }
            if (string.Equals(propertyName, WpfTree.PropertyNames.HorizontalScrollBar, StringComparison.OrdinalIgnoreCase))
            {
                return WpfScrollViewerPropertyProvier.GetScrollBar(uiTaskControl, OrientationType.Horizontal);
            }
            if (string.Equals(propertyName, WpfTree.PropertyNames.VerticalScrollBar, StringComparison.OrdinalIgnoreCase))
            {
                return WpfScrollViewerPropertyProvier.GetScrollBar(uiTaskControl, OrientationType.Vertical);
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfTree);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfTree.PropertyNames.Nodes, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfTree.PropertyNames.VerticalScrollBar, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfTree.PropertyNames.HorizontalScrollBar, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), s_ReadNonAssertPermissions));
            return m_ControlSpecificProperties;
        }

        public override ControlType SupportedControlType =>
            ControlType.Tree;
    }
}

