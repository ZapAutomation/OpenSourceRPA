using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Helper;
using Zappy.Plugins.Uia.Uia.Utilities;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class CheckBoxEventWrapper : PropertyChangeEventWrapper
    {
        private bool isCheckBoxPartofDGCell;
        private UiaElement NotifyElement;

        public CheckBoxEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink, TogglePattern.ToggleStateProperty, TreeScope.Element)
        {
            isCheckBoxPartofDGCell = DataGridUtility.IsCheckBoxPartOfDataGridCell(element.InnerElement);
            if (isCheckBoxPartofDGCell)
            {
                LatestSourceElement = Element;
                NotifyElement = Element.Parent;
            }
            else
            {
                NotifyElement = Element;
            }
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
                ToggleState newValue = (ToggleState)e.NewValue;
                ControlStates indeterminate = ControlStates.Indeterminate;
                switch (newValue)
                {
                    case ToggleState.Off:
                        indeterminate = ControlStates.None | ControlStates.Normal;
                        break;

                    case ToggleState.On:
                        indeterminate = ControlStates.Checked;
                        break;
                }
                CurrentValueOrState = indeterminate;
                Notify();
            }
        }
    }
}

