using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfTitleBar : WpfControl
    {
        public WpfTitleBar() : this(null)
        {
        }

        public WpfTitleBar(ZappyTaskControl parent) : base(parent, true)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.TitleBar.Name);
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

