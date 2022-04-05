using System;
using System.Collections.Concurrent;
using System.Threading;
using Zappy.ActionMap.ScreenMaps;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace ZappyPlaybackHelper.Interpreter
{
    public class ZappyTaskInterpreter : ZappyTaskInterpreterCore
    {
        protected bool disposed;
        internal ALUtility.Highlighter highlighterObject;
        protected bool isActionInvokerAssignedByDefault;

        public ZappyTaskInterpreter(ZappyTask uiTask) : base(uiTask)
        {


        }

        public ZappyTaskInterpreter(ZappyTask uiTask, string FileName) : base(uiTask)
        {

        }

        private CancellationTokenSource cts;

        public override void Cancel()
        {
            if (cts != null)
                cts.Cancel(true);
            base.Cancel();
        }

        public void CancelZappyTaskAction()
        {
            while (PlaybackInProgress)
            {
                if (CurrentAction is ZappyTaskAction)
                    DispatcherUtilities.ProcessEventsOnUIThread();
                else
                {
                    return;
                }
                Thread.Sleep(10);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing && highlighterObject != null)
                {
                    highlighterObject.Dispose();
                    highlighterObject = null;
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }

        protected bool HasSearchableUIElement(IZappyAction zaction)
        {
            if (!(zaction is ZappyTaskAction))
                return false;

            ZappyTaskAction action = zaction as ZappyTaskAction;

            if (action == null || string.IsNullOrEmpty(action.TaskActivityIdentifier))
            {
                return false;
            }
            return true;
        }

        protected void Highlight(IZappyAction action, ScreenIdentifier map)
        {
            if (action is ZappyTaskAction)
            {
                ITaskActivityElement uIElement = (action as ZappyTaskAction).ActivityElement;
                if (uIElement != null)
                {
                    int num;
                    int num2;
                    int num3;
                    int num4;
                    uIElement.GetBoundingRectangle(out num, out num2, out num3, out num4);
                    HighlighterObject.Highlight(num, num2, num3, num4);
                }
            }

        }

        public void HighlightElement()
        {
            
        }

        protected bool IsActionOnIENotificationToolBar(IZappyAction action, ScreenIdentifier map)
        {
            if (action != null && map != null)
            {
                if (action is ZappyTaskAction)
                {
                    for (string str = (action as ZappyTaskAction).TaskActivityIdentifier; !string.IsNullOrEmpty(str); str = ScreenIdentifier.ParentElementId(str))
                    {
                        PropertyExpressionCollection expressions;
                        PropertyExpressionCollection expressions2;
                        PropertyExpressionCollection.GetProperties(map.GetUIObjectFromUIObjectId(str).Condition, out expressions, out expressions2);
                        PropertyExpression expression = expressions.Find(ZappyTaskControl.PropertyNames.Name);
                        if (expression != null && string.Equals(expression.PropertyValue, LocalizedSystemStrings.Instance.IE9NotificationBar, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }

            }
            return false;
        }

        protected internal override void ExecuteAction(IZappyAction action, ScreenIdentifier map)
        {
            try
            {
                object _Sentinel = null;
                if (context.ContextData.TryGetValue(CrapyConstants.ExecutionSentinel, out _Sentinel))
                {
                    if (cts == null)
                        cts = new CancellationTokenSource();
                    string _ExecutionSentinelValue = null;
                    AuditLogger.Info("DEBUG Waiting!");
                    try
                    {
                        (_Sentinel as BlockingCollection<string>).TryTake(out _ExecutionSentinelValue, -1, cts.Token);
                        AuditLogger.Info("DEBUG  Step!" + _ExecutionSentinelValue);
                    }
                    catch (OperationCanceledException)
                    {
                        AuditLogger.Info("DEBUG Task Cancelled while waiting!");
                        return;
                    }
                    catch (Exception)
                    { }
                }

                if (playbackCancelled)
                    return;

                action.Invoke(context, ActionInvoker);

            }
            finally
            {
            }
        }

        protected internal override bool IsExpectedExceptionFromActionExecution(Exception ex)
        {
            if (!(ex is ZappyTaskException) && !ExecutionHandler.ExceptionFromPropertyProvider(ex))
            {
                return false;
            }
            return true;
        }

        protected internal override void LogActionExecutionException(Exception exception)
        {
            CrapyLogger.log.Error(exception);
           
        }

        protected internal override void OnErrorHandledByUser(ZappyActionErrorEventArgs eventArgs)
        {
            ZappyTaskErrorActionResult result = eventArgs.Result;
            if ((result == ZappyTaskErrorActionResult.StopPlaybackAndContinueManually || result - 3 <= ZappyTaskErrorActionResult.StopPlaybackAndContinueManually) && ActionInvoker != null && ActionInvoker is UIActionInterpreter)
            {
             
            }
        }

        public void RaiseWaitForThinkTimeEvent(IZappyAction action, int thinkTime)
        {
            ZappyTaskProgressEventArgs arg = new ZappyTaskProgressEventArgs(thinkTime, action);
            RaisePlaybackProgressEvent(arg);
        }

        public void Unhighlight()
        {
            if (HighlighterObject != null)
            {
                HighlighterObject.Hide();
            }
        }

        public override ZappyTaskActionInvoker ActionInvoker
        {
            get
            {
                if (base.ActionInvoker == null)
                {
                    base.ActionInvoker = new UIActionInterpreter(this);
                    isActionInvokerAssignedByDefault = true;
                }
                return base.ActionInvoker;
            }
            set
            {
                if (base.ActionInvoker != null && isActionInvokerAssignedByDefault)
                {
                    base.ActionInvoker.Dispose();
                }
                base.ActionInvoker = value;
                isActionInvokerAssignedByDefault = false;
            }
        }

        internal ALUtility.Highlighter HighlighterObject
        {
            get
            {
                if (highlighterObject == null)
                {
                    highlighterObject = new ALUtility.Highlighter();
                }
                return highlighterObject;
            }
        }

        public void Abort()
        {
            StaThreadWorker.Instance.Dispose();
            MsaaZappyPlugin.Instance.Dispose();
        }
    }
}
