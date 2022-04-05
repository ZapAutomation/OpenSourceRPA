using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class TreeViewEventWrapper : PropertyChangeEventWrapper
    {
        public TreeViewEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink, ExpandCollapsePattern.ExpandCollapseStateProperty, TreeScope.Element)
        {
        }

        protected override void OnPropertyChange(object sender, AutomationPropertyChangedEventArgs e)
        {
            if (e.NewValue is ExpandCollapseState)
            {
                CurrentValueOrState = null;
                if (TaskActivityElement.IsState(Element, AccessibleStates.Expanded))
                {
                    CurrentValueOrState = ControlStates.Expanded;
                }
                else if (TaskActivityElement.IsState(Element, AccessibleStates.Collapsed))
                {
                    CurrentValueOrState = ControlStates.Collapsed;
                }
                if (CurrentValueOrState != null)
                {
                    Notify();
                }
            }
        }
    }
}

