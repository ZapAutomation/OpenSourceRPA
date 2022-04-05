using System;

namespace Zappy.SharedInterface.Helper
{
    public abstract class ZappyTaskActionInvoker : IDisposable
    {
        public abstract void Cancel();
        public abstract void Dispose();

        public bool InRetryMode { get; set; }


    }
}
