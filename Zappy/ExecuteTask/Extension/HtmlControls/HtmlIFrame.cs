using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlIFrame : HtmlControl
    {
        public HtmlIFrame() : this(null)
        {
        }

        public HtmlIFrame(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Frame.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "IFRAME");
        }

        public virtual string AbsolutePath =>
            (string)GetProperty(HtmlFrame.PropertyNames.AbsolutePath);

        public virtual string PageUrl =>
            (string)GetProperty(HtmlFrame.PropertyNames.PageUrl);

        public virtual bool Scrollable =>
            (bool)GetProperty(HtmlFrame.PropertyNames.Scrollable);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string AbsolutePath = "AbsolutePath";
            public static readonly string PageUrl = "PageUrl";
            public static readonly string Scrollable = "Scrollable";
        }
    }
}

