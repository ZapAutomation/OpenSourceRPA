using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlFrame : HtmlControl
    {
        public HtmlFrame() : this(null)
        {
        }

        public HtmlFrame(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Frame.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "FRAME");
        }

        public virtual string AbsolutePath =>
            (string)GetProperty(PropertyNames.AbsolutePath);

        public virtual string PageUrl =>
            (string)GetProperty(PropertyNames.PageUrl);

        public virtual bool Scrollable =>
            (bool)GetProperty(PropertyNames.Scrollable);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string AbsolutePath = "AbsolutePath";
            public static readonly string PageUrl = "PageUrl";
            public static readonly string Scrollable = "Scrollable";
        }
    }
}

