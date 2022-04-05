using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class CalendarEventWrapper : AutomationEventWrapper
    {
        private object currentValue;

        public CalendarEventWrapper(UiaElement element, UiaElement sourceElement, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink)
        {
            DateTime time;
            LatestSourceElement = sourceElement;
            EventHandler = OnSelected;
            EventId.Add(SelectionItemPattern.ElementAddedToSelectionEvent);
            EventId.Add(SelectionItemPattern.ElementRemovedFromSelectionEvent);
            EventId.Add(SelectionItemPattern.ElementSelectedEvent);
            foreach (AutomationEvent event2 in EventId)
            {
                Automation.AddAutomationEventHandler(event2, element.InnerElement, TreeScope.Children, EventHandler);
            }
            if (!ZappyTaskUtilities.TryGetDate(sourceElement.Name, out time))
            {
                AutomationElement[] selection = PatternHelper.GetSelection(PatternHelper.GetSelectionPattern(element.InnerElement));
                if (selection != null && selection.Length != 0)
                {
                    NotifyEventDuringRemoval = true;
                }
            }
        }

        public override void Notify()
        {
            try
            {
                if (EventSink != null && !string.IsNullOrEmpty(CurrentValueOrState as string))
                {
                    EventSink.Notify(LatestSourceElement != null ? LatestSourceElement : Element, Element, ZappyTaskEventType, CurrentValueOrState);
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
        }

        private void OnSelected(object sender, AutomationEventArgs e)
        {
            if (LatestSourceElement == null)
            {
                LatestSourceElement = UiaElementFactory.GetUiaElement(sender as AutomationElement, false);
            }
            SelectionPattern selectionPattern = PatternHelper.GetSelectionPattern(Element.InnerElement);
            if (selectionPattern != null)
            {
                List<AutomationElement> selectedAutomationElements = null;
                List<string> selectedItemsUsingPattern = UiaUtility.GetSelectedItemsUsingPattern(selectionPattern, ref selectedAutomationElements);
                if (selectedItemsUsingPattern != null)
                {
                    CurrentValueOrState = UiaUtility.GetDateStringFromSelectionList(selectedItemsUsingPattern);
                    if (!string.IsNullOrEmpty(currentValue as string))
                    {
                        Notify();
                    }
                    CurrentValueOrState = null;
                }
            }
        }

        public override bool ShouldFireFakeEvent() =>
            true;

        protected override object CurrentValueOrState
        {
            get
            {
                if (currentValue == null)
                {
                    SelectionPattern selectionPattern = PatternHelper.GetSelectionPattern(Element.InnerElement);
                    if (selectionPattern != null)
                    {
                        List<AutomationElement> selectedAutomationElements = null;
                        List<string> selectedItemsUsingPattern = UiaUtility.GetSelectedItemsUsingPattern(selectionPattern, ref selectedAutomationElements);
                        currentValue = UiaUtility.GetDateStringFromSelectionList(selectedItemsUsingPattern);
                    }
                }
                return currentValue;
            }
            set
            {
                currentValue = value;
            }
        }
    }
}

