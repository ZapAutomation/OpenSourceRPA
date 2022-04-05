using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Helpers;

namespace Zappy.ExecuteTask.TaskExecutor
{
    internal class ZappyTaskActionExecutorCore
    {
        private List<ModifierKeys> pressedKeys = new List<ModifierKeys>();
        private MouseButtons pressedMouseButton;
        private static ZappyTaskActionExecutorCore s_instance = new ZappyTaskActionExecutorCore();

        private ZappyTaskActionExecutorCore()
        {
        }

        public void Click(ZappyTaskControl control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate)
        {
            try
            {
                if (control != null)
                {
                    control.Click(button, modifierKeys, relativeCoordinate);
                }
                else
                {
                    ScreenElement.Desktop.MouseButtonClick(relativeCoordinate.X, relativeCoordinate.Y, button, modifierKeys, 0);
                }
            }
            catch (Exception exception)
            {
                Execute.ExecutionHandler.MapAndThrowException(exception, "Click", null);
                throw;
            }
        }

        public void DoubleClick(ZappyTaskControl control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate)
        {
            try
            {
                if (control != null)
                {
                    control.DoubleClick(button, modifierKeys, relativeCoordinate);
                }
                else
                {
                    ScreenElement.Desktop.DoubleClick(relativeCoordinate.X, relativeCoordinate.Y, button, modifierKeys, 0);
                }
            }
            catch (Exception exception)
            {
                Execute.ExecutionHandler.MapAndThrowException(exception, "DoubleClick", null);
                throw;
            }
        }

        public void Hover(ZappyTaskControl control, Point relativeCoordinate, int millisecondsDuration)
        {
            try
            {
                if (control != null)
                {
                    Point point = new Point();
                    Rectangle boundingRectangle = new Rectangle();
                    if ((ControlType.Menu.NameEquals(control.ControlType.Name) || ControlType.MenuBar.NameEquals(control.ControlType.Name) || ControlType.MenuItem.NameEquals(control.ControlType.Name)) && control.TryGetClickablePoint(out point))
                    {
                        if (Equals(point, relativeCoordinate))
                        {
                            boundingRectangle = control.BoundingRectangle;
                            if (boundingRectangle.X + boundingRectangle.Width / 2 > point.X)
                            {
                                point.X += boundingRectangle.Width / 4;
                            }
                            else
                            {
                                point.X -= boundingRectangle.Width / 4;
                            }
                            if (boundingRectangle.Y + boundingRectangle.Height / 2 > point.Y)
                            {
                                point.Y += boundingRectangle.Height / 4;
                            }
                            else
                            {
                                point.Y -= boundingRectangle.Height / 4;
                            }
                        }
                        control.MouseHover(point, 0, -1);
                        Move(control, relativeCoordinate);
                    }
                    control.MouseHover(relativeCoordinate, millisecondsDuration, -1);
                }
                else
                {
                    ScreenElement.Desktop.MouseHover(relativeCoordinate.X, relativeCoordinate.Y, 0, -1, millisecondsDuration);
                }
            }
            catch (Exception exception)
            {
                Execute.ExecutionHandler.MapAndThrowException(exception, "Hover", null);
                throw;
            }
        }

        public void MouseMove(ZappyTaskControl control, Point relativeCoordinate)
        {
            try
            {
                if (control != null)
                {
                    control.MouseHover(relativeCoordinate, 0, Mouse.MouseMoveSpeed);
                }
                else
                {
                    ScreenElement.Desktop.MouseHover(relativeCoordinate.X, relativeCoordinate.Y, 0, Mouse.MouseMoveSpeed, 0);
                }
            }
            catch (Exception exception)
            {
                Execute.ExecutionHandler.MapAndThrowException(exception, "Hover", null);
                throw;
            }
        }

        private void Move(ZappyTaskControl control, Point relativeCoordinate)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                MouseMove(control, relativeCoordinate);
                return null;
            }, control, true, true);
        }

        public void MoveScrollWheel(ZappyTaskControl control, int wheelMoveCount, ModifierKeys modifierKeys)
        {
            int fEnable = -1;
            try
            {
                if (control != null)
                {
                    control.ScrollMouseWheel(wheelMoveCount, modifierKeys);
                }
                else
                {
                    fEnable = ScreenElement.Playback.EnableEnsureVisibleForPrimitive(0);
                    ScreenElement.Desktop.MouseWheel(wheelMoveCount, modifierKeys, false);
                }
            }
            catch (Exception exception)
            {
                Execute.ExecutionHandler.MapAndThrowException(exception, "Wheel", null);
                throw;
            }
            finally
            {
                if (fEnable != -1)
                {
                    ScreenElement.Playback.EnableEnsureVisibleForPrimitive(fEnable);
                }
            }
        }

        public void PressModifierKeys(ZappyTaskControl control, ModifierKeys modifierKeys)
        {
            try
            {
                if (control != null)
                {
                    control.PressModifierKeys(modifierKeys);
                }
                else
                {
                    ScreenElement.PressModifierKeysStatic(modifierKeys);
                }
                if (!pressedKeys.Contains(modifierKeys))
                {
                    pressedKeys.Add(modifierKeys);
                }
            }
            catch (Exception exception)
            {
                Execute.ExecutionHandler.MapAndThrowException(exception, "PressModifierKeys", control);
                throw;
            }
        }

        public void ReleaseKeyboard()
        {
            foreach (ModifierKeys keys in pressedKeys)
            {
                try
                {
                    ScreenElement.ReleaseModifierKeysStatic(keys);
                }
                catch (COMException)
                {
                                    }
            }
            pressedKeys.Clear();
        }

        public void ReleaseModifierKeys(ZappyTaskControl control, ModifierKeys keys)
        {
            try
            {
                if (control != null)
                {
                    control.ReleaseModifierKeys(keys);
                }
                else
                {
                    ScreenElement.ReleaseModifierKeysStatic(keys);
                }
                pressedKeys.Remove(keys);
            }
            catch (Exception exception)
            {
                Execute.ExecutionHandler.MapAndThrowException(exception, "ReleaseModifierKeys", control);
                throw;
            }
        }

        public void ReleaseMouse()
        {
            if (pressedMouseButton != MouseButtons.None && ALUtility.IsMouseButtonPressed(pressedMouseButton))
            {
                int fEnable = ScreenElement.Playback.EnableEnsureVisibleForPrimitive(0);
                try
                {
                    ScreenElement.Desktop.StopDragging(Mouse.Location.X, Mouse.Location.Y, 0);
                }
                catch (COMException)
                {
                                    }
                finally
                {
                    ScreenElement.Playback.EnableEnsureVisibleForPrimitive(fEnable);
                    pressedMouseButton = MouseButtons.None;
                }
            }
        }

        public void SendKeys(ZappyTaskControl control, string text, ModifierKeys modifierKeys, bool isUnicode, bool isEncoded)
        {
            try
            {
                if (control != null)
                {
                    control.SendKeys(text, modifierKeys, isEncoded, isUnicode);
                }
                else
                {
                    ScreenElement.TypeString(text, modifierKeys, isEncoded, isUnicode);
                }
            }
            catch (Exception exception)
            {
                string parameterValue = Utility.ConvertModiferKeysToString(modifierKeys, text);
                Execute.ExecutionHandler.MapAndThrowException(exception, "SendKeys", parameterValue, control);
                throw;
            }
        }

        public void StartDragging(ZappyTaskControl control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate)
        {
            if (pressedMouseButton != MouseButtons.None)
            {
                if (ALUtility.IsMouseButtonPressed(pressedMouseButton))
                {
                    throw new Exception();
                }
                pressedMouseButton = MouseButtons.None;
            }
            try
            {
                if (control != null)
                {
                    control.StartDragging(relativeCoordinate, button, modifierKeys);
                }
                else
                {
                    ScreenElement.Desktop.StartDragging(relativeCoordinate.X, relativeCoordinate.Y, button, modifierKeys, false);
                }
                pressedMouseButton = button;
            }
            catch (Exception exception)
            {
                Execute.ExecutionHandler.MapAndThrowException(exception, "Drag", null);
                throw;
            }
        }

        public void StopDragging(ZappyTaskControl control, Point coordinate, bool isDisplacement)
        {
            if (pressedMouseButton == MouseButtons.None)
            {
                            }
            try
            {
                if (control != null)
                {
                    control.StopDragging(coordinate.X, coordinate.Y, isDisplacement);
                }
                else
                {
                    int x = coordinate.X;
                    int y = coordinate.Y;
                    if (isDisplacement)
                    {
                        x = Mouse.Location.X + coordinate.X;
                        y = Mouse.Location.Y + coordinate.Y;
                    }
                    int fEnable = ScreenElement.Playback.EnableEnsureVisibleForPrimitive(0);
                    try
                    {
                        ScreenElement.Desktop.StopDragging(x, y, Mouse.MouseDragSpeed);
                    }
                    finally
                    {
                        ScreenElement.Playback.EnableEnsureVisibleForPrimitive(fEnable);
                    }
                }
                pressedMouseButton = MouseButtons.None;
            }
            catch (Exception exception)
            {
                Execute.ExecutionHandler.MapAndThrowException(exception, "Drag", null);
                throw;
            }
        }

        public static ZappyTaskActionExecutorCore Instance =>
            s_instance;
    }
}