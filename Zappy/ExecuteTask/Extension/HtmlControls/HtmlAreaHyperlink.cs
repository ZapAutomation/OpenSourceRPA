using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlAreaHyperlink : HtmlControl
    {
        public HtmlAreaHyperlink() : this(null)
        {
        }

        public HtmlAreaHyperlink(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Hyperlink.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "AREA");
        }

        public virtual string AbsolutePath =>
            (string)GetProperty(HtmlHyperlink.PropertyNames.AbsolutePath);

        public virtual string Alt =>
            (string)GetProperty(HtmlHyperlink.PropertyNames.Alt);

        public virtual string Href =>
            (string)GetProperty(HtmlHyperlink.PropertyNames.Href);

        public virtual string Target =>
            (string)GetProperty(HtmlHyperlink.PropertyNames.Target);

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

