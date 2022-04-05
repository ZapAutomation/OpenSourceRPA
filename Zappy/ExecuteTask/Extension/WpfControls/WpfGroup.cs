using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfGroup : WpfControl
    {
        public WpfGroup() : this(null)
        {
        }

        public WpfGroup(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Group.Name);
        }

        public virtual string Header =>
            (string)GetProperty(PropertyNames.Header);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Header = "Header";
        }
    }
}

