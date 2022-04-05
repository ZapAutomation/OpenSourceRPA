using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlList : HtmlControl
    {
        public HtmlList() : this(null)
        {
        }

        public HtmlList(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.List.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "SELECT");
        }

        public string[] GetContent()
        {
            ZappyTaskControlCollection items = Items;
            if (items != null)
            {
                return items.GetPropertyValuesOfControls<string>(HtmlControl.PropertyNames.InnerText);
            }
            return null;
        }

        public virtual bool IsMultipleSelection =>
            (bool)GetProperty(PropertyNames.IsMultipleSelection);

        public virtual int ItemCount =>
            (int)GetProperty(PropertyNames.ItemCount);

        public virtual ZappyTaskControlCollection Items =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Items);

        public virtual string LabeledBy =>
            (string)GetProperty(PropertyNames.LabeledBy);

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

        public virtual int Size =>
            (int)GetProperty(PropertyNames.Size);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string IsMultipleSelection = "IsMultipleSelection";
            public static readonly string ItemCount = "ItemCount";
            public static readonly string Items = "Items";
            public static readonly string LabeledBy = "LabeledBy";
            public static readonly string SelectedIndices = "SelectedIndices";
            public static readonly string SelectedItems = "SelectedItems";
            public static readonly string SelectedItemsAsString = "SelectedItemsAsString";
            public static readonly string Size = "Size";
        }
    }
}

