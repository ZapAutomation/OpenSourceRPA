using System;
using System.Threading;
using System.Windows.Threading;

namespace Zappy.Plugins.Uia.Uia
{
    internal class UiaWorker
    {
        private const DispatcherPriority AsyncDispatcherPriority = DispatcherPriority.ApplicationIdle;
        private Thread workerThread;
        private Dispatcher workerThreadDispatcher;

        public void BeginInvoke(Delegate delegateMethod, object arg)
        {
            workerThreadDispatcher.BeginInvoke(AsyncDispatcherPriority, delegateMethod, arg);
        }

        public void StartWorkerThread()
        {
            workerThread = new Thread(() =>
            {
                workerThreadDispatcher = Dispatcher.CurrentDispatcher;
                Dispatcher.Run();
            });
            workerThread.IsBackground = true;
            workerThread.Name = "Uia Manager Thread";
            workerThread.Start();
        }

        public void StopWorkerThread()
        {
            if (workerThreadDispatcher != null && workerThread != null)
            {
                workerThreadDispatcher.BeginInvokeShutdown(DispatcherPriority.ApplicationIdle);
                workerThreadDispatcher = null;
                workerThread = null;
            }
        }
    }
}

