using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Helper;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class ExpanderEventWrapper : PropertyChangeEventWrapper
    {
        private UiaElement NotifyElement;

        public ExpanderEventWrapper(UiaElement element, UiaElement targetElement, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink, TogglePattern.ToggleStateProperty, TreeScope.Element)
        {
            NotifyElement = targetElement;
        }

        public override void Notify()
        {
            try
            {
                if (EventSink != null)
                {
                    EventSink.Notify(LatestSourceElement != null ? LatestSourceElement : NotifyElement, NotifyElement, ZappyTaskEventType, CurrentValueOrState);
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
        }

        protected override void OnPropertyChange(object sender, AutomationPropertyChangedEventArgs e)
        {
            if (e.NewValue is ToggleState)
            {
                bool flag = true;
                ToggleState newValue = (ToggleState)e.NewValue;
                ControlStates collapsed = ControlStates.Default;
                switch (newValue)
                {
                    case ToggleState.Off:
                        collapsed = ControlStates.Collapsed;
                        break;

                    case ToggleState.On:
                        collapsed = ControlStates.Expanded;
                        break;

                    default:
                        flag = false;
                        break;
                }
                if (flag)
                {
                    if (LatestSourceElement == null)
                    {
                        LatestSourceElement = Element;
                    }
                    CurrentValueOrState = collapsed;
                    Notify();
                }
            }
        }
    }
}

