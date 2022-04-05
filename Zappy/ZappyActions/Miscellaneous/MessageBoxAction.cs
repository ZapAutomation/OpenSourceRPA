using System.ComponentModel;
using System.Windows.Forms;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
            
    [Description("To Display Message On Screen")]
    public class MessageBoxAction : TemplateAction
    {
        public MessageBoxAction()
        {
            Icon = MessageBoxIcon.Information;
        }
        [Category("Input")]
        [Description("InputBox Title")]
        public DynamicTextProperty Title { get; set; }

        [Category("Input")]
        [Description("InputBox Text")]
        public DynamicTextProperty Text { get; set; }

        [Category("Optional")]
        [Description("Icon for InputBox")]
        public MessageBoxIcon Icon { get; set; }

        [Category("Optional")]
        [Description("Buttons for InputBox")]
        public MessageBoxButtons Buttons { get; set; }

        [Category("Optional")]
        public MessageBoxDefaultButton DefaultButton { get; set; }

        [Category("Optional")]
        [Description("Options")]
        public MessageBoxOptions Options { get; set; }

        [Category("Output")]
        public DialogResult DialogResult { get; set; }

        [Category("Output")]
        public bool DialogResultBool { get; set; }

                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            DialogResult = MessageBox.Show(Text, Title, Buttons, Icon, DefaultButton, Options);
            if (DialogResult == DialogResult.OK || DialogResult == DialogResult.Yes)
                DialogResultBool = true;
            else
            {
                DialogResultBool = false;
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Title:" + this.Title + " Text:" + this.Text;
        }
    }

}
