using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.ElementPicker.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.ElementPicker
{
    [Description("Global Key Press")]
    public sealed class GlobalKeyPress : TemplateAction
    {
        [Category("Input")]
        [Description("Element")]
        [Editor(typeof(ElementPickerEditor), typeof(UITypeEditor))] [TypeConverter(typeof(ExpandableObjectConverter))] public string Element { get; set; }

        [Category("Input")]
        [Description("Input Text")]
        public DynamicProperty<string> Text { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyExecutionContext _context = context as ZappyExecutionContext;

            if (Element != null)
            {
                var e = _context.GetElement(this.Element);
                e.Focus();
            }

            var text = this.Text;



            SendKeys.SendWait(text);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Text:" + this.Text;
        }
    }
}
