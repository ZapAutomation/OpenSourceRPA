using System;
using Zappy.SharedInterface;

namespace Zappy.ExecuteTask.Execute
{
    public class ZappyActionErrorEventArgs : ZappyActionEventArgs
    {
        private Exception error;
        private ZappyTaskErrorActionResult result;

        public ZappyActionErrorEventArgs(IZappyAction action, Exception ex) : base(action)
        {
            error = ex;
            result = ZappyTaskErrorActionResult.Default;
        }

        public Exception Error =>
            error;

        public ZappyTaskErrorActionResult Result
        {
            get =>
                result;
            set
            {
                result = value;
            }
        }
    }
}