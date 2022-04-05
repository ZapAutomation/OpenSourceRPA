using MsgReader.Outlook;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Mail
{
    public class SaveOutlookMailAttachment : TemplateAction
    {
        [Category("Input"), RequiredArgument]
        [Description("Path of the saved mail")]
        public DynamicProperty<string> MailPath { get; set; }

        [Category("Input"), RequiredArgument]
        [Description("Path to save the attachements")]
        public DynamicProperty<string> SaveDirectory { get; set; }

        [Category("Output"), RequiredArgument]
        public List<string> AttachmentFileLocation { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ExtractMessageAttachments(MailPath);
        }

        void ExtractMessageAttachments(string mailPath)
        {
            MsgReader.Outlook.Storage.Message message = new MsgReader.Outlook.Storage.Message(mailPath);
            AttachmentFileLocation = new List<string>();
            foreach (var attachment in message.Attachments)
            {
                string fileName = string.Empty;
                if (attachment is Storage.Attachment)
                {
                    Storage.Attachment attach = (MsgReader.Outlook.Storage.Attachment)attachment;
                    fileName = Path.Combine(SaveDirectory, (attach).FileName);
                    AttachmentFileLocation.Add(fileName);
                    File.WriteAllBytes(fileName, attach.Data);
                    if (Path.GetExtension(fileName).ToLower() == ".msg")
                    {
                        ExtractMessageAttachments(fileName);
                    }
                }
                else if (attachment is Storage.Message sAttachment)
                {
                    fileName = Path.Combine(SaveDirectory, sAttachment.FileName);
                    sAttachment.Save(fileName);
                    AttachmentFileLocation.Add(fileName);
                    ExtractMessageAttachments(fileName);
                }
            }
            message.Dispose();
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " MailPath:" + this.MailPath + " SaveDirectory:" + this.SaveDirectory + " AttachmentFileLocation:" + this.AttachmentFileLocation;
        }
    }
}