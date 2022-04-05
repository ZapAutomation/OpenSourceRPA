using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfEdit : WpfControl
    {
        public WpfEdit() : this(null)
        {
        }

        public WpfEdit(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Edit.Name);
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

        public virtual bool IsPassword =>
            (bool)GetProperty(PropertyNames.IsPassword);

        public virtual string Password
        {
            set
            {
                SetProperty(PropertyNames.Password, value);
            }
        }

        public virtual bool ReadOnly =>
            (bool)GetProperty(PropertyNames.ReadOnly);

        public virtual string SelectionText =>
            (string)GetProperty(PropertyNames.SelectionText);

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
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string CopyPastedText = "CopyPastedText";
            public static readonly string IsPassword = "IsPassword";
            public static readonly string Password = "Password";
            public static readonly string ReadOnly = "ReadOnly";
            public static readonly string SelectionText = "SelectionText";
            public static readonly string Text = "Text";
        }
    }
}

