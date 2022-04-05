using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class RadioButtonEventWrapper : PropertyChangeEventWrapper
    {
        public RadioButtonEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink, SelectionItemPattern.IsSelectedProperty, TreeScope.Element)
        {
        }

        protected override void OnPropertyChange(object sender, AutomationPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && (bool)e.NewValue)
            {
                CurrentValueOrState = ControlStates.Checked;
                Notify();
            }
        }
    }
}

