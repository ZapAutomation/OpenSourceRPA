using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskAction;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.LowLevelHookEvent;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.LogManager;
using Zappy.Decode.Screenshot;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.Decode.Hooks.PropertyChanged
{
    internal class PropertyChangeEventCapture : EventCaptureBase, IEventCapture, IZappyTaskEventNotify
    {
        private ActionSnapshots actionSnapshots;
        private volatile bool addedEventHandlerForPreviousElement;
        private volatile int currentActionCount;
        private volatile TaskActivityElement currentElement;
        private AutoResetEvent elementReceiveEvent;
        private AutoResetEvent elementSendEvent;
                private volatile bool keepProcessing;
        private const string OrientationActonType = "Orientation";
        private volatile TaskActivityElement previousElement;
        private volatile bool processElement;

        public static event EventHandler<EventArgs> RemoveHandlerCompleted;

        public PropertyChangeEventCapture(IEventCapture accessoryEventCapture) : base(accessoryEventCapture)
        {
            this.elementSendEvent = new AutoResetEvent(false);
            this.elementReceiveEvent = new AutoResetEvent(false);
        }

        private void AddHandlerForCurrentElement()
        {
            if (currentElement != null)
            {
                previousElement = currentElement;
                try
                {
                    ZappyTaskEventType eventType = GetEventType(currentElement);
                    if (ZappyTaskService.Instance.AddEventHandler(currentElement, eventType, this))
                    {
                        addedEventHandlerForPreviousElement = true;
                    }
                    else
                    {
                        object[] args = { currentElement };
                        
                    }
                }
                catch (ZappyTaskControlNotAvailableException exception)
                {
                    CrapyLogger.log.Error(exception);
                    HandleZappyTaskControlNotAvailableException(exception);
                    currentElement = null;
                    previousElement = null;
                    addedEventHandlerForPreviousElement = false;
                }
            }
        }

        private static ZappyTaskEventType GetEventType(TaskActivityElement element)
        {
            if (ControlType.CheckBox.NameEquals(element.ControlTypeName) ||
                ControlType.RadioButton.NameEquals(element.ControlTypeName) || ControlType.TreeItem.NameEquals(element.ControlTypeName) || ControlType.CheckBoxTreeItem.NameEquals(element.ControlTypeName))
            {
                return ZappyTaskEventType.StateChanged;
            }
            if (!ControlType.Video.NameEquals(element.ControlTypeName) && !ControlType.Audio.NameEquals(element.ControlTypeName))
            {
                return ZappyTaskEventType.ValueChanged;
            }
            return ZappyTaskEventType.Media;
        }

        protected void HandleZappyTaskControlNotAvailableException(ZappyTaskControlNotAvailableException ex)
        {
                                                                                                                    }

        void IZappyTaskEventNotify.Notify(ITaskActivityElement sourceElement, ITaskActivityElement targetElement, ZappyTaskEventType eventType, object eventArgs)
        {
            TaskActivityElement target = TaskActivityElement.Cast(targetElement);
            TaskActivityElement element2 = TaskActivityElement.Cast(sourceElement);
            if (!Paused && (target != null || eventType == ZappyTaskEventType.OrientationChanged))
            {
                try
                {
                    object syncRoot = actions.SyncRoot;
                    lock (syncRoot)
                    {
                        TaskActivityElement[] sources = element2 != null ? new[] { element2 } : null;
                        NotifyInternal(sources, target, eventType, eventArgs, ElementForThumbnailCapture.TargetElement, false);
                    }
                }
                catch (Exception exception)
                {
                    EventCaptureException = exception;
                }
            }
        }

        void IZappyTaskEventNotify.NotifyMultiSource(ITaskActivityElement[] sourceElements, ITaskActivityElement targetElement, ZappyTaskEventType eventType, object eventArgs, ElementForThumbnailCapture elementForThumbnailCapture)
        {
            TaskActivityElement target = TaskActivityElement.Cast(targetElement);
            List<TaskActivityElement> list = new List<TaskActivityElement>();
            if (sourceElements != null)
            {
                foreach (ITaskActivityElement element2 in sourceElements)
                {
                    list.Add(TaskActivityElement.Cast(element2));
                }
            }
            if (!Paused && target != null)
            {
                try
                {
                    object syncRoot = actions.SyncRoot;
                    lock (syncRoot)
                    {
                        NotifyInternal(list.ToArray(), target, eventType, eventArgs, elementForThumbnailCapture, true);
                    }
                }
                catch (Exception exception)
                {
                    EventCaptureException = exception;
                }
            }
        }

                                                                
                                                                
                        
                        
                                                        
                
        bool IEventCapture.SetTrackingElement(TaskActivityElement element, Point interactionPoint, bool alwaysTakeSnapshot)
        {
            return false;
                        {
                                                                                                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    this.elementSendEvent.Reset();
                    this.elementReceiveEvent.Reset();
                                        if (actionSnapshots != null)
                    {
                        actionSnapshots.StartSnapshot(element, interactionPoint, alwaysTakeSnapshot);
                    }
                                        if (element == null)
                    {
                        if (previousElement != null)
                        {
                            currentElement = null;
                            processElement = true;
                            this.elementSendEvent.Set();
                        }
                    }
                    else if (!Equals(element, previousElement))
                    {
                        if (!ShouldTrackPreviousElement(element, previousElement))
                            RemoveHandlerForPreviousElement();

                        currentElement = element;
                        processElement = true;
                        AddHandlerForCurrentElement();
                        this.elementSendEvent.Set();
                    }


                                    }
                if (!processElement && RemoveHandlerCompleted != null)
                {
                    RemoveHandlerCompleted(this, null);
                }
                                                                                                                object obj3 = this.lockObject;
                lock (obj3)
                {
                    if (accessoryEventCapture != null)
                    {
                        accessoryEventCapture.SetTrackingElement(element, interactionPoint, alwaysTakeSnapshot);
                    }
                }
                                if (actionSnapshots != null)
                {
                    actionSnapshots.WaitForSnapshot();
                }
                                return addedEventHandlerForPreviousElement;
            }
        }

        public void Start()
        {
            
            Paused = false;
            keepProcessing = true;
                                                                                                                                                                                                                                                
                                                            
                                                                        
        }

        public void Stop()
        {
            
            keepProcessing = false;
                                    if (actionSnapshots != null)
            {
                actionSnapshots.Stop();
            }
            
        }

        private void NotifyInternal(TaskActivityElement[] sources, TaskActivityElement target,
            ZappyTaskEventType eventType, object eventArgs, ElementForThumbnailCapture elementForThumbnailCapture, bool multiSource)
        {
            switch (eventType)
            {
                case ZappyTaskEventType.ValueChanged:
                    if (!target.IsPassword)
                    {
                        object[] args = { target, eventArgs };
                        
                    }
                    try
                    {
                        target = elementManager.AddElement(target);
                        if (target == null)
                        {
                            CrapyLogger.log.ErrorFormat("Failed to get IUIElement for the ValueChanged.");
                        }
                        else if (eventArgs == null || RecorderUtilities.IsValidUtf32String(eventArgs.ToString()))
                        {
                            SetValueAction uiTaskAction = new SetValueAction(target, eventArgs);
                            if (sources != null)
                            {
                                for (int i = 0; i < sources.Length; i++)
                                {
                                    if (sources[i] != null)
                                    {
                                        if (i == 0)
                                        {
                                            uiTaskAction.SourceElement = sources[i];
                                        }
                                        if (multiSource)
                                        {
                                            sources[i] = elementManager.AddElement(sources[i]);
                                            uiTaskAction.SourceElements.Add(sources[i]);
                                        }
                                        object[] objArray2 = { sources[i] };
                                        
                                    }
                                }
                            }
                            if (actionSnapshots != null)
                            {
                                actionSnapshots.UpdateActionWithImageEntryCache(uiTaskAction, previousElement, elementForThumbnailCapture);
                            }
                            actions.Push(uiTaskAction);
                        }
                        else
                        {
                            CrapyLogger.log.ErrorFormat("IZappyTaskEventNotify.Notify: Not creating setvalue action for valuechanged as the value returned does not contain valid utf32 chars.");
                        }
                    }
                    catch (ZappyTaskControlNotAvailableException exception)
                    {
                        CrapyLogger.log.Error(exception);
                    }
                    break;

                case ZappyTaskEventType.KeyUp:
                case ZappyTaskEventType.KeyDown:
                case ZappyTaskEventType.MouseUp:
                case ZappyTaskEventType.MouseDown:
                case ZappyTaskEventType.MouseMove:
                case ZappyTaskEventType.MouseOver:
                    break;

                case ZappyTaskEventType.Hover:
                    try
                    {
                        target = elementManager.AddElement(target);
                        if (target == null)
                        {
                            CrapyLogger.log.ErrorFormat("Failed to get IUIElement for the Hover");
                        }
                        else
                        {
                            MouseAction element = new MouseAction(target, MouseButtons.Left, MouseActionType.Hover)
                            {
                                ImplicitHover = true
                            };
                            Point point = new Point(1, 1);
                            if (eventArgs != null && Equals(eventArgs.GetType(), point.GetType()))
                            {
                                point = (Point)eventArgs;
                            }
                            element.Location = point;
                            actions.Push(element);
                        }
                    }
                    catch (ZappyTaskControlNotAvailableException exception4)
                    {
                        CrapyLogger.log.Error(exception4);
                    }
                    break;

                case ZappyTaskEventType.StateChanged:
                    try
                    {
                        target = elementManager.AddElement(target);
                        if (target == null)
                        {
                            CrapyLogger.log.ErrorFormat("Failed to get IUIElement for the StateChanged.");
                        }
                        else
                        {
                            SetStateAction action2 = new SetStateAction(target, eventArgs);
                            if (sources != null)
                            {
                                for (int j = 0; j < sources.Length; j++)
                                {
                                    if (sources[j] != null)
                                    {
                                        if (j == 0)
                                        {
                                            action2.SourceElement = sources[j];
                                        }
                                        if (multiSource)
                                        {
                                            sources[j] = elementManager.AddElement(sources[j]);
                                            action2.SourceElements.Add(sources[j]);
                                        }
                                        object[] objArray3 = { sources[j] };
                                        
                                    }
                                }
                            }
                            if (actionSnapshots != null)
                            {
                                actionSnapshots.UpdateActionWithImageEntryCache(action2, previousElement, elementForThumbnailCapture);
                            }
                            actions.Push(action2);
                        }
                    }
                    catch (ZappyTaskControlNotAvailableException exception2)
                    {
                        CrapyLogger.log.Error(exception2);
                    }
                    break;

                case ZappyTaskEventType.OnFocus:
                    try
                    {
                        target = elementManager.AddElement(target);
                        if (target == null)
                        {
                            CrapyLogger.log.ErrorFormat("Failed to get IUIElement for the OnFocus");
                        }
                    }
                    catch (ZappyTaskControlNotAvailableException exception3)
                    {
                        CrapyLogger.log.Error(exception3);
                    }
                    break;

                case ZappyTaskEventType.Media:
                    try
                    {
                        target = elementManager.AddElement(target);
                        if (target == null)
                        {
                            CrapyLogger.log.ErrorFormat("Failed to get IUIElement for Media event");
                        }
                        else
                        {
                            MediaAction action4 = new MediaAction(target, (ZappyTaskMediaEventInfo)eventArgs);
                            actions.Push(action4);
                        }
                    }
                    catch (ZappyTaskControlNotAvailableException exception5)
                    {
                        CrapyLogger.log.Error(exception5);
                    }
                    break;

                case ZappyTaskEventType.OrientationChanged:
                    {
                        SystemAction action5 = new SystemAction((ZappyTaskActionLogEntry)eventArgs)
                        {
                            ActionType = "Orientation"
                        };
                        actions.Push(action5);
                        return;
                    }
                case ZappyTaskEventType.InvokedEvent:
                    try
                    {
                        target = elementManager.AddElement(target);
                        if (target == null)
                        {
                            CrapyLogger.log.ErrorFormat("Failed to get IUIElement for the InvokedEvent.");
                        }
                        else
                        {
                            InvokeAction action6 = new InvokeAction(target, eventArgs);
                            if (sources != null && sources.Length == 1)
                            {
                                action6.SourceElement = sources[0];
                            }
                            if (actionSnapshots != null)
                            {
                                actionSnapshots.UpdateActionWithImageEntry(action6);
                            }
                            actions.Push(action6);
                        }
                    }
                    catch (ZappyTaskControlNotAvailableException exception6)
                    {
                        CrapyLogger.log.Error(exception6);
                    }
                    break;

                default:
                    return;
            }
        }

        private void ProcessEvents()
        {
            try
            {
                EventCaptureException = null;
                this.OnStarted(this, null);
                
                            }
            catch (ZappyTaskException)
            {
                EventCaptureException = null;
                            }
                        
        }

        private void RemoveHandlerForPreviousElement()
        {
            if (previousElement != null && addedEventHandlerForPreviousElement)
            {
                try
                {
                    ZappyTaskEventType eventType = GetEventType(previousElement);
                                        {
                        if (!ZappyTaskService.Instance.RemoveEventHandler(previousElement, eventType, this))
                        {
                            object[] args = { previousElement };
                            
                        }
                    }
                }
                catch (ZappyTaskControlNotAvailableException exception)
                {
                    CrapyLogger.log.Error(exception);
                }
                finally
                {
                    previousElement = null;
                    addedEventHandlerForPreviousElement = false;
                }
            }
            RemoveHandlerCompleted?.Invoke(this, null);
        }

        private static void SaveScreenshot(object state)
        {
            object[] objArray = (object[])state;
            string filename = objArray[1] as string;
            Bitmap bitmap = (Bitmap)objArray[0];
            try
            {
                bitmap.Save(filename, ImageFormat.Jpeg);
            }
            catch (IOException exception)
            {
                CrapyLogger.log.Error(exception);
            }
            finally
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }
            }
        }

        private static bool ShouldTrackPreviousElement(TaskActivityElement currentElement, TaskActivityElement previousElement)
        {
            if (previousElement != null)
            {
                TaskActivityElement element = FrameworkUtilities.TopLevelElement(currentElement);
                if (element != null)
                {
                    return string.Equals(element.ClassName, "Auto-Suggest Dropdown", StringComparison.OrdinalIgnoreCase) && ControlType.Edit.NameEquals(previousElement.ControlTypeName);
                }
            }
            return false;
        }

        private void TakeScreenshot(Rectangle bounds)
        {
            if (!string.IsNullOrEmpty(ZappyTaskTraceUtility.LogFileDirectory))
            {
                int currentActionCount = this.currentActionCount;
                this.currentActionCount = currentActionCount + 1;
                string str = ZappyTaskUtilities.GetUniqueName(ZappyTaskTraceUtility.LogFileDirectory, "ActionImage", ".jpeg", currentActionCount);
                Rectangle rectangle = new Rectangle(bounds.Location, bounds.Size);
                rectangle.Inflate(100, 100);
                rectangle.Intersect(SystemInformation.VirtualScreen);
                if (rectangle.Width > 0 && rectangle.Height > 0)
                {
                    Bitmap image = new Bitmap(rectangle.Width, rectangle.Height);
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        graphics.CopyFromScreen(rectangle.Location, Point.Empty, rectangle.Size);
                    }
                    object[] state = { image, str };
                    ThreadPool.QueueUserWorkItem(SaveScreenshot, state);
                }
            }
        }

                                                                                                                                                                                                                                            }
}