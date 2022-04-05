using System;
using System.Windows.Automation;
using Zappy.ExecuteTask.TaskExecutor;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfScrollBar : WpfControl
    {
        public WpfScrollBar() : this(null)
        {
        }

        public WpfScrollBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ScrollBar.Name);
        }

        public virtual double MaximumPosition =>
            (double)GetProperty(PropertyNames.MaximumPosition);

        public virtual double MinimumPosition =>
            (double)GetProperty(PropertyNames.MinimumPosition);

        public virtual OrientationType Orientation =>
            (OrientationType)GetProperty(PropertyNames.Orientation);

        public virtual double Position
        {
            get =>
                (double)GetProperty(PropertyNames.Position);
            set
            {
                SetProperty(PropertyNames.Position, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string MaximumPosition = "MaximumPosition";
            public static readonly string MinimumPosition = "MinimumPosition";
            public static readonly string Orientation = "Orientation";
            public static readonly string Position = "Position";
        }
    }
}

