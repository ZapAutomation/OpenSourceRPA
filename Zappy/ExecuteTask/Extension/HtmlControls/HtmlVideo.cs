using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlVideo : HtmlMedia
    {
        public HtmlVideo() : this(null)
        {
        }

        public HtmlVideo(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Video.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "VIDEO");
        }

        public virtual string Poster =>
            (string)GetProperty(PropertyNames.Poster);

        public virtual int VideoHeight =>
            (int)GetProperty(PropertyNames.VideoHeight);

        public virtual int VideoWidth =>
            (int)GetProperty(PropertyNames.VideoWidth);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlMedia.PropertyNames
        {
            public static readonly string Poster = "Poster";
            public static readonly string VideoHeight = "VideoHeight";
            public static readonly string VideoWidth = "VideoWidth";
        }
    }
}

