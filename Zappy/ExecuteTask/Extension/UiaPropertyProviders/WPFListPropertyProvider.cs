using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WPFListPropertyProvider : WpfControlPropertyProvider
    {
        public WPFListPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        public override Type GetPropertyClassName() =>
            typeof(WpfList.PropertyNames);

        public override string GetPropertyForAction(ZappyTaskAction action)
        {
            SetValueAction action2 = action as SetValueAction;
            if (action2 != null)
            {
                return WpfList.PropertyNames.SelectedItemsAsString;
            }
            return null;
        }

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(propertyName, WpfList.PropertyNames.Items, StringComparison.OrdinalIgnoreCase))
            {
                ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
                foreach (ZappyTaskControl control in uiTaskControl.GetChildren())
                {
                    if (control.ControlType == ControlType.ListItem)
                    {
                        controls.Add(control);
                    }
                }
                return controls;
            }
            if (string.Equals(propertyName, WpfList.PropertyNames.SelectedItems, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.GetPropertyValue(WpfList.PropertyNames.SelectedItems);
            }
            if (string.Equals(propertyName, WpfList.PropertyNames.SelectedItemsAsString, StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                string[] propertyValue = this.GetPropertyValue(uiTaskControl, WpfList.PropertyNames.SelectedItems) as string[];
                CommaListBuilder builder = new CommaListBuilder();
                if (propertyValue != null)
                {
                    builder.AddRange(propertyValue);
                }
                return builder.ToString();
            }
            if (string.Equals(propertyName, WpfList.PropertyNames.IsMultipleSelection, StringComparison.OrdinalIgnoreCase))
            {
                return GetAutomationPattern<SelectionPattern>(uiTaskControl, SelectionPattern.Pattern, AutomationElement.IsSelectionPatternAvailableProperty).Current.CanSelectMultiple;
            }
            if (string.Equals(propertyName, WpfList.PropertyNames.SelectedIndices, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.GetPropertyValue(WpfList.PropertyNames.SelectedIndices);
            }
            return base.GetPropertyValue(uiTaskControl, propertyName);
        }

        public override Type GetSpecializedClass() =>
            typeof(WpfList);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfList.PropertyNames.IsMultipleSelection, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfList.PropertyNames.SelectedItems, new ZappyTaskPropertyDescriptor(typeof(string[]), s_ReadWritePermissions | s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfList.PropertyNames.SelectedItemsAsString, new ZappyTaskPropertyDescriptor(typeof(string), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfList.PropertyNames.Items, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfList.PropertyNames.SelectedIndices, new ZappyTaskPropertyDescriptor(typeof(int[]), s_ReadWritePermissions | s_ReadNonAssertPermissions));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            object[] args = new object[] { propertyName, value };
                        if (string.Equals(propertyName, WpfList.PropertyNames.SelectedItems, StringComparison.OrdinalIgnoreCase))
            {
                if (value == null)
                {
                    TechnologyElementPropertyProvider.SetValueAsListBox(uiTaskControl, new string[0]);
                }
                else
                {
                    string[] values = ZappyTaskUtilities.ConvertToType<string[]>(value);
                    TechnologyElementPropertyProvider.SetValueAsListBox(uiTaskControl, values);
                }
            }
            else if (string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName, WpfList.PropertyNames.SelectedItemsAsString, StringComparison.OrdinalIgnoreCase))
            {
                if (value == null)
                {
                    TechnologyElementPropertyProvider.SetValueAsListBox(uiTaskControl, new string[0]);
                }
                else
                {
                    string[] strArray2 = CommaListBuilder.GetCommaSeparatedValues(ZappyTaskUtilities.ConvertToType<string>(value)).ToArray();
                    TechnologyElementPropertyProvider.SetValueAsListBox(uiTaskControl, strArray2);
                }
            }
            else if (string.Equals(propertyName, WpfList.PropertyNames.SelectedIndices, StringComparison.OrdinalIgnoreCase))
            {
                int[] selectedIndices = ZappyTaskUtilities.ConvertToType<int[]>(value);
                int childListItemCount = GetChildListItemCount(uiTaskControl);
                TechnologyElementPropertyProvider.SetValueUsingQueryId(uiTaskControl, selectedIndices, uiTaskControl.TechnologyName, childListItemCount);
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.List;
    }
}

