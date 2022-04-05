using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinSpinner : WinControl
    {
        public WinSpinner() : this(null)
        {
        }

        public WinSpinner(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Spinner.Name);
        }

        public virtual int MaximumValue =>
            (int)GetProperty(PropertyNames.MaximumValue);

        public virtual int MinimumValue =>
            (int)GetProperty(PropertyNames.MinimumValue);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string MaximumValue = "MaximumValue";
            public static readonly string MinimumValue = "MinimumValue";
        }
    }
}

