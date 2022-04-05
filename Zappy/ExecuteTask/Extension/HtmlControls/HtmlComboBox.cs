using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlComboBox : HtmlControl
    {
        public HtmlComboBox() : this(null)
        {
        }

        public HtmlComboBox(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ComboBox.Name);
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

        public virtual int ItemCount =>
            (int)GetProperty(PropertyNames.ItemCount);

        public virtual ZappyTaskControlCollection Items =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Items);

        public virtual string LabeledBy =>
            (string)GetProperty(PropertyNames.LabeledBy);

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

        public virtual int Size =>
            (int)GetProperty(PropertyNames.Size);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string ItemCount = "ItemCount";
            public static readonly string Items = "Items";
            public static readonly string LabeledBy = "LabeledBy";
            public static readonly string SelectedIndex = "SelectedIndex";
            public static readonly string SelectedItem = "SelectedItem";
            public static readonly string Size = "Size";
        }
    }
}

