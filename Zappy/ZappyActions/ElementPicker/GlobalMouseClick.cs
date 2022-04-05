using System.ComponentModel;
using System.Drawing.Design;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.ElementPicker.Helper;
using Zappy.ZappyActions.ElementPicker.Input;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.ElementPicker
{
    [Description("Global Mouse Click")]
    public sealed class GlobalMouseClick : TemplateAction
    {
        [Category("Input")]
        [Description("Element")]
        [Editor(typeof(ElementPickerEditor), typeof(UITypeEditor))] [TypeConverter(typeof(ExpandableObjectConverter))] public string Element { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyExecutionContext _context = context as ZappyExecutionContext;

            var e = _context.GetElement(this.Element);

            using (var input = new EditorPageInputDriver())
            {
                e.Focus();
                var p = e.Bounds.Center;
                input.MouseMove(p.X, p.Y);
                input.Click(MouseButton.Left);
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Pick Element:" + this.Element;
        }
    }
}
