using System;
using System.Collections.Generic;
using System.IO;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Triggers;
using Zappy.ZappyActions.Triggers.Helpers;
using Zappy.ZappyActions.Triggers.Trigger;
using Zappy.ZappyTaskEditor.ExecutionHelpers;
using ZappyMessages;
using ZappyMessages.Helpers;

namespace ZappyPlaybackHelper.Interpreter
{
    public static class ZappyTriggerManagerHelper
    {
        public static event Action ActiveTriggersChanged;

        public static void LoadSavedTriggers()
        {
            string[] triggerFiles = Directory.GetFiles(ZappyMessagingConstants.TriggerFolder, "*.zappy");

            for (int i = 0; i < triggerFiles.Length; i++)
            {
                try
                {
                    //Send to Zappy which triggers are registered - Zappy Maintains the list of running triggers
                    //Same for robot hub
                    ZappyTask _Task = ZappyTask.Create(triggerFiles[i]);
                    //ZappyTaskGuidToFilePath.Add(_Task.Id, triggerFiles[i]);
                    RegisterTriggersForTask(_Task, false);
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.Error(ex);
                }
            }
        }

        public static TriggerStatus RegisterTriggersForTask(ZappyTask _Task, bool Unregister = true)
        {
            try
            {
                TriggerStatus triggerStatus = TriggerStatus.NoTrigger;
                bool triggerDetected = false;
                List<IZappyAction> _Activities = _Task.ExecuteActivities.Activities;
                bool _TriggerRegistered = false;
                for (int j = 0; j < _Activities.Count; j++)
                {
                    if (_Activities[j] is IZappyTrigger zappyTrigger)
                    {
                        if (triggerDetected)
                        {
                            PlaybackHelperService.PopUpNotificationFeedback("[ZappyTriggerManager] Error: Multiple " +
                                "Triggers Detected - first identified trigger will only be registered");
                            break;
                        }
                        triggerDetected = true;
                        if (zappyTrigger.IsDisabled)
                        {
                            UnRegisterTrigger(_Activities[j], _Task);
                            AuditLogger.Info("[ZappyTriggerManager] Disabled Trigger added:" + HelperFunctions.HumanizeNameForIZappyAction(_Activities[j]));
                            triggerStatus = TriggerStatus.Inactive;
                        }
                        else
                        {
                            RegisterTrigger(_Activities[j], _Task, Unregister);
                            _TriggerRegistered = true;
                        }
                    }
                }

                if (_TriggerRegistered)
                    triggerStatus = TriggerStatus.Active;

                //ActiveTriggersChanged?.Invoke();
                return triggerStatus;
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                PlaybackHelperService.PopUpNotificationFeedback(ex.Message);
                return TriggerStatus.Inactive;
            }
        }

        public static void RegisterTrigger(IZappyAction TriggerAction, ZappyTask Task, bool Unregister = true, IZappyExecutionContext context = null)
        {
            if (Unregister)
                UnRegisterTrigger(TriggerAction, Task);

            IZappyTrigger _Trigger = TriggerAction as IZappyTrigger;
            if (!_Trigger.IsDisabled)
            {
                lock (ZappyTriggerManager._TriggerRegisteredTasks)
                {
                    ZappyTriggerManager._TriggerRegisteredTasks[TriggerAction] = Task;
                    //Added to support dynamic property input for trigger
                    if (context == null)
                        context = new ZappyExecutionContext(Task);
                    context.LoadContext(TriggerAction);
                    ///////// - run the trigger function code here - not invoke
                    ZappyTriggerManager._Triggers[TriggerAction] = _Trigger.RegisterTrigger(context);
                    //switch (_Trigger.ZappyTriggerType)
                    //{
                    //    case ZappyTriggerType.FileSystem:
                    //        _Triggers[TriggerAction] = FolderChangeTrigger.ConfigureFileSystemTrigger(TriggerAction);
                    //        break;
                    //    case ZappyTriggerType.Timer:
                    //        _Triggers[TriggerAction] = TimerTriggerManager.ConfigureTimerTrigger(TriggerAction);
                    //        break;
                    //    case ZappyTriggerType.Outlook:
                    //        _Triggers[TriggerAction] = OutlookNewReadEmailTrigger.ConfigureOutlookEmailTrigger(TriggerAction);
                    //        break;
                    //    case ZappyTriggerType.GlobalUpdate:
                    //        _Triggers[TriggerAction] = GlobalUpdateTrigger.ConfigureGlobalUpdateTrigger(TriggerAction);
                    //        break;
                    //    //case ZappyTriggerType.SystemIdle:
                    //    //    _Triggers[TriggerAction] = ConfigureSystemIdleTrigger(TriggerAction);
                    //        //break;
                    //}

                    AuditLogger.Info("[ZappyTriggerManager] Successfully Registered  Trigger " + HelperFunctions.HumanizeNameForIZappyAction(TriggerAction) +
                                     " for Task: " + Task.Id); // + " Unregistered:" + Unregister
                }
            }

            if (ZappyTriggerManager._Triggers.ContainsKey(TriggerAction) && Unregister)
            {
                string _TriggerTaskPath = Path.Combine(ZappyMessagingConstants.TriggerFolder, Task.Id.ToString() + ".zappy");
                Task.Save(_TriggerTaskPath, null, false);
                AuditLogger.Info("[ZappyTriggerManager] Successfully Saved Trigger file" + Task.Id);
            }
            //Temporry TODO - STEPHENFIX
            //ActiveTriggersChanged?.Invoke();
        }
        public static Dictionary<T, Guid> GetActiveTriggersByType<T>()
           where T : IZappyTrigger
        {
            Dictionary<T, Guid> list = new Dictionary<T, Guid>();

            foreach (var trigger in ZappyTriggerManager._Triggers.Keys)
            {
                if (trigger.GetType() == typeof(T) && !((IZappyTrigger)trigger).IsDisabled)
                {
                    list.Add((T)trigger, ZappyTriggerManager._TriggerRegisteredTasks[trigger].Id);
                }
            }

            return list;
        }

        public static void UnRegisterTrigger(IZappyAction TriggerAction, ZappyTask Task)
        {
            //bool deleteTask = false;
            try
            {
                ZappyTriggerManager.UnregisterTriggerHelper(TriggerAction, Task);
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                PlaybackHelperService.PopUpNotificationFeedback(ex.Message);
                //Log error and pop up to user
            }
            //ActiveTriggersChanged?.Invoke();
        }

        public static void ProcessGlobalUpdate(string updateText)
        {
            if (string.IsNullOrEmpty(updateText))
                return;

            lock (ZappyTriggerManager._globalUpdateTriggers)
            {
                foreach (KeyValuePair<Guid, GlobalUpdateTrigger> item in ZappyTriggerManager._globalUpdateTriggers)
                {
                    if (item.Value.FilterText.ValueSpecified && updateText.Contains(item.Value.FilterText.Value))
                        ZappyTriggerManager.CheckAndFireTrigger(item.Value, updateText);
                }
            }
        }
    }
}
