using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                [Description("Select Folder For Opration")]
    public class SelectFolder : TemplateAction
    {
        [Category("Optional")]
        [Description("Title")]
        public DynamicProperty<string> Title { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Selected Folder")]
        public string SelectedFolder { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string selectedPath = null;
                        
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if(!string.IsNullOrWhiteSpace(Title))
                dialog.Title = this.Title;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedPath = dialog.FileName;
            }
            

                                    
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                throw new OperationCanceledException("Selection was canceled.");
            }
            this.SelectedFolder = selectedPath;
        }



        public override string AuditInfo()
        {
            return base.AuditInfo() + " SelectedFolder:" + this.SelectedFolder;
        }
    }
}

