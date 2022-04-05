using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinRowHeader : WinControl
    {
        public WinRowHeader() : this(null)
        {
        }

        public WinRowHeader(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.RowHeader.Name);
        }

        public virtual bool Selected =>
            (bool)GetProperty(PropertyNames.Selected);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string Selected = "Selected";
        }
    }
}

