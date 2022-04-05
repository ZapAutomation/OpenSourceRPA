using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinComboBox : WinControl
    {
        public WinComboBox() : this(null)
        {
        }

        public WinComboBox(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ComboBox.Name);
        }

        public string[] GetContent()
        {
            ZappyTaskControlCollection items = Items;
            if (items != null)
            {
                return items.GetNamesOfControls();
            }
            return null;
        }

        public virtual string EditableItem
        {
            get =>
                (string)GetProperty(PropertyNames.EditableItem);
            set
            {
                SetProperty(PropertyNames.EditableItem, value);
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

        public virtual ZappyTaskControl HorizontalScrollBar =>
            (ZappyTaskControl)GetProperty(PropertyNames.HorizontalScrollBar);

        public virtual bool IsEditable =>
            (bool)GetProperty(PropertyNames.IsEditable);

        public virtual ZappyTaskControlCollection Items =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Items);

        public virtual int SelectedIndex
        {
            get =>
                (int)GetProperty(PropertyNames.SelectedIndex);
            set
            {
                SetProperty(PropertyNames.SelectedIndex, value);
            }
        }

        public virtual string SelectedItem
        {
            get =>
                (string)GetProperty(PropertyNames.SelectedItem);
            set
            {
                SetProperty(PropertyNames.SelectedItem, value);
            }
        }

        public virtual ZappyTaskControl VerticalScrollBar =>
            (ZappyTaskControl)GetProperty(PropertyNames.VerticalScrollBar);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string EditableItem = "EditableItem";
            public static readonly string Expanded = "Expanded";
            public static readonly string HorizontalScrollBar = "HorizontalScrollBar";
            public static readonly string IsEditable = "IsEditable";
            public static readonly string Items = "Items";
            public static readonly string SelectedIndex = "SelectedIndex";
            public static readonly string SelectedItem = "SelectedItem";
            public static readonly string VerticalScrollBar = "VerticalScrollBar";
        }
    }
}

