using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.ElementPicker.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.ElementPicker
{
            
    [Description("Gets Properties Of Elements")]
    public sealed class ElementPropertyGet : TemplateAction
    {
        [Category("Input")]
        [Description("Property Name")]
        public string PropertyName { get; set; }

        [Category("Input")]
        [Description("Element")]
        [Editor(typeof(ElementPickerEditor), typeof(UITypeEditor))] [TypeConverter(typeof(ExpandableObjectConverter))] public string Element { get; set; }

        [Category("Output")]
        [Description("Gets the Property values from pick element")]
        public string PropertyValue { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyExecutionContext _context = context as ZappyExecutionContext;
            var e = _context.GetElement(this.Element);

            var name = this.PropertyName;

            if (e.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance) is PropertyInfo pi)
            {
                this.PropertyValue = pi.GetValue(e)?.ToString() ?? string.Empty;
            }
            else
            {
                throw new Exception(string.Format("Element property '{0}' not found.", name));
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Property Name which user want:" + this.PropertyName + " Property Value:" + this.PropertyValue;
        }
    }
}