using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Execute;
using Zappy.Helpers;
using Zappy.Plugins.ChromeBrowser.Chrome;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Excel.ExcelWorkbook;
using Zappy.ZappyActions.OCR;
using Zappy.ZappyActions.Triggers.Helpers;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace ZappyPlaybackHelper.Interpreter
{
    public class ZappyTaskInterpreterCore : IDisposable, IZappyTaskInterpreterCore
    {
        public Exception FailureMessage;
        //public static MetaData Metadata;

        public string TriggerMessage = string.Empty;

        private ZappyTaskActionInvoker actionInvoker;
        private IZappyAction currentAction;
        private bool disposed;
        protected volatile bool playbackCancelled;
        private volatile bool playbackInProgress;
        private ZappyTask _playbackUiTask;
        private int totalNumberOfActivities;
        private int retryCount = 0;

        public event EventHandler<ZappyActionEventArgs> ActionCompleted;
        public event EventHandler<ZappyActionEventArgs> ActionStarted;
        public event EventHandler<ZappyActionErrorEventArgs> InterpreterError;
        public event EventHandler<ZappyTaskProgressEventArgs> InterpreterProgress;
        public event EventHandler<ZappyTaskEventArgs> ZappyTaskCompleted;
        public event EventHandler<ZappyTaskEventArgs> ZappyTaskStarted;
        public event Action<bool, IZappyAction, string, Guid> DebugerProgress;

        public ZappyTaskInterpreterCore(ZappyTask uiTask)
        {
            ZappyTaskUtilities.CheckForNull(uiTask, "uiTask");
            _playbackUiTask = uiTask;
        }

        public virtual void Cancel()
        {
            playbackCancelled = true;
            ActionInvoker.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ActionInvoker = null;
                }
                disposed = true;
            }
        }

        protected internal virtual void ExecuteAction(IZappyAction action, ScreenIdentifier map)
        {
            action.Invoke(context, ActionInvoker);
        }

        private void ExecuteActionInternal(IZappyAction action)
        {
            ScreenIdentifier map = null;
            string id = string.Empty;

            //if (PlaybackUiTask.ScreenIdentifiers.Count > 0)
            //{
            //    map = PlaybackUiTask.ScreenIdentifiers[0];
            //    id = map.Id;
            //}
            if (action is ZappyTaskAction zaction)
            {
                map = zaction.WindowIdentifier;
                id = map.Id;
            }

            ActionStarted?.Invoke(this, new ZappyActionEventArgs(action));
            try
            {
                ExecuteAction(action, map);
            }
            finally
            {
                ActionCompleted?.Invoke(this, new ZappyActionEventArgs(action));
            }
        }

        private ZappyTaskPlaybackResult ExecuteStepInternal(IZappyAction action, out bool retryStep)
        {
            retryStep = false;
            if (playbackCancelled)
            {
                return ZappyTaskPlaybackResult.Canceled;
            }
            ZappyTaskPlaybackResult playbackResult = ZappyTaskPlaybackResult.Failed;
            try
            {
                ExecuteActionInternal(action);
                playbackResult = ZappyTaskPlaybackResult.Passed;
                retryCount = 0;
            }
            catch (Exception exception)
            {
                //IsDebugerAttached ? 0 :
                int _MaxRetryCount = action.NumberOfRetries;
                //if (action is ChromeAction || action is ZappyTaskAction || action is WorkbookOpen)
                //{
                //    _MaxRetryCount = CrapyConstants.ActionRetryCount;
                //}
                //= (action is ChromeAction
                //   ? Settings.Default.ChromeActionRetryCount
                //   : CrapyConstants.ActionRetryCount);

                CrapyLogger.log.ErrorFormat("action :{0}: {1}, MaxRetryCount:{2}, {3},", action.Id, retryCount,
                    _MaxRetryCount, exception);
                FailureMessage = exception;
                if (retryCount < _MaxRetryCount)
                {
                    Thread.Sleep(100);
                    retryCount++;
                    playbackResult = ExecuteStepInternal(action, out retryStep);
                }

                if (playbackResult != ZappyTaskPlaybackResult.Passed)
                {
                    if (!IsExpectedExceptionFromActionExecution(exception))
                    {
                        CrapyLogger.log.Error(exception);
                    }

                    if (!HandleStepExecutionException(exception, action, out playbackResult, out retryStep))
                    {
                        CrapyLogger.log.Error(exception);
                    }
                }
                //if (IsDebugerAttached)
                //    throw exception;
                return playbackResult;
            }
            return playbackResult;
        }

        public ZappyExecutionContext context { get; private set; }

        public virtual ZappyTaskPlaybackResult ExecuteTask(Dictionary<string, object> ContextData = null)
        {
            context = new ZappyExecutionContext(PlaybackUiTask, ContextData);
            ZappyTaskPlaybackResult result;

            if (playbackCancelled)
            {
                playbackCancelled = false;
                return ZappyTaskPlaybackResult.Canceled;
            }

            try
            {
                //ZappyTask.FireBeforeExecute(ActionInvoker, PlaybackUiTask);
                ZappyTaskStarted?.Invoke(this, new ZappyTaskEventArgs(PlaybackUiTask));
            }
            catch (ZappyTaskException)
            {
                throw;
            }

            playbackInProgress = true;

            try
            {
                object triggerFiredActionObject = null;
                //Differentiate actions that are fired by triggers
                context.ContextData.TryGetValue(CrapyConstants.TriggerKey, out triggerFiredActionObject);
                IZappyAction triggerFiredAction = null;
                if (triggerFiredActionObject != null && triggerFiredActionObject is IZappyAction)
                    triggerFiredAction = triggerFiredActionObject as IZappyAction;
                result = IterateActionList(triggerFiredAction);
            }
            catch
            {
                throw;
            }
            finally
            {

                try
                {
                    ZappyTaskCompleted?.Invoke(this, new ZappyTaskEventArgs(PlaybackUiTask));
                }
                catch (ZappyTaskException exception)
                {
                    RaiseErrorAction(exception);
                }
                playbackCancelled = false;
                //Added it as can freeup com GCs for EXCEL
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        private bool HandleStepExecutionException(Exception ex, IZappyAction action, out ZappyTaskPlaybackResult resultPlayback, out bool retryStep)
        {
            retryStep = false;
            resultPlayback = ZappyTaskPlaybackResult.Failed;
            bool flag = true;
            if (playbackCancelled)
            {
                resultPlayback = ZappyTaskPlaybackResult.Canceled;
                //SqmUtility.IntermediateResult = SessionResult.Fail;
                return flag;
            }
            CrapyLogger.log.Error(ex);
            if (action.ContinueOnError)
            {
                //CrapyLogger.log.InfoFormat("ContinueOnError set to true. Skipping action on Error.");

                resultPlayback = ZappyTaskPlaybackResult.Passed;
                //if (InterpreterWarning != null)
                //{
                //    ZappyActionWarningEventArgs args = new ZappyActionWarningEventArgs(action, ex, Resources.SkippingFailedAction);
                //    InterpreterWarning(this, args);
                //}
                return flag;
            }
            //if (!(action is TestStepMarkerAction))
            //{
            //    this.LogActionExecutionException(ex);
            //}
            ZappyActionErrorEventArgs e = new ZappyActionErrorEventArgs(action, ex);
            if (InterpreterError != null)
            {
                InterpreterError(this, e);
                object[] objArray1 = { e.Result };
                //CrapyLogger.log.InfoFormat("On Playback Error, user chose {0}.", objArray1);

            }
            //SqmUtility.IntermediateResult = SessionResult.Fail;
            OnErrorHandledByUser(e);
            switch (e.Result)
            {
                case ZappyTaskErrorActionResult.StopPlaybackAndContinueManually:
                    resultPlayback = ZappyTaskPlaybackResult.Failed;
                    return flag;

                case ZappyTaskErrorActionResult.StopPlaybackAndRerecord:
                    resultPlayback = ZappyTaskPlaybackResult.Failed;
                    return flag;

                case ZappyTaskErrorActionResult.RetryAction:
                    resultPlayback = ZappyTaskPlaybackResult.Failed;
                    retryStep = true;
                    return flag;

                case ZappyTaskErrorActionResult.SkipAction:
                    resultPlayback = ZappyTaskPlaybackResult.Passed;
                    retryStep = true;
                    return flag;
            }
            return false;
        }

        protected internal virtual bool IsExpectedExceptionFromActionExecution(Exception ex) =>
            ex is ZappyTaskException;

        //Allow singleton actions such as OCR
        private ZappyTaskPlaybackResult IterateActionList(IZappyAction triggerStartAction)
        {
            ActionList actionList = PlaybackUiTask.ExecuteActivities;

            ZappyTaskPlaybackResult passed = ZappyTaskPlaybackResult.Passed;
            bool _FullExecutionTraceEnabled = context.ContextData.ContainsKey(CrapyConstants.FullExecutionTrace);
            string _Result = string.Empty;

            try
            {
                if (actionList.Count <= 0)
                {
                    return passed;
                }

                int num = 0;
                int num2 = actionList.Count - 1;

                totalNumberOfActivities = num2 - num + 1;
                OnProgress(0, actionList.Activities[num]);
                bool retryStep = false;
                currentAction = null;

                Dictionary<Guid, IZappyAction> _Actions = (context.ContextData[CrapyConstants.ZappyTaskKey] as ZappyTask).ActionDictionary;
                IZappyAction _Action = null, _StartAction = null, _EndAction = null;

                for (int i = 0; i < actionList.Count; i++)
                {
                    _Actions[actionList.Activities[i].SelfGuid] = actionList.Activities[i];
                    if (actionList.Activities[i] is StartNodeAction)
                        _StartAction = _Action = actionList.Activities[i];
                    else if (actionList.Activities[i] is EndNodeAction)
                        _EndAction = actionList.Activities[i];
                    else if (actionList.Activities[i] is OcrToClipboardAction)
                        _Action = actionList.Activities[i];
                }
                //Overriding startaction with the given action
                if (triggerStartAction != null)
                    _StartAction = _Action = triggerStartAction;


                DebugerProgress?.Invoke(_FullExecutionTraceEnabled, null, _Result, _StartAction == null ? Guid.Empty : _StartAction.SelfGuid);

                while (_Action != null)
                {
                    ZappyTaskPlaybackResult _ActivityResult = ZappyTaskPlaybackResult.Passed;

                    PlaybackUiTask.LastPlaybackAction = _Action;
                    currentAction = _Action;
                    do
                    {
                        try
                        {
                            context.LoadContext(_Action);
                            if (_FullExecutionTraceEnabled)
                                DebugerProgress?.Invoke(_FullExecutionTraceEnabled, _Action, "ContextLoaded", _Action.SelfGuid);
                            auditLogInfo(false);
                            _ActivityResult = ExecuteStepInternal(_Action, out retryStep);
                            auditLogInfo(true);
                            context.SaveContext(_Action);
                            if (_Action.PauseTimeAfterAction > 0)
                                Thread.Sleep(_Action.PauseTimeAfterAction);
                        }
                        catch (Exception ex)
                        {
                            _ActivityResult = ZappyTaskPlaybackResult.Failed;
                            _Result = HandleActionFailure(ex);
                        }
                        finally
                        {
                            ActionInvoker.InRetryMode = retryStep;
                        }

                        #region Commented code
                        //try
                        //{
                        //    //Try messages like succesfully executed
                        //    //Or failed execution - Put message box when failing or delay notification
                        //    //Turn it to red

                        //    //string notificationInfo = String.Empty;
                        //    //if (_ActivityResult == ZappyTaskPlaybackResult.Passed)
                        //    //    notificationInfo = "Executed action "
                        //    //                       + auditInfo;
                        //    //else if (_ActivityResult == ZappyTaskPlaybackResult.Failed)
                        //    //    notificationInfo = "Failed execution of action "
                        //    //                       + auditInfo;
                        //    //else if (_ActivityResult == ZappyTaskPlaybackResult.Canceled)
                        //    //    notificationInfo = "Cancelled execution of action "
                        //    //                       + auditInfo;// + " with Result " + _ActivityResult;
                        //    //PlaybackHelperService.PopUpNotificationFeedback(notificationInfo);
                        //}
                        //// with Action Guid " + currentAction.SelfGuid 
                        //catch (Exception ex)
                        //{
                        //    CrapyLogger.log.Error(ex);
                        //}
                        #endregion
                    }
                    while (retryStep && _ActivityResult == ZappyTaskPlaybackResult.Failed);

                    //if (_ActivityResult == ZappyTaskPlaybackResult.Failed)
                    //{
                    //    //Logs failed action
                    //    AuditLogger.Info("Failed execution of action " + CommonProgram.HumanizeNameForGivenType(_Action.GetType())
                    //                     + " due to " + FailureMessage.Message);
                    //}

                    Guid _NextGuid = _Action.NextGuid;
                    IZappyAction _CurrentAction = _Action;

                    if (!(_Action is EndNodeAction) && (_ActivityResult == ZappyTaskPlaybackResult.Passed ||
                                                        (_ActivityResult == ZappyTaskPlaybackResult.Failed &&
                                                         _Action.ContinueOnError))
                                                    && _Actions.TryGetValue(_NextGuid, out IZappyAction _NextAction))
                    {
                        //comes here on suceess or failure if continue on error is true
                        bool continueOnError = _Action.ContinueOnError;
                        _Action = _NextAction;
                        if (continueOnError)
                            PlaybackHelperService.PopUpNotificationFeedback("Excecuting Exception Handeled Action " + CommonProgram.HumanizeNameForGivenType(_Action.GetType()));
                    }
                    //TODO: Uncomment after proper testing for error handeling GUID
                    else if (_ActivityResult == ZappyTaskPlaybackResult.Failed &&
                             _Action.ErrorHandlerGuid != Guid.Empty &&
                             _Actions.TryGetValue(_Action.ErrorHandlerGuid, out IZappyAction _NextActionEx))
                    {
                        _Action = _NextActionEx;
                        PlaybackHelperService.PopUpNotificationFeedback("Excecuting Exception Handeled Action " + CommonProgram.HumanizeNameForGivenType(_Action.GetType()));
                    }
                    else
                        _Action = null;

                    passed |= _ActivityResult;

                    DebugerProgress?.Invoke(_FullExecutionTraceEnabled, _CurrentAction,
                        _ActivityResult == ZappyTaskPlaybackResult.Passed ? "Success" : _ActivityResult.ToString() + _Result,
                        _Action == null ? Guid.Empty : _Action.SelfGuid);

                    //Check if the next _Action is not null and a trigger action - if a trigger action then register it
                    //and set the action to null
                    if(_Action != null && _Action is IZappyTrigger _triggerAction)
                    {
                        //Call this in a seperate function
                        try
                        {
                            ZappyTriggerManagerHelper.RegisterTrigger(_Action, PlaybackUiTask, true, context);
                            if (_triggerAction.IsDisabled)
                                TriggerMessage = "Disabled Trigger Registered: ";
                            else
                                TriggerMessage = "Successfully Registered Active Trigger: ";
                        }
                        catch(Exception ex)
                        {
                            _ActivityResult = ZappyTaskPlaybackResult.Failed;
                            _Result = HandleActionFailure(ex);
                            DebugerProgress?.Invoke(_FullExecutionTraceEnabled, _Action, _Result, Guid.Empty);
                        }

                        _Action = null;
                    }

                    passed = _ActivityResult;
                }
            }
            finally
            {
                //To stop taks editor page at the end
                DebugerProgress?.Invoke(_FullExecutionTraceEnabled, null,"", Guid.Empty);
            }
            return passed;
        }

        public string HandleActionFailure(Exception ex)
        {
            CrapyLogger.log.Error(ex);
            FailureMessage = ex;
            //if (IsDebugerAttached)
            return ex.ToString();
        }

        private void auditLogInfo(bool afterExecution)
        {
            try
            {
                string message;
                if (afterExecution)
                {
                    string auditInfoExecution = currentAction.AuditInfo();
                    String auditInfo = auditInfoExecution == null
                        ? string.Empty
                        : auditInfoExecution.Substring(0, Math.Min(400, auditInfoExecution.Length));
                    message = "Executed " + auditInfo + " with Guid: " + currentAction.SelfGuid;
                }
                else
                {
                    message = "Started Executing " + CommonProgram.HumanizeNameForGivenType(currentAction.GetType());
                }
                AuditLogger.Info(message);
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        protected internal virtual void LogActionExecutionException(Exception exception)
        {
        }

        protected internal virtual void OnErrorHandledByUser(ZappyActionErrorEventArgs eventArgs)
        {
        }

        private void OnProgress(int currentActionCount, IZappyAction action)
        {
            if (InterpreterProgress != null)
            {
                ZappyTaskProgressEventArgs e = new ZappyTaskProgressEventArgs(currentActionCount, totalNumberOfActivities, action);
                InterpreterProgress(this, e);
            }
        }

        private void RaiseErrorAction(ZappyTaskException ex)
        {
            ZappyActionErrorEventArgs e = new ZappyActionErrorEventArgs(null, ex);
            InterpreterError?.Invoke(this, e);
        }

        protected internal void RaisePlaybackProgressEvent(ZappyTaskProgressEventArgs arg)
        {
            InterpreterProgress?.Invoke(this, arg);
        }

        public virtual ZappyTaskActionInvoker ActionInvoker
        {
            get =>
                actionInvoker;
            set
            {
                actionInvoker = value;
            }
        }

        public IZappyAction CurrentAction =>
            currentAction;

        protected internal bool PlaybackInProgress =>
            playbackInProgress;

        public virtual ZappyTask PlaybackUiTask =>
            _playbackUiTask;

        public void RegisterExecutionSentinelUpdate(string Update)
        {
            if (context.ContextData.TryGetValue(CrapyConstants.ExecutionSentinel, out object _Sentinel))
            {
                try
                {
                    (_Sentinel as BlockingCollection<string>).TryAdd(Update);
                    CrapyLogger.log.Info("DEBUG  Step Added!" + Update);
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.ErrorFormat("Error adding Debug Step :{0}, {1}", Update, ex);
                }
            }
        }
    }
}