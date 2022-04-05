using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinHyperlink : WinControl
    {
        public WinHyperlink() : this(null)
        {
        }

        public WinHyperlink(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Hyperlink.Name);
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
        }
    }
}

