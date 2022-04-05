using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlLabel : HtmlControl
    {
        public HtmlLabel() : this(null)
        {
        }

        public HtmlLabel(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Label.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "LABEL");
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        public virtual string LabelFor =>
            (string)GetProperty(PropertyNames.LabelFor);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
            public static readonly string LabelFor = "LabelFor";
        }
    }
}

