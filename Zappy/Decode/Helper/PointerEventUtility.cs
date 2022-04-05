using System.Drawing;
using System.Windows.Forms;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.LogManager;

namespace Zappy.Decode.Helper
{
    internal static class PointerEventUtility
    {
        internal static bool handleMouseUpOnIgnoredProcess;
        internal static Rectangle lastActionBoundingRect = Rectangle.Empty;
        internal static MouseAction lastMouseAction;
        internal static MouseButtonState mouseButtonState;

        internal static void ClearActionTracking()
        {
            lastMouseAction = null;
            mouseButtonState = MouseButtonState.None;
            lastActionBoundingRect = Rectangle.Empty;
        }

                                                                                                                                                                                                                                                                
                        
                        
                        
                                                
        private static ModifierKeys GetModifiers()
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

        private static bool IsButtonDown(MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                return NativeMethods.IsKeyStateDown(Keys.LButton);
            }
            return button == MouseButtons.Right && NativeMethods.IsKeyStateDown(Keys.RButton);
        }

                                                                                                        
        internal static void NormalizeLocation(MouseAction action)
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

                                                                                                                                                                                                        
        internal static bool UpdateActionProperties(MouseAction mouseAction)
        {
            bool flag = true;
            if (mouseAction.ActionType == MouseActionType.ButtonUp)
            {
                
                if (lastMouseAction != null)
                {
                    mouseAction.StartTimestamp = lastMouseAction.StartTimestamp;
                    mouseAction.ModifierKeys = lastMouseAction.ModifierKeys;
                    mouseAction.ImageEntry = lastMouseAction.ImageEntry;
                    mouseAction.ActivityElement = lastMouseAction.ActivityElement;
                }
                mouseAction.ActionType = MouseActionType.Click;
                return flag;
            }
            if (mouseAction.ActionType == MouseActionType.Drag && lastMouseAction != null && lastMouseAction.ActionType == MouseActionType.ButtonDown)
            {
                
                mouseAction.Location = lastMouseAction.Location;
                mouseAction.AbsoluteLocation = lastMouseAction.AbsoluteLocation;
                mouseAction.ModifierKeys = lastMouseAction.ModifierKeys;
            }
            return flag;
        }

        internal static void UpdateMouseButtonState(MouseAction mouseAction)
        {
            if (mouseAction.MouseButton == MouseButtons.Left || mouseAction.MouseButton == MouseButtons.Right)
            {
                if (mouseAction.ActionType == MouseActionType.ButtonDown)
                {
                    mouseButtonState = MouseButtonState.ButtonDown;
                }
                else if (mouseAction.ActionType == MouseActionType.ButtonUp)
                {
                    MouseButtons button = mouseAction.MouseButton == MouseButtons.Left ? MouseButtons.Right : MouseButtons.Left;
                    if (!IsButtonDown(button))
                    {
                        mouseButtonState = MouseButtonState.None;
                    }
                    else
                    {
                        mouseButtonState = MouseButtonState.ButtonDown;
                    }
                }
            }
        }

        internal static bool UseCachedElement(MouseAction mouseAction)
        {
            if (lastMouseAction != null && lastMouseAction.ActivityElement != null)
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
                CrapyLogger.log.ErrorFormat("PointerEventUtility: Unexpected mouseAction.ActionType {0}", args);
            }
            return false;
        }

        internal enum MouseButtonState
        {
            None,
            ButtonDown,
            DragAction
        }
    }
}