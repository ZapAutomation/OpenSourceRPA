using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Zappy.ZappyActions.Picture.Helpers;

namespace Zappy.ZappyActions.Picture
{

    internal class ClipPickerEditor : UITypeEditor
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
                using (ZappySnippingForm form = new ZappySnippingForm(value))
                {
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                        ImageObject obj = new ImageObject();
                        obj.PatternFile = form.ImageLocation;
                        obj.ClickLocation = form.ClickLocation;
                        value = obj;
                    }
                }
            }
            return value;                     }
    }
}
