
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask;
using Zappy.ExecuteTask.Execute;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.Plugins.Excel;
using Zappy.Plugins.Outlook;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Triggers;
using Zappy.ZappyActions.Triggers.Trigger;
using ZappyMessages;
using ZappyMessages.Helpers;
using ZappyMessages.OutlookMessages;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;
using ZappyMessages.RecordPlayback;
using ZappyMessages.Triggers;
using ZappyPlaybackHelper.Interpreter;

namespace ZappyPlaybackHelper
{
    public class PlaybackHelperService
    {
        //public static NotificationForm UI_Instance { get; private set; }
        //private bool initialisePlayback = true;

        private static ZappyTaskInterpreter _PlaybackTaskInterpreter;
        internal static PubSubClient _Client;
        private BlockingCollection<ZappyExecutionRequest> _executionQueue;
        private Thread _executionEngine;
        private bool _fullTrace = false;
        private int _counter = 0;

        public static PlaybackHelperService Instance { get; private set; }
        public PlaybackHelperService()
        {
            _fullTrace = Environment.CommandLine.Contains("FullTrace=True");
            Instance = this;
            EndpointAddress _RemoteAddress = new EndpointAddress(ZappyMessagingConstants.EndpointLocationZappyService);
            _Client = new PubSubClient("PlaybackHelper", _RemoteAddress,
                new int[] {
                    PubSubTopicRegister.Chrome2ZappyPlaybackHelperResponse,
                    PubSubTopicRegister.StartPlaybackFromActions,
                    PubSubTopicRegister.StartPlaybackFromFile,
                    PubSubTopicRegister.DebugTask,
                    PubSubTopicRegister.DebugNextStep,
                    PubSubTopicRegister.ControlSignals,
                    PubSubTopicRegister.Excel2ZappyPlaybackHelperResponse,
                    PubSubTopicRegister.Outlook2ZappyPlaybackHelperResponse,
                    PubSubTopicRegister.AutoRunRequest,
                    PubSubTopicRegister.ZappyGlobalUpdate,
                    PubSubTopicRegister.ActiveTriggersRequest,
                    PubSubTopicRegister.UpdatedEnableComAppSettingsChannel,
                    PubSubTopicRegister.Zappy2ZappyPlayback,
                    //PubSubTopicRegister.ZappyPlayback2ZappyRequest,
                    PubSubTopicRegister.Zappy2ZappyPlaybackRequest
                });

            ExcelCommunicator.Init(_Client);
            //initialising outlook coomunication
            OutlookCommunicator.Init(_Client);
            ZappyPlaybackCommunicator.Init(_Client);

            _Client.DataPublished += _Client_DataPublished;
            if (_Client.IsConnected)
                _Client.Publish(PubSubTopicRegister.ControlSignals, PubSubMessages.RequestStateFromZappy);

            _executionQueue = new BlockingCollection<ZappyExecutionRequest>();
            _executionEngine = new Thread(new ThreadStart(ExecuteTasks));
            _executionEngine.SetApartmentState(ApartmentState.STA);
            _executionEngine.Start();

            //Un-register any previous ones
            ZappyTriggerManager.TriggerFired -= ZappyTriggerManager_TriggerFired;
            ZappyTriggerManager.TriggerFired += ZappyTriggerManager_TriggerFired;
            //ZappyTriggerManagerHelper.ActiveTriggersChanged += ZappyTriggerManager_ActiveTriggersChanged;
            Task.Factory.StartNew(ZappyTriggerManagerHelper.LoadSavedTriggers);

            //To initilise run script
            CommonProgram.CSharpPreCompileRunSctipt();
            //CSharpScript.EvaluateAsync("1 + 2");
        }

        private void _Client_DataPublished(PubSubClient Client, int arg1, string arg2)
        {
            //arg2 = StringCipher.Decrypt(arg2, ZappyMessagingConstants.MessageKey);
            try
            {
                switch (arg1)
                {
                    case PubSubTopicRegister.StartPlaybackFromActions:
                        StartPlaybackFromActions(arg2.Split(new string[] { CrapyConstants.StringArrayDelemiter },
                            StringSplitOptions.RemoveEmptyEntries));
                        break;
                    case PubSubTopicRegister.DebugTask:
                        StartPlaybackFromFile(arg2, true, true);
                        break;
                    case PubSubTopicRegister.DebugNextStep:
                        _PlaybackTaskInterpreter.RegisterExecutionSentinelUpdate(arg2);
                        break;
                    case PubSubTopicRegister.AutoRunRequest:
                    case PubSubTopicRegister.StartPlaybackFromFile:
                        StartPlaybackFromFile(arg2, false, _fullTrace);
                        break;
                    case PubSubTopicRegister.ControlSignals:
                        if (arg2 == PubSubMessages.CancelZappyExecutionMessage)
                            CancelTask();
                        else if (arg2 == PubSubMessages.StopZappyExecutionMessage)
                        {
                            CancelTask();
                            ZappyTriggerManager.TriggerFired -= ZappyTriggerManager_TriggerFired;
                        }
                        else if (arg2 == PubSubMessages.StartZappyExecutionMessage)
                        {
                            //Un-Register any previous ones
                            ZappyTriggerManager.TriggerFired -= ZappyTriggerManager_TriggerFired;
                            ZappyTriggerManager.TriggerFired += ZappyTriggerManager_TriggerFired;
                        }
                        else if (arg2 == PubSubMessages.ReloadAssembliesFromDllXMLMessage)
                        {
                            //call reload function here
                            ActionTypeInfo.LoadActionTypeInfo();
                        }
                        break;
                    case PubSubTopicRegister.ZappyGlobalUpdate:
                        ZappyTriggerManagerHelper.ProcessGlobalUpdate(arg2);
                        break;
                    case PubSubTopicRegister.ActiveTriggersRequest:
                        if (arg2 == PubSubMessages.TriggerRequestMessage)
                        {
                            GetActiveTriggers();
                        }
                        else
                        {
                            var triggerInfo = ZappySerializer.DeserializeObject<TriggerRequestInfo>(arg2);
                            //send reply as well
                            if (triggerInfo.RequestType == TriggerRequest.AddNew || triggerInfo.RequestType == TriggerRequest.Update)
                            {
                                ProcessTriggerUpdateRequest(triggerInfo.RequestBody);
                            }
                            else
                            {
                                ProcessTriggerDeleteRequest(triggerInfo.RequestBody);
                            }
                        }

                        break;
                    case PubSubTopicRegister.UpdatedEnableComAppSettingsChannel:
                        ApplicationSettingProperties.Instance.EnableComPlayback = Convert.ToBoolean(arg2);
                        break;
                    case PubSubTopicRegister.Outlook2ZappyPlaybackHelperResponse:
                        {
                            Tuple<OutlookRequest, OutlookNewEmailTriggerInfo> info = ZappySerializer.DeserializeObject<Tuple<OutlookRequest, OutlookNewEmailTriggerInfo>>(arg2);
                            if (info.Item1 == OutlookRequest.NotifyNewMails)
                            {
                                HandleNewEmailEvent(info.Item2);
                            }
                        }
                        break;                    
                    case PubSubTopicRegister.Zappy2ZappyPlaybackRequest:
                        {
                            Tuple<PlayBackHelperRequestEnum, string> _Request =
                                ZappySerializer.DeserializeObject<Tuple<PlayBackHelperRequestEnum, string>>(arg2);
                            if (_Request.Item1 == PlayBackHelperRequestEnum.WindowLaunchTriggerFire)
                            {
                                HandleWindowLaunchEvent(ZappySerializer.DeserializeObject<WindowLaunchTriggerHelper>(_Request.Item2));
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void ZappyTriggerManager_TriggerFired(IZappyAction arg1, ZappyTask arg2, object arg3)
        {
            AuditLogger.Info(string.Format("Processing Trigger Execution request {0}", _counter++));

            Dictionary<string, object> _InitContextData = new Dictionary<string, object>();
            _InitContextData[CrapyConstants.TriggerKey] = arg1;
            _InitContextData[CrapyConstants.TriggerDataKey] = arg3;

            EnqueueExecutionRequest(new ZappyExecutionRequest()
            {
                Task = arg2,
                ContextData = _InitContextData,
                FilePath = arg1.ToString()
            });


        }

        public void StartPlaybackFromFile(string input, bool Debug = false, bool FullTrace = false)
        {
            AuditLogger.Info(string.Format("Processing Execution request {0}, with FullTrace={1}", input, FullTrace));

            try
            {
                Dictionary<string, object> _ContextData = null;

                if (Debug)
                {
                    _ContextData = new Dictionary<string, object>();
                    _ContextData[CrapyConstants.ExecutionSentinel] = new BlockingCollection<string>();

                    if (FullTrace)
                        _ContextData[CrapyConstants.FullExecutionTrace] = true;
                }

                EnqueueExecutionRequest(new ZappyExecutionRequest()
                {
                    ContextData = _ContextData,
                    FilePath = input
                });
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        public void StartPlaybackFromActions(string[] tasksToPlay)
        {
            try
            {
                IZappyAction[] _ActionList = new IZappyAction[tasksToPlay.Length];
                for (int i = 0; i < tasksToPlay.Length; i++)
                {
                    XmlSerializer xcer = ActionTypeInfo.GetSerializerForAction(tasksToPlay[i]);
                    _ActionList[i] = xcer.Deserialize(new StringReader(tasksToPlay[i])) as IZappyAction;

                }

                ZappyTask taskToPlay = new ZappyTask(_ActionList);
                //to get start and stop nodes
                taskToPlay.BeforeSerialize(false);
                EnqueueExecutionRequest(new ZappyExecutionRequest()
                {
                    Task = taskToPlay,
                    FilePath = "Selected Actions"
                });

                //else
                //    Console.WriteLine("Cannot deserialize task!!!");
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex.ToString());
            }
        }

        private void EnqueueExecutionRequest(ZappyExecutionRequest request)
        {
            _executionQueue.Add(request);
            //ReportExecutionRequestStatus(request);
        }

        //private void ReportExecutionRequestStatus(ZappyExecutionRequest request)
        //{
        //    return;
        //    try
        //    {
        //        _Client.Publish(PubSubTopicRegister.ExecutionTaskQueueUpdate, ZappySerializer.SerializeObject(request));
        //    }
        //    catch (Exception ex)
        //    {
        //        CrapyLogger.log.Error(ex);
        //    }
        //}

        private void ReportExecutionRequestStatus(ZappyTaskExecutionResult res)
        {
            try
            {
                _Client.Publish(PubSubTopicRegister.ExecutionTaskQueueUpdate, ZappySerializer.SerializeObject(res));
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private Tuple<ZappyTaskExecutionState, string> ExecuteQueuedTask(ZappyTask taskToPlay, string filePath, Dictionary<string, object> ContextData)
        {
            string input = string.Empty;
            Tuple<ZappyTaskExecutionState, string> _Result = new Tuple<ZappyTaskExecutionState, string>(ZappyTaskExecutionState.Completed, string.Empty);

            try
            {
                AuditLogger.Info(string.Format("Executing task: {0}", filePath)); //, _Counter++

                if (taskToPlay == null)
                {
                    taskToPlay = ZappyTask.Create(filePath);
                }

                if (taskToPlay != null)
                {
                    List<IZappyAction> _Activities = taskToPlay.ExecuteActivities.Activities;

                    string applicationName = String.IsNullOrEmpty(_Activities[0].ExeName)
                        ? string.Empty
                        : Path.GetFileNameWithoutExtension(_Activities[0].ExeName).ToUpper() + " ";

                    input = Path.GetFileNameWithoutExtension(filePath);
                    // try to find and register a trigger, 
                    //contextdata can only be populated by Zappy system on a trigger being fired for a task
                    //if (ContextData == null)
                    //{

                    //    TriggerStatus _TriggerRegistered = ZappyTriggerManager.RegisterTriggersForTask(taskToPlay);

                    //    if (_TriggerRegistered == TriggerStatus.Active)
                    //    {
                    //        //ZappyTriggerManager.ZappyTaskGuidToFilePath[taskToPlay.Id] = filePath;
                    //        //AuditLogger.Info("Successfully Registered Trigger " + applicationName + "Task: " + input);
                    //        //PopUpNotificationFeedback messages are sent to auditlogger
                    //        PopUpNotificationFeedback("Successfully Registered Active Trigger:" + input);
                    //        return new Tuple<ZappyTaskExecutionState, string>(ZappyTaskExecutionState.Completed, "Trigger Registered");
                    //    }
                    //    if (_TriggerRegistered == TriggerStatus.Inactive)
                    //    {
                    //        //ZappyTriggerManager.ZappyTaskGuidToFilePath[taskToPlay.Id] = filePath;
                    //        //AuditLogger.Info("Successfully Registered Trigger " + applicationName + "Task: " + input);
                    //        //PopUpNotificationFeedback messages are sent to auditlogger
                    //        PopUpNotificationFeedback("Disabled Trigger Registered:" + input);
                    //        return new Tuple<ZappyTaskExecutionState, string>(ZappyTaskExecutionState.Completed, "Trigger Registered");
                    //    }
                    //}

                    _PlaybackTaskInterpreter = new ZappyTaskInterpreter(taskToPlay, filePath);

                    DateTime _StartTime = WallClock.Now;

                    if (ContextData == null)
                        ContextData = new Dictionary<string, object>();

                    if (_fullTrace)
                        ContextData[CrapyConstants.FullExecutionTrace] = true;

                    ContextData[CrapyConstants.ExternalActionPublisher] = _Client;

                    _PlaybackTaskInterpreter.DebugerProgress += Uit_DebugerProgress;

                    var Result = _PlaybackTaskInterpreter.ExecuteTask(ContextData);

                    DateTime _StopTime = WallClock.Now;

                    if (Result == ZappyTaskPlaybackResult.Passed)
                    {
                        if(!string.IsNullOrEmpty(_PlaybackTaskInterpreter.TriggerMessage))
                             PopUpNotificationFeedback(_PlaybackTaskInterpreter.TriggerMessage + input);                       
                        else
                            PopUpNotificationFeedback("Successfully Executed " + applicationName + "Task: " + input);                       
                        taskToPlay.ZappyExecutionTimeMillisecs =
                            (int)((_StopTime - _StartTime).Ticks / TimeSpan.TicksPerMillisecond);
                        AuditLogger.SaveSuccessfulExecutedTaskLog(taskToPlay, filePath);
                        _Result = new Tuple<ZappyTaskExecutionState, string>(ZappyTaskExecutionState.Completed, "Success");
                    }
                    else if (Result == ZappyTaskPlaybackResult.Canceled)
                    {
                        PopUpNotificationFeedback("Stopped " + applicationName + "Task: " + input);
                        _Result = new Tuple<ZappyTaskExecutionState, string>(ZappyTaskExecutionState.ForceStopped, "User Cancelled");

                    }
                    else if (Result == ZappyTaskPlaybackResult.Failed)
                    {
                        Guid _NextGuid = Guid.Empty;
                        if (taskToPlay != null)
                        {
                            IZappyAction _EndAction =
                                taskToPlay.ExecuteActivities.Activities.FirstOrDefault(
                                    Action => Action is EndNodeAction);
                            if (_EndAction != null)
                                _NextGuid = _EndAction.SelfGuid;
                        }

                        string failedAction = string.Empty;
                        string ErrorMsg = string.Empty;
                        if (!string.IsNullOrEmpty(_PlaybackTaskInterpreter.FailureMessage.ToString()))
                        {
                            ErrorMsg = " due to: " + _PlaybackTaskInterpreter.FailureMessage;
                        }

                        //create display string for use display helper : uit.PlaybackUiTask.LastPlaybackAction
                        if (!ReferenceEquals(_PlaybackTaskInterpreter.PlaybackUiTask.LastPlaybackAction, null))
                            failedAction = Environment.NewLine +
                                           DisplayHelper.NodeTextHelper(_PlaybackTaskInterpreter.CurrentAction, true) +
                                           " (" + DisplayHelper.GetActionElementName(_PlaybackTaskInterpreter
                                               .PlaybackUiTask.LastPlaybackAction) + ")";


                        _Result = new Tuple<ZappyTaskExecutionState, string>(ZappyTaskExecutionState.Failed,
                            "Failed Execution of " +
                                  applicationName + "Task: " +
                                  input + failedAction + ErrorMsg);
                        //Notify user in this case
                        PopUpNotificationFeedback(_Result.Item2);
                        //Dont perform subsequent task when task fails
                        ClearExecutionQueue();
                        PopUpNotificationFeedback(ZappyMessagingConstants.ZappyExecutionFailed + input
                                                        + " due to: " + _PlaybackTaskInterpreter.FailureMessage.Message);
                        AuditLogger.SaveFailedExecutedTaskLog(taskToPlay, filePath);
                    }

                    _PlaybackTaskInterpreter.DebugerProgress -= Uit_DebugerProgress;
                }
                else
                {
                    _Result = new Tuple<ZappyTaskExecutionState, string>(ZappyTaskExecutionState.Failed, "Cannot read Task: " + input);
                    PopUpNotificationFeedback(_Result.Item2);
                }
            }
            catch (ThreadAbortException ex)
            {
                CrapyLogger.log.Error(ex);

                PopUpNotificationFeedback("Stopped Task: " + input);
                _Result = new Tuple<ZappyTaskExecutionState, string>(ZappyTaskExecutionState.Failed, "Stopped Task - Thread Aborted: " + input + ex.ToString());
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                PopUpNotificationFeedback("Invalid task:" + input + " with Error:" + ex.Message);
                _Result = new Tuple<ZappyTaskExecutionState, string>(ZappyTaskExecutionState.Failed, "Invalid task:" + input + " with Error:" + ex.Message);
            }

            try
            {
                if (_PlaybackTaskInterpreter.context != null)
                    _PlaybackTaskInterpreter.context.DestroyContext();
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }

            return _Result;
        }

        private void Uit_DebugerProgress(bool FullExecutionTraceEnabled, IZappyAction arg1, string arg2, Guid arg3)
        {
            try
            {
                Guid _StartGuid = Guid.Empty;

                if (arg1 != null)
                    _StartGuid = arg1.SelfGuid;
                Tuple<Guid, string, Guid> _tuple = new Tuple<Guid, string, Guid>(_StartGuid, arg2, arg3);

                _Client.Publish(PubSubTopicRegister.DebugStepProgress,
                    ZappySerializer.SerializeObject(_tuple));

                if (FullExecutionTraceEnabled)
                {
                    //Use this for FULL ROBOT logs - DebugFullStepTrace Channel
                    Tuple<IZappyAction, string, Guid> _tuple1 = new Tuple<IZappyAction, string, Guid>(arg1, arg2, arg3);
                    //Write to file _tuple1 here
                    //Can do another file here as well
                    string fullAuditTuple = ZappySerializer.SerializeObject(_tuple1);
                    //Can skip publishing here
                    //_Client.Publish(PubSubTopicRegister.FullStepAuditLogJson,
                    //   fullAuditTuple);
                    //Can use another logger here as well
                    AuditLogger.Info(fullAuditTuple);
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void ExecuteTasks()
        {
        RESTART:
            try
            {
                foreach (ZappyExecutionRequest item in _executionQueue.GetConsumingEnumerable())
                {


                    if (item.IsActive)
                    {
                        //Report task is running to robot or other watchers
                        //var fhlpr = new ZappyMessages.Robot.ZappyFileHelper(item.FilePath);
                        //var res = new ZappyTaskExecutionResult(Guid.Parse(fhlpr.GetGUID()), item.FilePath, ZappyTaskExecutionState.Running, null);
                        //ReportExecutionRequestStatus(res);
                        item.Status = "Executing";
                        //ReportExecutionRequestStatus(item);
                        PopUpNotificationFeedback(ZappyMessagingConstants.ZappyExecutionStarted, false);

                        //Execute the task
                        var status = ExecuteQueuedTask(item.Task, item.FilePath, item.ContextData);
                        if (status != null)
                        {
                            item.Status = status.Item2;
                        }
                        //res.State = status.Item1;
                        //res.StatusMessage = status.Item2;

                        //Report state to robot or other watchers
                        PopUpNotificationFeedback(ZappyMessagingConstants.ZappyExecutionStopped, false);
                        //ReportExecutionRequestStatus(res);
                    }
                    else
                    {
                        item.Status = "Skipped, IsActive = False";
                    }
                    //ReportExecutionRequestStatus(item);

                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                goto RESTART;
            }
            PopUpNotificationFeedback("Task Execution Restart Fail");
        }

        private void ClearExecutionQueue()
        {
            ZappyExecutionRequest queuedItem;

            while (_executionQueue.TryTake(out queuedItem, 0))
                PopUpNotificationFeedback("Stopped Task: " + Path.GetFileNameWithoutExtension(queuedItem.ToString()));
        }

        public void CancelTask()
        {
            try
            {
                ClearExecutionQueue();

                if (_PlaybackTaskInterpreter != null)
                {
                    _PlaybackTaskInterpreter.Cancel();
                    _executionEngine.ExecutionContext?.Dispose();
                    _executionEngine.Abort();
                    PopUpNotificationFeedback(ZappyMessagingConstants.ZappyExecutionStopped, false);
                    //_ExecutionEngine.Start();
                    _executionEngine = new Thread(new ThreadStart(ExecuteTasks));
                    _executionEngine.SetApartmentState(ApartmentState.STA);
                    _executionEngine.Start();
                    //TODO: Check later abort execution
                    _PlaybackTaskInterpreter.CancelZappyTaskAction();
                }
                else
                {
                    AuditLogger.Info("No Zappy Task Running");
                }
            }
            catch (Exception e)
            {
                CrapyLogger.log.Error(e);
                PopUpNotificationFeedback("Error Stopping Zappy Task");
            }
        }

        internal static void PopUpNotificationFeedback(string text, bool addToAuditLog = true)
        {
            if (addToAuditLog)
                AuditLogger.Info(text);

            try
            {
                _Client.Publish(PubSubTopicRegister.Notification, text.Substring(0, Math.Min(200, text.Length)));
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void GetActiveTriggers()
        {
            ZappyTriggerManager._RegisteredTriggers = ZappyTriggerManager._TriggerRegisteredTasks.Keys.ToList();
            var triggerInfoList = new List<TriggerResponseInfo>();

            foreach (var trigger in ZappyTriggerManager._RegisteredTriggers)
            {
                var zappyTask = ZappyTriggerManager._TriggerRegisteredTasks[trigger];

                var triggerInfo = new TriggerResponseInfo()
                {
                    TriggerName = zappyTask.FilePath,
                    TriggerAction = trigger
                };

                triggerInfoList.Add(triggerInfo);
            }

            if (triggerInfoList.Any())
            {
                _Client.Publish(
                    PubSubTopicRegister.ActiveTriggersReponse,
                    ZappySerializer.SerializeObject(triggerInfoList));
            }
        }

        private void HandleNewEmailEvent(OutlookNewEmailTriggerInfo info)
        {
            //var existingTriggers = ZappyTriggerManager._TriggerRegisteredTasks.Keys.ToList();
            var existingTriggers = ZappyTriggerManagerHelper.GetActiveTriggersByType<OutlookNewReadEmailTrigger>();

            foreach (var triggerKvp in existingTriggers)
            {
                if (triggerKvp.Key.SelfGuid == info.TriggerId)
                {
                    ZappyTriggerManager.ValidateAndRaiseTrigger(triggerKvp.Key, info);
                }
            }
        }

        private void HandleWindowLaunchEvent(WindowLaunchTriggerHelper info)
        {
            //var existingTriggers = ZappyTriggerManager._TriggerRegisteredTasks.Keys.ToList();
            Dictionary<WindowLaunchTrigger, Guid> existingTriggers = ZappyTriggerManagerHelper.GetActiveTriggersByType<WindowLaunchTrigger>();

            foreach (KeyValuePair<WindowLaunchTrigger, Guid> triggerKvp in existingTriggers)
            {
                if (triggerKvp.Key.SelfGuid == info.SelfGuid)
                {
                    ZappyTriggerManager.ValidateAndRaiseTrigger(triggerKvp.Key, info);
                }
            }
        }

        //Slow function to optimise in future
        private void ProcessTriggerUpdateRequest(string arg2)
        {
            Dictionary<Guid, IZappyAction> guidToZappyActions = new Dictionary<Guid, IZappyAction>();
            foreach (IZappyAction key in ZappyTriggerManager._TriggerRegisteredTasks.Keys)
            {
                guidToZappyActions[key.SelfGuid] = key;
            }

            ZappyTriggerManager._RegisteredTriggers = ZappySerializer.DeserializeObject<List<IZappyAction>>(arg2);

            foreach (var triggerAction in ZappyTriggerManager._RegisteredTriggers)
            {
                IZappyAction trigger = null;
                guidToZappyActions.TryGetValue(triggerAction.SelfGuid, out trigger);
                if (trigger != null)
                {
                    ZappyTask task = null;
                    ZappyTriggerManager._TriggerRegisteredTasks.TryGetValue(trigger, out task);
                    if (task != null)
                    {
                        //unregister old trigger and register new trigger
                        //ZappyTriggerManager.UnRegisterTrigger(trigger, task);
                        task.ExecuteActivities.Activities.Remove(trigger);
                        task.ExecuteActivities.Activities.Add(triggerAction);
                        //registers the new trigger - unregister false as already called in above function
                        ZappyTriggerManagerHelper.RegisterTriggersForTask(task);
                    }
                }
            }
        }

        private void ProcessTriggerDeleteRequest(string arg2)
        {
            //Make this global once
            Dictionary<Guid, IZappyAction> guidToZappyActions = new Dictionary<Guid, IZappyAction>();
            foreach (IZappyAction key in ZappyTriggerManager._TriggerRegisteredTasks.Keys)
            {
                guidToZappyActions[key.SelfGuid] = key;
            }

            ZappyTriggerManager._RegisteredTriggers = ZappySerializer.DeserializeObject<List<IZappyAction>>(arg2);

            foreach (var triggerAction in ZappyTriggerManager._RegisteredTriggers)
            {
                IZappyAction trigger = null;
                guidToZappyActions.TryGetValue(triggerAction.SelfGuid, out trigger);
                if (trigger != null)
                {
                    ZappyTask task = null;
                    ZappyTriggerManager._TriggerRegisteredTasks.TryGetValue(trigger, out task);
                    if (task != null)
                    {
                        ZappyTriggerManagerHelper.UnRegisterTrigger(trigger, task);
                    }
                }
            }
        }

    }
}
