using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Zappy.ZappyActions.Picture
{
    internal class LocationPickerEditor : UITypeEditor
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
                using (ClickLocationForm form = new ClickLocationForm(value))
                {
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                        value = form.SelectedItem;
                    }
                }
            }
            return value;         }
    }
}
