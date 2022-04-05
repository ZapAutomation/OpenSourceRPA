using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Zappy.ZappyTaskEditor.EditorPage.ElementPicker;

namespace Zappy.ZappyActions.ElementPicker.Helper
{
    internal class ElementPickerEditor : UITypeEditor
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
                using (ElementPickerForm form = new ElementPickerForm(value))
                {
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                        value = form.Query.ToString();
                    }
                }
            }
            return value;         }
    }
}