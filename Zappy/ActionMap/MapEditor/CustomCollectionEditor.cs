using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Zappy.ActionMap.MapEditor
{
    internal class CustomCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context == null || context.Instance == null)
                return base.GetEditStyle(context);

            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService;

            if (context == null || context.Instance == null || provider == null)
                return value;

            editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            ScreenMapEditor CollectionEditor = new ScreenMapEditor();

            if (editorService.ShowDialog(CollectionEditor) == System.Windows.Forms.DialogResult.OK)
                return CollectionEditor.Programmed;

            return value;
                    }
    }

}