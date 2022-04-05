using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlSlider : HtmlControl
    {
        public HtmlSlider() : this(null)
        {
        }

        public HtmlSlider(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Slider.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "INPUT");
        }

        public bool Disabled =>
            (bool)GetProperty(PropertyNames.Disabled);

        public virtual string Max =>
            (string)GetProperty(PropertyNames.Max);

        public virtual string Min =>
            (string)GetProperty(PropertyNames.Min);

        public bool Required =>
            (bool)GetProperty(PropertyNames.Required);

        public virtual string Step =>
            (string)GetProperty(PropertyNames.Step);

        public string Value
        {
            get =>
                (string)GetProperty(PropertyNames.Value);
            set
            {
                SetProperty(PropertyNames.Value, value);
            }
        }

        public virtual double ValueAsNumber
        {
            get =>
                (double)GetProperty(PropertyNames.ValueAsNumber);
            set
            {
                SetProperty(PropertyNames.ValueAsNumber, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string Disabled = "Disabled";
            public static readonly string Max = "Max";
            public static readonly string Min = "Min";
            public static readonly string Required = "Required";
            public static readonly string Step = "Step";
            public static readonly string Value = "Value";
            public static readonly string ValueAsNumber = "ValueAsNumber";
        }
    }
}

