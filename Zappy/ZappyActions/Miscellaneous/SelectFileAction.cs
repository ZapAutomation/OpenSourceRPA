using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    [Description("Select File Action")]
    public class SelectFileAction : TemplateAction
    {
        public SelectFileAction()
        {
            Title = "Select File";
        }

        [Description("Set true if user required multiple files; otherwise,false")]
        [Category("Input")]
        public DynamicProperty<bool> MultiSelect { get; set; }

        [Category("Input")]
        public DynamicProperty<string> Title { get; set; }

        [Category("Output")]
        [Description("Path of the selected file")]
        public string FilePath { get; set; }

        [Category("Output")]
        [Description("Path of the selected files")]
        public string[] MultiFilePaths { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string selectedPath = null;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { };
            dialog.Title = this.Title;
            dialog.Multiselect = MultiSelect;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedPath = dialog.FileName;
                if (MultiSelect)
                    MultiFilePaths = dialog.FileNames.ToArray();
            }
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                throw new OperationCanceledException("Selection was canceled.");
            }
            this.FilePath = selectedPath;

        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath: " + this.FilePath + " MultiplePaths: " + this.MultiFilePaths;
        }
    }
}