using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinCheckBoxTreeItem : WinControl
    {
        public WinCheckBoxTreeItem() : this(null)
        {
        }

        public WinCheckBoxTreeItem(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.CheckBoxTreeItem.Name);
            SearchConfigurations.Remove(SearchConfiguration.VisibleOnly);
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
                (bool)GetProperty(WinTreeItem.PropertyNames.Expanded);
            set
            {
                SetProperty(WinTreeItem.PropertyNames.Expanded, value);
            }
        }

        public virtual bool HasChildNodes =>
            (bool)GetProperty(WinTreeItem.PropertyNames.HasChildNodes);

        public virtual bool Indeterminate
        {
            get =>
                (bool)GetProperty(PropertyNames.Indeterminate);
            set
            {
                SetProperty(PropertyNames.Indeterminate, value);
            }
        }

        public virtual ZappyTaskControlCollection Nodes =>
            (ZappyTaskControlCollection)GetProperty(WinTreeItem.PropertyNames.Nodes);

        public virtual ZappyTaskControl ParentNode =>
            (ZappyTaskControl)GetProperty(WinTreeItem.PropertyNames.ParentNode);

        public virtual bool Selected
        {
            get =>
                (bool)GetProperty(WinTreeItem.PropertyNames.Selected);
            set
            {
                SetProperty(WinTreeItem.PropertyNames.Selected, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinTreeItem.PropertyNames
        {
            public static readonly string Checked = "Checked";
            public static readonly string Indeterminate = "Indeterminate";
        }
    }
}

