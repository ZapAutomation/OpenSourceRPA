using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinSlider : WinControl
    {
        public WinSlider() : this(null)
        {
        }

        public WinSlider(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Slider.Name);
        }

        public virtual int LineSize =>
            (int)GetProperty(PropertyNames.LineSize);

        public virtual int MaximumPosition =>
            (int)GetProperty(PropertyNames.MaximumPosition);

        public virtual int MinimumPosition =>
            (int)GetProperty(PropertyNames.MinimumPosition);

        public virtual int PageSize =>
            (int)GetProperty(PropertyNames.PageSize);

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

        public virtual int TickCount =>
            (int)GetProperty(PropertyNames.TickCount);

        public virtual int TickPosition =>
            (int)GetProperty(PropertyNames.TickPosition);

        public virtual int TickValue =>
            (int)GetProperty(PropertyNames.TickValue);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string LineSize = "LineSize";
            public static readonly string MaximumPosition = "MaximumPosition";
            public static readonly string MinimumPosition = "MinimumPosition";
            public static readonly string PageSize = "PageSize";
            public static readonly string Position = "Position";
            public static readonly string PositionAsString = "PositionAsString";
            public static readonly string TickCount = "TickCount";
            public static readonly string TickPosition = "TickPosition";
            public static readonly string TickValue = "TickValue";
        }
    }
}

