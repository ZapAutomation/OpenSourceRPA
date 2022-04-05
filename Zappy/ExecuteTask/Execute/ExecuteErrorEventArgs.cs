using System;

namespace Zappy.ExecuteTask.Execute
{
    public class ExecuteErrorEventArgs : EventArgs
    {
        private Exception error;

        public ExecuteErrorEventArgs(Exception ex)
        {
            error = ex;
            Result = ExecuteErrorOptions.Default;
        }

        public Exception Error =>
            error;

        public ExecuteErrorOptions Result { get; set; }
    }
}