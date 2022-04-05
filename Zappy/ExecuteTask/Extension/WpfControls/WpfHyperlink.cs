using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfHyperlink : WpfControl
    {
        public WpfHyperlink() : this(null)
        {
        }

        public WpfHyperlink(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Hyperlink.Name);
        }

        public virtual string Alt =>
            (string)GetProperty(PropertyNames.Alt);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Alt = "Alt";
        }
    }
}

