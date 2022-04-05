using System;
using System.Drawing;
using System.Windows.Forms;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Helper;
using Zappy.Decode.Helper.Enums;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.LogManager;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Hooks.LowLevelHookEvent
{
    internal abstract class LowLevelHookEventCapture : EventCaptureBase, IEventCapture
    {
        protected static Rectangle lastActionBoundingRect = Rectangle.Empty;
        protected static bool lastCaptureOfUIElementFailed;
        protected static KeyboardAction lastModifierKeyPress;
        protected static MouseAction lastMouseAction;
                                protected static MouseButtonState mouseButtonState;
                
        public LowLevelHookEventCapture(IEventCapture accessoryEventCapture) : base(accessoryEventCapture)
        {
                    }

        protected virtual void ClearActionTracking()
        {
            ((IEventCapture)this).SetTrackingElement(null, new Point(), false);
            lastActionBoundingRect = Rectangle.Empty;
            mouseButtonState = MouseButtonState.None;
            lastModifierKeyPress = null;
            lastMouseAction = null;
            lastCaptureOfUIElementFailed = false;
        }

        protected internal static ModifierKeys GetModifiers()
        {
            bool flag = NativeMethods.IsKeyStateDown(Keys.ShiftKey);
            bool flag2 = NativeMethods.IsKeyStateDown(Keys.Menu);
            bool flag3 = NativeMethods.IsKeyStateDown(Keys.ControlKey);
            bool flag4 = NativeMethods.IsKeyStateDown(Keys.LWin);
            bool flag5 = NativeMethods.IsKeyStateDown(Keys.RWin);
            ModifierKeys none = ModifierKeys.None;
            if (flag)
            {
                none |= ModifierKeys.Shift;
            }
            if (flag2)
            {
                none |= ModifierKeys.Alt;
            }
            if (flag3)
            {
                none |= ModifierKeys.Control;
            }
            if (flag4 | flag5)
            {
                none |= ModifierKeys.Windows;
            }
            return none;
        }

        protected abstract ProcessType GetProcessType(InputAction inputAction, out int processId);
        protected void HandleNetworkException(ApplicationFromNetworkShareException ex)
        {
                                    ClearActionTracking();
            ErrorAction element = new ErrorAction(ex.Message);
            actions.Push(element);
            lastCaptureOfUIElementFailed = true;
        }

        protected void HandleZappyTaskControlInvalidActionException(ZappyTaskControlInvalidActionException ex)
        {
            CrapyLogger.log.Error(ex);
            ClearActionTracking();
        }

        protected void HandleZappyTaskControlNotAvailableException(ZappyTaskControlNotAvailableException ex)
        {
                        CrapyLogger.log.Error(ex);
            ClearActionTracking();
            ErrorAction element = new ErrorAction(ex.Message)
            {
                ContinueOnError = true
            };
            actions.Push(element);
            lastCaptureOfUIElementFailed = true;
        }

                        protected bool IsActionOnIgnoredOrTransparentProcess(InputAction action)
        {
            try
            {
                int processId = 0;
                ProcessType processType = GetProcessType(action, out processId);
                if (ProcessManager.IgnoredOrTransparentProcess(processType))
                {
                    if (ProcessManager.GetHigherPrivelegeFlagForProcessId(processId))
                    {
                        ErrorAction element = new ErrorAction("Properties.Resources.HigherPrivilegeError");
                        actions.Push(element);
                        ProcessManager.SetHigherPrivelegeFlagForProcessId(processId, false);
                    }
                    if (processType == ProcessType.IgnoredProcess)
                    {
                        accessoryEventCapture.SetTrackingElement(null, new Point(), false);
                    }
                    object[] args = { action.ActionName };
                    
                    return true;
                }
            }
            catch (ApplicationFromNetworkShareException exception)
            {
                HandleNetworkException(exception);
            }
            return false;
        }

        protected static bool IsButtonDown(MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                return NativeMethods.IsKeyStateDown(Keys.LButton);
            }
            return button == MouseButtons.Right && NativeMethods.IsKeyStateDown(Keys.RButton);
        }

        protected abstract bool IsMessageOfInterest(LowLevelHookMessage message, IntPtr lParam);

        
                                                                                        
                                                                                        
                                                        
                
                                                                                                                                                        
                                                                                                                                                                                
                                                                                                                                                                                                                        
                        
                        
                                                                
                
                        
                                                                                        
        
                        
                                        
        
        protected static void NormalizeLocation(MouseAction action)
        {
            try
            {
                lastActionBoundingRect = ElementExtension.GetBoundingRectangle(action.ActivityElement);
                action.Location = new Point(action.Location.X - lastActionBoundingRect.X, action.Location.Y - lastActionBoundingRect.Y);
            }
            catch (ZappyTaskControlNotAvailableException)
            {
                action.Location = new Point(lastActionBoundingRect.X, lastActionBoundingRect.Y);
            }
        }

                                        
                                                                                                                                                                                                                                                                
        protected void UpdateActionWithCursorElement(MouseAction mouseAction, Point location)
        {
                                    TaskActivityElement elementFromPoint = ZappyTaskService.Instance.GetElementFromPoint(location.X, location.Y);
            mouseAction.ActivityElement = elementManager.AddElement(elementFromPoint);
            NormalizeLocation(mouseAction);
            lastCaptureOfUIElementFailed = false;
                    }

        protected void UpdateActionWithFocusedElement(ZappyTaskAction action)
        {
                                    TaskActivityElement focusedElement = ZappyTaskService.Instance.GetFocusedElement();
            action.ActivityElement = elementManager.AddElement(focusedElement);
            lastCaptureOfUIElementFailed = false;
                    }

                                                                                                
        protected abstract int LowLevelHookId { get; }

                                                                                                                                                        
        [Flags]
        protected enum MouseButtonState
        {
            None,
            ButtonDown,
            DragAction
        }
    }



}
