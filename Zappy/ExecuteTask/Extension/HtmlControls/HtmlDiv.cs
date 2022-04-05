using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlDiv : HtmlControl
    {
        public HtmlDiv() : this(null)
        {
        }

        public HtmlDiv(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Pane.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "DIV");
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

