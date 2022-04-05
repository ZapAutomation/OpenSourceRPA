using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfList : WpfControl
    {
        public WpfList() : this(null)
        {
        }

        public WpfList(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.List.Name);
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

        public virtual bool IsMultipleSelection =>
            (bool)GetProperty(PropertyNames.IsMultipleSelection);

        public virtual ZappyTaskControlCollection Items =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Items);


        public virtual int[] SelectedIndices
        {
            get =>
                (int[])GetProperty(PropertyNames.SelectedIndices);
            set
            {
                SetProperty(PropertyNames.SelectedIndices, value);
            }
        }


        public virtual string[] SelectedItems
        {
            get =>
                (string[])GetProperty(PropertyNames.SelectedItems);
            set
            {
                SetProperty(PropertyNames.SelectedItems, value);
            }
        }

        public virtual string SelectedItemsAsString
        {
            get =>
                (string)GetProperty(PropertyNames.SelectedItemsAsString);
            set
            {
                SetProperty(PropertyNames.SelectedItemsAsString, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string IsMultipleSelection = "IsMultipleSelection";
            public static readonly string Items = "Items";
            public static readonly string SelectedIndices = "SelectedIndices";
            public static readonly string SelectedItems = "SelectedItems";
            public static readonly string SelectedItemsAsString = "SelectedItemsAsString";
        }
    }
}

