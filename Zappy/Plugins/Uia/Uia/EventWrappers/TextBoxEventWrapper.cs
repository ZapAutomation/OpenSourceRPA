using System;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Helper;
using Zappy.Plugins.Uia.Uia.Utilities;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class TextBoxEventWrapper : AutomationEventWrapper
    {
        private bool isDataGridTextBox;
        private UiaElement NotifyElement;
        private TextPattern textPattern;

        public TextBoxEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink)
        {
            EventId.Add(TextPattern.TextChangedEvent);
            EventHandler = OnTextChanged;
            textPattern = PatternHelper.GetTextPattern(element.InnerElement);
            Automation.AddAutomationEventHandler(EventId[0], element.InnerElement, TreeScope.Element, EventHandler);
            NotifyElement = Element;
            if (DataGridUtility.IsElementNotTemplateContentOfCell(element.InnerElement))
            {
                LatestSourceElement = Element;
                NotifyElement = Element.Parent;
                isDataGridTextBox = true;
            }
        }

        private bool IsTextBoxOffScreenForDataGridCell() =>
            isDataGridTextBox && LatestSourceElement != null && LatestSourceElement.InnerElement.Current.IsOffscreen;

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

        private void OnTextChanged(object sender, AutomationEventArgs e)
        {
            string text = PatternHelper.GetText(textPattern);
            string currentValueOrState = CurrentValueOrState as string;
            if (!string.Equals(text, currentValueOrState, StringComparison.Ordinal))
            {
                if (IsTextBoxOffScreenForDataGridCell())
                {
                    CurrentValueOrState = Element.Parent.Value;
                }
                else
                {
                    CurrentValueOrState = text;
                }
                Notify();
            }
        }
    }
}

