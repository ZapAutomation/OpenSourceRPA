using Microsoft.Office.Interop.Outlook;
using System;
using System.ComponentModel;
using System.IO;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Exception = System.Exception;
namespace Zappy.ZappyActions.Mail
{
    [Description("Send The Outlook Mail")]
    public class OutlookMailSend : TemplateAction
    {
        [Category("Email")]
        [Description("The subject of the email message")]
        public DynamicProperty<string> Subject { get; set; }

        [Category("Email")]
        [Description("The body of the email message")]
        public DynamicProperty<string> Body { get; set; }

        [Category("Attachment")]
        [Description("The attachments to be added to the email message. For multiple attachments seperate them using ;")]
        public DynamicProperty<string> Attachment { get; set; }

        //check why giving errors
        //[Category("Options")]
        //[Description("OlAttachmentType like olByValue,olByReference,olEmbeddeditem,olOLE")]
        //public OlAttachmentType OlAttachmentType { get; set; }

        [Category("Options")]
        [Description("Specifies whether the body of the message is written in HTML format.")]
        public DynamicProperty<Boolean> IsHtmlBody { get; set; }

        [Category("Options")]
        [Description("Specifies whether the message should be saved as a draft.")]
        public DynamicProperty<Boolean> IsDraft { get; set; }

        [Category("Receiver")]
        [Description("The main recipients of the email message")]
        public DynamicProperty<string> To { get; set; }

        [Category("Receiver")]
        [Description("The secondary recipients of the email message")]
        public DynamicProperty<string> Cc { get; set; }

        [Category("Receiver")]
        [Description(" The hidden recipients of the email message.")]
        public DynamicProperty<string> Bcc { get; set; }

        [Category("Sender")]
        [Description("Sender's mail id")]
        public DynamicProperty<string> From { get; set; }


        /// <summary>
        /// to send outlook mail
        /// contain attributes of mail like from,to,cc,bcc,etc
        /// create instance of application ,set the mailitem
        /// to send mail their is must some properties then set those
        /// </summary>
        /// <param name="context"></param>
        /// <param name="actionInvoker"></param>
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            var from = this.From;
            var to = this.To;
            var cc = this.Cc;
            var bcc = this.Bcc;
            var subject = this.Subject;
            var body = this.Body;
            var attachment = this.Attachment;
            var isHtmlBody = this.IsHtmlBody;
            var isdraft = this.IsDraft;

            dynamic app = Activator.CreateInstance(Type.GetTypeFromProgID("Outlook.Application"));

            //dynamic mailItem = app.CreateItem(OlItemType.olMailItem);
            MailItem mailItem = (MailItem)app.CreateItem(
                OlItemType.olMailItem);
            if (from.ToString().Length > 0)
            {
                var fromAccount = app.Session.Accounts.Item(from.ToString());
                if (fromAccount == null)
                {
                    throw new Exception("Cannot find " + from + " account.");
                }
                mailItem.SendUsingAccount = fromAccount;
            }

            mailItem.To = to;
            mailItem.CC = cc;
            mailItem.BCC = bcc;
            mailItem.Subject = subject;
            string messageBody = body;

            if (messageBody.Contains(CrapyConstants.pasteChar))
            {
                if (!CommonProgram.GetTextFromClipboard(out string clipBoardChar))
                    throw new Exception("Unable to read clipboard contents!");
                messageBody = messageBody.Replace(CrapyConstants.pasteChar, clipBoardChar);
            }

            if (attachment != null && attachment.ToString().Length > 0)
            {
                string[] attachmentSplit = attachment.ToString().Split(';');
                foreach (string _attachment in attachmentSplit)
                {
                    Attachment mattachment = mailItem.Attachments.Add(_attachment.ToString(), OlAttachmentType.olByValue, 1, null);

                    mattachment.PropertyAccessor.SetProperty(
                        "http://schemas.microsoft.com/mapi/proptag/0x3712001E"
                        , Path.GetFileName(_attachment)
                    );
                }

            }

            if (isHtmlBody)
            {
                mailItem.HTMLBody = messageBody;
            }
            else
            {
                mailItem.Body = messageBody;
            }

            if (isdraft)
            {
                mailItem.Display();
            }
            else
            {
                //mailItem.Display();
                mailItem.Send();
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " From:" + this.From + " To:" + this.To + " Body:" + this.Body + " Subject:" + this.Subject + " Attachment:" + this.Attachment;
        }
    }
}