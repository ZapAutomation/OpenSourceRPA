using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlFileInput : HtmlControl
    {
        public HtmlFileInput() : this(null)
        {
        }

        public HtmlFileInput(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.FileInput.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "INPUT");
        }

        public virtual string FileName
        {
            get =>
                (string)GetProperty(PropertyNames.FileName);
            set
            {
                SetProperty(PropertyNames.FileName, value);
            }
        }

        public virtual string LabeledBy =>
            (string)GetProperty(PropertyNames.LabeledBy);

        public virtual bool ReadOnly =>
            (bool)GetProperty(PropertyNames.ReadOnly);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string FileName = "FileName";
            public static readonly string LabeledBy = "LabeledBy";
            public static readonly string ReadOnly = "ReadOnly";
        }
    }
}

