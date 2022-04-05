using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlDocument : HtmlControl
    {
        public HtmlDocument() : this(null)
        {
        }

        public HtmlDocument(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Document.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "BODY");
        }

        public virtual string AbsolutePath =>
            (string)GetProperty(PropertyNames.AbsolutePath);

        public virtual bool FrameDocument =>
            (bool)GetProperty(PropertyNames.FrameDocument);


        public virtual string PageUrl =>
            (string)GetProperty(PropertyNames.PageUrl);

        public virtual bool RedirectingPage =>
            (bool)GetProperty(PropertyNames.RedirectingPage);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string AbsolutePath = "AbsolutePath";
            public static readonly string FrameDocument = "FrameDocument";
            public static readonly string PageUrl = "PageUrl";
            public static readonly string RedirectingPage = "RedirectingPage";
        }
    }
}

