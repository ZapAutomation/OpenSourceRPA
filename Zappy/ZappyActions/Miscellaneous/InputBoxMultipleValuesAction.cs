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
    [Description("Sets The Multiple Inputs At A Time")]
    public class InputBoxMultipleValuesAction : TemplateAction
    {
        [Category("Input")]
        public DynamicTextProperty Title { get; set; }

        [Category("Input")]
        public DynamicTextProperty PromptText { get; set; }

        [Category("Input")]
        public DynamicTextProperty FieldName1 { get; set; }

        [Category("Input")]
        public DynamicTextProperty FieldName2 { get; set; }

        [Category("Input")]
        public DynamicTextProperty FieldName3 { get; set; }

        [Category("Input")]
        public DynamicTextProperty FieldName4 { get; set; }

        [Category("Input")]
        public DynamicTextProperty FieldName5 { get; set; }

        [Category("Input")]
        public DynamicTextProperty FieldName6 { get; set; }

        [Category("Input")]
        public DynamicTextProperty FieldName7 { get; set; }

        [Category("Optional")]
        public DynamicProperty<bool> ContinueOnCancel { get; set; }

        [Category("Output")]
        public string Value1 { get; set; }

        [Category("Output")]
        public string Value2 { get; set; }

        [Category("Output")]
        public string Value3 { get; set; }

        [Category("Output")]
        public string Value4 { get; set; }

        [Category("Output")]
        public string Value5 { get; set; }

        [Category("Output")]
        public string Value6 { get; set; }

        [Category("Output")]
        public string Value7 { get; set; }

        [Category("Output")]
        public DialogResult DialogResult { get; set; }

                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        InputBoxMultipleValues form = new InputBoxMultipleValues(Title, PromptText, SystemIcons.Information,
                FieldName1, FieldName2, FieldName3, FieldName4, FieldName5, FieldName6, FieldName7,
                Value1, Value2, Value3, Value4, Value5, Value6, Value7);
            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                Value1 = form.Input1;
                Value2 = form.Input2;
                Value3 = form.Input3;
                Value4 = form.Input4;
                Value5 = form.Input5;
                Value6 = form.Input6;
                Value7 = form.Input7;
            }
            else if (dialogResult == DialogResult.Cancel)
            {
                if (!ContinueOnCancel)
                    throw new Exception("Cancelled task by user");
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Title:" + this.Title;
        }
    }
}