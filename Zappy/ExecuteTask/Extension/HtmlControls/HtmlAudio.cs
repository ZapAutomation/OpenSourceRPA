using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlAudio : HtmlMedia
    {
        public HtmlAudio() : this(null)
        {
        }

        public HtmlAudio(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Audio.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "AUDIO");
        }
    }
}

