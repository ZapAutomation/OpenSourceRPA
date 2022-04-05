using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfCheckBox : WpfControl
    {
        public WpfCheckBox() : this(null)
        {
        }

        public WpfCheckBox(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.CheckBox.Name);
        }

        public virtual bool Checked
        {
            get =>
                (bool)GetProperty(PropertyNames.Checked);
            set
            {
                SetProperty(PropertyNames.Checked, value);
            }
        }

        public virtual bool Indeterminate
        {
            get =>
                (bool)GetProperty(PropertyNames.Indeterminate);
            set
            {
                SetProperty(PropertyNames.Indeterminate, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Checked = "Checked";
            public static readonly string Indeterminate = "Indeterminate";
        }
    }
}

