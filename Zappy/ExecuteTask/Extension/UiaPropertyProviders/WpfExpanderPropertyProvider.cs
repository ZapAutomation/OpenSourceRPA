using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfExpanderPropertyProvider : WpfControlPropertyProvider
    {
        public WpfExpanderPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfExpander.PropertyNames);

        public override List<string> GetPropertyForControlState(ControlStates uiState, ref List<bool> stateValues)
        {
            List<string> list = new List<string>();
            stateValues = new List<bool>();
            if ((uiState & ControlStates.Expanded) != ControlStates.None)
            {
                list.Add(WpfExpander.PropertyNames.Expanded);
                stateValues.Add(true);
                return list;
            }
            if ((uiState & ControlStates.Collapsed) != ControlStates.None)
            {
                list.Add(WpfExpander.PropertyNames.Expanded);
                stateValues.Add(false);
            }
            return list;
        }

        public override object GetPropertyValue(ZappyTaskControl UIControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfExpander.PropertyNames.Expanded, StringComparison.OrdinalIgnoreCase))
            {
                return ((UIControl.StateValue & ControlStates.Expanded) == ControlStates.Expanded);
            }
            if (string.Equals(propertyName, WpfExpander.PropertyNames.Header, StringComparison.OrdinalIgnoreCase))
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
            typeof(WpfExpander);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfExpander.PropertyNames.Expanded, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfExpander.PropertyNames.Header, new ZappyTaskPropertyDescriptor(typeof(string)));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        private void SetExpanderState(ZappyTaskControl UIControl, ControlStates state)
        {
            if (((UIControl == null) || !ControlType.Expander.NameEquals(UIControl.ControlType.Name)) || ((state != ControlStates.Collapsed) && (state != ControlStates.Expanded)))
            {
                ALUtility.ThrowNotSupportedException(true);
            }
            ControlStates stateValue = UIControl.StateValue;
            ZappyTaskControl control = UIControl;
            ZappyTaskControlCollection controls = ALUtility.GetDescendantsByControlType(UIControl, UIControl.TechnologyName, ControlType.Button, 1);
            if (controls.Count <= 0)
            {
                                ALUtility.ThrowNotSupportedException(false);
            }
            if ((stateValue & state) == ControlStates.None)
            {
                try
                {
                    Mouse.Click(controls[0]);
                }
                catch (Exception exception)
                {
                    object[] args = new object[] { exception.Message };
                                        ALUtility.ThrowNotSupportedException(false);
                }
            }
            try
            {
                if (!this.VerifyElementState(UIControl, state))
                {
                    Mouse.Click(controls[0]);
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
                object[] objArray2 = new object[] { UIControl.TechnologyElement };
                            }
        }

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if (propertyName.Equals(ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates state = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                this.SetExpanderState(uiTaskControl, state);
            }
            else if (propertyName.Equals(WpfExpander.PropertyNames.Expanded, StringComparison.OrdinalIgnoreCase))
            {
                bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                this.SetExpanderState(uiTaskControl, flag ? ControlStates.Expanded : ControlStates.Collapsed);
            }
        }

        private bool VerifyElementState(ZappyTaskControl UIControl, ControlStates requiredState)
        {
            ControlStates stateValue = UIControl.StateValue;
            if ((stateValue & (ControlStates.None | ControlStates.Unavailable)) != ControlStates.None)
            {
                            }
            return ((requiredState & stateValue) > ControlStates.None);
        }

        public override ControlType SupportedControlType =>
            ControlType.Expander;
    }
}

