using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class ToggleButtonEventWrapper : PropertyChangeEventWrapper
    {
        public ToggleButtonEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink, TogglePattern.ToggleStateProperty, TreeScope.Element)
        {
        }

        protected override void OnPropertyChange(object sender, AutomationPropertyChangedEventArgs e)
        {
            if (e.NewValue is ToggleState)
            {
                bool flag = true;
                ToggleState newValue = (ToggleState)e.NewValue;
                ControlStates indeterminate = ControlStates.Default;
                switch (newValue)
                {
                    case ToggleState.Off:
                        indeterminate = ControlStates.None | ControlStates.Normal;
                        break;

                    case ToggleState.On:
                        indeterminate = ControlStates.None | ControlStates.Pressed;
                        break;

                    case ToggleState.Indeterminate:
                        indeterminate = ControlStates.Indeterminate;
                        break;

                    default:
                        flag = false;
                        break;
                }
                if (flag)
                {
                    CurrentValueOrState = indeterminate;
                    Notify();
                }
            }
        }
    }
}

