using System;
using System.Windows.Forms;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfCalendar : WpfControl
    {
        public WpfCalendar() : this(null)
        {
        }

        public WpfCalendar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add("ControlType", "Calendar");
        }

        public virtual bool MultiSelectable =>
            (bool)GetProperty(PropertyNames.MultiSelectable);

        public virtual SelectionRange SelectedDateRange
        {
            set
            {
                SetProperty(PropertyNames.SelectedDateRange, value);
            }
        }

        public virtual string SelectedDateRangeAsString
        {
            set
            {
                SetProperty(PropertyNames.SelectedDateRangeAsString, value);
            }
        }

        public virtual DateTime[] SelectedDates
        {
            get =>
                (DateTime[])GetProperty(PropertyNames.SelectedDates);
            set
            {
                SetProperty(PropertyNames.SelectedDates, value);
            }
        }

        public virtual string SelectedDatesAsString
        {
            get =>
                (string)GetProperty(PropertyNames.SelectedDatesAsString);
            set
            {
                SetProperty(PropertyNames.SelectedDatesAsString, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {

            public static readonly string MultiSelectable = "MultiSelectable";
            public static readonly string SelectedDateRange = "SelectedDateRange";
            public static readonly string SelectedDateRangeAsString = "SelectedDateRangeAsString";
            public static readonly string SelectedDates = "SelectedDates";
            public static readonly string SelectedDatesAsString = "SelectedDatesAsString";
        }
    }
}

