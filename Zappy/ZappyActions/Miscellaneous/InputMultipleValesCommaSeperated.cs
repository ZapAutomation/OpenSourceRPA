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
    [Description("Input Values - Gets Converted To Array")]
    public class InputMultipleValesCommaSeperated : TemplateAction
    {
        [Category("Input")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Title of ")]
        public DynamicTextProperty Title { get; set; }

        [Category("Input")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DynamicTextProperty PromptText { get; set; }

        [Category("Input")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DynamicTextProperty FieldName { get; set; }

        [Category("Output")]
        [Description("Gets Values in array")]
        public string[] Value { get; set; }

        [Category("Output")]
        public DialogResult DialogResult { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            string _Value = String.Empty;

            this.DialogResult = CommonProgram.ShowInputBox(Title, PromptText, FieldName, SystemIcons.Information, ref _Value, false);
            if (DialogResult == DialogResult.OK)
                Value = _Value.Split(',');
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FieldName:" + this.FieldName + " Value:" + this.Value;
        }
    }
}