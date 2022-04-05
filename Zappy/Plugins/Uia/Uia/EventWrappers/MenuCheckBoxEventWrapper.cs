using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal sealed class MenuCheckBoxEventWrapper : AutomationEventWrapper
    {
        public MenuCheckBoxEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink)
        {
            if (TaskActivityElement.IsState(element, AccessibleStates.Checked))
            {
                CurrentValueOrState = ControlStates.Checked;
            }
            else
            {
                CurrentValueOrState = ControlStates.None | ControlStates.Normal;
            }
            EventId.Add(InvokePattern.InvokedEvent);
            EventHandler = OnInvoke;
            Automation.AddAutomationEventHandler(EventId[0], element.InnerElement, TreeScope.Element, EventHandler);
        }

        private void OnInvoke(object sender, AutomationEventArgs e)
        {
            ControlStates states;
            if (TaskActivityElement.IsState(Element, AccessibleStates.Checked))
            {
                states = ControlStates.Checked;
            }
            else
            {
                states = ControlStates.None | ControlStates.Normal;
            }
            if (states != (ControlStates)CurrentValueOrState)
            {
                CurrentValueOrState = states;
                Notify();
            }
        }
    }
}

