using System;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinCalendar : WinControl
    {
        public WinCalendar() : this(null)
        {
        }

        public WinCalendar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Calendar.Name);
        }

        public virtual SelectionRange SelectionRange
        {
            get =>
                (SelectionRange)GetProperty(PropertyNames.SelectionRange);
            set
            {
                SetProperty(PropertyNames.SelectionRange, value);
            }
        }

        public virtual string SelectionRangeAsString
        {
            get =>
                (string)GetProperty(PropertyNames.SelectionRangeAsString);
            set
            {
                SetProperty(PropertyNames.SelectionRangeAsString, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string SelectionRange = "SelectionRange";
            public static readonly string SelectionRangeAsString = "SelectionRangeAsString";
        }
    }
}

