using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfToggleButton : WpfControl
    {
        public WpfToggleButton() : this(null)
        {
        }

        public WpfToggleButton(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ToggleButton.Name);
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        public virtual bool Indeterminate
        {
            get =>
                (bool)GetProperty(PropertyNames.Indeterminate);
            set
            {
                SetProperty(PropertyNames.Indeterminate, value);
            }
        }

        public virtual bool Pressed
        {
            get =>
                (bool)GetProperty(PropertyNames.Pressed);
            set
            {
                SetProperty(PropertyNames.Pressed, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
            public static readonly string Indeterminate = "Indeterminate";
            public static readonly string Pressed = "Pressed";
        }
    }
}

