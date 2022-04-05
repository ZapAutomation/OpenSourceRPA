using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfSlider : WpfControl
    {
        public WpfSlider() : this(null)
        {
        }

        public WpfSlider(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Slider.Name);
        }

        public virtual double LargeChange =>
            (double)GetProperty(PropertyNames.LargeChange);

        public virtual double MaximumPosition =>
            (double)GetProperty(PropertyNames.MaximumPosition);

        public virtual double MinimumPosition =>
            (double)GetProperty(PropertyNames.MinimumPosition);

        public virtual double Position
        {
            get =>
                (double)GetProperty(PropertyNames.Position);
            set
            {
                SetProperty(PropertyNames.Position, value);
            }
        }

        public virtual string PositionAsString
        {
            get =>
                (string)GetProperty(PropertyNames.PositionAsString);
            set
            {
                SetProperty(PropertyNames.PositionAsString, value);
            }
        }

        public virtual double SmallChange =>
            (double)GetProperty(PropertyNames.SmallChange);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string LargeChange = "LargeChange";
            public static readonly string MaximumPosition = "MaximumPosition";
            public static readonly string MinimumPosition = "MinimumPosition";
            public static readonly string Position = "Position";
            public static readonly string PositionAsString = "PositionAsString";
            public static readonly string SmallChange = "SmallChange";
        }
    }
}

