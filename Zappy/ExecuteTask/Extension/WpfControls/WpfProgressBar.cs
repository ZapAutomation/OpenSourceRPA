using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfProgressBar : WpfControl
    {
        public WpfProgressBar() : this(null)
        {
        }

        public WpfProgressBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ProgressBar.Name);
        }

        public virtual double MaximumValue =>
            (double)GetProperty(PropertyNames.MaximumValue);

        public virtual double MinimumValue =>
            (double)GetProperty(PropertyNames.MinimumValue);

        public virtual double Position =>
            (double)GetProperty(PropertyNames.Position);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string MaximumValue = "MaximumValue";
            public static readonly string MinimumValue = "MinimumValue";
            public static readonly string Position = "Position";
        }
    }
}

