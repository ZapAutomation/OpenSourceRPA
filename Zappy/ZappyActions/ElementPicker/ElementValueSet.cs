using System.ComponentModel;
using System.Drawing.Design;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.ElementPicker.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.ElementPicker
{
                    [Description("Element Value Set")]
    public sealed class ElementValueSet : TemplateAction
    {
        [Category("Input")]
        [Description("Set Values to Element")]
        public string Value { get; set; }

        [Category("Input")]
        [Description("Element")]
        [Editor(typeof(ElementPickerEditor), typeof(UITypeEditor))] [TypeConverter(typeof(ExpandableObjectConverter))] public string Element { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyExecutionContext _context = context as ZappyExecutionContext;

            var e = _context.GetElement(this.Element);

            e.Focus();

            e.Value = Value;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Element:" + this.Element + " Value:" + this.Value;
        }
    }
}
