using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinScrollBar : WinControl
    {
        public WinScrollBar() : this(null)
        {
        }

        public WinScrollBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ScrollBar.Name);
        }

        public virtual int MaximumPosition =>
            (int)GetProperty(PropertyNames.MaximumPosition);

        public virtual int MinimumPosition =>
            (int)GetProperty(PropertyNames.MinimumPosition);

        public virtual int Position
        {
            get =>
                (int)GetProperty(PropertyNames.Position);
            set
            {
                SetProperty(PropertyNames.Position, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string MaximumPosition = "MaximumPosition";
            public static readonly string MinimumPosition = "MinimumPosition";
            public static readonly string Position = "Position";
        }
    }
}

