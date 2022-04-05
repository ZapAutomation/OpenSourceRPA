using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinButton : WinControl
    {
        public WinButton() : this(null)
        {
        }

        public WinButton(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Button.Name);
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        public virtual string Shortcut =>
            (string)GetProperty(PropertyNames.Shortcut);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
            public static readonly string Shortcut = "Shortcut";
        }
    }
}

