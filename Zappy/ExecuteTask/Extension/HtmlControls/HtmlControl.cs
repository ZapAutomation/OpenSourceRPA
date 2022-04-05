using System;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlControl : ZappyTaskControl
    {
        public HtmlControl() : this(null)
        {
        }

        public HtmlControl(ZappyTaskControl parent) : base(parent)
        {
            TechnologyName = "Web";
        }

        public virtual string AccessKey =>
            (string)GetProperty(PropertyNames.AccessKey);


        public virtual string Class =>
            (string)GetProperty(PropertyNames.Class);

        public virtual string ControlDefinition =>
            (string)GetProperty(PropertyNames.ControlDefinition);

        public virtual string HelpText =>
            (string)GetProperty(PropertyNames.HelpText);

        public virtual string Id =>
            (string)GetProperty(PropertyNames.Id);

        public virtual string InnerText =>
            (string)GetProperty(PropertyNames.InnerText);

        public virtual int TagInstance =>
            (int)GetProperty(PropertyNames.TagInstance);

        public virtual string TagName =>
            (string)GetProperty(PropertyNames.TagName);

        public virtual string Title =>
            (string)GetProperty(PropertyNames.Title);


        public virtual string Type =>
            (string)GetProperty(PropertyNames.Type);

        public virtual string ValueAttribute =>
            (string)GetProperty(PropertyNames.ValueAttribute);

        [CLSCompliant(true)]
        public abstract class PropertyNames : ZappyTaskControl.PropertyNames
        {
            public static readonly string AccessKey = "AccessKey";
            public static readonly string Class = "Class";
            public static readonly string ControlDefinition = "ControlDefinition";
            public static readonly string HelpText = "HelpText";
            public static readonly string Id = "Id";
            public static readonly string InnerText = "InnerText";
            public static readonly string TagInstance = "TagInstance";
            public static readonly string TagName = "TagName";
            public static readonly string Title = "Title";
            public static readonly string Type = "Type";
            public static readonly string ValueAttribute = "ValueAttribute";
        }
    }
}

