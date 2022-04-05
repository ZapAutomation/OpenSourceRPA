using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlImage : HtmlControl
    {
        public HtmlImage() : this(null)
        {
        }

        public HtmlImage(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Image.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "IMG");
        }

        public virtual string AbsolutePath =>
            (string)GetProperty(PropertyNames.AbsolutePath);

        public virtual string Alt =>
            (string)GetProperty(PropertyNames.Alt);

        public virtual string Href =>
            (string)GetProperty(PropertyNames.Href);

        public virtual string LinkAbsolutePath =>
            (string)GetProperty(PropertyNames.LinkAbsolutePath);

        public virtual string Src =>
            (string)GetProperty(PropertyNames.Src);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string AbsolutePath = "AbsolutePath";
            public static readonly string Alt = "Alt";
            public static readonly string Href = "Href";
            public static readonly string LinkAbsolutePath = "LinkAbsolutePath";

            public static readonly string Src = "Src";
        }
    }
}

