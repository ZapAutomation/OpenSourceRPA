using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfTreeItemPropertyProvider : WpfControlPropertyProvider
    {
        public WpfTreeItemPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfTreeItem.PropertyNames);

        public override List<string> GetPropertyForControlState(ControlStates uiState, ref List<bool> stateValues)
        {
            List<string> list = new List<string>();
            stateValues = new List<bool>();
            if ((uiState & (ControlStates.None | ControlStates.Selected)) != ControlStates.None)
            {
                list.Add(WpfTreeItem.PropertyNames.Selected);
                stateValues.Add(true);
            }
            if ((uiState & ControlStates.Expanded) != ControlStates.None)
            {
                list.Add(WpfTreeItem.PropertyNames.Expanded);
                stateValues.Add(true);
            }
            if ((uiState & ControlStates.Collapsed) != ControlStates.None)
            {
                list.Add(WpfTreeItem.PropertyNames.Expanded);
                stateValues.Add(false);
            }
            return list;
        }

        public override object GetPropertyValue(ZappyTaskControl UIControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfTreeItem.PropertyNames.Expanded, StringComparison.OrdinalIgnoreCase))
            {
                return ((UIControl.StateValue & ControlStates.Expanded) == ControlStates.Expanded);
            }
            if (string.Equals(propertyName, WpfTreeItem.PropertyNames.Selected, StringComparison.OrdinalIgnoreCase))
            {
                return ((UIControl.StateValue & (ControlStates.None | ControlStates.Selected)) == (ControlStates.None | ControlStates.Selected));
            }
            if (string.Equals(propertyName, WpfTreeItem.PropertyNames.ParentNode, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.GetParent();
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            if (string.Equals(propertyName, WpfTreeItem.PropertyNames.Nodes, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
                foreach (ZappyTaskControl control in UIControl.GetChildren())
                {
                    if (control.ControlType.NameEquals(ControlType.TreeItem.Name))
                    {
                        controls.Add(control);
                    }
                }
                return controls;
            }
            if (string.Equals(propertyName, WpfTreeItem.PropertyNames.HasChildNodes, StringComparison.OrdinalIgnoreCase))
            {
                return (GetAutomationPattern<ExpandCollapsePattern>(UIControl, ExpandCollapsePattern.Pattern, AutomationElement.IsExpandCollapsePatternAvailableProperty).Current.ExpandCollapseState != ExpandCollapseState.LeafNode);
            }
            if (string.Equals(propertyName, WpfTreeItem.PropertyNames.Header, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.Name;
            }
            return base.GetPropertyValue(UIControl, propertyName);
        }

        public override System.Type GetSpecializedClass() =>
            typeof(WpfTreeItem);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfTreeItem.PropertyNames.Expanded, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfTreeItem.PropertyNames.ParentNode, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfTreeItem.PropertyNames.Selected, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfTreeItem.PropertyNames.Nodes, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfTreeItem.PropertyNames.HasChildNodes, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfTreeItem.PropertyNames.Header, new ZappyTaskPropertyDescriptor(typeof(string)));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if (propertyName.Equals(ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates state = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                this.SetTreeItemState(uiTaskControl, state);
            }
            else if (propertyName.Equals(WpfTreeItem.PropertyNames.Selected, StringComparison.OrdinalIgnoreCase))
            {
                if (ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    this.SetTreeItemState(uiTaskControl, ControlStates.None | ControlStates.Selected);
                }
                else
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
            }
            else if (propertyName.Equals(WpfTreeItem.PropertyNames.Expanded, StringComparison.OrdinalIgnoreCase))
            {
                bool flag2 = ZappyTaskUtilities.ConvertToType<bool>(value);
                this.SetTreeItemState(uiTaskControl, flag2 ? ControlStates.Expanded : ControlStates.Collapsed);
            }
        }

        private void SetTreeItemState(ZappyTaskControl UIControl, ControlStates state)
        {
            if (((ControlStates.None | ControlStates.Selected) & state) != ControlStates.None)
            {
                UIControl.ScreenElement.Select();
            }
            else if (GetAutomationPattern<ExpandCollapsePattern>(UIControl, ExpandCollapsePattern.Pattern, AutomationElement.IsExpandCollapsePatternAvailableProperty).Current.ExpandCollapseState != ExpandCollapseState.LeafNode)
            {
                if ((ControlStates.Expanded & state) != ControlStates.None)
                {
                    UIControl.ScreenElement.Expand(null);
                }
                else if ((ControlStates.Collapsed & state) != ControlStates.None)
                {
                    UIControl.ScreenElement.Collapse(null);
                }
            }
            else
            {
                ALUtility.ThrowNotSupportedException(true);
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.TreeItem;
    }
}

