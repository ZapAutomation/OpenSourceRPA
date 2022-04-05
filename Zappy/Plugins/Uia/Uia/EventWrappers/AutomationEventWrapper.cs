using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal abstract class AutomationEventWrapper : EventWrapper
    {
        protected AutomationEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink)
        {
            EventId = new List<AutomationEvent>();
        }

        protected override void DisposeInternal()
        {
            if (EventHandler != null)
            {
                foreach (AutomationEvent event2 in EventId)
                {
                    Automation.RemoveAutomationEventHandler(event2, Element.InnerElement, EventHandler);
                }
                EventHandler = null;
            }
        }

        protected AutomationEventHandler EventHandler { get; set; }

        protected List<AutomationEvent> EventId { get; set; }
    }
}

