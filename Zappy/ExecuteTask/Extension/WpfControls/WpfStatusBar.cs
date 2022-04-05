using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfStatusBar : WpfControl
    {
        public WpfStatusBar() : this(null)
        {
        }

        public WpfStatusBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.StatusBar.Name);
        }

        public virtual ZappyTaskControlCollection Panels =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Panels);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Panels = "Panels";
        }
    }
}

