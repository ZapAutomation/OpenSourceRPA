using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinTreeItem : WinControl
    {
        public WinTreeItem() : this(null)
        {
        }

        public WinTreeItem(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.TreeItem.Name);
            SearchConfigurations.Remove(SearchConfiguration.VisibleOnly);
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

        public virtual ZappyTaskControlCollection Nodes =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Nodes);

        public virtual ZappyTaskControl ParentNode =>
            (ZappyTaskControl)GetProperty(PropertyNames.ParentNode);

        public virtual bool Selected
        {
            get =>
                (bool)GetProperty(PropertyNames.Selected);
            set
            {
                SetProperty(PropertyNames.Selected, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string Expanded = "Expanded";
            public static readonly string HasChildNodes = "HasChildNodes";
            public static readonly string Nodes = "Nodes";
            public static readonly string ParentNode = "ParentNode";
            public static readonly string Selected = "Selected";
        }
    }
}

