using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinProgressBar : WinControl
    {
        public WinProgressBar() : this(null)
        {
        }

        public WinProgressBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ProgressBar.Name);
        }

        public virtual int MaximumValue =>
            (int)GetProperty(PropertyNames.MaximumValue);

        public virtual int MinimumValue =>
            (int)GetProperty(PropertyNames.MinimumValue);

        public virtual int Value =>
            (int)GetProperty(PropertyNames.Value);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string MaximumValue = "MaximumValue";
            public static readonly string MinimumValue = "MinimumValue";
            public static readonly string Value = "Value";
        }
    }
}

