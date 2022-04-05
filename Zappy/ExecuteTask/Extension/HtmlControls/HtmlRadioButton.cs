using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlRadioButton : HtmlControl
    {
        public HtmlRadioButton() : this(null)
        {
        }

        public HtmlRadioButton(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.RadioButton.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "INPUT");
        }

        public virtual ZappyTaskControlCollection Group =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Group);

        public virtual int ItemCount =>
            (int)GetProperty(PropertyNames.ItemCount);

        public virtual string LabeledBy =>
            (string)GetProperty(PropertyNames.LabeledBy);

        public virtual bool Selected
        {
            get =>
                (bool)GetProperty(PropertyNames.Selected);
            set
            {
                SetProperty(PropertyNames.Selected, value);
            }
        }

        public virtual string Value =>
            (string)GetProperty(PropertyNames.Value);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string Group = "Group";
            public static readonly string ItemCount = "ItemCount";
            public static readonly string LabeledBy = "LabeledBy";
            public static readonly string Selected = "Selected";
            public static readonly string Value = "Value";
        }
    }
}

