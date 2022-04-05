using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Helper.Enums;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.LowLevelHookEvent;
using Zappy.Decode.LogManager;
using Zappy.Decode.Screenshot;
using Zappy.Properties;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Hooks.Mouse
{
    internal sealed class MouseEventCapture : LowLevelHookEventCapture
    {
        private bool handleMouseUpOnIgnoredProcess;
        private ProcessType lastMouseDownProcessType;
        private IntPtr lastTrackedHandle;
        private const uint MI_WP_SIGNATURE = 0xff515700;
                private const uint SIGNATURE_MASK = 0xffffff00;

        public MouseEventCapture(IEventCapture accessoryEventCapture) : base(accessoryEventCapture)
        {
            lastTrackedHandle = IntPtr.Zero;
        }

        private static void AggregateDoubleClick(MouseAction mouseAction)
        {
            if (mouseAction.ActionType == MouseActionType.ButtonDown &&
                !ReferenceEquals(lastMouseAction, null))
            {
                if (lastMouseAction.ActionType == MouseActionType.Click &&
                    lastMouseAction.MouseButton == mouseAction.MouseButton)
                    if (RecorderUtilities.ClickTimeWithinDoubleClickRange(mouseAction.MouseDownTimeStamp,
                            lastMouseAction.MouseDownTimeStamp) &&
                        RecorderUtilities.PointsWithinRange(mouseAction.AbsoluteMouseDownLocation,
                            lastMouseAction.AbsoluteMouseDownLocation,
                            SystemInformation.DoubleClickSize)
                        && mouseAction.ActivityElement == lastMouseAction.ActivityElement)
                    {
                        
                        mouseAction.ActionType = MouseActionType.DoubleClick;
                        mouseAction.StartTimestamp = lastMouseAction.StartTimestamp;
                        mouseButtonState = MouseButtonState.None;
                        mouseAction.NeedFiltering = lastMouseAction.NeedFiltering;
                    }
            }
        }

        private void CheckForOrphanDrag(MouseAction action)
        {
            bool flag = false;
            if (action.MouseButton == MouseButtons.Left || action.MouseButton == MouseButtons.Right || action.MouseButton == MouseButtons.Middle)
            {
                if (action.ActionType == MouseActionType.ButtonDown)
                {
                    if (mouseButtonState != MouseButtonState.None)
                    {
                        if (lastMouseAction == null || lastMouseAction.MouseButton == action.MouseButton)
                        {
                            object[] args = { action.MouseButton };
                            CrapyLogger.log.WarnFormat("Received two consecutive mouse downs for the button {0}", args);
                            flag = true;
                        }
                        else if (!ReferenceEquals(lastMouseAction, null) && !IsButtonDown(lastMouseAction.MouseButton))
                        {
                            object[] objArray2 = { lastMouseAction.MouseButton };
                            CrapyLogger.log.WarnFormat("Missed mouse up for the button {0}", objArray2);
                            flag = true;
                        }
                    }
                }
                else if (action.ActionType != MouseActionType.Drag && action.ActionType == MouseActionType.ButtonUp && mouseButtonState == MouseButtonState.None && !ReferenceEquals(lastMouseAction, null) && lastMouseAction.ActionType != MouseActionType.DoubleClick)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                                                object[] objArray3 = { action, lastMouseAction };
                
                ErrorAction element = new ErrorAction(Resources.FailedToRecordMouseAction)
                {
                    ContinueOnError = true
                };
                actions.Push(element);
                ClearActionTracking();
                lastMouseDownProcessType = ProcessType.None;
            }
        }

        protected override void ClearActionTracking()
        {
            lastTrackedHandle = IntPtr.Zero;
            base.ClearActionTracking();
        }

        private static bool DoubleClickCandidate(MouseAction mouseAction) =>
            mouseAction.ActionType == MouseActionType.ButtonDown && !ReferenceEquals(lastMouseAction, null) && lastMouseAction.ActionType == MouseActionType.Click && lastMouseAction.MouseButton == mouseAction.MouseButton && RecorderUtilities.ClickTimeWithinDoubleClickRange(mouseAction.MouseDownTimeStamp, lastMouseAction.MouseDownTimeStamp) && RecorderUtilities.PointsWithinRange(mouseAction.AbsoluteMouseDownLocation, lastMouseAction.AbsoluteMouseDownLocation, SystemInformation.DoubleClickSize) && lastMouseAction.ActivityElement != null;

        private ProcessType GetProcessType(MouseAction action)
        {
            int processId = 0;
            return GetProcessType(action, out processId);
        }

        protected override ProcessType GetProcessType(InputAction inputAction, out int processId)
        {
            ProcessType type;
            processId = 0;
            MouseAction action = inputAction as MouseAction;
            handleMouseUpOnIgnoredProcess = false;
            if (action.ActionType == MouseActionType.WheelRotate)
            {
                return ProcessManager.IgnoreAction(NativeMethods.GetForegroundWindow(), true, out processId);
            }
            if ((type = ProcessManager.IgnoreAction(action.Location, true, out processId)) == ProcessType.IgnoredProcess
                || type == ProcessType.TransparentProcess)
            {
                lastMouseDownProcessType = action.ActionType == MouseActionType.ButtonDown ? type : ProcessType.None;
                if (action.ActionType == MouseActionType.ButtonUp)
                {
                    if (mouseButtonState == MouseButtonState.DragAction)
                    {
                        handleMouseUpOnIgnoredProcess = true;
                        return ProcessType.None;
                    }
                    mouseButtonState = MouseButtonState.None;
                }
                return type;
            }
            if (this.lastMouseDownProcessType == ProcessType.IgnoredProcess || this.lastMouseDownProcessType == ProcessType.TransparentProcess)
            {
                if (action.ActionType == MouseActionType.ButtonUp)
                {
                    ProcessType lastMouseDownProcessType = this.lastMouseDownProcessType;
                    this.lastMouseDownProcessType = ProcessType.None;
                    return lastMouseDownProcessType;
                }
                if (action.ActionType == MouseActionType.ButtonDown)
                {
                    lastMouseDownProcessType = ProcessType.None;
                }
            }
            return ProcessType.None;
        }

                                
                                                                
        protected override bool IsMessageOfInterest(LowLevelHookMessage message, IntPtr lParam)
        {
            switch (message)
            {
                case LowLevelHookMessage.MouseMove:
                    if (mouseButtonState != MouseButtonState.ButtonDown)
                    {
                        NotifyMouseEnter(message, lParam);
                        return false;
                    }
                    return true;

                case LowLevelHookMessage.LeftButtonDown:
                case LowLevelHookMessage.LeftButtonUp:
                case LowLevelHookMessage.RightButtonDown:
                case LowLevelHookMessage.RightButtonUp:
                case LowLevelHookMessage.MiddleButtonDown:
                case LowLevelHookMessage.MiddleButtonUp:
                case LowLevelHookMessage.MouseWheel:
                case LowLevelHookMessage.XButtonDown:
                case LowLevelHookMessage.XButtonUp:
                    return true;
            }
            return false;
        }

        private bool IsMouseInputPromoted(IntPtr extraInfo)
        {
            if (extraInfo == IntPtr.Zero)
            {
                return false;
            }
            if (NativeMethods.Is64BitOperatingSystem)
            {
                return ((ulong)extraInfo.ToInt64() & 0xffffff00L) == 0xff515700L;
            }
            return (extraInfo.ToInt32() & 0xffffff00L) == 0xff515700L;
        }

        private static bool IsMouseUpAfterDoubleClickOnSameElement(MouseAction mouseAction)
        {
            if (mouseAction.ActionType != MouseActionType.ButtonUp || lastMouseAction == null || lastMouseAction.ActionType != MouseActionType.DoubleClick || mouseAction.ActivityElement != lastMouseAction.ActivityElement && lastMouseAction.ActivityElement.ControlTypeName != ControlType.TitleBar)
            {
                return false;
            }
            return true;
        }

        private static MouseAction LowLevelCodeToMouseAction(LowLevelHookMessage message,
            NativeMethods.MouseLLHookStruct mouseStruct)
        {
            MouseAction action = new MouseAction();
            int xButtonNum = 1;
            if (message == LowLevelHookMessage.XButtonDown || message == LowLevelHookMessage.XButtonUp)
            {
                xButtonNum = mouseStruct.mouseData >> 0x10;
            }
            action.MouseButton = MapMessageToMouseButton(message, xButtonNum);
            action.ActionType = MapMessageToMouseAction(message);
            action.ModifierKeys = GetModifiers();
            action.Location = new Point(mouseStruct.pt.x, mouseStruct.pt.y);
            action.AbsoluteLocation = action.Location;
            action.MouseDownTimeStamp = mouseStruct.time;
            if (action.ActionType == MouseActionType.ButtonDown)
            {
                action.AbsoluteMouseDownLocation = action.Location;
            }
            else if (!ReferenceEquals(lastMouseAction, null))
            {
                action.AbsoluteMouseDownLocation = lastMouseAction.AbsoluteMouseDownLocation;
                action.MouseDownTimeStamp = lastMouseAction.MouseDownTimeStamp;
            }
            TimeSpan span = new TimeSpan(WallClock.Now.Ticks);
            action.StartTimestamp = action.EndTimestamp = (long)span.TotalMilliseconds;
            if (message == LowLevelHookMessage.MouseWheel)
            {
                action.WheelDirection = mouseStruct.mouseData >> 0x10;
            }
            return action;
        }

                                        
        public void LowLevelHookHandlerInternal(LowLevelHookMessage message, NativeMethods.MouseLLHookStruct mouseStruct)
        {
            MouseAction action = LowLevelCodeToMouseAction(message, mouseStruct);
            if (IsMouseInputPromoted(mouseStruct.dwExtraInfo))
            {
                                
            }
            else
            {
                CheckForOrphanDrag(action);
                if (IsActionOnIgnoredOrTransparentProcess(action))
                {
                    if (action.ActionType != MouseActionType.Drag)
                    {
                        ClearActionTracking();
                    }
                }
                else
                {
                                        
                    UpdateMouseButtonState(action);
                    if (action.ModifierKeys == ModifierKeys.None)
                    {
                        lastModifierKeyPress = null;
                    }
                                                            if (action.ActionType == MouseActionType.WheelRotate)
                    {
                        
                        UpdateActionWithFocusedElement(action);
                    }
                    else
                    {
                        if (lastCaptureOfUIElementFailed && (action.ActionType == MouseActionType.ButtonUp || action.ActionType == MouseActionType.Drag))
                        {
                            lastCaptureOfUIElementFailed = false;
                            return;
                        }
                        if (ZappyTaskUtilities.IsImageActionLogEnabled && action.ActionType == MouseActionType.ButtonDown && !DoubleClickCandidate(action))
                        {
                            Screen activeScreen = Screen.FromHandle(NativeMethods.WindowFromPoint(new NativeMethods.POINT(action.Location.X, action.Location.Y)));
                            SnapshotGenerator.Instance.StartSnapshotOfScreenTask(activeScreen);
                        }
                        UpdateActionWithCachedOrCursorElement(action);
                    }
                                        UpdateAndEnqueueMouseAction(action);
                }
            }
        }


        private static MouseActionType MapMessageToMouseAction(LowLevelHookMessage message)
        {
            switch (message)
            {
                case LowLevelHookMessage.MouseMove:
                    return MouseActionType.Drag;

                case LowLevelHookMessage.LeftButtonDown:
                case LowLevelHookMessage.RightButtonDown:
                case LowLevelHookMessage.MiddleButtonDown:
                case LowLevelHookMessage.XButtonDown:
                    return MouseActionType.ButtonDown;

                case LowLevelHookMessage.LeftButtonUp:
                case LowLevelHookMessage.RightButtonUp:
                case LowLevelHookMessage.MiddleButtonUp:
                case LowLevelHookMessage.XButtonUp:
                    return MouseActionType.ButtonUp;

                case LowLevelHookMessage.MouseWheel:
                case LowLevelHookMessage.MouseHWheel:
                    return MouseActionType.WheelRotate;
            }
            throw new ArgumentOutOfRangeException("message");
        }

        private static MouseButtons MapMessageToMouseButton(LowLevelHookMessage message, int xButtonNum)
        {
            switch (message)
            {
                case LowLevelHookMessage.MouseMove:
                    if (!NativeMethods.IsKeyStateDown(Keys.LButton))
                    {
                        if (NativeMethods.IsKeyStateDown(Keys.RButton))
                        {
                            return MouseButtons.Right;
                        }
                        if (NativeMethods.IsKeyStateDown(Keys.MButton))
                        {
                            return MouseButtons.Middle;
                        }
                        return MouseButtons.None;
                    }
                    return MouseButtons.Left;

                case LowLevelHookMessage.LeftButtonDown:
                case LowLevelHookMessage.LeftButtonUp:
                    return MouseButtons.Left;

                case LowLevelHookMessage.RightButtonDown:
                case LowLevelHookMessage.RightButtonUp:
                    return MouseButtons.Right;

                case LowLevelHookMessage.MiddleButtonDown:
                case LowLevelHookMessage.MiddleButtonUp:
                    return MouseButtons.Middle;

                case LowLevelHookMessage.MouseWheel:
                case LowLevelHookMessage.MouseHWheel:
                    return MouseButtons.None;

                case LowLevelHookMessage.XButtonDown:
                case LowLevelHookMessage.XButtonUp:
                    if (xButtonNum != 1)
                    {
                        return MouseButtons.XButton2;
                    }
                    return MouseButtons.XButton1;
            }
            throw new ArgumentOutOfRangeException("message " + message.ToString());
        }

        private void NotifyMouseEnter(LowLevelHookMessage message, IntPtr lParam)
        {
            NativeMethods.MouseLLHookStruct mouseStruct =
                (NativeMethods.MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MouseLLHookStruct));
            MouseAction action = LowLevelCodeToMouseAction(message, mouseStruct);
            try
            {
                if (ProcessManager.IgnoredOrTransparentProcess(GetProcessType(action)))
                {
                    return;
                }
            }
            catch (ApplicationFromNetworkShareException exception)
            {
                HandleNetworkException(exception);
            }
                                    try
            {
                IntPtr handle = NativeMethods.WindowFromPoint(mouseStruct.pt);
                if (handle != IntPtr.Zero && handle != lastTrackedHandle)
                {
                    UITechnologyManager manager = ZappyTaskService.Instance.TechnologyManagerFromHandle(handle);
                    if (manager != null)
                    {
                        manager.ProcessMouseEnter(handle);
                    }
                    lastTrackedHandle = handle;
                }
            }
            catch (ZappyTaskException exception2)
            {
                object[] args = { exception2 };
                
            }
                                                                    }

        private static bool UpdateActionProperties(MouseAction mouseAction)
        {
            bool flag = true;
            if (mouseAction.ActionType == MouseActionType.ButtonDown)
            {
                AggregateDoubleClick(mouseAction);
                return flag;
            }
            if (IsMouseUpAfterDoubleClickOnSameElement(mouseAction))
            {
                
                return false;
            }
            if (mouseAction.ActionType == MouseActionType.ButtonUp)
            {
                
                if (!ReferenceEquals(lastMouseAction, null))
                {
                    mouseAction.StartTimestamp = lastMouseAction.StartTimestamp;
                    mouseAction.ModifierKeys = lastMouseAction.ModifierKeys;
                }
                mouseAction.ActionType = MouseActionType.Click;
                return flag;
            }
            if (mouseAction.ActionType == MouseActionType.Drag && !ReferenceEquals(lastMouseAction, null) && lastMouseAction.ActionType == MouseActionType.ButtonDown)
            {
                
                mouseAction.Location = lastMouseAction.Location;
                mouseAction.AbsoluteLocation = lastMouseAction.AbsoluteLocation;
                mouseAction.ModifierKeys = lastMouseAction.ModifierKeys;
            }
            return flag;
        }

        private void UpdateActionWithCachedOrCursorElement(MouseAction mouseAction)
        {
            if (UseCachedElement(mouseAction))
            {
                mouseAction.ActivityElement = lastMouseAction.ActivityElement;
                if (mouseAction.ActionType == MouseActionType.ButtonUp && (lastMouseAction.ActionType != MouseActionType.Drag || RecorderUtilities.PointsWithinRange(mouseAction.Location, lastMouseAction.Location, SystemInformation.DragSize)) || ControlType.Button.NameEquals(mouseAction.ActivityElement.ControlTypeName))
                {
                    mouseAction.Location = new Point(mouseAction.Location.X - lastActionBoundingRect.X, mouseAction.Location.Y - lastActionBoundingRect.Y);
                }
                else if (mouseAction.ActionType != MouseActionType.Drag || mouseAction.MouseButton != MouseButtons.Right || !ControlType.Indicator.NameEquals(mouseAction.ActivityElement.ControlTypeName))
                {
                    NormalizeLocation(mouseAction);
                }
                
            }
            else
            {
                
                UpdateActionWithCursorElement(mouseAction, mouseAction.AbsoluteLocation);
            }
        }

        private void UpdateAndEnqueueMouseAction(MouseAction mouseAction)
        {
                                    bool flag = UpdateActionProperties(mouseAction);
                        lastMouseAction = mouseAction;
            if (flag)
            {
                ZappyTaskImageInfo info;
                Point updatedInteractionPointforActiveScreen = new Point();
                if (ZappyTaskUtilities.IsImageActionLogEnabled)
                {
                    updatedInteractionPointforActiveScreen = new Point(mouseAction.AbsoluteLocation.X, mouseAction.AbsoluteLocation.Y);
                    updatedInteractionPointforActiveScreen = RecorderUtilities.GetUpdatedInteractionPointforActiveScreen(mouseAction.ActivityElement, updatedInteractionPointforActiveScreen);
                }
                                accessoryEventCapture.SetTrackingElement(mouseAction.ActivityElement, updatedInteractionPointforActiveScreen, mouseAction.ActionType == MouseActionType.ButtonDown);
                                if (mouseAction.ActivityElement != null && (info = SnapshotGenerator.Instance.LastSnapshot) != null && info.ImagePath != null)
                {
                    mouseAction.ImageEntry = new ZappyTaskImageEntry(info, updatedInteractionPointforActiveScreen);
                }
                                                                                
                                actions.Push(mouseAction);
                            }
            else
            {
                object[] objArray2 = { mouseAction, mouseAction.ActivityElement };
                
            }
                    }

        private static void UpdateMouseButtonState(MouseAction mouseAction)
        {
            if (mouseAction.MouseButton == MouseButtons.Left || mouseAction.MouseButton == MouseButtons.Right || mouseAction.MouseButton == MouseButtons.Middle)
            {
                if (mouseAction.ActionType == MouseActionType.ButtonDown)
                {
                    mouseButtonState = MouseButtonState.ButtonDown;
                }
                else if (mouseAction.ActionType == MouseActionType.ButtonUp)
                {
                    bool flag = false;
                    if (mouseAction.MouseButton != MouseButtons.Left)
                    {
                        flag = flag || IsButtonDown(MouseButtons.Left);
                    }
                    if (mouseAction.MouseButton != MouseButtons.Right)
                    {
                        flag = flag || IsButtonDown(MouseButtons.Right);
                    }
                    if (mouseAction.MouseButton != MouseButtons.Middle)
                    {
                        flag = flag || IsButtonDown(MouseButtons.Middle);
                    }
                    if (!flag)
                    {
                        mouseButtonState = MouseButtonState.None;
                    }
                    else
                    {
                        mouseButtonState = MouseButtonState.ButtonDown;
                    }
                }
                else if (mouseAction.ActionType == MouseActionType.Drag && mouseButtonState == MouseButtonState.ButtonDown)
                {
                    mouseButtonState = MouseButtonState.DragAction;
                }
            }
        }

        private bool UseCachedElement(MouseAction mouseAction)
        {
            if (!ReferenceEquals(lastMouseAction, null) && lastMouseAction.ActivityElement != null)
            {
                switch (mouseAction.ActionType)
                {
                    case MouseActionType.ButtonDown:
                        return false;

                    case MouseActionType.ButtonUp:
                        return (lastMouseAction.ActionType == MouseActionType.ButtonDown || lastMouseAction.ActionType == MouseActionType.Drag) && RecorderUtilities.PointsWithinRange(mouseAction.AbsoluteLocation, lastMouseAction.AbsoluteMouseDownLocation, SystemInformation.DragSize) || lastActionBoundingRect.Contains(mouseAction.AbsoluteLocation) || handleMouseUpOnIgnoredProcess;

                    case MouseActionType.Drag:
                        return true;
                }
                object[] args = { mouseAction.ActionType };
                CrapyLogger.log.ErrorFormat("Unexpected mouseAction.ActionType {0}", args);
            }
            return false;
        }

        protected override int LowLevelHookId =>
            14;
    }

}
