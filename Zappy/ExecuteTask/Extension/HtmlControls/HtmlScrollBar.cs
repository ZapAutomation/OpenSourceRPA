using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlScrollBar : HtmlControl
    {
        public HtmlScrollBar() : this(null)
        {
        }

        public HtmlScrollBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ScrollBar.Name);
        }

        public virtual string Orientation =>
            (string)GetProperty(PropertyNames.Orientation);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string Orientation = "Orientation";
        }
    }
}

