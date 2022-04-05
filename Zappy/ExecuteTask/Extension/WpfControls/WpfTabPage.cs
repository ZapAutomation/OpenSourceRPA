using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfTabPage : WpfControl
    {
        public WpfTabPage() : this(null)
        {
        }

        public WpfTabPage(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.TabPage.Name);
        }

        public virtual string Header =>
            (string)GetProperty(PropertyNames.Header);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Header = "Header";
        }
    }
}

