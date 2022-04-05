using System;
using System.Drawing;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Helpers
{
    public class Mouse : IDisposable
    {
        private static Mouse instance;
        private static readonly object lockObject = new object();

        protected Mouse()
        {
        }

        internal static void Cleanup()
        {
            object lockObject = Mouse.lockObject;
            lock (lockObject)
            {
                if (instance != null)
                {
                    instance.Dispose();
                    instance = null;
                }
            }
        }

        public static void Click()
        {
            Click(null, MouseButtons.Left, ModifierKeys.None, Location);
        }

        public static void Click(ZappyTaskControl control)
        {
            Click(control, MouseButtons.Left, ModifierKeys.None, new Point(-1, -1));
        }

        public static void Click(Point screenCoordinate)
        {
            Click(null, MouseButtons.Left, ModifierKeys.None, screenCoordinate);
        }

        public static void Click(MouseButtons button)
        {
            Click(null, button, ModifierKeys.None, Location);
        }

        public static void Click(ModifierKeys modifierKeys)
        {
            Click(null, MouseButtons.Left, modifierKeys, Location);
        }

        public static void Click(ZappyTaskControl control, Point relativeCoordinate)
        {
            Click(control, MouseButtons.Left, ModifierKeys.None, relativeCoordinate);
        }

        public static void Click(ZappyTaskControl control, MouseButtons button)
        {
            Click(control, button, ModifierKeys.None, new Point(-1, -1));
        }

        public static void Click(ZappyTaskControl control, ModifierKeys modifierKeys)
        {
            Click(control, MouseButtons.Left, modifierKeys, new Point(-1, -1));
        }

        public static void Click(MouseButtons button, ModifierKeys modifierKeys, Point screenCoordinate)
        {
            Click(null, button, modifierKeys, screenCoordinate);
        }

        public static void Click(ZappyTaskControl control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                                        Instance.ClickImplementation(control, button, modifierKeys, relativeCoordinate);
                }
                return null;
            }, control, true, true);
        }

        protected virtual void ClickImplementation(ZappyTaskControl control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate)
        {
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().Click(control, button, modifierKeys, relativeCoordinate);
                }
            }
            catch (Exception exception)
            {
                if (!ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    throw;
                }
                ActionExecutorManager.Instance.GetDefaultExecutor().Click(control, button, modifierKeys, relativeCoordinate);
            }
            finally
            {
                ExecutionHandler.WaitForDelayBetweenActivities();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                if (disposing && ActionExecutorManager.Instance.ActionExecutors != null)
                {
                    foreach (ZappyTaskActionExecutor executor in ActionExecutorManager.Instance.ActionExecutors)
                    {
                        try
                        {
                            executor.ReleaseMouse();
                        }
                        catch (Exception)
                        {
                            object[] args = { executor.GetType().Name };
                            
                        }
                    }
                    ZappyTaskActionExecutorCore.Instance.ReleaseMouse();
                }
                return null;
            }, null, true, false);
        }

        public static void DoubleClick()
        {
            DoubleClick(null, MouseButtons.Left, ModifierKeys.None, Location);
        }

        public static void DoubleClick(ZappyTaskControl control)
        {
            DoubleClick(control, MouseButtons.Left, ModifierKeys.None, new Point(-1, -1));
        }

        public static void DoubleClick(Point screenCoordinate)
        {
            DoubleClick(null, MouseButtons.Left, ModifierKeys.None, screenCoordinate);
        }

        public static void DoubleClick(MouseButtons button)
        {
            DoubleClick(null, button, ModifierKeys.None, Location);
        }

        public static void DoubleClick(ModifierKeys modifierKeys)
        {
            DoubleClick(null, MouseButtons.Left, modifierKeys, Location);
        }

        public static void DoubleClick(ZappyTaskControl control, Point relativeCoordinate)
        {
            DoubleClick(control, MouseButtons.Left, ModifierKeys.None, relativeCoordinate);
        }

        public static void DoubleClick(ZappyTaskControl control, MouseButtons button)
        {
            DoubleClick(control, button, ModifierKeys.None, new Point(-1, -1));
        }

        public static void DoubleClick(ZappyTaskControl control, ModifierKeys modifierKeys)
        {
            DoubleClick(control, MouseButtons.Left, modifierKeys, new Point(-1, -1));
        }

        public static void DoubleClick(MouseButtons button, ModifierKeys modifierKeys, Point screenCoordinates)
        {
            DoubleClick(null, button, modifierKeys, screenCoordinates);
        }

        public static void DoubleClick(ZappyTaskControl control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinates)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                                        Instance.DoubleClickImplementation(control, button, modifierKeys, relativeCoordinates);
                }
                return null;
            }, control, true, true);
        }

        protected virtual void DoubleClickImplementation(ZappyTaskControl control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate)
        {
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().DoubleClick(control, button, modifierKeys, relativeCoordinate);
                }
                else
                {
                    actionExecutor.DoubleClick(ALUtility.GetTechElementFromZappyTaskControl(control), button, modifierKeys, relativeCoordinate);
                }
            }
            catch (Exception exception)
            {
                if (!ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    throw;
                }
                ActionExecutorManager.Instance.GetDefaultExecutor().DoubleClick(control, button, modifierKeys, relativeCoordinate);
            }
            finally
            {
                ExecutionHandler.WaitForDelayBetweenActivities();
            }
        }

        ~Mouse()
        {
            Dispose(false);
        }

        public static void Hover(ZappyTaskControl control)
        {
            Hover(control, new Point(-1, -1), HoverDuration);
        }

        public static void Hover(Point screenCoordinate)
        {
            Hover(null, screenCoordinate, HoverDuration);
        }

        public static void Hover(ZappyTaskControl control, Point relativeCoordinate)
        {
            Hover(control, relativeCoordinate, HoverDuration);
        }

        public static void Hover(Point screenCoordinate, int millisecondsDuration)
        {
            Hover(null, screenCoordinate, millisecondsDuration);
        }

        public static void Hover(ZappyTaskControl control, Point relativeCoordinate, int millisecondDuration)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                                        if (millisecondDuration < 0)
                    {
                        Instance.HoverImplementation(control, relativeCoordinate, 0);
                    }
                    else
                    {
                        Instance.HoverImplementation(control, relativeCoordinate, millisecondDuration);
                    }
                }
                return null;
            }, control, true, true);
        }

        protected virtual void HoverImplementation(ZappyTaskControl control, Point relativeCoordinate, int millisecondsDuration)
        {
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().Hover(control, relativeCoordinate, millisecondsDuration);
                }
                else
                {
                    actionExecutor.Hover(ALUtility.GetTechElementFromZappyTaskControl(control), relativeCoordinate, millisecondsDuration);
                }
            }
            catch (Exception exception)
            {
                if (!ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    throw;
                }
                ActionExecutorManager.Instance.GetDefaultExecutor().Hover(control, relativeCoordinate, millisecondsDuration);
            }
        }

        public static void Move(Point screenCoordinate)
        {
            Move(null, screenCoordinate);
        }

        public static void Move(ZappyTaskControl control, Point relativeCoordinate)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                                        Instance.MoveImplementation(control, relativeCoordinate);
                }
                return null;
            }, control, true, true);
        }

        protected virtual void MoveImplementation(ZappyTaskControl control, Point relativeCoordinate)
        {
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().MouseMove(control, relativeCoordinate);
                }
                else
                {
                    actionExecutor.MouseMove(ALUtility.GetTechElementFromZappyTaskControl(control), relativeCoordinate);
                }
            }
            catch (Exception exception)
            {
                if (!ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    throw;
                }
                ActionExecutorManager.Instance.GetDefaultExecutor().MouseMove(control, relativeCoordinate);
            }
        }

        public static void MoveScrollWheel(int wheelMoveCount)
        {
            MoveScrollWheel(null, wheelMoveCount, ModifierKeys.None);
        }

        public static void MoveScrollWheel(ZappyTaskControl control, int wheelMoveCount)
        {
            MoveScrollWheel(control, wheelMoveCount, ModifierKeys.None);
        }

        public static void MoveScrollWheel(int wheelMoveCount, ModifierKeys modifierKeys)
        {
            MoveScrollWheel(null, wheelMoveCount, modifierKeys);
        }

        public static void MoveScrollWheel(ZappyTaskControl control, int wheelMoveCount, ModifierKeys modifierKeys)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                                        Instance.MoveScrollWheelImplementation(control, wheelMoveCount, modifierKeys);
                }
                return null;
            }, control, true, true);
        }

        protected virtual void MoveScrollWheelImplementation(ZappyTaskControl control, int wheelMoveCount, ModifierKeys modifierKeys)
        {
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().MoveScrollWheel(control, wheelMoveCount, modifierKeys);
                }
                else
                {
                    actionExecutor.MoveScrollWheel(ALUtility.GetTechElementFromZappyTaskControl(control), wheelMoveCount, modifierKeys);
                }
            }
            catch (Exception exception)
            {
                if (!ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    throw;
                }
                ActionExecutorManager.Instance.GetDefaultExecutor().MoveScrollWheel(control, wheelMoveCount, modifierKeys);
            }
            finally
            {
                ExecutionHandler.WaitForDelayBetweenActivities();
            }
        }

        public static void StartDragging()
        {
            StartDragging(null, Location, MouseButtons.Left, ModifierKeys.None);
        }

        public static void StartDragging(ZappyTaskControl control)
        {
            StartDragging(control, new Point(-1, -1), MouseButtons.Left, ModifierKeys.None);
        }

        public static void StartDragging(ZappyTaskControl control, Point relativeCoordinate)
        {
            StartDragging(control, relativeCoordinate, MouseButtons.Left, ModifierKeys.None);
        }

        public static void StartDragging(ZappyTaskControl control, MouseButtons button)
        {
            StartDragging(control, new Point(-1, -1), button, ModifierKeys.None);
        }

        public static void StartDragging(ZappyTaskControl control, Point relativeCoordinate, MouseButtons button, ModifierKeys modifierKeys)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                                        Instance.StartDraggingImplementation(control, button, modifierKeys, relativeCoordinate);
                }
                return null;
            }, control, true, true);
        }

        protected virtual void StartDraggingImplementation(ZappyTaskControl control, MouseButtons button, ModifierKeys modifierKeys, Point relativeCoordinate)
        {
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().StartDragging(control, button, modifierKeys, relativeCoordinate);
                }
                else
                {
                    actionExecutor.StartDragging(ALUtility.GetTechElementFromZappyTaskControl(control), button, modifierKeys, relativeCoordinate);
                }
            }
            catch (Exception exception)
            {
                if (!ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    throw;
                }
                ActionExecutorManager.Instance.GetDefaultExecutor().StartDragging(control, button, modifierKeys, relativeCoordinate);
            }
            finally
            {
                ExecutionHandler.WaitForDelayBetweenActivities();
            }
        }

        public static void StopDragging(ZappyTaskControl control)
        {
            StopDragging(control, new Point(-1, -1));
        }

        public static void StopDragging(Point pointToStop)
        {
            StopDragging(null, pointToStop);
        }

        public static void StopDragging(ZappyTaskControl control, Point relativeCoordinate)
        {
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                                        Instance.StopDraggingImplementation(control, relativeCoordinate, false);
                }
                return null;
            }, control, true, true);
        }

        public static void StopDragging(int moveByX, int moveByY)
        {
            StopDragging(null, moveByX, moveByY);
        }

        public static void StopDragging(ZappyTaskControl control, int moveByX, int moveByY)
        {
            Point point = new Point(moveByX, moveByY);
            TaskMethodInvoker.Instance.InvokeMethod<object>(delegate
            {
                                {
                                        Instance.StopDraggingImplementation(control, point, true);
                }
                return null;
            }, control, true, true);
        }

        protected virtual void StopDraggingImplementation(ZappyTaskControl control, Point coordinate, bool isDisplacement)
        {
            try
            {
                ZappyTaskActionExecutor actionExecutor = ActionExecutorManager.Instance.GetActionExecutor(control);
                if (actionExecutor == null)
                {
                    ActionExecutorManager.Instance.GetDefaultExecutor().StopDragging(control, coordinate, isDisplacement);
                }
                else
                {
                    actionExecutor.StopDragging(ALUtility.GetTechElementFromZappyTaskControl(control), coordinate, isDisplacement);
                }
            }
            catch (Exception exception)
            {
                if (!ALUtility.RetryUsingDefaultExecutor(exception))
                {
                    throw;
                }
                ActionExecutorManager.Instance.GetDefaultExecutor().StopDragging(control, coordinate, isDisplacement);
            }
            finally
            {
                ExecutionHandler.WaitForDelayBetweenActivities();
            }
        }

        public static int HoverDuration
        {
            get =>
                ExecutionHandler.Settings.HoverDuration;
            set
            {
                ExecutionHandler.Settings.HoverDuration = value;
            }
        }

        public static Mouse Instance
        {
            get
            {
                if (instance == null)
                {
                    object lockObject = Mouse.lockObject;
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new Mouse();
                        }
                    }
                }
                return instance;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                instance = value;
            }
        }

        public static Point Location
        {
            get =>
                Instance.LocationImplementation;
            set
            {
                Instance.LocationImplementation = value;
            }
        }

        protected virtual Point LocationImplementation
        {
            get =>
                new Point(ZappyTaskUtilities.CursorPositionX, ZappyTaskUtilities.CursorPositionY);
            set
            {
                ZappyTaskUtilities.SetCursorPosition(value.X, value.Y);
            }
        }

        public static int MouseDragSpeed
        {
            get =>
                ExecutionHandler.Settings.MouseDragSpeed;
            set
            {
                ExecutionHandler.Settings.MouseDragSpeed = value;
            }
        }

        public static int MouseMoveSpeed
        {
            get =>
                ExecutionHandler.Settings.MouseMoveSpeed;
            set
            {
                ExecutionHandler.Settings.MouseMoveSpeed = value;
            }
        }



    }
}