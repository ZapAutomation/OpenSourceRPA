using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.SharedInterface.Helper;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfDatePickerPropertyProvider : WpfControlPropertyProvider
    {
        private const string DatePickerEditBoxAutomationId = "PART_TextBox";

        public WpfDatePickerPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        private void ExpandCollapseDatePickerControl(ZappyTaskControl UIControl, bool expand)
        {
            AccessibleStates requestedState = UIControl.TechnologyElement.GetRequestedState(AccessibleStates.Expanded);
            AutomationElement nativeElement = UIControl.TechnologyElement.NativeElement as AutomationElement;
            ExpandCollapsePattern pattern = GetAutomationPattern<ExpandCollapsePattern>(nativeElement, ExpandCollapsePattern.Pattern, AutomationElement.IsExpandCollapsePatternAvailableProperty);
            if (pattern != null)
            {
                if (expand && ((requestedState & AccessibleStates.Expanded) != AccessibleStates.Expanded))
                {
                    pattern.Expand();
                }
                else if (!expand && ((requestedState & AccessibleStates.Expanded) == AccessibleStates.Expanded))
                {
                    pattern.Collapse();
                }
            }
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfDatePicker.PropertyNames);

        public override string GetPropertyForAction(ZappyTaskAction action) =>
            WpfDatePicker.PropertyNames.DateAsString;

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || string.Equals(WpfDatePicker.PropertyNames.DateAsString, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                string dateTimeToString = string.Empty;
                object propertyValue = uiTaskControl.TechnologyElement.GetPropertyValue(WpfDatePicker.PropertyNames.Date);
                if (propertyValue != null)
                {
                    dateTimeToString = ZappyTaskUtilities.GetDateTimeToString((DateTime)propertyValue, false);
                }
                return dateTimeToString;
            }
            if (string.Equals(WpfDatePicker.PropertyNames.Date, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return uiTaskControl.TechnologyElement.GetPropertyValue(WpfDatePicker.PropertyNames.Date);
            }
            if (string.Equals(WpfDatePicker.PropertyNames.ShowCalendar, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return TaskActivityElement.IsState(uiTaskControl.TechnologyElement, AccessibleStates.Expanded);
            }
            if (!string.Equals(WpfDatePicker.PropertyNames.Calendar, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(uiTaskControl, propertyName);
            }
            AutomationElement nativeElement = uiTaskControl.TechnologyElement.NativeElement as AutomationElement;
            AutomationElement element2 = null;
            if (nativeElement != null)
            {
                System.Windows.Automation.PropertyCondition condition = new System.Windows.Automation.PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Calendar);
                element2 = nativeElement.FindFirst(TreeScope.Children, condition);
            }
            if (element2 == null)
            {
                return null;
            }
            return ZappyTaskControlFactory.FromNativeElement(element2, "UIA");
        }

        public override System.Type GetSpecializedClass() =>
            typeof(WpfDatePicker);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfDatePicker.PropertyNames.Date, new ZappyTaskPropertyDescriptor(typeof(DateTime), s_ReadWritePermissions | ZappyTaskPropertyAttributes.NonAssertable));
            m_ControlSpecificProperties.Add(WpfDatePicker.PropertyNames.DateAsString, new ZappyTaskPropertyDescriptor(typeof(string), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfDatePicker.PropertyNames.ShowCalendar, new ZappyTaskPropertyDescriptor(typeof(bool), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfDatePicker.PropertyNames.Calendar, new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControl), ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable));
            return m_ControlSpecificProperties;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            AutomationElement nativeElement = uiTaskControl.TechnologyElement.NativeElement as AutomationElement;
            if (string.Equals(WpfDatePicker.PropertyNames.ShowCalendar, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                bool expand = ZappyTaskUtilities.ConvertToType<bool>(value);
                this.ExpandCollapseDatePickerControl(uiTaskControl, expand);
            }
            else if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || string.Equals(WpfDatePicker.PropertyNames.DateAsString, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                DateTime time;
                string str = ZappyTaskUtilities.ConvertToType<string>(value);
                if (!ZappyTaskUtilities.TryGetShortDateAndLongTime(str, out time))
                {
                    object[] args = new object[] { str, ZappyTaskUtilities.DateFormat, WallClock.Now.ToShortDateString(), ZappyTaskUtilities.DateFormat, ZappyTaskUtilities.GetDateTimeToString(WallClock.Now, false) };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidDateFormat, args));
                }
                System.Windows.Automation.PropertyCondition condition = new System.Windows.Automation.PropertyCondition(AutomationElement.AutomationIdProperty, "PART_TextBox");
                AutomationElement element2 = nativeElement.FindFirst(TreeScope.Children, condition);
                if (element2 != null)
                {
                    try
                    {
                        ZappyTaskControl.FromNativeElement(element2, "UIA").Click(MouseButtons.Left, ModifierKeys.None, new Point(-1, -1));
                    }
                    catch (ZappyTaskException)
                    {
                    }
                }
                ValuePattern pattern = GetAutomationPattern<ValuePattern>(nativeElement, ValuePattern.Pattern, AutomationElement.IsValuePatternAvailableProperty);
                if (pattern != null)
                {
                    pattern.SetValue(str);
                    string dateTimeToString = ZappyTaskUtilities.GetDateTimeToString(time, false);
                    string propertyValue = this.GetPropertyValue(uiTaskControl, WpfDatePicker.PropertyNames.DateAsString) as string;
                    if (!string.Equals(dateTimeToString, propertyValue, StringComparison.OrdinalIgnoreCase))
                    {
                        object[] objArray2 = new object[] { str };
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.BlackoutDate, objArray2));
                    }
                }
            }
            else if (string.Equals(WpfDatePicker.PropertyNames.Date, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                this.SetPropertyValue(uiTaskControl, WpfDatePicker.PropertyNames.DateAsString, ZappyTaskUtilities.ConvertToType<DateTime>(value).ToShortDateString());
            }
            else
            {
                object[] objArray3 = new object[] { propertyName, uiTaskControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SetPropertyFailed, objArray3));
            }
        }

        public override ControlType SupportedControlType =>
            ControlType.DatePicker;
    }
}

