using System.ComponentModel;
using System.Drawing.Design;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.ElementPicker.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.ElementPicker
{

                public sealed class ElementValueGet : TemplateAction
    {
        [Category("Input")]
        [Description("Element")]
        [Editor(typeof(ElementPickerEditor), typeof(UITypeEditor))] [TypeConverter(typeof(ExpandableObjectConverter))] public string Element { get; set; }

        [Category("Output")]
        [Description("Gets Element Value")]
        public string Value { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyExecutionContext _context = context as ZappyExecutionContext;

            var e = _context.GetElement(this.Element);

            e.Focus();

            this.Value = e.Value;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Element:" + this.Element + " Value:" + this.Value;
        }
    }
}
