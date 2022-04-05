using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfImage : WpfControl
    {
        public WpfImage() : this(null)
        {
        }

        public WpfImage(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Image.Name);
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

