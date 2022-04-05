using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinGroup : WinControl
    {
        public WinGroup() : this(null)
        {
        }

        public WinGroup(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Group.Name);
        }
    }
}

