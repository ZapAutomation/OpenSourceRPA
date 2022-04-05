using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.Decode.Mssa
{
    internal class MsaaEventManager : IDisposable
    {
        private const string carriageReturn = "\r";
        private List<AccessibleEvents> currentWinHookEvents;
        private ITaskActivityElement delayedRemovalElement;
        private readonly object dictLock = new object();
        private bool disposed;
        private static ConcurrentDictionary<IntPtr, Rectangle> elementBoundingRectMapping;
        private Dictionary<MsaaElement, ElementEventSink> elementNotifySinkMapping;
        private int eventProcessId;
        private Dictionary<AccessibleEvents, NativeMethods.WinEventProc> eventToHandlerMap;
        private Dictionary<AccessibleEvents, IntPtr> eventToHookMap;
        private Dictionary<IntPtr, NativeMethods.WinEventProc> hookToDelegateMap;
        private MsaaElement lastIE8AddressBarEdit;
        private List<AccessibleEvents> lastWinHookEvents;
        private MsaaZappyPlugin msaaPlugin;
        private MsaaElement richTextBox;
        private MsaaElement slider;
        private MsaaElement sliderSource;
        private readonly object syncLock = new object();

        public MsaaEventManager(MsaaZappyPlugin plugin)
        {
            msaaPlugin = plugin;
            hookToDelegateMap = new Dictionary<IntPtr, NativeMethods.WinEventProc>();
            eventToHookMap = new Dictionary<AccessibleEvents, IntPtr>();
            eventToHandlerMap = InitializeWinEventToHandlerMap();
            lastWinHookEvents = new List<AccessibleEvents>();
            currentWinHookEvents = new List<AccessibleEvents>();
            elementNotifySinkMapping = new Dictionary<MsaaElement, ElementEventSink>(new MsaaElementComparer());
            elementBoundingRectMapping = new ConcurrentDictionary<IntPtr, Rectangle>();
        }

        public bool AddEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            MsaaElement element2;
            bool flag = false;
            if (richTextBox != null && richTextBox != element)
            {
                NotifyEventForRichTextBox(richTextBox);
                richTextBox = null;
            }
            if (slider != null && sliderSource != element)
            {
                NotifyEventForSlider();
                sliderSource = null;
                slider = null;
            }
            if ((eventType == ZappyTaskEventType.ValueChanged || eventType == ZappyTaskEventType.StateChanged) && ShouldProcessValueChangedEvent(element, out element2))
            {
                ElementEventSink sink;
                object[] args = { element2, element };
                
                if (MsaaUtility.IsElementDataGridCheckBox(element2.AccessibleWrapper) || ControlType.MenuItem.NameEquals(element2.ControlTypeName))
                {
                    eventType = ZappyTaskEventType.StateChanged;
                }
                SetEventsToHook(element2);
                if (!AddWinEventHook(element2.ProcessId))
                {
                    return flag;
                }
                lastWinHookEvents.Clear();
                foreach (AccessibleEvents events in currentWinHookEvents)
                {
                    lastWinHookEvents.Add(events);
                }
                                                                                                {
                    lastIE8AddressBarEdit = null;
                }
                if (elementNotifySinkMapping.TryGetValue(element2, out sink))
                {
                    sink.refCount++;
                }
                else
                {
                    sink = new ElementEventSink(eventSink);
                    object dictLock = this.dictLock;
                    lock (dictLock)
                    {
                        elementNotifySinkMapping.Add(element2, sink);
                    }
                    if (IsElementForDelayedRemoval(element2.ControlTypeName))
                    {
                        delayedRemovalElement = element;
                    }
                    if (ControlType.TreeItem.NameEquals(element.ControlTypeName) || ControlType.CheckBoxTreeItem.NameEquals(element.ControlTypeName))
                    {
                        sink.refCount++;
                    }
                }
                if (ControlType.ComboBox.NameEquals(element2.ControlTypeName) || ControlType.Edit.NameEquals(element2.ControlTypeName))
                {
                                        Rectangle rect = element2.AccessibleWrapper.GetBoundingRectangle();
                    if (!MsaaUtility.InvalidRectangle.Contains(rect))
                    {
                        elementBoundingRectMapping.AddOrUpdate(element2.WindowHandle, rect, (key, oldValue) => rect);
                    }
                }
                if (eventType == ZappyTaskEventType.StateChanged)
                {
                    try
                    {
                        sink.eventArg = ConvertState(element2.GetRequestedState(~AccessibleStates.None));
                    }
                    catch (ZappyTaskException)
                    {
                    }
                }
                if (element2.IsSimpleComboBox)
                {
                    sink.latestSourceElement = element2.SimpleComboBoxSourceElement;
                }
                else if (element == element2)
                {
                    sink.latestSourceElement = null;
                }
                else
                {
                    sink.latestSourceElement = element;
                }
                if (!MsaaUtility.IsWindowsVistaOrAbove() && element.ControlTypeName == ControlType.Edit && MsaaUtility.IsRichTextBoxClassName(element.ClassName))
                {
                    sink.eventArg = TrimRichTextBoxValue(element.Value);
                    richTextBox = element as MsaaElement;
                }
                if (ControlType.Slider.NameEquals(element2.ControlTypeName) && ControlType.Indicator.NameEquals(element.ControlTypeName))
                {
                    slider = element2;
                    sliderSource = element as MsaaElement;
                }
                return true;
            }
            if (eventType == ZappyTaskEventType.ValueChanged && ControlType.Cell.NameEquals(element.ControlTypeName))
            {
                DataGridUtility.cachedDatagridElement = element as MsaaElement;
            }
            return flag;
        }

        private bool AddWinEventForEvent(int processId, AccessibleEvents eventMin, AccessibleEvents eventMax, NativeMethods.WinEventProc eventHandler)
        {
            if (eventMin == AccessibleEvents.Selection)
            {
                eventMax = AccessibleEvents.SelectionWithin;
            }
            IntPtr key = NativeMethods.SetWinEventHook(eventMin, eventMax, IntPtr.Zero, eventHandler, (uint)processId, 0, NativeMethods.SetWinEventHookParameter.WINEVENT_SKIPOWNPROCESS);
            if (key != IntPtr.Zero)
            {
                hookToDelegateMap.Add(key, eventHandler);
                eventToHookMap.Add(eventMin, key);
                return true;
            }
            object[] args = { processId };
            CrapyLogger.log.ErrorFormat("AddWinEventForEvent failed for process {0}.", args);
            return false;
        }

        private bool AddWinEventHook(int processId)
        {
            bool flag = true;
            if (processId == Process.GetCurrentProcess().Id)
            {
                CrapyLogger.log.ErrorFormat("AddWinEventHook() called with self process.");
                return false;
            }
            List<AccessibleEvents> list = new List<AccessibleEvents>();
            List<AccessibleEvents> list2 = new List<AccessibleEvents>();
            if (eventProcessId == processId)
            {
                object[] args = { processId };
                
                foreach (AccessibleEvents events in lastWinHookEvents)
                {
                    if (!currentWinHookEvents.Contains(events))
                    {
                        list.Add(events);
                    }
                }
                foreach (AccessibleEvents events2 in currentWinHookEvents)
                {
                    if (!lastWinHookEvents.Contains(events2))
                    {
                        list2.Add(events2);
                    }
                }
            }
            else
            {
                object[] objArray2 = { processId };
                
                foreach (AccessibleEvents events3 in lastWinHookEvents)
                {
                    list.Add(events3);
                }
                foreach (AccessibleEvents events4 in currentWinHookEvents)
                {
                    list2.Add(events4);
                }
                eventProcessId = processId;
            }
            foreach (AccessibleEvents events5 in list)
            {
                IntPtr ptr;
                if (eventToHookMap.TryGetValue(events5, out ptr) && NativeMethods.UnhookWinEvent(ptr))
                {
                    hookToDelegateMap.Remove(ptr);
                    eventToHookMap.Remove(events5);
                }
                else
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                foreach (AccessibleEvents events6 in list2)
                {
                    flag = AddWinEventForEvent(processId, events6, events6, eventToHandlerMap[events6].Invoke);
                    if (!flag)
                    {
                        break;
                    }
                }
            }
            list.Clear();
            list2.Clear();
            return flag;
        }

        private bool CheckControlType(MsaaElement element, ControlType elementControlType, ControlType parentControlType)
        {
            if (element != null && elementControlType.NameEquals(element.ControlTypeName))
            {
                MsaaElement parent = msaaPlugin.GetParent(element) as MsaaElement;
                if (parent != null && parentControlType.NameEquals(parent.ControlTypeName))
                {
                    return true;
                }
            }
            return false;
        }

        private static ControlStates ConvertState(AccessibleStates accState)
        {
            ControlStates none = ControlStates.None;
            if ((accState & AccessibleStates.Expanded) != AccessibleStates.None)
            {
                none |= ControlStates.Expanded;
            }
            else if ((accState & AccessibleStates.Collapsed) != AccessibleStates.None)
            {
                none |= ControlStates.Collapsed;
            }
            if ((accState & AccessibleStates.Checked) != AccessibleStates.None)
            {
                return none | ControlStates.Checked;
            }
            if ((accState & AccessibleStates.Indeterminate) != AccessibleStates.None)
            {
                return none | ControlStates.Indeterminate;
            }
            return none | ControlStates.None | ControlStates.Normal;
        }

        private static ControlStates DiffControlStates(ControlStates newState, ControlStates oldState)
        {
            ControlStates states = ~ControlStates.None ^ oldState;
            return newState & states;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                
                foreach (IntPtr ptr in hookToDelegateMap.Keys)
                {
                    NativeMethods.UnhookWinEvent(ptr);
                }
                elementNotifySinkMapping.Clear();
                hookToDelegateMap.Clear();
                eventToHookMap.Clear();
                eventToHandlerMap.Clear();
                lastWinHookEvents.Clear();
                currentWinHookEvents.Clear();
                disposed = true;
                GC.SuppressFinalize(this);
                
            }
        }


        private AccWrapper GetElementFromEvent(IntPtr windowHandle, int objectId, int childId)
        {
            AccWrapper accElement = AccWrapper.GetAccWrapperFromEvent(windowHandle, objectId, childId);
            if (accElement != null && accElement.RoleInt == AccessibleRole.None)
            {
                
                TryGetMsaaElementFromWindowHandle(windowHandle, out accElement);
            }
            object[] args = { windowHandle, objectId, childId };
            if (accElement == null)
                CrapyLogger.log.ErrorFormat("GetElementFromEvent: Failed to get element - window handle {0}, object id {1}, child id {2}.", args);
            return accElement;
        }

        private ElementEventSink GetEventSinkForElement(MsaaElement element, out MsaaElement elementForEvent)
        {
            ElementEventSink sink = null;
            elementForEvent = null;
            if (ShouldProcessValueChangedEvent(element, out elementForEvent))
            {
                object[] objArray1 = { elementForEvent, elementForEvent.WindowHandle, element, element.WindowHandle };
                
                elementNotifySinkMapping.TryGetValue(elementForEvent, out sink);
            }
            object[] args = { element };
            if (sink == null)
                CrapyLogger.log.InfoFormat("GetEventSinkForElement: No handler added for element [{0}]", args);

            return sink;
        }


        private void HandleRadioButtonInGroup(MsaaElement elementForEvent, ElementEventSink elementEventSink, IntPtr windowHandle)
        {
            MsaaElement source = elementForEvent;
            elementForEvent = msaaPlugin.GetFocusedElement(windowHandle) as MsaaElement;
            if (elementForEvent != null && ControlType.RadioButton.NameEquals(elementForEvent.ControlTypeName) && TaskActivityElement.IsState(elementForEvent, AccessibleStates.Checked))
            {
                elementEventSink.eventSink.Notify(source, elementForEvent, ZappyTaskEventType.StateChanged, ControlStates.Checked);
            }
        }

        private static bool HasDataGridElementLocationChanged(MsaaElement elementForEvent)
        {
            Rectangle rectangle;
            if ((ControlType.ComboBox.NameEquals(elementForEvent.ControlTypeName) || ControlType.Edit.NameEquals(elementForEvent.ControlTypeName)) && elementBoundingRectMapping.TryGetValue(elementForEvent.WindowHandle, out rectangle))
            {
                Rectangle boundingRectangle = elementForEvent.AccessibleWrapper.GetBoundingRectangle();
                if (!MsaaUtility.InvalidRectangle.Contains(boundingRectangle) && (boundingRectangle.Left != rectangle.Left || boundingRectangle.Top != rectangle.Top))
                {
                    elementBoundingRectMapping[elementForEvent.WindowHandle] = boundingRectangle;
                    return true;
                }
            }
            return false;
        }

        private Dictionary<AccessibleEvents, NativeMethods.WinEventProc> InitializeWinEventToHandlerMap() =>
            new Dictionary<AccessibleEvents, NativeMethods.WinEventProc> {
                {
                    AccessibleEvents.ValueChange,
                    ValueChangedCallback
                },
                {
                    AccessibleEvents.StateChange,
                    StateChangedCallback
                },
                {
                    AccessibleEvents.SystemCaptureStart,
                    OtherEventCallback
                },
                {
                    AccessibleEvents.SystemCaptureEnd,
                    OtherEventCallback
                },
                {
                    AccessibleEvents.Selection,
                    OtherEventCallback
                }
            };

        private bool IsElementDateTimePicker(ITaskActivityElement element, MsaaElement parent, ref MsaaElement elementForValueChangedEvent)
        {
            if (ControlType.DateTimePicker.NameEquals(element.ControlTypeName))
            {
                elementForValueChangedEvent = element as MsaaElement;
                return true;
            }
            if (ControlType.Button.NameEquals(element.ControlTypeName) && parent != null && ControlType.Spinner.NameEquals(parent.ControlTypeName))
            {
                parent = msaaPlugin.GetParent(parent) as MsaaElement;
                if (CheckControlType(parent, ControlType.Window, ControlType.DateTimePicker))
                {
                    elementForValueChangedEvent = msaaPlugin.GetParent(parent) as MsaaElement;
                    return true;
                }
            }
            return false;
        }

        private static bool IsElementForDelayedRemoval(string controlTypeName)
        {
            if (!ControlType.ComboBox.NameEquals(controlTypeName) && !ControlType.List.NameEquals(controlTypeName) && !ControlType.DateTimePicker.NameEquals(controlTypeName))
            {
                return ControlType.Calendar.NameEquals(controlTypeName);
            }
            return true;
        }

        private static bool IsEventNotSupported(AccessibleEvents accEvent) =>
            accEvent != AccessibleEvents.SystemCaptureStart && accEvent != AccessibleEvents.SystemCaptureEnd && accEvent != AccessibleEvents.Selection && accEvent != AccessibleEvents.SelectionAdd && accEvent != AccessibleEvents.SelectionRemove && accEvent != AccessibleEvents.SelectionWithin;

        private bool IsLastSelectionOnCell(MsaaElement element) =>
            (ControlType.ComboBox.NameEquals(element.ControlTypeName) || ControlType.Edit.NameEquals(element.ControlTypeName)) && msaaPlugin.WinEvent != null && msaaPlugin.WinEvent.SelectedDataGridCell != null && msaaPlugin.WinEvent.SelectedDataGridCell.RoleInt == AccessibleRole.Cell;

        private bool IsStateChangeSupportedControl(MsaaElement element)
        {
            if (!ControlType.CheckBox.NameEquals(element.ControlTypeName) && !ControlType.CheckBoxTreeItem.NameEquals(element.ControlTypeName) && !ControlType.TreeItem.NameEquals(element.ControlTypeName) && !ControlType.RadioButton.NameEquals(element.ControlTypeName) && !MsaaUtility.IsElementDataGridCheckBox(element))
            {
                return ControlType.MenuItem.NameEquals(element.ControlTypeName);
            }
            return true;
        }

        private static bool IsValidElementForEvent(ITaskActivityElement element)
        {
            if ((element == null || element.ControlTypeName != ControlType.CheckBox && element.ControlTypeName != ControlType.List && element.ControlTypeName != ControlType.Slider && element.ControlTypeName != ControlType.TreeItem && element.ControlTypeName != ControlType.CheckBoxTreeItem && element.ControlTypeName != ControlType.RadioButton && (element.ControlTypeName != ControlType.ComboBox && element.ControlTypeName != ControlType.Edit || TaskActivityElement.IsState(element, AccessibleStates.ReadOnly))) && !MsaaUtility.IsElementDataGridCheckBox(element as MsaaElement) && element.ControlTypeName != ControlType.MenuItem)
            {
                return false;
            }
            return true;
        }

        private bool IsValueChangeSupportedControl(MsaaElement element)
        {
            if (!ControlType.ComboBox.NameEquals(element.ControlTypeName) && !ControlType.DateTimePicker.NameEquals(element.ControlTypeName) && !ControlType.Edit.NameEquals(element.ControlTypeName) && !ControlType.List.NameEquals(element.ControlTypeName) && !ControlType.Calendar.NameEquals(element.ControlTypeName) && !ControlType.Slider.NameEquals(element.ControlTypeName) && !ControlType.Spinner.NameEquals(element.ControlTypeName))
            {
                return ControlType.Text.NameEquals(element.ControlTypeName);
            }
            return true;
        }

        private void NotifyEventForRichTextBox(MsaaElement element)
        {
            ElementEventSink sink;
            if (element != null && elementNotifySinkMapping.TryGetValue(element, out sink))
            {
                object objA = TrimRichTextBoxValue(element.Value);
                if (!Equals(objA, sink.eventArg))
                {
                    sink.eventArg = objA;
                    sink.eventSink.Notify(sink.latestSourceElement, element, ZappyTaskEventType.ValueChanged, objA);
                }
            }
        }

        private void NotifyEventForSlider()
        {
            ElementEventSink sink;
            if (slider != null && sliderSource != null && elementNotifySinkMapping.TryGetValue(slider, out sink))
            {
                double num;
                object eventArgs = slider.Value;
                if (double.TryParse(eventArgs.ToString(), out num))
                {
                    sink.latestSourceElement = sliderSource;
                    sink.eventArg = slider.Value;
                    sink.eventSink.Notify(sliderSource, slider, ZappyTaskEventType.ValueChanged, eventArgs);
                }
            }
        }


        private void OtherChangedHelper(object state)
        {
            object syncLock = this.syncLock;
            lock (syncLock)
            {
                WinEventParameters parameters = state as WinEventParameters;
                AccessibleEvents accEvent = parameters.AccEvent;
                IntPtr windowHandle = parameters.WindowHandle;
                object[] args = { windowHandle, accEvent };
                
                AccWrapper wrapper = GetElementFromEvent(windowHandle, parameters.ObjectId, parameters.ChildId);
                if (wrapper != null)
                {
                    string className = NativeMethods.GetClassName(windowHandle);
                    if (wrapper.RoleInt == AccessibleRole.Window)
                    {
                        wrapper = wrapper.Navigate(AccessibleNavigation.FirstChild);
                    }
                    if (wrapper != null)
                    {
                        MsaaElement element2;
                        if (!MonthCalendarUtilities.IsMonthCalendarClassName(className) && wrapper.RoleInt != AccessibleRole.ListItem)
                        {
                            AccWrapper wrapper2 = null;
                            if (wrapper.RoleInt == AccessibleRole.List)
                            {
                                wrapper2 = wrapper.Navigate(AccessibleNavigation.FirstChild);
                            }
                            if (wrapper2 == null || wrapper2.RoleInt != AccessibleRole.ListItem)
                            {
                                goto Label_01A9;
                            }
                        }
                        MsaaElement element = new MsaaElement(wrapper.WindowHandle, wrapper.AccessibleObject, wrapper.ChildId);
                        ElementEventSink eventSinkForElement = GetEventSinkForElement(element, out element2);
                        if (eventSinkForElement != null)
                        {
                            try
                            {
                                object eventArgs = null;
                                if (ControlType.Calendar.NameEquals(element2.ControlTypeName))
                                {
                                    eventArgs = MonthCalendarUtilities.GetValue(accEvent, element2);
                                    if (eventArgs != null)
                                    {
                                        eventSinkForElement.eventSink.Notify(eventSinkForElement.latestSourceElement, element2, ZappyTaskEventType.ValueChanged, eventArgs);
                                    }
                                }
                                else if (ControlType.List.NameEquals(element2.ControlTypeName) && accEvent != AccessibleEvents.SystemCaptureStart && ListBoxUtilities.TryGetValue(accEvent, element2, ref eventSinkForElement.isLastEventGood, eventSinkForElement.eventArg, out eventArgs))
                                {
                                    eventSinkForElement.eventArg = eventArgs;
                                    eventSinkForElement.eventSink.Notify(eventSinkForElement.latestSourceElement, element2, ZappyTaskEventType.ValueChanged, eventArgs);
                                }
                            }
                            catch (ZappyTaskException exception)
                            {
                                CrapyLogger.log.Error(exception);
                            }
                        }
                    }
                }
            Label_01A9:;
            }
        }

        private void OtherEventCallback(IntPtr winEventHookHandle, AccessibleEvents accEvent, IntPtr windowHandle, int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds)
        {
            if (elementNotifySinkMapping.Count != 0 && !IsEventNotSupported(accEvent))
            {
                ThreadPool.QueueUserWorkItem(OtherChangedHelper, new WinEventParameters(accEvent, windowHandle, objectId, childId));
            }
        }

        public bool RemoveEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType)
        {
            MsaaElement element2;
            ElementEventSink sink;
            bool flag = false;
            if (eventType != ZappyTaskEventType.ValueChanged && eventType != ZappyTaskEventType.StateChanged)
            {
                return flag;
            }
            if (!ShouldProcessValueChangedEvent(element, out element2))
            {
                return flag;
            }
            object[] args = { element2, element };
            
            if (!elementNotifySinkMapping.TryGetValue(element2, out sink))
            {
                
                return flag;
            }
            int num = sink.refCount - 1;
            sink.refCount = num;
            if (num == 0)
            {
                if (delayedRemovalElement == element)
                {
                    sink.refCount = 1;
                }
                else
                {
                    if (richTextBox != null && element.ControlTypeName == ControlType.Edit && MsaaUtility.IsRichTextBoxClassName(richTextBox.ClassName))
                    {
                        NotifyEventForRichTextBox(element as MsaaElement);
                    }
                    else if (slider != null && ControlType.Slider.NameEquals(element2.ControlTypeName) && ControlType.Indicator.NameEquals(element.ControlTypeName))
                    {
                        NotifyEventForSlider();
                    }
                    
                    object dictLock = this.dictLock;
                    lock (dictLock)
                    {
                        elementNotifySinkMapping.Remove(element2);
                    }
                    if (elementBoundingRectMapping.ContainsKey(element2.WindowHandle))
                    {
                                            }
                }
            }
            return true;
        }

        private void SetEventsToHook(MsaaElement element)
        {
            currentWinHookEvents = new List<AccessibleEvents>();
            if (IsValueChangeSupportedControl(element))
            {
                currentWinHookEvents.Add(AccessibleEvents.ValueChange);
            }
            if (IsStateChangeSupportedControl(element))
            {
                currentWinHookEvents.Add(AccessibleEvents.StateChange);
            }
            if (ControlType.Calendar.NameEquals(element.ControlTypeName))
            {
                currentWinHookEvents.Add(AccessibleEvents.SystemCaptureStart);
            }
            if (ControlType.Calendar.NameEquals(element.ControlTypeName) || ControlType.List.NameEquals(element.ControlTypeName))
            {
                currentWinHookEvents.Add(AccessibleEvents.SystemCaptureEnd);
                currentWinHookEvents.Add(AccessibleEvents.Selection);
            }
        }


        private bool ShouldProcessValueChangedEvent(ITaskActivityElement element, out MsaaElement elementForValueChangedEvent)
        {
            elementForValueChangedEvent = null;
            string controlTypeName = element.ControlTypeName;
            string str2 = string.Empty;
            MsaaElement parent = null;
            if (MsaaUtility.IsDesktopListOrListItem(element as MsaaElement))
            {
                return false;
            }
            try
            {
                parent = msaaPlugin.GetParent(element) as MsaaElement;
                if (parent != null)
                {
                    if (parent.ControlTypeName == ControlType.Window)
                    {
                        parent = msaaPlugin.GetParent(parent) as MsaaElement;
                        if (parent != null)
                        {
                            str2 = parent.ControlTypeName;
                        }
                    }
                    else
                    {
                        str2 = parent.ControlTypeName;
                    }
                }
                if (!IsElementDateTimePicker(element, parent, ref elementForValueChangedEvent))
                {
                    if (ControlType.Calendar.NameEquals(element.ControlTypeName) && ZappyTaskUtilities.IsWinformsClassName(element.ClassName))
                    {
                        elementForValueChangedEvent = element as MsaaElement;
                    }
                    else
                    {
                        switch (str2)
                        {
                                                                                                                                        }
                        if (element.ControlTypeName == ControlType.ListItem)
                        {
                            ITaskActivityElement grandParent = msaaPlugin.GetParent(parent);
                                                                                                                                                                        {
                                if (grandParent != null && grandParent.ControlTypeName == ControlType.Window)
                                {
                                    grandParent = msaaPlugin.GetParent(grandParent);
                                }
                                if (grandParent != null && grandParent.ControlTypeName == ControlType.ComboBox)
                                {
                                    elementForValueChangedEvent = grandParent as MsaaElement;
                                }
                                else if (str2 == ControlType.List && grandParent != null && !string.Equals(grandParent.ClassName, "Auto-Suggest Dropdown", StringComparison.OrdinalIgnoreCase))
                                {
                                    elementForValueChangedEvent = parent;
                                }
                            }
                        }
                        else if (CheckControlType(parent, ControlType.Spinner, ControlType.Window))
                        {
                            MsaaElement element4 = msaaPlugin.GetParent(parent) as MsaaElement;
                            if (CheckControlType(element4, ControlType.Window, ControlType.ComboBox))
                            {
                                elementForValueChangedEvent = msaaPlugin.GetParent(element4) as MsaaElement;
                            }
                        }
                                                                                                                        else if (IsValidElementForEvent(element))
                        {
                            elementForValueChangedEvent = element as MsaaElement;
                        }
                    }
                }
            }
            catch (ZappyTaskException exception)
            {
                CrapyLogger.log.Error(exception);
            }
            return elementForValueChangedEvent != null;
        }

        private void StateChangedCallback(IntPtr winEventHookHandle, AccessibleEvents accEvent, IntPtr windowHandle, int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds)
        {
            if (elementNotifySinkMapping.Count != 0 && accEvent == AccessibleEvents.StateChange)
            {
                ThreadPool.QueueUserWorkItem(StateChangedHelper, new WinEventParameters(accEvent, windowHandle, objectId, childId));
            }
        }


        private void StateChangedHelper(object state)
        {
            object syncLock = this.syncLock;
            lock (syncLock)
            {
                WinEventParameters parameters = state as WinEventParameters;
                IntPtr windowHandle = parameters.WindowHandle;
                object[] args = { windowHandle };
                
                AccWrapper wrapper = GetElementFromEvent(windowHandle, parameters.ObjectId, parameters.ChildId);
                if (wrapper != null && (wrapper.RoleInt == AccessibleRole.CheckButton || wrapper.RoleInt == AccessibleRole.RadioButton || wrapper.RoleInt == AccessibleRole.OutlineItem || wrapper.RoleInt == AccessibleRole.MenuItem || MsaaUtility.IsElementDataGridCheckBox(wrapper)))
                {
                    MsaaElement element2;
                    MsaaElement element = new MsaaElement(wrapper.WindowHandle, wrapper.AccessibleObject, wrapper.ChildId);
                    ElementEventSink eventSinkForElement = GetEventSinkForElement(element, out element2);
                    if (eventSinkForElement != null)
                    {
                        try
                        {
                            ControlStates newState = ConvertState(element2.GetRequestedState(~AccessibleStates.None));
                            ControlStates eventArg = (ControlStates)eventSinkForElement.eventArg;
                            if (eventArg != newState)
                            {
                                eventSinkForElement.eventArg = newState;
                                if (ControlType.RadioButton.NameEquals(element2.ControlTypeName) && newState == (ControlStates.None | ControlStates.Normal))
                                {
                                    HandleRadioButtonInGroup(element2, eventSinkForElement, windowHandle);
                                }
                                else
                                {
                                    eventSinkForElement.eventSink.Notify(eventSinkForElement.latestSourceElement, element2, ZappyTaskEventType.StateChanged, DiffControlStates(newState, eventArg));
                                }
                            }
                        }
                        catch (ZappyTaskException exception)
                        {
                            CrapyLogger.log.Error(exception);
                        }
                    }
                }
            }
        }

        private static string TrimRichTextBoxValue(string value)
        {
            if (!string.IsNullOrEmpty(value) && value.EndsWith("\r", StringComparison.Ordinal))
            {
                value = value.Remove(value.Length - 1);
            }
            return value;
        }

        private bool TryGetMsaaElementFromWindowHandle(IntPtr windowHandle, out AccWrapper accElement)
        {
            accElement = null;
            foreach (MsaaElement element in elementNotifySinkMapping.Keys)
            {
                if (element.WindowHandle == windowHandle)
                {
                    accElement = element.AccessibleWrapper;
                    return true;
                }
            }
            return false;
        }

        private void ValueChangedCallback(IntPtr winEventHookHandle, AccessibleEvents accEvent, IntPtr windowHandle, int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds)
        {
            if (elementNotifySinkMapping.Count != 0 && accEvent == AccessibleEvents.ValueChange)
            {
                ThreadPool.QueueUserWorkItem(ValueChangedHelper, new WinEventParameters(accEvent, windowHandle, objectId, childId));
            }
        }


        private void ValueChangedHelper(object state)
        {
            object syncLock = this.syncLock;
            lock (syncLock)
            {
                WinEventParameters parameters = state as WinEventParameters;
                IntPtr windowHandle = parameters.WindowHandle;
                object[] args = { windowHandle };
                
                AccWrapper wrapper = GetElementFromEvent(windowHandle, parameters.ObjectId, parameters.ChildId);
                if (wrapper != null && (wrapper.RoleInt == AccessibleRole.ComboBox || wrapper.RoleInt == AccessibleRole.Text || wrapper.RoleInt == AccessibleRole.DropList || wrapper.RoleInt == AccessibleRole.Slider))
                {
                    MsaaElement element2;
                    MsaaElement element = new MsaaElement(wrapper.WindowHandle, wrapper.AccessibleObject, wrapper.ChildId);
                    ElementEventSink eventSinkForElement = GetEventSinkForElement(element, out element2);
                    if (eventSinkForElement != null && element2 != null)
                    {
                        try
                        {
                            double num;
                            object eventArgs = null;
                            if (ControlType.DateTimePicker.NameEquals(element2.ControlTypeName))
                            {
                                eventArgs = DateTimePickerUtilities.GetValue(element2);
                            }
                            else
                            {
                                if (element.RoleInt == AccessibleRole.Text)
                                {
                                    eventArgs = element.Value;
                                }
                                if (eventArgs == null)
                                {
                                    eventArgs = element2.Value;
                                }
                                if (ControlType.Edit.NameEquals(element2.ControlTypeName))
                                {
                                    eventArgs = TrimRichTextBoxValue(eventArgs as string);
                                }
                            }
                            if (eventArgs == null || wrapper.RoleInt == AccessibleRole.Slider && !double.TryParse(eventArgs.ToString(), out num))
                            {
                                goto Label_02A2;
                            }
                            MsaaElement element3 = element2;
                            if (msaaPlugin.WinEvent != null)
                            {
                                if (string.Equals(element2.Name, "Editing Control", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!HasDataGridElementLocationChanged(element2))
                                    {
                                        DataGridUtility.GetDataGridCellFromElement(msaaPlugin, ref element2, true);
                                        if (element2 != null)
                                        {
                                            goto Label_0230;
                                        }
                                        
                                    }
                                    goto Label_02A2;
                                }
                                if (IsLastSelectionOnCell(element2))
                                {
                                    if (HasDataGridElementLocationChanged(element2))
                                    {
                                        goto Label_02A2;
                                    }
                                    DataGridUtility.GetDataGridCellFromElement(msaaPlugin, ref element2, false);
                                    if (element2 == null)
                                    {
                                        
                                        goto Label_02A2;
                                    }
                                    if (msaaPlugin.WinEvent != null && element2.AccessibleObject.Equals(msaaPlugin.WinEvent.SelectedDataGridCell.AccessibleObject))
                                    {
                                        msaaPlugin.WinEvent.ValueChangeDataGridCell = null;
                                        element2 = new MsaaElement(msaaPlugin.WinEvent.SelectedDataGridCell);
                                    }
                                }
                            }
                        Label_0230:
                            if (ControlType.Cell.NameEquals(element2.ControlTypeName))
                            {
                                DataGridUtility.SetSwitchingElement(element2);
                            }
                            if (!element2.Equals(element3) && eventSinkForElement.latestSourceElement == null)
                            {
                                eventSinkForElement.eventSink.Notify(element3, element2, ZappyTaskEventType.ValueChanged, eventArgs);
                            }
                            else
                            {
                                eventSinkForElement.eventSink.Notify(eventSinkForElement.latestSourceElement, element2, ZappyTaskEventType.ValueChanged, eventArgs);
                            }
                        }
                        catch (ZappyTaskException exception)
                        {
                            CrapyLogger.log.Error(exception);
                        }
                    }
                }
            Label_02A2:;
            }
        }

        internal static ConcurrentDictionary<IntPtr, Rectangle> ElementBoundingRectMapping =>
            elementBoundingRectMapping;

        private class ElementEventSink
        {
            internal object eventArg;
            internal IZappyTaskEventNotify eventSink;
            internal bool isLastEventGood;
            internal ITaskActivityElement latestSourceElement;
            internal int refCount;

            internal ElementEventSink(IZappyTaskEventNotify eventSink)
            {
                this.eventSink = eventSink;
                refCount = 1;
            }
        }

        private class MsaaElementComparer : IEqualityComparer<MsaaElement>
        {
            public bool Equals(MsaaElement objLeft, MsaaElement objRight)
            {
                if (objLeft == null || objRight == null)
                {
                    return objLeft == objRight;
                }
                if (!objLeft.EqualsIgnoreContainer(objRight))
                {
                    return false;
                }
                if (objLeft.SwitchingElement == null && objRight.SwitchingElement != null && objRight.SwitchingElement is MsaaElement)
                {
                    objLeft.SwitchingElement = objRight.SwitchingElement;
                }
                if (objRight.SwitchingElement == null && objLeft.SwitchingElement != null && objLeft.SwitchingElement is MsaaElement)
                {
                    objRight.SwitchingElement = objLeft.SwitchingElement;
                }
                return objLeft.Equals(objRight);
            }

            public int GetHashCode(MsaaElement obj)
            {
                if (obj != null)
                {
                    return obj.GetHashCode();
                }
                return -1;
            }
        }
    }
}