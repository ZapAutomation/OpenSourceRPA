using System;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Helpers
{
    internal interface ITaskMethodInvoker
    {
        T InvokeMethod<T>(Func<object> function, ZappyTaskControl control, bool callRetry, bool logAsAction);
    }
}