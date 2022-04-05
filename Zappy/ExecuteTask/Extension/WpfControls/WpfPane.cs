using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfPane : WpfControl
    {
        public WpfPane() : this(null)
        {
        }

        public WpfPane(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Pane.Name);
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string HorizontalScrollBar = "HorizontalScrollBar";
            public static readonly string VerticalScrollBar = "VerticalScrollBar";
        }
    }
}

