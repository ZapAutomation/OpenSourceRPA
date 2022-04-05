using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinMenuItem : WinControl
    {
        public WinMenuItem() : this(null)
        {
        }

        public WinMenuItem(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.MenuItem.Name);
            SearchConfigurations.Remove(SearchConfiguration.VisibleOnly);
        }

        public virtual string AcceleratorKey =>
            (string)GetProperty(PropertyNames.AcceleratorKey);

        public virtual bool Checked
        {
            get =>
                (bool)GetProperty(PropertyNames.Checked);
            set
            {
                SetProperty(PropertyNames.Checked, value);
            }
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        public virtual bool HasChildNodes =>
            (bool)GetProperty(PropertyNames.HasChildNodes);

        public virtual bool IsTopLevelMenu =>
            (bool)GetProperty(PropertyNames.IsTopLevelMenu);

        public virtual ZappyTaskControlCollection Items =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Items);

        public virtual string Shortcut =>
            (string)GetProperty(PropertyNames.Shortcut);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string AcceleratorKey = "AcceleratorKey";
            public static readonly string Checked = "Checked";
            public static readonly string DisplayText = "DisplayText";
            public static readonly string HasChildNodes = "HasChildNodes";
            public static readonly string IsTopLevelMenu = "IsTopLevelMenu";
            public static readonly string Items = "Items";
            public static readonly string Shortcut = "Shortcut";
        }
    }
}

