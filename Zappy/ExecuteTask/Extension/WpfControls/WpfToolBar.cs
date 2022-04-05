using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfToolBar : WpfControl
    {
        public WpfToolBar() : this(null)
        {
        }

        public WpfToolBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ToolBar.Name);
        }

        public virtual string Header =>
            (string)GetProperty(PropertyNames.Header);

        public virtual ZappyTaskControlCollection Items =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Items);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Header = "Header";
            public static readonly string Items = "Items";
        }
    }
}

