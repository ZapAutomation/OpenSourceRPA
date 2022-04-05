using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlListItem : HtmlControl
    {
        public HtmlListItem() : this(null)
        {
        }

        public HtmlListItem(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ListItem.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "OPTION");
        }

        public void Select()
        {
            ZappyTaskControl parent = GetParent();
            if (parent is HtmlCustom)
            {
                parent = parent.GetParent();
            }
            if (parent is HtmlComboBox)
            {
                HtmlComboBox box = parent as HtmlComboBox;
                box.SelectedItem = DisplayText;
            }
            else
            {
                if (!(parent is HtmlList))
                {
                    throw new InvalidOperationException(Resources.InvalidListItemOperation);
                }
                HtmlList list = parent as HtmlList;
                list.SelectedItems = new[] { DisplayText };
            }
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        public virtual bool Selected =>
            (bool)GetProperty(PropertyNames.Selected);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
            public static readonly string Selected = "Selected";
        }
    }
}

