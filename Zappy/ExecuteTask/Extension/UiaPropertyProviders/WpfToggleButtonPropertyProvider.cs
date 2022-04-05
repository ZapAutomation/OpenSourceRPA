using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfToggleButtonPropertyProvider : WpfControlPropertyProvider
    {
        public WpfToggleButtonPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfToggleButton.PropertyNames);

        public override List<string> GetPropertyForControlState(ControlStates uiState, ref List<bool> stateValues)
        {
            List<string> list = new List<string>();
            stateValues = new List<bool>();
            if ((uiState & (ControlStates.None | ControlStates.Pressed)) != ControlStates.None)
            {
                list.Add(WpfToggleButton.PropertyNames.Pressed);
                stateValues.Add(true);
                return list;
            }
            if ((uiState & (ControlStates.None | ControlStates.Normal)) != ControlStates.None)
            {
                list.Add(WpfToggleButton.PropertyNames.Pressed);
                stateValues.Add(false);
                return list;
            }
            if ((uiState & ControlStates.Indeterminate) != ControlStates.None)
            {
                list.Add(WpfToggleButton.PropertyNames.Indeterminate);
                stateValues.Add(true);
            }
            return list;
        }

        public override object GetPropertyValue(ZappyTaskControl UIControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfToggleButton.PropertyNames.Pressed, StringComparison.OrdinalIgnoreCase))
            {
                return this.VerifyTriStateForToggleButton(UIControl, ControlStates.None | ControlStates.Pressed);
            }
            if (string.Equals(propertyName, WpfToggleButton.PropertyNames.Indeterminate, StringComparison.OrdinalIgnoreCase))
            {
                return this.VerifyTriStateForToggleButton(UIControl, ControlStates.Indeterminate);
            }
            if (string.Equals(propertyName, WpfToggleButton.PropertyNames.DisplayText, StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.Name;
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                return UIControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            return base.GetPropertyValue(UIControl, propertyName);
        }

        public override System.Type GetSpecializedClass() =>
            typeof(WpfToggleButton);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfToggleButton.PropertyNames.Pressed, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfToggleButton.PropertyNames.DisplayText, new ZappyTaskPropertyDescriptor(typeof(string)));
            m_ControlSpecificProperties.Add(WpfToggleButton.PropertyNames.Indeterminate, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
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

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if (propertyName.Equals(ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates state = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                this.SetToggleButtonState(uiTaskControl, propertyName, state);
            }
            else if (propertyName.Equals(WpfToggleButton.PropertyNames.Pressed, StringComparison.OrdinalIgnoreCase))
            {
                bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                this.SetToggleButtonState(uiTaskControl, propertyName, flag ? (ControlStates.None | ControlStates.Pressed) : (ControlStates.None | ControlStates.Normal));
            }
            else if (propertyName.Equals(WpfToggleButton.PropertyNames.Indeterminate, StringComparison.OrdinalIgnoreCase))
            {
                bool flag2 = ZappyTaskUtilities.ConvertToType<bool>(value);
                this.SetToggleButtonState(uiTaskControl, propertyName, flag2 ? ControlStates.Indeterminate : (ControlStates.None | ControlStates.Normal));
            }
        }

        private void SetToggleButtonState(ZappyTaskControl UIControl, string PropertyName, ControlStates state)
        {
            if ((((ControlStates.None | ControlStates.Normal) != state) && (((ControlStates.None | ControlStates.Pressed) & state) == ControlStates.None)) && ((ControlStates.Indeterminate & state) == ControlStates.None))
            {
                ALUtility.ThrowNotSupportedException(true);
            }
            if (!this.VerifyTriStateForToggleButton(UIControl, state))
            {
                int num = 0;
                while (num < 5)
                {
                    Mouse.Click(UIControl);
                    num++;
                    try
                    {
                        if (this.VerifyTriStateForToggleButton(UIControl, state))
                        {
                            return;
                        }
                        continue;
                    }
                    catch (ZappyTaskControlNotAvailableException)
                    {
                        object[] objArray1 = new object[] { UIControl.TechnologyElement };
                                                return;
                    }
                }
                object[] args = new object[] { PropertyName, UIControl.Name };
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SetPropertyFailed, args));
            }
        }

        private bool VerifyTriStateForToggleButton(ZappyTaskControl UIControl, ControlStates requiredState)
        {
            if ((UIControl.StateValue & (ControlStates.None | ControlStates.Unavailable)) != ControlStates.None)
            {
                            }
            if (((((ControlStates.None | ControlStates.Pressed) & requiredState) == ControlStates.None) || !TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Pressed)) && (((ControlStates.Indeterminate & requiredState) == ControlStates.None) || !TaskActivityElement.IsState(UIControl.TechnologyElement, AccessibleStates.Indeterminate)))
            {
                if ((ControlStates.None | ControlStates.Normal) == requiredState)
                {
                    AccessibleStates[] states = new AccessibleStates[] { AccessibleStates.Pressed, AccessibleStates.Indeterminate };
                    return !TaskActivityElement.IsAnyState(UIControl.TechnologyElement, states);
                }
                return false;
            }
            return true;
        }

        public override ControlType SupportedControlType =>
            ControlType.ToggleButton;
    }
}

