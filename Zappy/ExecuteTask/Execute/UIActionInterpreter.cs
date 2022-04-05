using System;
using System.Collections.Generic;
using System.Security;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ExecuteTask.Execute
{
    public sealed class UIActionInterpreter : ZappyTaskActionInvoker
    {
        private static Dictionary<string, ZappyTaskControl> cachedControls = new Dictionary<string, ZappyTaskControl>(StringComparer.OrdinalIgnoreCase);
        private int? delayBetweenActivities;
        private bool disposed;
        private static bool inRetryMode;
        private IZappyTaskInterpreterCore interpreter;
        private volatile bool playbackCancelled;
        private int? searchTimeout;
        private double? thinkTimeMultiplier;
        private bool wasPlaybackInitializedByMe;

        public UIActionInterpreter(IZappyTaskInterpreterCore interpreter)
        {
            if (interpreter == null)
            {
                throw new ArgumentNullException("interpreter");
            }
            this.interpreter = interpreter;
            this.interpreter.ActionCompleted += ActionCompletedWorker;
            this.interpreter.ZappyTaskStarted += OnZappyTaskStarted;
            this.interpreter.ZappyTaskCompleted += OnZappyTaskCompleted;
        }

        private void ActionCompletedWorker(object sender, ZappyActionEventArgs eventArg)
        {
            ExecutionHandler.PlaybackContext = null;
        }

        private void ApplyInterpreterSettings()
        {
                                                            if (searchTimeout.HasValue)
            {
                ExecutionHandler.Settings.SearchTimeout = searchTimeout.Value;
            }
            if (delayBetweenActivities.HasValue)
            {
                ExecutionHandler.Settings.DelayBetweenActivities = delayBetweenActivities.Value;
            }
            if (thinkTimeMultiplier.HasValue)
            {
                ExecutionHandler.Settings.ThinkTimeMultiplier = thinkTimeMultiplier.Value;
            }
            ExecutionHandler.Settings.UpdateTitleInWindowSearch = false;
            ExecutionHandler.Settings.AutoRefetchEnabled = true;
        }

        public override void Cancel()
        {
            ExecutionHandler.Cancel();
            PlaybackCanceled = true;
        }

        private static ZappyTaskControl CreateZappyTaskControl(string uiObjectName, ScreenIdentifier map)
        {
                                                            ZappyTaskControl uiControl = null;
                        TaskActivityObject uIObjectFromUIObjectId = map.TopLevelWindows[0];
            ZappyTaskControl searchLimitContainer = null;
            while (uIObjectFromUIObjectId != null)
            {
                searchLimitContainer =
                    CreateZappyTaskControlFromTaskActivityObject(uIObjectFromUIObjectId, searchLimitContainer);
                if (uIObjectFromUIObjectId.Descendants == null || uIObjectFromUIObjectId.Descendants.Count <= 0)
                    break;
                uIObjectFromUIObjectId = uIObjectFromUIObjectId.Descendants[0];
            }

            uiControl = searchLimitContainer;

            return uiControl;
        }




        private static ZappyTaskControl CreateZappyTaskControlFromTaskActivityObject(
            TaskActivityObject uIObjectFromUIObjectId, ZappyTaskControl searchLimitContainer)
        {

            ZappyTaskControl uiControl = null;

            uiControl = new ZappyTaskControl(searchLimitContainer);
            if (uIObjectFromUIObjectId.SearchConfigurations != null &&
                uIObjectFromUIObjectId.SearchConfigurations.Length != 0)
            {
                foreach (string str2 in uIObjectFromUIObjectId.SearchConfigurations)
                {
                    uiControl.SearchConfigurations.Add(str2);
                }
            }

            uiControl.SessionId = uIObjectFromUIObjectId.SessionId;
            foreach (string str3 in uIObjectFromUIObjectId.WindowTitles)
            {
                uiControl.WindowTitles.Add(str3);
            }
                                                                        
            uiControl.TechnologyName = uIObjectFromUIObjectId.Framework;
            ExecuteTaskUtility.UpdateControlProperties(uIObjectFromUIObjectId, uiControl);
            if (uiControl.SearchProperties.Find(ZappyTaskControl.PropertyNames.ControlType) == null)
            {
                uiControl.SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType,
                    uIObjectFromUIObjectId.ControlType);
            }

            return uiControl;
        }

                                                                                                                                                                                                                                                                                                                                                                                                                                                        
        public override void Dispose()
        {
            
            if (!disposed)
            {
                                if (wasPlaybackInitializedByMe)
                {
                    ExecutionHandler.Cleanup();
                }
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        private static ZappyTaskControl GetFromCache(string uiObjectId)
        {
            if (cachedControls.ContainsKey(uiObjectId))
            {
                return cachedControls[uiObjectId];
            }
            return null;
        }

        public static ZappyTaskControl GetZappyTaskControl(string uiObjectName, ScreenIdentifier map)
        {
            ZappyTaskControl uiControl = null;
            if (!string.IsNullOrEmpty(uiObjectName))
            {
                                                                                                {
                    uiControl = CreateZappyTaskControl(uiObjectName, map);
                                                                                                }
            }
            return uiControl;
        }

                                
                                                                                                                                
                        
                                
                                        
                                                                
                                                                        
                                
                                                                                                                                                        
                        
                        
                        
                                                        
                                
                                                                                                                
                
                                                                                                                                                                                                        
                                
        
                                
                                                                                                                                                                                                                                                
                                
                                                                                                                                                                
                                
        
                                
                                                                                                                                                                                                                                                                                                        
                        
                        
                        
                        
                                                                        
                        


                                
        
                                
                        
                        
                                
        
                                                                        
        private void OnZappyTaskCompleted(object sender, ZappyTaskEventArgs eventArg)
        {
            ResetCache();
        }

        private void OnZappyTaskStarted(object sender, ZappyTaskEventArgs eventArg)
        {
                        CommonProgram.CleanupFolderAndAllFiles(CrapyConstants.TempFolder);

            if (!ExecutionHandler.IsInitialized)
            {
                ExecutionHandler.Initialize();
                wasPlaybackInitializedByMe = true;
            }
            else if (ExecutionHandler.IsSessionStarted)
            {
                ExecutionHandler.StopSession();
                ExecutionHandler.StartSession();
            }
            else
            {
                ExecutionHandler.StartSession();
            }
            ApplyInterpreterSettings();
        }

        private void ResetCache()
        {
            cachedControls.Clear();
            PlaybackCanceled = false;
            InRetryMode = false;
            SearchHelper.Instance.ResetCache();
        }

        public void SearchAndInvoke(IZappyAction action, ScreenIdentifier map, CustomInvoker func)
        {
            if (action is ZappyTaskAction)
            {
                ZappyTaskControl uITaskControl = GetZappyTaskControl((action as ZappyTaskAction).TaskActivityIdentifier, map);
                (action as ZappyTaskAction).ActivityElement = uITaskControl.TechnologyElement;
            }
            func(action, map);
            if (action is ZappyTaskAction)
                (action as ZappyTaskAction).ActivityElement = null;
        }

        public static SecureString StringToSecureString(string data)
        {
            SecureString str = new SecureString();
            foreach (char ch in data)
            {
                str.AppendChar(ch);
            }
            return str;
        }

        public static void UpdateControlsCache(string uiObjectId, ZappyTaskControl uiControl)
        {
            if (!ReferenceEquals(uiControl, null))
            {
                if (cachedControls.ContainsKey(uiObjectId))
                {
                    cachedControls.Remove(uiObjectId);
                }
                cachedControls.Add(uiObjectId, uiControl);
                string str = ScreenIdentifier.ParentElementId(uiObjectId);
                if (!string.IsNullOrEmpty(str))
                {
                    UpdateControlsCache(str, uiControl.Container);
                }
            }
        }

                                                                                
                        
        public string CurrentBrowser { get; set; }

        public int DelayBetweenActivities
        {
            get
            {
                if (delayBetweenActivities.HasValue)
                {
                    return delayBetweenActivities.Value;
                }
                return ExecutionHandler.Settings.DelayBetweenActivities;
            }
            set
            {
                delayBetweenActivities = value;
            }
        }

        public static bool InRetryMode
        {
            get =>
                inRetryMode;
            set
            {
                inRetryMode = value;
                SearchHelper.Instance.InRetryMode = value;
            }
        }



                                
                                                                                
                                                                                                                                                                                        
                
        internal bool PlaybackCanceled
        {
            get =>
                playbackCancelled;
            set
            {
                UIActionInterpreter interpreter = this;
                lock (interpreter)
                {
                    playbackCancelled = value;
                    SearchHelper.Instance.PlaybackCanceled = value;
                }
            }
        }

        public int SearchTimeout
        {
            get
            {
                if (searchTimeout.HasValue)
                {
                    return searchTimeout.Value;
                }
                return ExecutionHandler.Settings.SearchTimeout;
            }
            set
            {
                searchTimeout = value;
            }
        }

        public double ThinkTimeMultiplier
        {
            get
            {
                if (thinkTimeMultiplier.HasValue)
                {
                    return thinkTimeMultiplier.Value;
                }
                return ExecutionHandler.Settings.ThinkTimeMultiplier;
            }
            set
            {
                thinkTimeMultiplier = value;
            }
        }

        public bool TopLevelWindowSinglePassSearch
        {
            get =>
                ExecutionHandler.Settings.TopLevelWindowSinglePassSearch;
            set
            {
                ExecutionHandler.Settings.TopLevelWindowSinglePassSearch = value;
            }
        }
    }
}