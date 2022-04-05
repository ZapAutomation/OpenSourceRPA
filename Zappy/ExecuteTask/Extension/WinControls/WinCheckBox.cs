using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinCheckBox : WinControl
    {
        public WinCheckBox() : this(null)
        {
        }

        public WinCheckBox(ZappyTaskControl parent) : base(parent)
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
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string Checked = "Checked";
            public static readonly string Indeterminate = "Indeterminate";
        }
    }
}

