using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlButton : HtmlControl
    {
        public HtmlButton() : this(null)
        {
        }

        public HtmlButton(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Button.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "BUTTON");
        }

        public virtual string DisplayText =>
            (string)GetProperty(PropertyNames.DisplayText);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
        }
    }
}

