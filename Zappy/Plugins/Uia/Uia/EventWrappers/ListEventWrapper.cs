using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Mssa;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class ListEventWrapper : AutomationEventWrapper
    {
        public ListEventWrapper(UiaElement element, UiaElement sourceElement, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink)
        {
            LatestSourceElement = sourceElement;
            EventHandler = OnSelected;
            EventId.Add(SelectionItemPattern.ElementAddedToSelectionEvent);
            EventId.Add(SelectionItemPattern.ElementRemovedFromSelectionEvent);
            EventId.Add(SelectionItemPattern.ElementSelectedEvent);
            foreach (AutomationEvent event2 in EventId)
            {
                Automation.AddAutomationEventHandler(event2, element.InnerElement, TreeScope.Subtree, EventHandler);
            }
        }

        private void OnSelected(object sender, AutomationEventArgs e)
        {
            if (LatestSourceElement == null)
            {
                LatestSourceElement = UiaElementFactory.GetUiaElement(sender as AutomationElement, false);
            }
            AutomationElement innerElement = Element.InnerElement;
            SelectionPattern selectionPattern = PatternHelper.GetSelectionPattern(innerElement);
            if (selectionPattern != null)
            {
                List<string> selectedItems = null;
                List<AutomationElement> selectedAutomationElements = null;
                if (selectionPattern.Current.CanSelectMultiple)
                {
                    selectedItems = UiaUtility.GetSelectedItems(innerElement, ref selectedAutomationElements);
                }
                else
                {
                    selectedItems = UiaUtility.GetSelectedItemsUsingPattern(selectionPattern, ref selectedAutomationElements);
                }
                if (selectedItems != null)
                {
                    CommaListBuilder builder = new CommaListBuilder();
                    builder.AddRange(selectedItems);
                    CurrentValueOrState = builder.ToString();
                    Notify();
                }
            }
        }
    }
}

