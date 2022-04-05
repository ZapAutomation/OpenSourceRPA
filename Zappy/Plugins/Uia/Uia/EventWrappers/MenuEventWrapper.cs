using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class MenuEventWrapper : PropertyChangeEventWrapper
    {
        public MenuEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink, ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, TreeScope.Descendants)
        {
        }

        protected override void OnPropertyChange(object sender, AutomationPropertyChangedEventArgs e)
        {
            AutomationElement automationElement = sender as AutomationElement;
            if (automationElement != null && UiaTechnologyManager.Instance != null && e.NewValue is ExpandCollapseState)
            {
                if ((ExpandCollapseState)e.NewValue == ExpandCollapseState.Expanded)
                {
                    UiaElement uiaElement = UiaElementFactory.GetUiaElement(automationElement, false);
                    if (uiaElement != null && UiaTechnologyManager.Instance != null && uiaElement.Parent != null && uiaElement.Parent.ControlTypeName == ControlType.Menu)
                    {
                        if (UiaTechnologyManager.Instance.LastAccessedMenu == null)
                        {
                            UiaTechnologyManager.Instance.LastAccessedMenu = uiaElement.Parent;
                        }
                        UiaTechnologyManager.Instance.PropertyChangeMenuItemStack.Clear();
                    }
                    if (uiaElement != null)
                    {
                        UiaTechnologyManager.Instance.PropertyChangeMenuItemStack.Push(uiaElement);
                    }
                }
                else if ((ExpandCollapseState)e.NewValue == ExpandCollapseState.Collapsed && UiaTechnologyManager.Instance.PropertyChangeMenuItemStack.Count > 0 && UiaTechnologyManager.Instance.PropertyChangeMenuItemStack.Peek().InnerElement.Equals(automationElement))
                {
                    UiaTechnologyManager.Instance.PropertyChangeMenuItemStack.Pop();
                }
            }
        }
    }
}

