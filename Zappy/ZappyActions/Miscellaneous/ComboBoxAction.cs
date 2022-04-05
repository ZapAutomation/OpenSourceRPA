using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zappy.Helpers;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    public class ComboBoxAction : TemplateAction
    {
        [Category("Input")]
        [Description("Title for ComboBox")]
        public DynamicTextProperty Title { get; set; }

        [Category("Input")]
        [Description("PromptText for ComboBox")]
        public DynamicTextProperty PromptText { get; set; }

        [Category("Input")]
        [Description("FieldName for ComboBox")]
        public DynamicTextProperty FieldName { get; set; }

        [Category("Optional")]
        public DynamicProperty<bool> ContinueOnCancel { get; set; }

        [Category("Input")]         [Description("Drop Down string to get required text - multiple search value seperated on new lines ")]
        public DynamicProperty<string> DropDownValues { get; set; }

        [Category("Output")] 
        [Description("It return combo-box selected text")]
        public string SelectedText { get; set; }

        [Category("Output")]
        [Browsable(false)]
        public DialogResult DialogResult { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string[] Seperator = new[] { Environment.NewLine, "\n" };
            if (string.IsNullOrEmpty(DropDownValues))
            {
                throw new Exception("DropDownValues is empty");
            }
            string[] DropDownArray = DropDownValues.Value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
  
            ComboBoxcs form = new ComboBoxcs(Title, PromptText, FieldName, SystemIcons.Information, DropDownArray);
            DialogResult = form.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                SelectedText = form.SetvalueCombobox();
            }
            else if (DialogResult == DialogResult.Cancel)
            {
                if (!ContinueOnCancel)
                    throw new Exception("Cancelled task by user");
            }

        }
    }
}
