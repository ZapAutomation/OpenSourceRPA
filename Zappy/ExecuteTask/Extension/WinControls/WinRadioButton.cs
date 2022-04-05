using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinRadioButton : WinControl
    {
        public WinRadioButton() : this(null)
        {
        }

        public WinRadioButton(ZappyTaskControl parent) : base(parent)
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
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string Group = "Group";
            public static readonly string Selected = "Selected";
        }
    }
}

