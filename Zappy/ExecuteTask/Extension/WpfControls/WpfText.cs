using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfText : WpfControl
    {
        public WpfText() : this(null)
        {
        }

        public WpfText(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Text.Name);
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
        }
    }
}

