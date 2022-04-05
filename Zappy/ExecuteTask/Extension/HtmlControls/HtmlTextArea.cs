using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlTextArea : HtmlControl
    {
        public HtmlTextArea() : this(null)
        {
        }

        public HtmlTextArea(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Edit.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "TEXTAREA");
        }

        public virtual string CopyPastedText
        {
            get =>
                (string)GetProperty(HtmlEdit.PropertyNames.CopyPastedText);
            set
            {
                SetProperty(HtmlEdit.PropertyNames.CopyPastedText, value);
            }
        }

        public virtual string DefaultText =>
            (string)GetProperty(HtmlEdit.PropertyNames.DefaultText);

        public virtual bool IsPassword =>
            (bool)GetProperty(HtmlEdit.PropertyNames.IsPassword);

        public virtual string LabeledBy =>
            (string)GetProperty(HtmlEdit.PropertyNames.LabeledBy);

        public virtual string Password
        {
            set
            {
                SetProperty(HtmlEdit.PropertyNames.Password, value);
            }
        }

        public virtual bool ReadOnly =>
            (bool)GetProperty(HtmlEdit.PropertyNames.ReadOnly);

        public virtual string Text
        {
            get =>
                (string)GetProperty(HtmlEdit.PropertyNames.Text);
            set
            {
                SetProperty(HtmlEdit.PropertyNames.Text, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string CopyPastedText = "CopyPastedText";
            public static readonly string DefaultText = "DefaultText";
            public static readonly string IsPassword = "IsPassword";
            public static readonly string LabeledBy = "LabeledBy";
            public static readonly string Password = "Password";
            public static readonly string ReadOnly = "ReadOnly";
            public static readonly string Text = "Text";
        }
    }
}

