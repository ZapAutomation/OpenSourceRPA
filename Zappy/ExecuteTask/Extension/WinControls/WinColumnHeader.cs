using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinColumnHeader : WinControl
    {
        public WinColumnHeader() : this(null)
        {
        }

        public WinColumnHeader(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ColumnHeader.Name);
        }
    }
}

