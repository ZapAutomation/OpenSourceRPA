using System;
using System.Collections.Generic;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ExecuteTask.Execute
{
    public interface IZappyTaskInterpreterCore
    {
        event EventHandler<ZappyActionEventArgs> ActionCompleted;
        event EventHandler<ZappyActionEventArgs> ActionStarted;
        event EventHandler<ZappyActionErrorEventArgs> InterpreterError;
        event EventHandler<ZappyTaskProgressEventArgs> InterpreterProgress;
                event EventHandler<ZappyTaskEventArgs> ZappyTaskCompleted;
        event EventHandler<ZappyTaskEventArgs> ZappyTaskStarted;
        event Action<bool, IZappyAction, string, Guid> DebugerProgress;
        ZappyExecutionContext context { get; }
        ZappyTaskActionInvoker ActionInvoker { get; set; }
        IZappyAction CurrentAction { get; }
        ZappyTask PlaybackUiTask { get; }
        void Cancel();
        void Dispose();
        ZappyTaskPlaybackResult ExecuteTask(Dictionary<string, object> ContextData = null);
        void RegisterExecutionSentinelUpdate(string Update);
    }
}