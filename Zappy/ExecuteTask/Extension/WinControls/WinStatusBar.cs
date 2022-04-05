using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinStatusBar : WinControl
    {
        public WinStatusBar() : this(null)
        {
        }

        public WinStatusBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.StatusBar.Name);
        }

        public virtual ZappyTaskControlCollection Panels =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Panels);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string Panels = "Panels";
        }
    }
}

