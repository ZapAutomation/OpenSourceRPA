using Microsoft.Office.Interop.Outlook;
using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Mail
{
    public class CreateFolderInOutlook : TemplateAction
    {
        [Description("New Folder Name")]
        [Category("Input")]
        public DynamicProperty<string> FolderName { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            AddMyNewFolder(FolderName.Value);
        }

        static void AddMyNewFolder(string foldername)
        {
            dynamic app = Activator.CreateInstance(Type.GetTypeFromProgID("Outlook.Application"));
            Microsoft.Office.Interop.Outlook.Folder folder = app.Session.GetDefaultFolder(OlDefaultFolders.olFolderInbox) as Microsoft.Office.Interop.Outlook.Folder;
            Microsoft.Office.Interop.Outlook.Folders folders = folder.Folders;
            try
            {
                Microsoft.Office.Interop.Outlook.Folder newFolder = folders.Add(
                    foldername, Type.Missing)
                    as Microsoft.Office.Interop.Outlook.Folder;
                newFolder.Display();
            }
            catch
            {
                                                                                            }
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " FolderName:" + this.FolderName;
        }
    }
}
