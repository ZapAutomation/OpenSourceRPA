using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfExpander : WpfControl
    {
        public WpfExpander() : this(null)
        {
        }

        public WpfExpander(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Expander.Name);
        }

        public virtual bool Expanded
        {
            get =>
                (bool)GetProperty(PropertyNames.Expanded);
            set
            {
                SetProperty(PropertyNames.Expanded, value);
            }
        }

        public virtual string Header =>
            (string)GetProperty(PropertyNames.Header);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Expanded = "Expanded";
            public static readonly string Header = "Header";
        }
    }
}

