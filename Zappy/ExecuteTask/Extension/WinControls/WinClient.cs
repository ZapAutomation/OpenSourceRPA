using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinClient : WinControl
    {
        public WinClient() : this(null)
        {
        }

        public WinClient(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Client.Name);
        }
    }
}

