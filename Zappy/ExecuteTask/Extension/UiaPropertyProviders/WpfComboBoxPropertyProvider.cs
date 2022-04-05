using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.TaskTechnology;
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
    internal class WpfComboBoxPropertyProvider : WpfControlPropertyProvider
    {
        private ZappyTaskControl horizontalScrollBar;
        private ZappyTaskControl uiTaskControl;
        private ZappyTaskControl verticalScrollBar;

        public WpfComboBoxPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        private bool EnumChildWinForComboBoxScrollBar(IntPtr hWnd, ref IntPtr lParam)
        {
            if (NativeMethods.IsWpfClassName(hWnd))
            {
                try
                {
                    AutomationElement element = AutomationElement.FromHandle(hWnd);
                    if (((element != null) && (element.Current.ControlType == System.Windows.Automation.ControlType.Window)) && (string.Equals(element.Current.ClassName, "Popup") && ((this.uiTaskControl.StateValue & ControlStates.Expanded) != ControlStates.None)))
                    {
                        AutomationElement nativeElement = this.uiTaskControl.TechnologyElement.NativeElement as AutomationElement;
                        if (element.Current.ProcessId == nativeElement.Current.ProcessId)
                        {
                            AutomationElement lastChild = TreeWalker.ControlViewWalker.GetLastChild(element);
                            if (WpfScrollViewerPropertyProvier.IsScrollBarOfGivenQrientation(lastChild, OrientationType.Horizontal))
                            {
                                AutomationElement previousSibling = TreeWalker.ControlViewWalker.GetPreviousSibling(lastChild);
                                if (WpfScrollViewerPropertyProvier.IsScrollBarOfGivenQrientation(previousSibling, OrientationType.Vertical))
                                {
                                    string technologyName = this.uiTaskControl.TechnologyName;
                                    if (WpfScrollViewerPropertyProvier.IsScrollBarVisibleToUser(lastChild))
                                    {
                                        this.horizontalScrollBar = ZappyTaskControlFactory.FromNativeElement(lastChild, technologyName);
                                    }
                                    if (WpfScrollViewerPropertyProvier.IsScrollBarVisibleToUser(previousSibling))
                                    {
                                        this.verticalScrollBar = ZappyTaskControlFactory.FromNativeElement(previousSibling, technologyName);
                                    }
                                    return false;
                                }
                            }
                        }
                    }
                }
                catch (ElementNotAvailableException)
                {
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return true;
                }
            }
            return true;
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfComboBox.PropertyNames);

        public override string GetPropertyForAction(ZappyTaskAction action)
        {
            SetValueAction action2 = action as SetValueAction;
            if (action2 == null)
            {
                return null;
            }
            if (action2.PreferEdit)
            {
                return WpfComboBox.PropertyNames.EditableItem;
            }
            return WpfComboBox.PropertyNames.SelectedItem;
        }

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            object[] args = new object[] { propertyName };
                        if (string.Equals(propertyName, WpfComboBox.PropertyNames.SelectedIndex, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.GetPropertyValue(WpfComboBox.PropertyNames.SelectedIndex);
            }
            if (string.Equals(propertyName, WpfComboBox.PropertyNames.IsEditable, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    ValuePattern pattern = GetAutomationPattern<ValuePattern>(uiTaskControl, ValuePattern.Pattern, AutomationElement.IsValuePatternAvailableProperty);
                    return ((pattern == null) ? ((object)0) : ((object)!pattern.Current.IsReadOnly));
                }
                catch (NotSupportedException)
                {
                    return false;
                }
            }
            if (string.Equals(propertyName, WpfComboBox.PropertyNames.Expanded, StringComparison.OrdinalIgnoreCase))
            {
                return (GetAutomationPattern<ExpandCollapsePattern>(uiTaskControl, ExpandCollapsePattern.Pattern, AutomationElement.IsExpandCollapsePatternAvailableProperty).Current.ExpandCollapseState == ExpandCollapseState.Expanded);
            }
            if (string.Equals(propertyName, WpfComboBox.PropertyNames.Items, StringComparison.OrdinalIgnoreCase))
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
            if (string.Equals(propertyName, WpfComboBox.PropertyNames.SelectedItem, StringComparison.OrdinalIgnoreCase) || string.Equals(propertyName, ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase))
            {
                string str = uiTaskControl.TechnologyElement.Value;
                if (str != null)
                {
                    return str;
                }
                return uiTaskControl.TechnologyElement.GetPropertyValue(WpfComboBox.PropertyNames.SelectedItem);
            }
            if (string.Equals(propertyName, WpfComboBox.PropertyNames.EditableItem, StringComparison.OrdinalIgnoreCase))
            {
                if (!((bool)this.GetPropertyValue(uiTaskControl, WpfComboBox.PropertyNames.IsEditable)))
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                return this.GetPropertyValue(uiTaskControl, ZappyTaskControl.PropertyNames.Value);
            }
            if ((!string.Equals(propertyName, WpfComboBox.PropertyNames.VerticalScrollBar, StringComparison.OrdinalIgnoreCase) && !string.Equals(propertyName, WpfComboBox.PropertyNames.HorizontalScrollBar, StringComparison.OrdinalIgnoreCase)) || !TaskActivityElement.IsState(uiTaskControl.TechnologyElement, AccessibleStates.Expanded))
            {
                return base.GetPropertyValue(uiTaskControl, propertyName);
            }
            if (((this.verticalScrollBar == null) || (this.horizontalScrollBar == null)) || (!this.horizontalScrollBar.Exists || !this.verticalScrollBar.Exists))
            {
                this.verticalScrollBar = (ZappyTaskControl)(this.horizontalScrollBar = null);
                try
                {
                    GetAutomationPattern<ScrollPattern>(uiTaskControl, ScrollPattern.Pattern, AutomationElement.IsScrollPatternAvailableProperty);
                    IntPtr zero = IntPtr.Zero;
                    this.uiTaskControl = uiTaskControl;
                    NativeMethods.EnumWindowsProc lpEnumFunc = new NativeMethods.EnumWindowsProc(this.EnumChildWinForComboBoxScrollBar);
                    NativeMethods.EnumChildWindows(IntPtr.Zero, lpEnumFunc, ref zero);
                }
                catch (NotSupportedException)
                {
                }
            }
            if (string.Equals(propertyName, WpfComboBox.PropertyNames.VerticalScrollBar, StringComparison.OrdinalIgnoreCase))
            {
                return this.verticalScrollBar;
            }
            return this.horizontalScrollBar;
        }

        public override System.Type GetSpecializedClass() =>
            typeof(WpfComboBox);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfComboBox.PropertyNames.Expanded, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfComboBox.PropertyNames.IsEditable, new ZappyTaskPropertyDescriptor(typeof(bool)));
            m_ControlSpecificProperties.Add(WpfComboBox.PropertyNames.SelectedIndex, new ZappyTaskPropertyDescriptor(typeof(int), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfComboBox.PropertyNames.SelectedItem, new ZappyTaskPropertyDescriptor(typeof(string), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfComboBox.PropertyNames.EditableItem, new ZappyTaskPropertyDescriptor(typeof(string), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfComboBox.PropertyNames.Items, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfComboBox.PropertyNames.VerticalScrollBar, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), s_ReadNonAssertPermissions));
            m_ControlSpecificProperties.Add(WpfComboBox.PropertyNames.HorizontalScrollBar, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), s_ReadNonAssertPermissions));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        private void SetComboBoxState(ExpandCollapseState state, ZappyTaskControl uiControl)
        {
            ExpandCollapsePattern pattern = GetAutomationPattern<ExpandCollapsePattern>(uiControl, ExpandCollapsePattern.Pattern, AutomationElement.IsExpandCollapsePatternAvailableProperty);
            string buttonQueryId = ";[UIA]Crapy.ActionMap.HelperClasses.ControlType='Button'";
            int num = 0;
            bool flag = false;
            num = 0;
            while (((num < 2) && (pattern.Current.ExpandCollapseState != state)) && !flag)
            {
                flag = uiControl.ScreenElement.ExpandCollapseComboBox(buttonQueryId);
                num++;
            }
            if ((num == 2) | flag)
            {
                if (state == ExpandCollapseState.Expanded)
                {
                    pattern.Expand();
                }
                else if (pattern.Current.ExpandCollapseState == ExpandCollapseState.Expanded)
                {
                    Keyboard.SendKeys("{Escape}");
                }
            }
        }

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            object[] args = new object[] { propertyName, value };
                        if (string.Equals(WpfComboBox.PropertyNames.EditableItem, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (!((bool)this.GetPropertyValue(uiTaskControl, WpfComboBox.PropertyNames.IsEditable)))
                {
                    ALUtility.ThrowNotSupportedException(true);
                }
                string str = ZappyTaskUtilities.ConvertToType<string>(value, false);
                TechnologyElementPropertyProvider.SetValueAsComboBox(uiTaskControl, str, true);
            }
            else if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if ((bool)this.GetPropertyValue(uiTaskControl, WpfComboBox.PropertyNames.IsEditable))
                {
                    this.SetPropertyValue(uiTaskControl, WpfComboBox.PropertyNames.EditableItem, value);
                }
                else
                {
                    this.SetPropertyValue(uiTaskControl, WpfComboBox.PropertyNames.SelectedItem, value);
                }
            }
            else if (string.Equals(WpfComboBox.PropertyNames.SelectedItem, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                string str2 = ZappyTaskUtilities.ConvertToType<string>(value, false);
                TechnologyElementPropertyProvider.SetValueAsComboBox(uiTaskControl, str2, false);
            }
            else if (string.Equals(WpfComboBox.PropertyNames.Expanded, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                bool flag = ZappyTaskUtilities.ConvertToType<bool>(value, false);
                this.SetComboBoxState(flag ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed, uiTaskControl);
            }
            else if (string.Equals(WpfComboBox.PropertyNames.SelectedIndex, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                int num = ZappyTaskUtilities.ConvertToType<int>(value, false);
                int childListItemCount = GetChildListItemCount(uiTaskControl);
                int[] selectedIndices = new int[] { num };
                TechnologyElementPropertyProvider.SetValueUsingQueryId(uiTaskControl, selectedIndices, uiTaskControl.TechnologyName, childListItemCount);
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.ComboBox;
    }
}

