using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.EditorPage;
using Zappy.ZappyTaskEditor.EditorPage.ElementPicker;

namespace Zappy.ZappyActions.ElementPicker.Helper
{
    [Description("")]
    internal class DynamicPropertyPicker : UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (svc != null)
            {
                IZappyAction _Action = context.Instance as IZappyAction;

                TaskEditorPage _Page = (context.GetType().GetProperty("OwnerGrid").GetValue(context) as PropertyGrid).Tag as TaskEditorPage;

                if (value == null)
                    value = context.PropertyDescriptor.PropertyType.GetConstructor(new System.Type[0] { }).Invoke(null);

                using (DynamicPropertyPickerForm form = new DynamicPropertyPickerForm(value, _Action, context.PropertyDescriptor, _Page))
                {
                    if (svc.ShowDialog(form) == DialogResult.OK)
                        form.BuildVariableValue();
                }
            }
            return value;         }
    }
}