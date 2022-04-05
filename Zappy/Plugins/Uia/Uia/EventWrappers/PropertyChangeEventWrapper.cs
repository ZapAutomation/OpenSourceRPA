using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal abstract class PropertyChangeEventWrapper : EventWrapper
    {
        protected PropertyChangeEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink, AutomationProperty propertyId, TreeScope scope) : base(element, eventType, eventSink)
        {
            PropertyChangeHandler = OnPropertyChange;
            AutomationProperty[] properties = { propertyId };
            Automation.AddAutomationPropertyChangedEventHandler(element.InnerElement, scope, PropertyChangeHandler, properties);
        }

        protected override void DisposeInternal()
        {
            if (PropertyChangeHandler != null)
            {
                Automation.RemoveAutomationPropertyChangedEventHandler(Element.InnerElement, PropertyChangeHandler);
                PropertyChangeHandler = null;
            }
        }

        protected abstract void OnPropertyChange(object sender, AutomationPropertyChangedEventArgs e);

        protected AutomationPropertyChangedEventHandler PropertyChangeHandler { get; set; }
    }
}

