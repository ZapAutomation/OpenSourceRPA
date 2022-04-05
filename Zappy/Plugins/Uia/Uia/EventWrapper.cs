using System;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Plugins.Uia.Uia.EventWrappers;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Plugins.Uia.Uia
{
    internal abstract class EventWrapper : IDisposable
    {
        private bool disposed;

        protected EventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            Element = element;
            ZappyTaskEventType = eventType;
            EventSink = eventSink;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                object[] args = { GetType().Name, Element };
                                try
                {
                    DisposeInternal();
                }
                catch (ElementNotAvailableException)
                {
                }
                finally
                {
                                                                                                }
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected abstract void DisposeInternal();
        public static EventWrapper GetEventWrapper(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            UiaElement uiaElement = UiaElementFactory.GetUiaElement(element.Parent?.InnerElement, false);
            if (eventType == ZappyTaskEventType.ValueChanged)
            {
                if (!ControlType.Button.NameEquals(element.ControlTypeName))
                {
                    if (!ControlType.ToggleButton.NameEquals(element.ControlTypeName))
                    {
                        if (ControlType.Edit.NameEquals(element.ControlTypeName) && !TaskActivityElement.IsState(element, AccessibleStates.ReadOnly))
                        {
                            if (uiaElement != null)
                            {
                                if (ControlType.ComboBox.NameEquals(uiaElement.ControlTypeName))
                                {
                                    return new ComboBoxEventWrapper(uiaElement, element, eventType, eventSink);
                                }
                                if (ControlType.DatePicker.NameEquals(uiaElement.ControlTypeName))
                                {
                                    return new DatePickerEventWrapper(uiaElement, element, element, eventType, eventSink);
                                }
                            }
                            if (!element.IsPassword)
                            {
                                return new TextBoxEventWrapper(element, eventType, eventSink);
                            }
                            goto Label_040C;
                        }
                        if (ControlType.ComboBox.NameEquals(element.ControlTypeName))
                        {
                            return new ComboBoxEventWrapper(element, eventType, eventSink);
                        }
                        if (ControlType.ListItem.NameEquals(element.ControlTypeName) || ControlType.List.NameEquals(element.ControlTypeName))
                        {
                            if (uiaElement != null)
                            {
                                if (ControlType.ComboBox.NameEquals(uiaElement.ControlTypeName))
                                {
                                    return new ComboBoxEventWrapper(uiaElement, element, eventType, eventSink);
                                }
                                return new ListEventWrapper(uiaElement, element, eventType, eventSink);
                            }
                            object[] args = { element };
                            
                            goto Label_040C;
                        }
                        if (ControlType.Indicator.NameEquals(element.ControlTypeName))
                        {
                            if (uiaElement != null && ControlType.Slider.NameEquals(uiaElement.ControlTypeName) && !TaskActivityElement.IsState(uiaElement, AccessibleStates.ReadOnly))
                            {
                                return new SliderEventWrapper(uiaElement, element, eventType, eventSink);
                            }
                            goto Label_040C;
                        }
                        if (ControlType.Slider.NameEquals(element.ControlTypeName) && !TaskActivityElement.IsState(element, AccessibleStates.ReadOnly))
                        {
                            return new SliderEventWrapper(element, element, eventType, eventSink);
                        }
                        if (ControlType.Spinner.NameEquals(element.ControlTypeName))
                        {
                            goto Label_040C;
                        }
                        if (ControlType.Menu.NameEquals(element.ControlTypeName))
                        {
                            return new MenuEventWrapper(element, eventType, eventSink);
                        }
                        if (ControlType.MenuItem.NameEquals(element.ControlTypeName))
                        {
                            return new MenuCheckBoxEventWrapper(element, ZappyTaskEventType.StateChanged, eventSink);
                        }
                        if (ControlType.Cell.NameEquals(element.ControlTypeName))
                        {
                            AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(element.InnerElement);
                            if (firstChild != null && firstChild.Current.ControlType == System.Windows.Automation.ControlType.CheckBox)
                            {
                                return new CheckBoxEventWrapper(UiaElementFactory.GetUiaElement(firstChild, false), ZappyTaskEventType.StateChanged, eventSink);
                            }
                            goto Label_040C;
                        }
                        if (!ControlType.ToggleSwitch.NameEquals(element.ControlTypeName))
                        {
                            goto Label_040C;
                        }
                    }
                    return new ToggleButtonEventWrapper(element, ZappyTaskEventType.StateChanged, eventSink);
                }
                if (uiaElement != null)
                {
                    if (ControlType.TreeItem.NameEquals(uiaElement.ControlTypeName))
                    {
                        return new TreeViewEventWrapper(uiaElement, ZappyTaskEventType.StateChanged, eventSink);
                    }
                    if (ControlType.Slider.NameEquals(uiaElement.ControlTypeName) && !TaskActivityElement.IsState(uiaElement, AccessibleStates.ReadOnly))
                    {
                        return new SliderEventWrapper(uiaElement, element, eventType, eventSink);
                    }
                    if (ControlType.Calendar.NameEquals(uiaElement.ControlTypeName))
                    {
                        UiaElement parent = uiaElement.Parent;
                        if (parent != null)
                        {
                            UiaElement datePickerElement = null;
                            if (string.Equals(parent.ClassName, "Uia.Popup", StringComparison.OrdinalIgnoreCase) && UiaTechnologyManager.Instance.LastAccessedDatePicker != null)
                            {
                                datePickerElement = UiaTechnologyManager.Instance.LastAccessedDatePicker;
                            }
                            else if (ControlType.DatePicker.NameEquals(parent.ControlTypeName))
                            {
                                UiaTechnologyManager.Instance.LastAccessedDatePicker = parent;
                                datePickerElement = parent;
                            }
                            if (datePickerElement != null)
                            {
                                UiaElement element6 = UiaElementFactory.GetUiaElement(UiaUtility.GetDatePickerEditBox(datePickerElement.InnerElement), false);
                                if (element6 != null)
                                {
                                    return new DatePickerEventWrapper(datePickerElement, element6, element, eventType, eventSink);
                                }
                            }
                        }
                        return new CalendarEventWrapper(uiaElement, element, eventType, eventSink);
                    }
                    if (ControlType.DatePicker.NameEquals(uiaElement.ControlTypeName))
                    {
                        UiaElement element8 = UiaElementFactory.GetUiaElement(UiaUtility.GetDatePickerEditBox(uiaElement.InnerElement), false);
                        if (element8 != null)
                        {
                            return new DatePickerEventWrapper(uiaElement, element8, element, eventType, eventSink);
                        }
                    }
                }
                if (PatternHelper.GetTogglePattern(element.InnerElement) != null && ControlType.Expander.NameEquals(uiaElement.ControlTypeName))
                {
                    return new ExpanderEventWrapper(element, uiaElement, ZappyTaskEventType.StateChanged, eventSink);
                }
            }
            else if (eventType == ZappyTaskEventType.StateChanged)
            {
                if (ControlType.CheckBox.NameEquals(element.ControlTypeName))
                {
                    return new CheckBoxEventWrapper(element, eventType, eventSink);
                }
                if (ControlType.RadioButton.NameEquals(element.ControlTypeName))
                {
                    return new RadioButtonEventWrapper(element, eventType, eventSink);
                }
                if (ControlType.TreeItem.NameEquals(element.ControlTypeName))
                {
                    return new TreeViewEventWrapper(element, eventType, eventSink);
                }
            }
        Label_040C:
            return null;
        }

        public static EventWrapper GetEventWrapper(UiaElement element, ControlType elementType, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            if (eventType == ZappyTaskEventType.ValueChanged && elementType == ControlType.Menu)
            {
                return new MenuEventWrapper(element, eventType, eventSink);
            }
            return null;
        }

        public virtual void Notify()
        {
            try
            {
                if (EventSink != null)
                {
                    EventSink.Notify(LatestSourceElement != null ? LatestSourceElement : Element, Element, ZappyTaskEventType, CurrentValueOrState);
                }
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
        }

        public virtual bool ShouldFireFakeEvent() =>
            false;

        public virtual void UpdateSourceElement(UiaElement element)
        {
            LatestSourceElement = element;
        }

        protected virtual object CurrentValueOrState { get; set; }

        internal UiaElement Element { get; set; }

        protected IZappyTaskEventNotify EventSink { get; set; }

        protected UiaElement LatestSourceElement { get; set; }

        internal bool NotifyEventDuringRemoval { get; set; }

        protected ZappyTaskEventType ZappyTaskEventType { get; set; }
    }
}

