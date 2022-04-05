using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfMenuItemPropertyProvider : WpfControlPropertyProvider
    {
        public WpfMenuItemPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfMenuItem.PropertyNames);

        public override List<string> GetPropertyForControlState(ControlStates uiState, ref List<bool> stateValues)
        {
            List<string> list = new List<string>();
            stateValues = new List<bool>();
            if ((uiState & ControlStates.Checked) != ControlStates.None)
            {
                list.Add(WpfMenuItem.PropertyNames.Checked);
                stateValues.Add(true);
                return list;
            }
            if ((uiState & (ControlStates.None | ControlStates.Normal)) != ControlStates.None)
            {
                list.Add(WpfMenuItem.PropertyNames.Checked);
                stateValues.Add(false);
            }
            return list;
        }

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfMenuItem.PropertyNames.Header, StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.Name;
            }
            if (string.Equals(propertyName, WpfMenuItem.PropertyNames.Checked, StringComparison.OrdinalIgnoreCase))
            {
                return TaskActivityElement.IsState(uiTaskControl.TechnologyElement, AccessibleStates.Checked);
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            if (string.Equals(propertyName, WpfMenuItem.PropertyNames.IsTopLevelMenu, StringComparison.OrdinalIgnoreCase))
            {
                if (!(uiTaskControl.ControlType == ControlType.Menu) && !(uiTaskControl.GetParent().ControlType != ControlType.MenuItem))
                {
                    return false;
                }
                return true;
            }
            if (string.Equals(propertyName, WpfMenuItem.PropertyNames.Expanded, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    ExpandCollapsePattern pattern = GetAutomationPattern<ExpandCollapsePattern>(uiTaskControl, ExpandCollapsePattern.Pattern, AutomationElement.IsExpandCollapsePatternAvailableProperty);
                    return ((pattern != null) && (pattern.Current.ExpandCollapseState == ExpandCollapseState.Expanded));
                }
                catch (NotSupportedException)
                {
                    return false;
                }
            }
            if (string.Equals(propertyName, WpfMenuItem.PropertyNames.HasChildNodes, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    ExpandCollapsePattern pattern2 = GetAutomationPattern<ExpandCollapsePattern>(uiTaskControl, ExpandCollapsePattern.Pattern, AutomationElement.IsExpandCollapsePatternAvailableProperty);
                    return ((pattern2 != null) && (pattern2.Current.ExpandCollapseState != ExpandCollapseState.LeafNode));
                }
                catch (NotSupportedException)
                {
                    return false;
                }
            }
            ALUtility.ThrowNotSupportedException(true);
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override System.Type GetSpecializedClass() =>
            typeof(WpfMenuItem);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfMenuItem.PropertyNames.Header, new ZappyTaskPropertyDescriptor(typeof(string)));
            m_ControlSpecificProperties.Add(WpfMenuItem.PropertyNames.Checked, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfMenuItem.PropertyNames.IsTopLevelMenu, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfMenuItem.PropertyNames.HasChildNodes, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfMenuItem.PropertyNames.Expanded, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName)
        {
            if (!string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) && !string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return base.IsCommonReadableProperty(propertyName);
            }
            return true;
        }

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        private void SetMenuItemState(ZappyTaskControl UIControl, ControlStates state)
        {
            if (((ControlStates.Checked & state) == ControlStates.None) && ((ControlStates.None | ControlStates.Normal) != state))
            {
                ALUtility.ThrowNotSupportedException(false);
            }
            if ((((ControlStates.Checked & state) != ControlStates.None) && !TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Checked)) || (((ControlStates.None | ControlStates.Normal) == state) && TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Checked)))
            {
                Mouse.Click(UIControl);
            }
        }

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if (propertyName.Equals(ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates state = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                this.SetMenuItemState(uiTaskControl, state);
            }
            else if (propertyName.Equals(WpfMenuItem.PropertyNames.Checked, StringComparison.OrdinalIgnoreCase))
            {
                AutomationElement nativeElement = uiTaskControl.NativeElement as AutomationElement;
                if (!((bool)nativeElement.GetCurrentPropertyValue(AutomationElement.IsTogglePatternAvailableProperty)))
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                this.SetMenuItemState(uiTaskControl, flag ? ControlStates.Checked : (ControlStates.None | ControlStates.Normal));
            }
            else if (propertyName.Equals(WpfMenuItem.PropertyNames.Expanded, StringComparison.OrdinalIgnoreCase))
            {
                if (!ZappyTaskUtilities.ConvertToType<bool>(value))
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                else if (!((bool)this.GetPropertyValue(uiTaskControl, WpfMenuItem.PropertyNames.HasChildNodes)))
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                ExpandCollapsePattern pattern = GetAutomationPattern<ExpandCollapsePattern>(uiTaskControl, ExpandCollapsePattern.Pattern, AutomationElement.IsExpandCollapsePatternAvailableProperty);
                if (pattern != null)
                {
                    pattern.Expand();
                }
                else
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.MenuItem;
    }
}

