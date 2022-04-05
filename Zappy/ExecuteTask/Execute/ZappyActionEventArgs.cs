using System;
using Zappy.SharedInterface;

namespace Zappy.ExecuteTask.Execute
{
    public class ZappyActionEventArgs : EventArgs
    {
        private IZappyAction action;

        public ZappyActionEventArgs(IZappyAction action)
        {
            this.action = action;
        }

        public IZappyAction Action =>
            action;
    }
}