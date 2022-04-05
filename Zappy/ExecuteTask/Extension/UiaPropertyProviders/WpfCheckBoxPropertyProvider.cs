using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfCheckBoxPropertyProvider : WpfControlPropertyProvider
    {
        public WpfCheckBoxPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfCheckBox.PropertyNames);

        public override List<string> GetPropertyForControlState(ControlStates uiState, ref List<bool> propertyValues)
        {
            List<string> list = new List<string>();
            if ((uiState & ControlStates.Checked) != ControlStates.None)
            {
                list.Add(WpfCheckBox.PropertyNames.Checked);
                propertyValues.Add(true);
                return list;
            }
            if ((uiState & (ControlStates.None | ControlStates.Normal)) != ControlStates.None)
            {
                list.Add(WpfCheckBox.PropertyNames.Checked);
                propertyValues.Add(false);
                return list;
            }
            if ((uiState & ControlStates.Indeterminate) != ControlStates.None)
            {
                list.Add(WpfCheckBox.PropertyNames.Indeterminate);
                propertyValues.Add(true);
            }
            return list;
        }

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfCheckBox.PropertyNames.Checked, StringComparison.OrdinalIgnoreCase))
            {
                return ((uiTaskControl.StateValue & ControlStates.Checked) == ControlStates.Checked);
            }
            if (string.Equals(propertyName, WpfCheckBox.PropertyNames.Indeterminate, StringComparison.OrdinalIgnoreCase))
            {
                return ((uiTaskControl.StateValue & ControlStates.Indeterminate) == ControlStates.Indeterminate);
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override System.Type GetSpecializedClass() =>
            typeof(WpfCheckBox);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfCheckBox.PropertyNames.Checked, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfCheckBox.PropertyNames.Indeterminate, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates states = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                if (states == (ControlStates.None | ControlStates.Normal))
                {
                    propertyName = ControlStates.Checked.ToString();
                    value = false;
                }
                else
                {
                    propertyName = states.ToString();
                    value = true;
                }
            }
            if (string.Equals(propertyName, WpfCheckBox.PropertyNames.Checked, StringComparison.OrdinalIgnoreCase))
            {
                bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                TechnologyElementPropertyProvider.SetCheckBoxState(uiTaskControl, flag ? ControlStates.Checked : (ControlStates.None | ControlStates.Normal));
            }
            else if (string.Equals(propertyName, WpfCheckBox.PropertyNames.Indeterminate, StringComparison.OrdinalIgnoreCase))
            {
                bool flag2 = ZappyTaskUtilities.ConvertToType<bool>(value);
                TechnologyElementPropertyProvider.SetCheckBoxState(uiTaskControl, flag2 ? ControlStates.Indeterminate : (ControlStates.None | ControlStates.Normal));
                if (flag2 && !((bool)this.GetPropertyValue(uiTaskControl, WpfCheckBox.PropertyNames.Indeterminate)))
                {
                    ALUtility.ThrowNotSupportedException(false);
                }
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.CheckBox;
    }
}

