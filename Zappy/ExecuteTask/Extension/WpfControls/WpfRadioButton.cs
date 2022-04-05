using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfRadioButton : WpfControl
    {
        public WpfRadioButton() : this(null)
        {
        }

        public WpfRadioButton(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.RadioButton.Name);
        }

        public virtual ZappyTaskControl Group =>
            (ZappyTaskControl)GetProperty(PropertyNames.Group);

        public virtual bool Selected
        {
            get =>
                (bool)GetProperty(PropertyNames.Selected);
            set
            {
                SetProperty(PropertyNames.Selected, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Group = "Group";
            public static readonly string Selected = "Selected";
        }
    }
}

