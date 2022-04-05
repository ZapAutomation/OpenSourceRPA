using System;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    public class ZappyTaskEventArgs : EventArgs
    {
        public ZappyTaskEventArgs(ZappyTask uiTask)
        {
            UiTask = uiTask;
        }

        public ZappyTask UiTask { get; private set; }
    }
}