using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Zappy.Plugins.ChromeBrowser.Chrome.Helper;

namespace Zappy.Plugins.ChromeBrowser.Chrome
{
    internal class HtmlTablePickerEditor : UITypeEditor
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
                    using (WebExtractDataForm form = new WebExtractDataForm(value))
                    {
                        if (svc.ShowDialog(form) == DialogResult.OK)
                        {
                            value = form.htmlTableId;
                        }
                    }
                }
                return value;             }
     
    }
}
