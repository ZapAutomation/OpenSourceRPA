using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfButton : WpfControl
    {
        public WpfButton() : this(null)
        {
        }

        public WpfButton(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Button.Name);
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        public virtual string Shortcut =>
            (string)GetProperty(PropertyNames.Shortcut);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
            public static readonly string Shortcut = "Shortcut";
        }
    }
}

