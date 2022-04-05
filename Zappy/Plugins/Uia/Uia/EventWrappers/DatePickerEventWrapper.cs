using System;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class DatePickerEventWrapper : AutomationEventWrapper
    {
        private object currentValue;
        private UiaElement datepickerElement;
        private TextPattern textPattern;

        public DatePickerEventWrapper(UiaElement datePickerElement, UiaElement element, UiaElement sourceElement, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink)
        {
            DateTime time;
            datepickerElement = datePickerElement;
            LatestSourceElement = sourceElement;
            EventId.Add(TextPattern.TextChangedEvent);
            EventHandler = OnTextChanged;
            textPattern = PatternHelper.GetTextPattern(element.InnerElement);
            foreach (AutomationEvent event2 in EventId)
            {
                Automation.AddAutomationEventHandler(event2, element.InnerElement, TreeScope.Element, EventHandler);
            }
            if (ControlType.Edit != sourceElement.ControlTypeName && !ZappyTaskUtilities.TryGetDate(sourceElement.Name, out time) && !string.IsNullOrEmpty(PatternHelper.GetText(textPattern)))
            {
                NotifyEventDuringRemoval = true;
            }
        }

        public override void Notify()
        {
            try
            {
                if (EventSink != null && !string.IsNullOrEmpty(CurrentValueOrState as string))
                {
                    EventSink.Notify(LatestSourceElement != null ? LatestSourceElement : Element, datepickerElement, ZappyTaskEventType, currentValue);
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
        }

        private void OnTextChanged(object sender, AutomationEventArgs e)
        {
            if (LatestSourceElement == null)
            {
                LatestSourceElement = UiaElementFactory.GetUiaElement(sender as AutomationElement, false);
            }
            Notify();
        }

        public override bool ShouldFireFakeEvent() =>
            true;

        protected override object CurrentValueOrState
        {
            get
            {
                DateTime time;
                currentValue = null;
                if (ZappyTaskUtilities.TryGetShortDateAndLongTime(PatternHelper.GetText(textPattern), out time))
                {
                    currentValue = ZappyTaskUtilities.GetDateTimeToString(time, false);
                }
                return currentValue;
            }
            set
            {
                currentValue = value;
            }
        }
    }
}

