using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlHyperlink : HtmlControl
    {
        public HtmlHyperlink() : this(null)
        {
        }

        public HtmlHyperlink(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Hyperlink.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "A");
        }

        public virtual string AbsolutePath =>
            (string)GetProperty(PropertyNames.AbsolutePath);

        public virtual string Alt =>
            (string)GetProperty(PropertyNames.Alt);

        public virtual string Href =>
            (string)GetProperty(PropertyNames.Href);

        public virtual string Target =>
            (string)GetProperty(PropertyNames.Target);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string AbsolutePath = "AbsolutePath";
            public static readonly string Alt = "Alt";
            public static readonly string Href = "Href";
            public static readonly string Target = "Target";
        }
    }
}

