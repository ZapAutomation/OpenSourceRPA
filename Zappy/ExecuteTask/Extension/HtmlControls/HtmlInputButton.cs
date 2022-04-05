using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlInputButton : HtmlControl
    {
        public HtmlInputButton() : this(null)
        {
        }

        public HtmlInputButton(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Button.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "INPUT");
        }

        public virtual string DisplayText =>
            (string)GetProperty(HtmlButton.PropertyNames.DisplayText);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
        }
    }
}

