using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlProgressBar : HtmlControl
    {
        public HtmlProgressBar() : this(null)
        {
        }

        public HtmlProgressBar(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.ProgressBar.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "PROGRESS");
        }

        public virtual float Max =>
            (float)GetProperty(PropertyNames.Max);

        public float Value =>
            (float)GetProperty(PropertyNames.Value);

        public override string ValueAttribute
        {
            get
            {
                object property = GetProperty(HtmlControl.PropertyNames.ValueAttribute);
                if (property != null)
                {
                    return property.ToString();
                }
                return string.Empty;
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string Max = "Max";
            public static readonly string Value = "Value";
        }
    }
}

