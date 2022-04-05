using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfMenuItem : WpfControl
    {
        public WpfMenuItem() : this(null)
        {
        }

        public WpfMenuItem(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.MenuItem.Name);
        }

        public virtual bool Checked
        {
            get =>
                (bool)GetProperty(PropertyNames.Checked);
            set
            {
                SetProperty(PropertyNames.Checked, value);
            }
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

        public virtual bool HasChildNodes =>
            (bool)GetProperty(PropertyNames.HasChildNodes);

        public virtual string Header =>
            (string)GetProperty(PropertyNames.Header);

        public virtual bool IsTopLevelMenu =>
            (bool)GetProperty(PropertyNames.IsTopLevelMenu);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string Checked = "Checked";
            public static readonly string Expanded = "Expanded";
            public static readonly string HasChildNodes = "HasChildNodes";
            public static readonly string Header = "Header";
            public static readonly string IsTopLevelMenu = "IsTopLevelMenu";
        }
    }
}

