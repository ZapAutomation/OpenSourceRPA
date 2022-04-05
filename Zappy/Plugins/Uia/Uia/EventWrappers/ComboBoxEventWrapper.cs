using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Plugins.Uia.Uia.Utilities;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class ComboBoxEventWrapper : AutomationEventWrapper
    {
        private bool isDataGridComboBox;
        private UiaElement NotifyElement;
        private object sender;
        private TextPattern textPattern;

        public ComboBoxEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink)
        {
            EventHandler = OnValueChange;
            EventId.Add(SelectionItemPattern.ElementSelectedEvent);
            Automation.AddAutomationEventHandler(EventId[0], Element.InnerElement, TreeScope.Subtree, EventHandler);
            if (DataGridUtility.IsElementNotTemplateContentOfCell(element.InnerElement))
            {
                LatestSourceElement = Element;
                NotifyElement = element.Parent;
                isDataGridComboBox = true;
            }
            else if (element.Parent != null && DataGridUtility.IsElementNotTemplateContentOfCell(element.Parent.InnerElement))
            {
                LatestSourceElement = Element;
                NotifyElement = element.Parent.Parent;
                isDataGridComboBox = true;
                if (LatestSourceElement.ControlTypeName == ControlType.ListItem.Name)
                {
                    object parent = LatestSourceElement.Parent;
                }
            }
            else
            {
                NotifyElement = Element;
            }
        }

        public ComboBoxEventWrapper(UiaElement element, UiaElement sourceElement, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : this(element, eventType, eventSink)
        {
            LatestSourceElement = sourceElement;
            textPattern = PatternHelper.GetTextPattern(sourceElement.InnerElement);
            if (textPattern != null)
            {
                EventId.Add(TextPattern.TextChangedEvent);
                Automation.AddAutomationEventHandler(EventId[1], element.InnerElement, TreeScope.Subtree, EventHandler);
            }
            AutomationElement[] selection = PatternHelper.GetSelection(PatternHelper.GetSelectionPattern(element.InnerElement));
            if (selection != null && selection.Length == 1 && Automation.Compare(selection[0], sourceElement.InnerElement))
            {
                NotifyEventDuringRemoval = true;
            }
        }

        private bool CanFireEvent()
        {
            UiaElement element = isDataGridComboBox ? LatestSourceElement : NotifyElement;
            return element != null && element.Value != null || UiaUtility.IsValidListItemForEvent(SelectedListItem.InnerElement);
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

        private void OnValueChange(object senderObject, AutomationEventArgs e)
        {
            sender = senderObject;
            if (CanFireEvent())
            {
                object[] args = { e.EventId.ProgrammaticName, NotifyElement };
                
                Notify();
            }
        }

        public override bool ShouldFireFakeEvent()
        {
            try
            {
                if (NotifyElement != null && TaskActivityElement.IsState(NotifyElement, AccessibleStates.Collapsed) && CanFireEvent())
                {
                    return true;
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
            return false;
        }

        protected override object CurrentValueOrState
        {
            get
            {
                string str = null;
                UiaElement element = isDataGridComboBox ? LatestSourceElement : NotifyElement;
                if (element != null && (ControlType.ComboBox.NameEquals(element.ControlTypeName) || isDataGridComboBox) && (element.Value != null || !string.IsNullOrEmpty(SelectedListItem.Name)))
                {
                    str = element.Value != null ? element.Value : SelectedListItem.Name;
                }
                return str;
            }
        }

        private UiaElement SelectedListItem
        {
            get
            {
                if (isDataGridComboBox)
                {
                    return UiaElementFactory.GetUiaElement(sender as AutomationElement, false);
                }
                return LatestSourceElement ?? UiaElementFactory.GetUiaElement(sender as AutomationElement, false);
            }
        }
    }
}

