using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.Trapy;

namespace Zappy.Decode.Mssa
{
    internal class WinEvent : IDisposable
    {
        private IZappyTaskEventNotify actionNotify;
        private AccWrapper currentParentChangeParentAccWrapper;
        private bool hasMenuStarted;
        private volatile bool keepProcessing;
        private volatile AccessibleEvents lastAccEvent;
        private volatile uint lastEventTimeStamp;
        private volatile AccWrapper lastFocusedAccWrapper;
        private volatile MsaaElement lastFocusedElement;
        private AccWrapper lastParentChangeParentAccWrapper;
        private static bool listenToSelfProcess;
        private readonly object lockObject = new object();
        private AccWrapper selectedDataGridCell;
        private MsaaElement valueChangeDataGridCell;
        private Thread winEventHandlerThread;

        public WinEvent(bool recordingSession)
        {
                                                                                    
            TrapyService.WinInfoEvent += TrapyService_WinInfoEvent;
        }

        private void TrapyService_WinInfoEvent(global::ZappyMessages.RecordPlayback.TrapyWinEvent obj)
        {
                                                                        if (obj.Event >= AccessibleEvents.SystemMenuStart && obj.Event <= AccessibleEvents.SystemMenuPopupEnd)
            {
                ObjectFocusCallback(IntPtr.Zero, obj.Event, obj.Hwnd, obj.idObject, obj.idChild, obj.EventThreadID, (uint)obj.EventTime);
            }
            else if (obj.Event == AccessibleEvents.Focus)
            {
                ObjectFocusCallback(IntPtr.Zero, obj.Event, obj.Hwnd, obj.idObject, obj.idChild, obj.EventThreadID, (uint)obj.EventTime);
                
            }
            else if (obj.Event == AccessibleEvents.ParentChange)
            {
                ObjectMiscCallback(IntPtr.Zero, obj.Event, obj.Hwnd, obj.idObject, obj.idChild, obj.EventThreadID, (uint)obj.EventTime);

            }
            else if (obj.Event == AccessibleEvents.Selection)
            {
                ObjectMiscCallback(IntPtr.Zero, obj.Event, obj.Hwnd, obj.idObject, obj.idChild, obj.EventThreadID, (uint)obj.EventTime);

            }
        }

        private void CacheLastRowCellQueryId(AccWrapper accWrapper)
        {
            if (ActionNotify != null)
            {
                AccWrapper wrapper = accWrapper.Parent.Navigate(AccessibleNavigation.Next);
                if (wrapper != null && string.Equals(wrapper.Value, "(Create New)", StringComparison.OrdinalIgnoreCase))
                {
                    AccWrapper accessibleWrapper = wrapper.Navigate(AccessibleNavigation.FirstChild);
                    if (accessibleWrapper != null)
                    {
                        MsaaElement target = new MsaaElement(accessibleWrapper);
                        ActionNotify.Notify(null, target, ZappyTaskEventType.OnFocus, null);
                    }
                }
            }
        }

        public void Dispose()
        {
            TrapyService.WinInfoEvent -= TrapyService_WinInfoEvent;
            ActionNotify = null;
            GC.SuppressFinalize(this);

            if (winEventHandlerThread != null)
            {
                
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    keepProcessing = false;
                }
                ZappyTaskUtilities.SafeThreadJoin(winEventHandlerThread);
                winEventHandlerThread = null;
                ActionNotify = null;
                GC.SuppressFinalize(this);
                
            }
        }

        internal MsaaElement GetLastFocusedElement()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                if (lastFocusedElement == null)
                {
                    if (lastFocusedAccWrapper != null && NativeMethods.IsWindow(lastFocusedAccWrapper.WindowHandle))
                    {
                        lastFocusedElement = new MsaaElement(lastFocusedAccWrapper);
                    }
                }
                else if (!NativeMethods.IsWindow(lastFocusedElement.WindowHandle))
                {
                    lastFocusedElement = null;
                }
                if (lastFocusedElement == null)
                {
                    lastFocusedAccWrapper = null;
                }
                return lastFocusedElement;
            }
        }

        private static bool HasFocusAndNotHover(AccWrapper accWrapper)
        {
            if (accWrapper == null)
            {
                return false;
            }
            AccessibleStates state = accWrapper.State;
            if ((state & AccessibleStates.Focused) != AccessibleStates.Focused && !IsSelectedDataGridCell(accWrapper))
            {
                return false;
            }
            return (state & AccessibleStates.HotTracked) != AccessibleStates.HotTracked;
        }

        private bool IsDatagridElement(AccWrapper accWrapper)
        {
            if (accWrapper == null)
            {
                return false;
            }
            string name = accWrapper.Name;
            if (!string.Equals(name, "Editing Control", StringComparison.OrdinalIgnoreCase) && !string.Equals(name, "Editing Panel", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(name, "DataGridView", StringComparison.OrdinalIgnoreCase);
            }
            return true;
        }

        private static bool IsMenuItemOrPopupWithFocus(AccWrapper accWrapper)
        {
            if (accWrapper == null)
            {
                return false;
            }
            if (accWrapper.RoleInt == AccessibleRole.MenuItem)
            {
                return true;
            }
            AccessibleStates state = accWrapper.State;
            return (state & AccessibleStates.HasPopup) == AccessibleStates.HasPopup && (state & AccessibleStates.Focused) == AccessibleStates.Focused;
        }

        private static bool IsSelectedDataGridCell(AccWrapper accWrapper) =>
            accWrapper.RoleInt == AccessibleRole.Cell && (accWrapper.State & AccessibleStates.Selected) == AccessibleStates.Selected;

        internal static void ListenToSelfProcess(bool enable)
        {
            listenToSelfProcess = enable;
        }

        private void ObjectFocusCallback(IntPtr hWinEventHook, AccessibleEvents accEvent, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (ShouldProcessEvent(hwnd) && (accEvent == AccessibleEvents.Focus || accEvent == AccessibleEvents.SystemMenuStart || accEvent == AccessibleEvents.SystemMenuEnd || accEvent == AccessibleEvents.SystemMenuPopupStart || accEvent == AccessibleEvents.SystemMenuPopupEnd))
            {
                ObjectFocusHelper(accEvent, hwnd, idObject, idChild, dwmsEventTime);
            }
        }

        private void ObjectFocusHelper(AccessibleEvents accEvent, IntPtr hwnd, int idObject, int idChild, uint dwmsEventTime)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                if (keepProcessing)
                {
                    lastFocusedElement = null;
                    try
                    {
                        switch (accEvent)
                        {
                            case AccessibleEvents.SystemMenuStart:
                                hasMenuStarted = true;
                                lastFocusedAccWrapper = null;
                                UpdateFocussedObject(accEvent, hwnd, idObject, idChild, dwmsEventTime);
                                goto Label_00A4;

                            case AccessibleEvents.SystemMenuEnd:
                            case AccessibleEvents.SystemMenuPopupEnd:
                                hasMenuStarted = false;
                                lastFocusedAccWrapper = null;
                                goto Label_00A4;

                            case AccessibleEvents.SystemMenuPopupStart:
                            case AccessibleEvents.Focus:
                                UpdateFocussedObject(accEvent, hwnd, idObject, idChild, dwmsEventTime);
                                goto Label_00A4;
                        }
                    }
                    catch (ZappyTaskException exception)
                    {
                        CrapyLogger.log.Error(exception);
                        lastFocusedAccWrapper = null;
                    }
                }
            Label_00A4:;
            }
        }

        private void ObjectMiscCallback(IntPtr hWinEventHook, AccessibleEvents accEvent, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (ShouldProcessEvent(hwnd) && (accEvent == AccessibleEvents.ParentChange || accEvent == AccessibleEvents.Selection))
            {
                ThreadPool.QueueUserWorkItem(ObjectMiscHelper, new WinEventParameters(accEvent, hwnd, idObject, idChild));
            }
        }

        private void ObjectMiscHelper(object state)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                if (keepProcessing)
                {
                    try
                    {
                        WinEventParameters parameters = state as WinEventParameters;
                        AccWrapper accWrapper = AccWrapper.GetAccWrapperFromEvent(parameters.WindowHandle, parameters.ObjectId, parameters.ChildId);
                        if (accWrapper != null)
                        {
                            switch (parameters.AccEvent)
                            {
                                case AccessibleEvents.Selection:
                                    UpdateSelectionChange(accWrapper);
                                    break;

                                case AccessibleEvents.ParentChange:
                                    UpdateParentChange(accWrapper);
                                    break;
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

        private bool ShouldProcessEvent(IntPtr windowHandle) =>
            keepProcessing && NativeMethods.IsWindow(windowHandle) && ZappyTaskUtilities.GetProcessIdForWindow(windowHandle) != Process.GetCurrentProcess().Id;

        private void UpdateFocusOnDataGridCell()
        {
            if (lastFocusedAccWrapper != null && lastFocusedAccWrapper.RoleInt == AccessibleRole.Cell)
            {
                if (lastFocusedAccWrapper.Parent != null && lastFocusedAccWrapper.Parent.Parent != null && lastFocusedAccWrapper.Parent.Parent.RoleInt == AccessibleRole.Table)
                {
                    if (string.Equals(lastFocusedAccWrapper.Parent.Parent.Name, "DataGridView", StringComparison.OrdinalIgnoreCase))
                    {
                        if (ActionNotify != null && lastFocusedAccWrapper.Parent != null && string.Equals(lastFocusedAccWrapper.Parent.Value, "(Create New)", StringComparison.OrdinalIgnoreCase))
                        {
                            ActionNotify.Notify(null, new MsaaElement(lastFocusedAccWrapper), ZappyTaskEventType.OnFocus, null);
                        }
                    }
                    else
                    {
                        CacheLastRowCellQueryId(lastFocusedAccWrapper);
                    }
                }
                lastFocusedAccWrapper = null;
                lastFocusedElement = null;
            }
        }

        private void UpdateFocussedObject(AccessibleEvents accEvent, IntPtr hWnd, int idObject, int idChild, uint dwmsEventTime)
        {
            AccWrapper accWrapper = AccWrapper.GetAccWrapperFromEvent(hWnd, idObject, idChild);
            if (accWrapper != null && (dwmsEventTime != lastEventTimeStamp || accEvent != lastAccEvent || lastFocusedAccWrapper == null || hWnd == lastFocusedAccWrapper.WindowHandle || accWrapper.RoleInt == AccessibleRole.Cell))
            {
                lastFocusedAccWrapper = null;
                lastAccEvent = accEvent;
                lastEventTimeStamp = dwmsEventTime;
                if (accEvent == AccessibleEvents.Focus || accEvent == AccessibleEvents.SystemMenuStart)
                {
                    if (hasMenuStarted || HasFocusAndNotHover(accWrapper))
                    {
                        lastFocusedAccWrapper = accWrapper;
                        UpdateFocusOnDataGridCell();
                    }
                }
                else if (accEvent == AccessibleEvents.SystemMenuPopupStart)
                {
                    if (IsMenuItemOrPopupWithFocus(accWrapper))
                    {
                        accWrapper = accWrapper.Selection;
                    }
                    if (accWrapper.RoleInt == AccessibleRole.MenuItem || accWrapper.RoleInt == AccessibleRole.MenuPopup)
                    {
                        lastFocusedAccWrapper = accWrapper;
                    }
                    else
                    {
                        lastFocusedElement = null;
                    }
                }
            }
        }

        private void UpdateParentChange(AccWrapper accWrapper)
        {
            Rectangle rectangle;
            valueChangeDataGridCell = null;
            if (currentParentChangeParentAccWrapper != null && currentParentChangeParentAccWrapper.RoleInt == AccessibleRole.Table && IsDatagridElement(accWrapper.Parent) && MsaaEventManager.ElementBoundingRectMapping.TryGetValue(accWrapper.WindowHandle, out rectangle))
            {
                Rectangle boundingRectangle = accWrapper.GetBoundingRectangle();
                if (!MsaaUtility.InvalidRectangle.Contains(boundingRectangle))
                {
                    MsaaEventManager.ElementBoundingRectMapping.TryUpdate(accWrapper.WindowHandle, boundingRectangle, rectangle);
                }
            }
            lastParentChangeParentAccWrapper = currentParentChangeParentAccWrapper;
            currentParentChangeParentAccWrapper = accWrapper.Parent;
        }

        private void UpdateSelectionChange(AccWrapper accWrapper)
        {
            if (accWrapper.RoleInt == AccessibleRole.Cell)
            {
                SelectedDataGridCell = accWrapper;
            }
            else if (accWrapper.RoleInt != AccessibleRole.ListItem && accWrapper.RoleInt != AccessibleRole.List && (accWrapper.State & AccessibleStates.Invisible) != AccessibleStates.Invisible)
            {
                SelectedDataGridCell = null;
            }
        }

                                                                                                
        
                                                                                                                                                                                                                                
                                                        
                                                        
                        


        internal IZappyTaskEventNotify ActionNotify
        {
            get
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    return actionNotify;
                }
            }
            set
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    actionNotify = value;
                }
            }
        }

        internal AccWrapper LastParentChangeParentAccWrapper
        {
            get
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    return lastParentChangeParentAccWrapper;
                }
            }
            set
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    lastParentChangeParentAccWrapper = value;
                }
            }
        }

        internal AccWrapper SelectedDataGridCell
        {
            get =>
                selectedDataGridCell;
            private set
            {
                selectedDataGridCell = value;
            }
        }

        internal MsaaElement ValueChangeDataGridCell
        {
            get
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    return valueChangeDataGridCell;
                }
            }
            set
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    valueChangeDataGridCell = value;
                }
            }
        }
    }
}