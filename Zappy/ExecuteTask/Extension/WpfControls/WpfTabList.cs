using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfTabList : WpfControl
    {
        public WpfTabList() : this(null)
        {
        }

        public WpfTabList(ZappyTaskControl parent) : base(parent)
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

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string SelectedIndex = "SelectedIndex";
            public static readonly string Tabs = "Tabs";
        }
    }
}

