using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinTabList : WinControl
    {
        public WinTabList() : this(null)
        {
        }

        public WinTabList(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.TabList.Name);
        }

        public virtual int SelectedIndex
        {
            get =>
                (int)GetProperty(PropertyNames.SelectedIndex);
            set
            {
                SetProperty(PropertyNames.SelectedIndex, value);
            }
        }

        public virtual ZappyTaskControlCollection Tabs =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Tabs);

        public virtual ZappyTaskControl TabSpinner =>
            (ZappyTaskControl)GetProperty(PropertyNames.TabSpinner);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string SelectedIndex = "SelectedIndex";
            public static readonly string Tabs = "Tabs";
            public static readonly string TabSpinner = "TabSpinner";
        }
    }
}

