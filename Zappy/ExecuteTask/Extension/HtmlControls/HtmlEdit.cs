using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlEdit : HtmlControl
    {
        public HtmlEdit() : this(null)
        {
        }

        public HtmlEdit(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Edit.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "INPUT");
        }

        public virtual string CopyPastedText
        {
            get =>
                (string)GetProperty(PropertyNames.CopyPastedText);
            set
            {
                SetProperty(PropertyNames.CopyPastedText, value);
            }
        }

        public virtual string DefaultText =>
            (string)GetProperty(PropertyNames.DefaultText);

        public virtual bool IsPassword =>
            (bool)GetProperty(PropertyNames.IsPassword);

        public virtual string LabeledBy =>
            (string)GetProperty(PropertyNames.LabeledBy);

        public virtual int MaxLength =>
            (int)GetProperty(PropertyNames.MaxLength);


        public virtual string Password
        {
            set
            {
                SetProperty(PropertyNames.Password, value);
            }
        }


        public virtual bool ReadOnly =>
            (bool)GetProperty(PropertyNames.ReadOnly);

        public virtual string Text
        {
            get =>
                (string)GetProperty(PropertyNames.Text);
            set
            {
                SetProperty(PropertyNames.Text, value);
            }
        }

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string CopyPastedText = "CopyPastedText";
            public static readonly string DefaultText = "DefaultText";
            public static readonly string IsPassword = "IsPassword";
            public static readonly string LabeledBy = "LabeledBy";
            public static readonly string MaxLength = "MaxLength";
            public static readonly string Password = "Password";
            public static readonly string ReadOnly = "ReadOnly";
            public static readonly string Text = "Text";
        }
    }
}

