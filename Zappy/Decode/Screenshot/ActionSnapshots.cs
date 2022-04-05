using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.PropertyChanged;
using Zappy.Decode.LogManager;
using Zappy.SharedInterface.Helper;

namespace Zappy.Decode.Screenshot
{
    internal class ActionSnapshots
    {
        private const double AddhandlerTimeout = 2000.0;
        private TaskActivityElement cachedPreviousElement;
        private DateTime currentAddHandlerTimeStamp;
        private readonly object imageHashLock = new object();
        private DateTime previousAddHandlerTimeStamp = WallClock.UtcNow;
        private bool previousElementIsInteractable;
        private Dictionary<TaskActivityElement, ZappyTaskImageEntry> previousElementsToImageEntryHash = new Dictionary<TaskActivityElement, ZappyTaskImageEntry>();
        private ZappyTaskImageInfo previousGoodImageInfo;
        private Point previousInteractionPoint;
        private Screen previouslyInteractedScreen;
        private AutoResetEvent removeHandlerEvent;
        private Task snapShotTask;

        private ZappyTaskImageEntry AddAndGetImageEntryToElementHash(TaskActivityElement element)
        {
            ZappyTaskImageEntry entry = null;
            object imageHashLock = this.imageHashLock;
            lock (imageHashLock)
            {
                if (element != null && !previousElementsToImageEntryHash.TryGetValue(element, out entry))
                {
                    entry = new ZappyTaskImageEntry(previousGoodImageInfo, previousInteractionPoint);
                    previousElementsToImageEntryHash.Add(element, entry);
                }
            }
            return entry;
        }

        private bool CanTakeSnapShot(TaskActivityElement currentInteractedElement, double ellapsedMilliSeconds, out bool differentElements)
        {
                                                                        
                        differentElements = true;
            if (ellapsedMilliSeconds >= 2000.0 && (cachedPreviousElement != null || currentInteractedElement != null))
            {
                if (cachedPreviousElement != null && currentInteractedElement != null && cachedPreviousElement.Equals(currentInteractedElement))
                {
                    differentElements = false;
                }
                
                return true;
            }
            if (cachedPreviousElement != null && currentInteractedElement == null)
            {
                
                return true;
            }
            if (cachedPreviousElement == null && currentInteractedElement != null)
            {
                
                return true;
            }
            if (cachedPreviousElement != null && currentInteractedElement != null)
            {
                if (!cachedPreviousElement.Equals(currentInteractedElement))
                {
                    
                    return true;
                }
                differentElements = false;
            }
            
            return false;
                    }

        private void CheckIfPreviousElementIsInteractable()
        {
                                    if (cachedPreviousElement != null)
            {
                bool? nullable;
                if (previousElementsToImageEntryHash != null && previousElementsToImageEntryHash.Count > 0)
                {
                    nullable = cachedPreviousElement.IsInteractable();
                    previousElementIsInteractable = nullable.HasValue ? nullable.GetValueOrDefault() : false;
                }
                else
                {
                    removeHandlerEvent.WaitOne();
                    if (previousElementsToImageEntryHash != null && previousElementsToImageEntryHash.Count > 0)
                    {
                        nullable = cachedPreviousElement.IsInteractable();
                        previousElementIsInteractable = nullable.HasValue ? nullable.GetValueOrDefault() : false;
                    }
                }
            }
                    }

        public void Initialize()
        {
                                    previouslyInteractedScreen = Screen.PrimaryScreen;
            SnapshotGenerator.Instance.TakeSnapshotOfScreen(previouslyInteractedScreen);
                    }

        private static bool? IsElementLikelyToBeVisible(TaskActivityElement element)
        {
            try
            {
                Screen screen = Screen.FromHandle(element.WindowHandle);
                if (screen == null || screen.Bounds.IsEmpty || screen.Bounds.Size.IsEmpty)
                {
                    return false;
                }
                Rectangle boundingRectangle = ElementExtension.GetBoundingRectangle(element);
                Rectangle rectangle2 = new Rectangle(boundingRectangle.X, boundingRectangle.Y, boundingRectangle.Width, boundingRectangle.Height);
                return !rectangle2.IsEmpty && !rectangle2.Size.IsEmpty && rectangle2.IntersectsWith(screen.Bounds);
            }
            catch (Exception exception)
            {
                object[] args = { exception.ToString() };
                
            }
            return null;
        }

        internal void RemoveHandlerCompleted(object sender, EventArgs e)
        {
            removeHandlerEvent.Set();
        }

        private void SetPreviouslyInteractedScreen(TaskActivityElement element)
        {
            if (element != null)
            {
                Screen screen = Screen.FromHandle(element.WindowHandle);
                if (screen != null && !screen.Bounds.IsEmpty && !screen.Bounds.Size.IsEmpty)
                {
                    previouslyInteractedScreen = screen;
                }
            }
        }

        public void Start()
        {
            PropertyChangeEventCapture.RemoveHandlerCompleted += RemoveHandlerCompleted;
            removeHandlerEvent = new AutoResetEvent(false);
        }

        public void StartSnapshot(TaskActivityElement currentElement, Point interactionPoint, bool alwaysTakeSnapshot)
        {
            ZappyTaskImageInfo imgInfo = null;
            bool differentElement = true;
            currentAddHandlerTimeStamp = WallClock.UtcNow;
            snapShotTask = null;
            previousElementIsInteractable = false;
            double ellapsedMilliSeconds = currentAddHandlerTimeStamp.Subtract(previousAddHandlerTimeStamp).TotalMilliseconds;
            snapShotTask = Task.Factory.StartNew(delegate
            {
                                                if (currentElement == null)
                {
                    
                    CheckIfPreviousElementIsInteractable();
                    if (previousElementIsInteractable)
                    {
                        TakeSnapShotInternal(currentElement, alwaysTakeSnapshot, ref imgInfo, ref differentElement, ellapsedMilliSeconds);
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    Action[] actions = { () => TakeSnapShotInternal(currentElement, alwaysTakeSnapshot, ref imgInfo, ref differentElement, ellapsedMilliSeconds), () => CheckIfPreviousElementIsInteractable() };
                    Parallel.Invoke(actions);
                }
                UpdateSnapShot(imgInfo, differentElement, alwaysTakeSnapshot, ellapsedMilliSeconds >= 2000.0, interactionPoint);
                cachedPreviousElement = currentElement;
                            });
        }

        public void Stop()
        {
            PropertyChangeEventCapture.RemoveHandlerCompleted -= RemoveHandlerCompleted;
            removeHandlerEvent.Dispose();
        }

        private ZappyTaskImageInfo TakeSnapShot(TaskActivityElement element, ref bool differentElement, double ellapsedMilliSeconds, bool alwaysTakeSnapshot)
        {
                                    
            differentElement = true;
            ZappyTaskImageInfo info = null;
            try
            {
                if (CanTakeSnapShot(element, ellapsedMilliSeconds, out differentElement) | alwaysTakeSnapshot)
                {
                    info = SnapshotGenerator.Instance.TakeSnapshotOfScreen(previouslyInteractedScreen);
                }
            }
            catch (SystemException exception)
            {
                object[] args = { exception };
                CrapyLogger.log.ErrorFormat("ActionSnapshots: Exception {0} occurred during TakeSnapShot", args);
            }
            return info;
                    }

        private void TakeSnapShotInternal(TaskActivityElement currentElement, bool alwaysTakeSnapshot, ref ZappyTaskImageInfo imgInfo, ref bool differentElement, double ellapsedMilliSeconds)
        {
            SetPreviouslyInteractedScreen(currentElement);
            imgInfo = TakeSnapShot(currentElement, ref differentElement, ellapsedMilliSeconds, alwaysTakeSnapshot);
        }

        public void UpdateActionWithImageEntry(ZappyTaskAction uiTaskAction)
        {
                                    if (uiTaskAction.ActivityElement != null && previousGoodImageInfo != null)
            {
                Point updatedInteractionPointforActiveScreen = RecorderUtilities.GetUpdatedInteractionPointforActiveScreen(uiTaskAction.ActivityElement, previousInteractionPoint);
                uiTaskAction.ImageEntry = new ZappyTaskImageEntry(previousGoodImageInfo, updatedInteractionPointforActiveScreen);
            }
                    }

        public void UpdateActionWithImageEntryCache(ZappyTaskAction uiTaskAction, TaskActivityElement previousElement, ElementForThumbnailCapture elementForCapture)
        {
                        {
                TaskActivityElement uIElement = previousElement;
                if (elementForCapture == ElementForThumbnailCapture.TargetElement && uiTaskAction.ActivityElement != null)
                {
                    uIElement = uiTaskAction.ActivityElement;
                }
                uiTaskAction.ImageEntry = AddAndGetImageEntryToElementHash(uIElement);
            }
        }

        private void UpdateImageEntryWithAfterEffects(ZappyTaskImageEntry imageEntry, ZappyTaskImageInfo currentImgInfo, TaskActivityElement element, bool isElementAvailable)
        {
            if (isElementAvailable)
            {
                Point point = previousElementIsInteractable ? RecorderUtilities.GetUpdatedInteractionPointforActiveScreen(element, previousInteractionPoint) : RecorderUtilities.GetDefaultInteractionPoint(element);
                imageEntry.UpdateZappyTaskImageEntry(currentImgInfo, point);
            }
            else
            {
                object[] args = { imageEntry.ImageInformation.ImagePath };
                
            }
        }

        private void UpdatePreviousImageEntries(ZappyTaskImageInfo currentImgInfo, TaskActivityElement cachedPreviousElement)
        {
                        {
                object imageHashLock = this.imageHashLock;
                lock (imageHashLock)
                {
                    foreach (KeyValuePair<TaskActivityElement, ZappyTaskImageEntry> pair in previousElementsToImageEntryHash)
                    {
                        TaskActivityElement key = null;
                        bool isElementAvailable = false;
                        if (pair.Key.Equals(cachedPreviousElement))
                        {
                            key = this.cachedPreviousElement;
                            isElementAvailable = previousElementIsInteractable;
                        }
                        else
                        {
                            key = pair.Key;
                            bool? nullable = IsElementLikelyToBeVisible(key);
                            isElementAvailable = nullable.HasValue ? nullable.GetValueOrDefault() : false;
                        }
                        UpdateImageEntryWithAfterEffects(pair.Value, currentImgInfo, key, isElementAvailable);
                    }
                }
            }
        }

        private void UpdateSnapShot(ZappyTaskImageInfo currentImgInfo, bool differentElement, bool forceSnapShot, bool timeout, Point currentInteractionPoint)
        {
            if (currentImgInfo == null)
            {
                if (!differentElement)
                {
                    previousInteractionPoint = currentInteractionPoint;
                }
            }
            else
            {
                if (differentElement | timeout | forceSnapShot)
                {
                    UpdatePreviousImageEntries(currentImgInfo, cachedPreviousElement);
                    previousElementsToImageEntryHash.Clear();
                }
                previousGoodImageInfo = currentImgInfo;
                previousInteractionPoint = currentInteractionPoint;
            }
        }

        public void WaitForSnapshot()
        {
            previousAddHandlerTimeStamp = currentAddHandlerTimeStamp;
                        {
                if (snapShotTask != null)
                {
                    snapShotTask.Wait();
                }
            }
        }
    }
}