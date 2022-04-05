using System;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfDatePicker : WpfControl
    {
        public WpfDatePicker() : this(null)
        {
        }

        public WpfDatePicker(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add("ControlType", "DatePicker");
        }

        public virtual ZappyTaskControl Calendar =>
            (ZappyTaskControl)GetProperty(PropertyNames.Calendar);

        public virtual DateTime Date
        {
            get =>
                (DateTime)GetProperty(PropertyNames.Date);
            set
            {
                SetProperty(PropertyNames.Date, value);
            }
        }

        public virtual string DateAsString
        {
            get =>
                (string)GetProperty(PropertyNames.DateAsString);
            set
            {
                SetProperty(PropertyNames.DateAsString, value);
            }
        }

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
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Calendar = "Calendar";
            public static readonly string Date = "Date";
            public static readonly string DateAsString = "DateAsString";
            public static readonly string ShowCalendar = "ShowCalendar";
        }
    }
}

