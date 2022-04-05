using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfEditPropertyProvider : WpfControlPropertyProvider
    {
        public WpfEditPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfEdit.PropertyNames);

        public override string GetPropertyForAction(ZappyTaskAction action)
        {
            SetValueAction action2 = action as SetValueAction;
            if (action2 == null)
            {
                throw new NotSupportedException();
            }
            if (action2.IsActionOnProtectedElement())
            {
                return WpfEdit.PropertyNames.Password;
            }
            return WpfEdit.PropertyNames.Text;
        }

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (uiTaskControl.TechnologyElement.IsPassword && !string.Equals(WpfEdit.PropertyNames.IsPassword, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                object[] args = new object[] { uiTaskControl.TechnologyName };
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.GetPropertyFailedOnPassword, args));
            }
            if (string.Equals(propertyName, WpfEdit.PropertyNames.Text, StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.Value;
            }
            if (string.Equals(propertyName, WpfEdit.PropertyNames.SelectionText, StringComparison.OrdinalIgnoreCase))
            {
                TextPattern pattern = GetAutomationPattern<TextPattern>(uiTaskControl, TextPattern.Pattern, AutomationElement.IsTextPatternAvailableProperty);
                if (pattern != null)
                {
                    TextPatternRange[] selection = pattern.GetSelection();
                    if ((selection != null) && (selection.Length != 0))
                    {
                        return selection[0].GetText(-1);
                    }
                }
            }
            else
            {
                if (string.Equals(propertyName, WpfEdit.PropertyNames.IsPassword, StringComparison.OrdinalIgnoreCase))
                {
                    return uiTaskControl.TechnologyElement.IsPassword;
                }
                if (string.Equals(propertyName, WpfEdit.PropertyNames.Password, StringComparison.OrdinalIgnoreCase))
                {
                    ALUtility.ThrowNotSupportedException(true);
                    return null;
                }
                if (string.Equals(propertyName, WpfEdit.PropertyNames.ReadOnly, StringComparison.OrdinalIgnoreCase))
                {
                    ValuePattern pattern2 = GetAutomationPattern<ValuePattern>(uiTaskControl, ValuePattern.Pattern, AutomationElement.IsValuePatternAvailableProperty);
                    return ((pattern2 != null) && pattern2.Current.IsReadOnly);
                }
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfEdit);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfEdit.PropertyNames.Text, new ZappyTaskPropertyDescriptor(typeof(string), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfEdit.PropertyNames.SelectionText, new ZappyTaskPropertyDescriptor(typeof(string)));
            m_ControlSpecificProperties.Add(WpfEdit.PropertyNames.IsPassword, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfEdit.PropertyNames.Password, new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Writable));
            m_ControlSpecificProperties.Add(WpfEdit.PropertyNames.ReadOnly, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfEdit.PropertyNames.CopyPastedText, new ZappyTaskPropertyDescriptor(typeof(string), s_ReadWritePermissions));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            string dataToEncode = string.Empty;
            if (value != null)
            {
                dataToEncode = ZappyTaskUtilities.ConvertToType<string>(value);
            }
            if (string.Equals(propertyName, WpfEdit.PropertyNames.Text, StringComparison.OrdinalIgnoreCase))
            {
                if (uiTaskControl.TechnologyElement.IsPassword)
                {
                    TechnologyElementPropertyProvider.SetValueAsEditBox(uiTaskControl, EncodeDecode.EncodeString(dataToEncode), true, false);
                }
                else
                {
                    TechnologyElementPropertyProvider.SetValueAsEditBox(uiTaskControl, dataToEncode, false, false);
                }
            }
            else if (string.Equals(propertyName, WpfEdit.PropertyNames.Password, StringComparison.OrdinalIgnoreCase))
            {
                if (uiTaskControl.TechnologyElement.IsPassword)
                {
                    TechnologyElementPropertyProvider.SetValueAsEditBox(uiTaskControl, dataToEncode, true, false);
                }
                else
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
            }
            else if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                if (uiTaskControl.TechnologyElement.IsPassword)
                {
                    TechnologyElementPropertyProvider.SetValueAsEditBox(uiTaskControl, dataToEncode, true, false);
                }
                else
                {
                    TechnologyElementPropertyProvider.SetValueAsEditBox(uiTaskControl, dataToEncode, false, false);
                }
            }
            else if (string.Equals(propertyName, WpfEdit.PropertyNames.CopyPastedText, StringComparison.OrdinalIgnoreCase))
            {
                TechnologyElementPropertyProvider.SetValueAsEditBox(uiTaskControl, dataToEncode, false, true);
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.Edit;
    }
}

