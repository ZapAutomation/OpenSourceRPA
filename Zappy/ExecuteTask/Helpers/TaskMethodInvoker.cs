using System;
using System.Globalization;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Helpers
{
    internal class TaskMethodInvoker : ITaskMethodInvoker
    {
        private static ITaskMethodInvoker instance;
        private bool isZappyTaskInvokerCalled;

        private TaskMethodInvoker()
        {
        }

        public T InvokeMethod<T>(Func<object> function, ZappyTaskControl control, bool firePlaybackErrorEvent, bool logAsAction)
        {
            bool flag = false;
            if (!isZappyTaskInvokerCalled)
            {
                flag = true;
                isZappyTaskInvokerCalled = true;
            }
            try
            {
                for (int i = 0; i <= ExecutionHandler.Settings.MaximumRetryCount; i++)
                {
                    object obj2 = null;
                    try
                    {
                        obj2 = function();
                        if (obj2 is T)
                        {
                            return (T)obj2;
                        }
                        return (T)Convert.ChangeType(obj2, typeof(T), CultureInfo.InvariantCulture);
                    }
                    catch (Exception exception)
                    {
                        ExecuteErrorOptions options = ExecutionHandler.HandleExceptionAndRetry(exception, control, firePlaybackErrorEvent & flag && i < ExecutionHandler.Settings.MaximumRetryCount);
                        if (!flag || !ExecutionHandler.Settings.ContinueOnError && options == ExecuteErrorOptions.Default)
                        {
                            throw;
                        }
                        if (options != ExecuteErrorOptions.Retry)
                        {
                            return default(T);
                        }
                    }
                    finally
                    {
                        if (logAsAction)
                        {
                            control = control == null && obj2 is ZappyTaskControl ? obj2 as ZappyTaskControl : control;
                                                    }
                    }
                }
            }
            finally
            {
                if (flag)
                {
                    isZappyTaskInvokerCalled = false;
                }
            }
            return default(T);
        }

        public static ITaskMethodInvoker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TaskMethodInvoker();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }
    }
}