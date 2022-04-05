using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinEdit : WinControl
    {
        public WinEdit() : this(null)
        {
        }

        public WinEdit(ZappyTaskControl parent) : base(parent)
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

        public virtual int CurrentLine =>
            (int)GetProperty(PropertyNames.CurrentLine);

        public virtual int InsertionIndexAbsolute
        {
            get =>
                (int)GetProperty(PropertyNames.InsertionIndexAbsolute);
            set
            {
                SetProperty(PropertyNames.InsertionIndexAbsolute, value);
            }
        }

        public virtual int InsertionIndexLineRelative =>
            (int)GetProperty(PropertyNames.InsertionIndexLineRelative);

        public virtual bool IsPassword =>
            (bool)GetProperty(PropertyNames.IsPassword);

        public virtual int LineCount =>
            (int)GetProperty(PropertyNames.LineCount);

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

        public virtual int SelectionEnd
        {
            get =>
                (int)GetProperty(PropertyNames.SelectionEnd);
            set
            {
                SetProperty(PropertyNames.SelectionEnd, value);
            }
        }

        public virtual int SelectionStart
        {
            get =>
                (int)GetProperty(PropertyNames.SelectionStart);
            set
            {
                SetProperty(PropertyNames.SelectionStart, value);
            }
        }

        public virtual string SelectionText
        {
            get =>
                (string)GetProperty(PropertyNames.SelectionText);
            set
            {
                SetProperty(PropertyNames.SelectionText, value);
            }
        }

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
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string CopyPastedText = "CopyPastedText";
            public static readonly string CurrentLine = "CurrentLine";
            public static readonly string InsertionIndexAbsolute = "InsertionIndexAbsolute";
            public static readonly string InsertionIndexLineRelative = "InsertionIndexLineRelative";
            public static readonly string IsPassword = "IsPassword";
            public static readonly string LineCount = "LineCount";
            public static readonly string MaxLength = "MaxLength";
            public static readonly string Password = "Password";
            public static readonly string ReadOnly = "ReadOnly";
            public static readonly string SelectionEnd = "SelectionEnd";
            public static readonly string SelectionStart = "SelectionStart";
            public static readonly string SelectionText = "SelectionText";
            public static readonly string Text = "Text";
        }
    }
}

