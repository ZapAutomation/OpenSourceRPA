using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Zappy.Helpers;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    [Description("Sets The Inputs To Box")]
    public class InputBoxAction : TemplateAction
    {
        [Category("Input")]
        [Description("Title for InputBox")]
        public DynamicTextProperty Title { get; set; }

        [Category("Input")]
        [Description("PromptText for InputBox")]
        public DynamicTextProperty PromptText { get; set; }

        [Category("Input")]
        [Description("FieldName for InputBox")]
        public DynamicTextProperty FieldName { get; set; }

        [Category("Optional")]
        public DynamicProperty<bool> ContinueOnCancel { get; set; }

        [Category("Optional")]
        public DynamicProperty<bool> IsPasswordInput { get; set; }

        [Category("Input/Output")]         [Description("Value for InputBox")]
        public string Value { get; set; }
        [Category("Output")]
        public DialogResult DialogResult { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            
            string _Value = Value;
            if (string.IsNullOrEmpty(_Value))
                _Value = string.Empty;
            this.DialogResult = CommonProgram.ShowInputBox(Title, PromptText, FieldName, SystemIcons.Information, ref _Value, IsPasswordInput);
            if (DialogResult == DialogResult.OK)
                Value = _Value;
            else if (DialogResult == DialogResult.Cancel)
            {
                if (!ContinueOnCancel)
                    throw new Exception("Cancelled task by user");
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FieldName:" + this.FieldName + " Value:" + this.Value + " DialogResult:" + this.DialogResult;
        }
    }

}
