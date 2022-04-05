using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfRadioButtonPropertyProvider : WpfControlPropertyProvider
    {
        public WpfRadioButtonPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfRadioButton.PropertyNames);

        public override List<string> GetPropertyForControlState(ControlStates uiState, ref List<bool> stateValues)
        {
            List<string> list = new List<string>();
            stateValues = new List<bool>();
            if ((uiState & ControlStates.Checked) != ControlStates.None)
            {
                list.Add(WpfRadioButton.PropertyNames.Selected);
                stateValues.Add(true);
            }
            return list;
        }

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (string.Equals(propertyName, WpfRadioButton.PropertyNames.Selected, StringComparison.OrdinalIgnoreCase))
            {
                return ((ControlStates.Checked & uiTaskControl.StateValue) == ControlStates.Checked);
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.GetRequestedState(~AccessibleStates.None);
            }
            if (!string.Equals(propertyName, WpfRadioButton.PropertyNames.Group, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(uiTaskControl, propertyName);
            }
            ZappyTaskControl parent = uiTaskControl.GetParent();
            if ((parent != null) && (parent.ControlType == ControlType.Group))
            {
                return parent;
            }
            return null;
        }

        public override System.Type GetSpecializedClass() =>
            typeof(WpfRadioButton);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfRadioButton.PropertyNames.Selected, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfRadioButton.PropertyNames.Group, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), s_ReadNonAssertPermissions));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.State, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            if (string.Equals(propertyName, WpfRadioButton.PropertyNames.Selected, StringComparison.OrdinalIgnoreCase))
            {
                bool flag = ZappyTaskUtilities.ConvertToType<bool>(value);
                if (flag)
                {
                    TechnologyElementPropertyProvider.SetRadioButtonState(uiTaskControl, ControlStates.Checked);
                    return;
                }
                object[] args = new object[] { flag, uiTaskControl.ControlType.Name, WpfRadioButton.PropertyNames.Selected };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidParameterValue, args));
            }
            if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.State, StringComparison.OrdinalIgnoreCase))
            {
                ControlStates state = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                TechnologyElementPropertyProvider.SetRadioButtonState(uiTaskControl, state);
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.RadioButton;
    }
}

