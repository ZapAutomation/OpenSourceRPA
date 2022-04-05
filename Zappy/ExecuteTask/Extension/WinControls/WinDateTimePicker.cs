using System;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinDateTimePicker : WinControl
    {
        public WinDateTimePicker() : this(null)
        {
        }

        public WinDateTimePicker(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.DateTimePicker.Name);
        }

        public virtual ZappyTaskControl Calendar =>
            (ZappyTaskControl)GetProperty(PropertyNames.Calendar);

        public virtual bool Checked
        {
            get =>
                (bool)GetProperty(PropertyNames.Checked);
            set
            {
                SetProperty(PropertyNames.Checked, value);
            }
        }

        public virtual DateTime DateTime
        {
            get =>
                (DateTime)GetProperty(PropertyNames.DateTime);
            set
            {
                SetProperty(PropertyNames.DateTime, value);
            }
        }

        public virtual string DateTimeAsString
        {
            get =>
                (string)GetProperty(PropertyNames.DateTimeAsString);
            set
            {
                SetProperty(PropertyNames.DateTimeAsString, value);
            }
        }

        public virtual DateTimePickerFormat Format =>
            (DateTimePickerFormat)GetProperty(PropertyNames.Format);

        public virtual bool HasCheckBox =>
            (bool)GetProperty(PropertyNames.HasCheckBox);

        public virtual bool HasDropDownButton =>
            (bool)GetProperty(PropertyNames.HasDropDownButton);

        public virtual bool HasSpinner =>
            (bool)GetProperty(PropertyNames.HasSpinner);

        public virtual bool ShowCalendar
        {
            get =>
                (bool)GetProperty(PropertyNames.ShowCalendar);
            set
            {
                SetProperty(PropertyNames.ShowCalendar, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string Calendar = "Calendar";
            public static readonly string Checked = "Checked";
            public static readonly string DateTime = "DateTime";
            public static readonly string DateTimeAsString = "DateTimeAsString";
            public static readonly string Format = "Format";
            public static readonly string HasCheckBox = "HasCheckBox";
            public static readonly string HasDropDownButton = "HasDropDownButton";
            public static readonly string HasSpinner = "HasSpinner";
            public static readonly string ShowCalendar = "ShowCalendar";
        }
    }
}

