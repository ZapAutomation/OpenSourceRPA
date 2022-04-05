using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Mail
{
    public class ReadOutlookMail : TemplateAction
    {
        [Category("Input"), RequiredArgument]
        public DynamicProperty<string> MailPath { get; set; }

        [Category("Output")]
        public string From { get; set; }

        [Category("Output")]
        public string SenderName { get; set; }

        [Category("Output")]
        public DateTime? SentOn { get; set; }

        [Category("Output")]
        public string RecipientsTo { get; set; }

        [Category("Output")]
        public string RecipientsCc { get; set; }

        [Category("Output")]
        public string Subject { get; set; }

        [Category("Output")]
        public string HtmlBody { get; set; }

        [Category("Output")]
        public string TextBody { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (Path.GetExtension(MailPath) == ".eml")
            {

                var fileInfo = new FileInfo(MailPath);
                var eml = MsgReader.Mime.Message.Load(fileInfo);

                if (eml.Headers != null)
                {
                    if (eml.Headers.To != null)
                    {
                        foreach (var recipient in eml.Headers.To)
                        {
                            RecipientsTo += recipient.Address;
                        }
                    }
                }

                Subject = eml.Headers.Subject;

                if (eml.TextBody != null)
                {
                    TextBody = System.Text.Encoding.UTF8.GetString(eml.TextBody.Body);
                }
                if (eml.HtmlBody != null)
                {
                    HtmlBody = System.Text.Encoding.UTF8.GetString(eml.HtmlBody.Body);
                }
                SentOn = eml.Headers.DateSent;
                            }
            else
            {
                using (var msg = new MsgReader.Outlook.Storage.Message(MailPath))
                {
                    From = msg.Sender.Email;
                    SenderName = msg.Sender.DisplayName;
                    SentOn = msg.SentOn;
                    RecipientsTo = msg.GetEmailRecipients(MsgReader.Outlook.RecipientType.To, false, false);
                    RecipientsCc = msg.GetEmailRecipients(MsgReader.Outlook.RecipientType.Cc, false, false);
                    Subject = msg.Subject;
                    HtmlBody = msg.BodyHtml;
                    TextBody = msg.BodyText;
                }
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " MailPath:" + this.MailPath + " SenderName:" + this.SenderName + " From:" + this.From + " SentOn Time:" + this.SentOn + " Subject:" + this.Subject + " Receipts To:" + this.RecipientsTo + " ReceiptCC:" + this.RecipientsCc;
        }
    }
}