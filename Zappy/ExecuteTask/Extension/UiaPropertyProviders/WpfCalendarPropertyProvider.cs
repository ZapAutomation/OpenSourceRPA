using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Extension.WpfControls;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.SharedInterface.Helper;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.UiaPropertyProviders
{
    internal class WpfCalendarPropertyProvider : WpfControlPropertyProvider
    {
        private static Regex blackoutDateRegex = new Regex(@"Blackout\sDay\s.+", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private const string DateRangeDelimiter = "-";
        private const string DoubleQuotes = "\"";

        public WpfCalendarPropertyProvider()
        {
            m_DescriptorDictionary = this.InitializeDescriptorDictionary();
        }

        private void DoDateSelection(string dateValue, int invokeCount, bool isRangeSelection, out DateTime dateTime, ZappyTaskControl UIControl, MultipleViewPattern CalendarMultiViewPattern, ItemContainerPattern calendarItemContainerPattern)
        {
            bool flag;
            string name = string.Empty;
            if (ZappyTaskUtilities.TryGetShortDate(dateValue, out dateTime))
            {
                DateTimeFormatInfo dateFormat = ZappyTaskUtilities.GetDateFormat(CultureInfo.CurrentCulture);
                name = dateTime.ToString(dateFormat.LongDatePattern, dateFormat);
            }
            else
            {
                object[] args = new object[] { dateValue, ZappyTaskUtilities.DateFormat, WallClock.Now.ToShortDateString(), ZappyTaskUtilities.DateFormat, ZappyTaskUtilities.GetDateTimeToString(WallClock.Now, false) };
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidDateFormat, args));
            }
            CalendarMultiViewPattern.SetCurrentView(0);
            this.RealizeCalendarElement(calendarItemContainerPattern, name, out flag);
            if (flag)
            {
                object[] objArray2 = new object[] { dateTime.ToShortDateString() };
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.BlackoutDate, objArray2));
            }
            ModifierKeys none = ModifierKeys.None;
            if (invokeCount > 0)
            {
                none = isRangeSelection ? ModifierKeys.Shift : ModifierKeys.Control;
            }
            ZappyTaskControl control = new ZappyTaskControl(UIControl)
            {
                TechnologyName = UIControl.TechnologyName,
                SearchProperties = { {
                    ZappyTaskControl.PropertyNames.Name,
                    name
                } }
            };
            control.Click(MouseButtons.Left, none, new Point(-1, -1));
            AutomationElement nativeElement = control.TechnologyElement.NativeElement as AutomationElement;
            if (nativeElement != null)
            {
                SelectionItemPattern pattern = GetAutomationPattern<SelectionItemPattern>(nativeElement, SelectionItemPattern.Pattern, AutomationElement.IsSelectionItemPatternAvailableProperty);
                if ((pattern != null) && !pattern.Current.IsSelected)
                {
                    control.Click(MouseButtons.Left, none, new Point(-1, -1));
                }
            }
            invokeCount++;
        }

        public override System.Type GetPropertyClassName() =>
            typeof(WpfCalendar.PropertyNames);

        public override string GetPropertyForAction(ZappyTaskAction action) =>
            WpfCalendar.PropertyNames.SelectedDatesAsString;

        public override object GetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || string.Equals(WpfCalendar.PropertyNames.SelectedDatesAsString, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                CommaListBuilder builder = new CommaListBuilder();
                string[] strArray = uiTaskControl.TechnologyElement.GetPropertyValue(WpfCalendar.PropertyNames.SelectedDates) as string[];
                if ((strArray != null) && (strArray.Length != 0))
                {
                    foreach (string str in strArray)
                    {
                        string str2;
                        if (ZappyTaskUtilities.TryGetDateString(str, out str2))
                        {
                            builder.AddValue(str2);
                        }
                    }
                }
                return builder.ToString();
            }
            if (string.Equals(WpfCalendar.PropertyNames.MultiSelectable, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                AutomationElement nativeElement = uiTaskControl.TechnologyElement.NativeElement as AutomationElement;
                return this.IsCalendarMultiSelectable(nativeElement);
            }
            if (!string.Equals(WpfCalendar.PropertyNames.SelectedDates, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return base.GetPropertyValue(uiTaskControl, propertyName);
            }
            List<DateTime> list = new List<DateTime>();
            string[] propertyValue = uiTaskControl.TechnologyElement.GetPropertyValue(WpfCalendar.PropertyNames.SelectedDates) as string[];
            if ((propertyValue != null) && (propertyValue.Length != 0))
            {
                foreach (string str3 in propertyValue)
                {
                    DateTime time;
                    if (ZappyTaskUtilities.TryGetDate(str3, out time))
                    {
                        list.Add(time);
                    }
                }
            }
            return list.ToArray();
        }

        public override System.Type GetSpecializedClass() =>
            typeof(WpfCalendar);

        private Dictionary<string, ZappyTaskPropertyDescriptor> InitializeDescriptorDictionary()
        {
            m_ControlSpecificProperties = new Dictionary<string, ZappyTaskPropertyDescriptor>(m_CommonPropertyNames, StringComparer.OrdinalIgnoreCase);
            m_ControlSpecificProperties.Add(WpfCalendar.PropertyNames.SelectedDates, new ZappyTaskPropertyDescriptor(typeof(DateTime[]), s_ReadWritePermissions | ZappyTaskPropertyAttributes.NonAssertable));
            m_ControlSpecificProperties.Add(WpfCalendar.PropertyNames.SelectedDatesAsString, new ZappyTaskPropertyDescriptor(typeof(string), s_ReadWritePermissions));
            m_ControlSpecificProperties.Add(WpfCalendar.PropertyNames.SelectedDateRange, new ZappyTaskPropertyDescriptor(typeof(SelectionRange), ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Writable));
            m_ControlSpecificProperties.Add(WpfCalendar.PropertyNames.SelectedDateRangeAsString, new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Writable));
            m_ControlSpecificProperties.Add(WpfCalendar.PropertyNames.MultiSelectable, new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable));
            return m_ControlSpecificProperties;
        }

        private bool IsCalendarMultiSelectable(AutomationElement calendarAutomationElement)
        {
            bool canSelectMultiple = false;
            SelectionPattern pattern = GetAutomationPattern<SelectionPattern>(calendarAutomationElement, SelectionPattern.Pattern, AutomationElement.IsSelectionPatternAvailableProperty);
            if (pattern != null)
            {
                canSelectMultiple = pattern.Current.CanSelectMultiple;
            }
            return canSelectMultiple;
        }

        public override bool IsCommonReadableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonReadableProperty(propertyName));

        public override bool IsCommonWritableProperty(string propertyName) =>
            (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || base.IsCommonWritableProperty(propertyName));

        private bool IsDateRangeValid(string dateMinString, string dateMaxString)
        {
            DateTime time;
            DateTime time2;
            return ((ZappyTaskUtilities.TryGetShortDate(dateMinString, out time) && ZappyTaskUtilities.TryGetShortDate(dateMaxString, out time2)) && (DateTime.Compare(time, time2) <= 0));
        }

        private void RealizeCalendarElement(ItemContainerPattern calendarItemContainerPattern, string name, out bool isblackoutDate)
        {
            isblackoutDate = false;
            if (calendarItemContainerPattern != null)
            {
                AutomationElement automationElement = calendarItemContainerPattern.FindItemByProperty(null, AutomationElement.NameProperty, name);
                if (automationElement != null)
                {
                    VirtualizedItemPattern pattern = null;
                    try
                    {
                        pattern = GetAutomationPattern<VirtualizedItemPattern>(automationElement, VirtualizedItemPattern.Pattern, AutomationElement.IsVirtualizedItemPatternAvailableProperty);
                    }
                    catch (NotSupportedException)
                    {
                    }
                    if (pattern != null)
                    {
                        pattern.Realize();
                    }
                    string helpText = automationElement.Current.HelpText;
                    if (blackoutDateRegex.IsMatch(helpText))
                    {
                        isblackoutDate = true;
                    }
                }
                else
                {
                    isblackoutDate = true;
                }
            }
        }

        public override void SetPropertyValue(ZappyTaskControl uiTaskControl, string propertyName, object value)
        {
            string str = propertyName;
            object[] args = new object[] { propertyName };
                        AutomationElement nativeElement = uiTaskControl.TechnologyElement.NativeElement as AutomationElement;
            MultipleViewPattern calendarMultiViewPattern = null;
            ItemContainerPattern calendarItemContainerPattern = null;
            calendarItemContainerPattern = GetAutomationPattern<ItemContainerPattern>(nativeElement, ItemContainerPattern.Pattern, AutomationElement.IsItemContainerPatternAvailableProperty);
            calendarMultiViewPattern = GetAutomationPattern<MultipleViewPattern>(nativeElement, MultipleViewPattern.Pattern, AutomationElement.IsMultipleViewPatternAvailableProperty);
            if (string.Equals(ZappyTaskControl.PropertyNames.Value, propertyName, StringComparison.OrdinalIgnoreCase) || string.Equals(WpfCalendar.PropertyNames.SelectedDatesAsString, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                string str2 = ZappyTaskUtilities.ConvertToType<string>(value);
                List<string> commaSeparatedValues = CommaListBuilder.GetCommaSeparatedValues(str2);
                if ((commaSeparatedValues != null) && (commaSeparatedValues.Count > 0))
                {
                    if ((commaSeparatedValues.Count > 1) && !this.IsCalendarMultiSelectable(nativeElement))
                    {
                        object[] objArray2 = new object[] { str2, uiTaskControl.ControlType.Name };
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidMultiSelectionState, objArray2));
                    }
                    int num = 0;
                    CommaListBuilder builder = new CommaListBuilder();
                    DateTimeFormatInfo dateFormat = ZappyTaskUtilities.GetDateFormat(CultureInfo.CurrentCulture);
                    foreach (string str5 in commaSeparatedValues)
                    {
                        DateTime time;
                        this.DoDateSelection(str5, num++, false, out time, uiTaskControl, calendarMultiViewPattern, calendarItemContainerPattern);
                        string dateTimeToString = ZappyTaskUtilities.GetDateTimeToString(time, false);
                        builder.AddValue(dateTimeToString);
                    }
                    string b = builder.ToString();
                    string propertyValue = this.GetPropertyValue(uiTaskControl, WpfCalendar.PropertyNames.SelectedDatesAsString) as string;
                    if (!string.Equals(propertyValue, b, StringComparison.OrdinalIgnoreCase))
                    {
                        object[] objArray3 = new object[] { str2 };
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.BlackoutDate, objArray3));
                    }
                }
            }
            else if (string.Equals(WpfCalendar.PropertyNames.SelectedDateRangeAsString, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                string str8;
                string str9;
                DateTime time2;
                DateTime time3;
                string str7 = ZappyTaskUtilities.ConvertToType<string>(value);
                ZappyTaskUtilities.TryGetDateTimeRangeString(str7, out str8, out str9);
                if (string.IsNullOrEmpty(str8) || string.IsNullOrEmpty(str9))
                {
                    string str10 = this.SetQuotes(ZappyTaskUtilities.DateFormat) + "-" + this.SetQuotes(ZappyTaskUtilities.DateFormat);
                    string str11 = this.SetQuotes(WallClock.Now.ToShortDateString()) + "-" + this.SetQuotes(WallClock.Now.AddDays(2.0).ToShortDateString());
                    string str12 = this.SetQuotes(ZappyTaskUtilities.GetDateTimeToString(WallClock.Now, false)) + "-" + this.SetQuotes(ZappyTaskUtilities.GetDateTimeToString(WallClock.Now, false));
                    object[] objArray7 = new object[] { str7, str10, str11, str10, str12 };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidDateFormat, objArray7));
                }
                if (!this.IsDateRangeValid(str8, str9))
                {
                    object[] objArray4 = new object[] { str7, str };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidParameterValue, objArray4));
                }
                if (!string.Equals(str8, str9, StringComparison.OrdinalIgnoreCase) && !this.IsCalendarMultiSelectable(nativeElement))
                {
                    object[] objArray5 = new object[] { str7, uiTaskControl.ControlType.Name };
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidMultiSelectionState, objArray5));
                }
                this.DoDateSelection(str8, 0, true, out time2, uiTaskControl, calendarMultiViewPattern, calendarItemContainerPattern);
                if (!string.Equals(str8, str9, StringComparison.OrdinalIgnoreCase))
                {
                    this.DoDateSelection(str9, 1, true, out time3, uiTaskControl, calendarMultiViewPattern, calendarItemContainerPattern);
                }
                else
                {
                    time3 = time2;
                }
                if (!this.ValidateRangeSelection(uiTaskControl, time2, time3))
                {
                    object[] objArray6 = new object[] { str7 };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.BlackoutDateRange, objArray6));
                }
            }
            else if (string.Equals(WpfCalendar.PropertyNames.SelectedDates, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                DateTime[] timeArray = ZappyTaskUtilities.ConvertToType<DateTime[]>(value);
                if ((timeArray != null) && (timeArray.Length != 0))
                {
                    CommaListBuilder builder2 = new CommaListBuilder();
                    foreach (DateTime time5 in timeArray)
                    {
                        builder2.AddValue(time5.ToShortDateString());
                    }
                    this.SetPropertyValue(uiTaskControl, WpfCalendar.PropertyNames.SelectedDatesAsString, builder2.ToString());
                }
            }
            else if (string.Equals(WpfCalendar.PropertyNames.SelectedDateRange, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                SelectionRange range = ZappyTaskUtilities.ConvertToType<SelectionRange>(value);
                string shortDateRangeString = ZappyTaskUtilities.GetShortDateRangeString(range.Start, range.End);
                this.SetPropertyValue(uiTaskControl, WpfCalendar.PropertyNames.SelectedDateRangeAsString, shortDateRangeString);
            }
            else
            {
                object[] objArray8 = new object[] { propertyName, uiTaskControl.ControlType.Name };
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SetPropertyFailed, objArray8));
            }
        }

        private string SetQuotes(string date) =>
            ("\"" + date + "\"");

        private bool ValidateRangeSelection(ZappyTaskControl UIControl, DateTime minDate, DateTime maxDate)
        {
            string propertyValue = this.GetPropertyValue(UIControl, WpfCalendar.PropertyNames.SelectedDatesAsString) as string;
            for (DateTime time = minDate; DateTime.Compare(time, maxDate) <= 0; time = time.AddDays(1.0))
            {
                string dateTimeToString = ZappyTaskUtilities.GetDateTimeToString(time, false);
                if (!propertyValue.Contains(dateTimeToString))
                {
                    return false;
                }
            }
            return true;
        }

        public override ControlType SupportedControlType =>
            ControlType.Calendar;
    }
}

