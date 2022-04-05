using System;
using Zappy.SharedInterface;

namespace Zappy.ExecuteTask.Execute
{
    public class ZappyActionWarningEventArgs : ZappyActionEventArgs
    {
        private Exception error;
        private string warningMessage;

        public ZappyActionWarningEventArgs(IZappyAction action, Exception ex, string warning) : base(action)
        {
            error = ex;
            WarningMessage = warning;
        }

        public Exception Error =>
            error;

        public string WarningMessage
        {
            get =>
                warningMessage;
            set
            {
                warningMessage = value;
            }
        }
    }
}