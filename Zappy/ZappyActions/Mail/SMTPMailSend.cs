using System.ComponentModel;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Exception = System.Exception;

namespace Zappy.ZappyActions.Mail
{
                [Description("Sends Mails Using SMTP Protocal")]
    public class SMTPMailSend : TemplateAction
    {
        [Category("Input")]
        [Description("Mail Sender")]
        public DynamicProperty<string> From { get; set; }

        [Category("Input")]
        [Description("Mail Receiver")]
        public DynamicProperty<string> To { get; set; }

        [Category("Input")]
        [Description("Sending an Email to Multiple Recipients With Cc And Bcc")]
        public DynamicProperty<string> Cc { get; set; }

        [Category("Input")]
        [Description("Sending an Email to Multiple Recipients With Cc And Bcc")]
        public DynamicProperty<string> Bcc { get; set; }

        [Category("Input")]
        [Description("Subject of mail")]
        public DynamicProperty<string> Subject { get; set; }

        [Category("Input")]
        [Description("Body of the send mail")]
        public DynamicProperty<string> Body { get; set; }

        [Category("Input")]
        [Description("Attachment for sending the mail")]
        public DynamicProperty<string> Attachment { get; set; }

        [Category("Input")]
        [Description("Check IsHtmlBody or not")]
        public DynamicProperty<bool> IsHtmlBody { get; set; }

        [Category("Input")]

        public DynamicProperty<bool> IsLinkedResource { get; set; }

        [Category("Input")]
        public DynamicProperty<string> Host { get; set; }

        [Category("Input")]
        [Description("Enable Secure Sockets Layer,global standard security technology that enables encrypted communication between a web browser and a web server")]
        public DynamicProperty<bool> EnableSSL { get; set; }

        [Category("Input")]
        [Description("Port no")]
        public DynamicProperty<int> Port { get; set; }

        [Category("Input")]
        [Description("UserName of sender")]
        public DynamicProperty<string> UserName { get; set; }

        [Category("Input")]
        [Description("Password of sender")]
        public DynamicProperty<string> Password { get; set; }

                                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            var from = this.From;
            var to = this.To;
            var cc = this.Cc;
            var bcc = this.Bcc;
            var subject = this.Subject;
            var body = this.Body;
            var isHtmlBody = this.IsHtmlBody;
            string messageBody = body;

            if (messageBody.Contains(CrapyConstants.pasteChar))
            {
                if (!CommonProgram.GetTextFromClipboard(out string clipBoardChar))
                    throw new Exception("Unable to read clipboard contents!");
                messageBody = messageBody.Replace(CrapyConstants.pasteChar, clipBoardChar);
            }

            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(to);
            if (cc != null) message.CC.Add(cc);
            if (bcc != null) message.Bcc.Add(bcc);
            message.Subject = subject;
            message.From = new System.Net.Mail.MailAddress(from);
            message.Body = messageBody;
            message.IsBodyHtml = isHtmlBody;

            if (IsLinkedResource && isHtmlBody)
            {
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                    (messageBody, null, MediaTypeNames.Text.Html);
                string filename = Path.GetFileName(this.Attachment);
                LinkedResource inline = new LinkedResource(this.Attachment, MediaTypeNames.Image.Jpeg);
                inline.ContentId = filename;
                avHtml.LinkedResources.Add(inline);

                message.AlternateViews.Add(avHtml);
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(this.Attachment);
                attachment.ContentDisposition.Inline = true;
                message.Attachments.Add(attachment);
            }
            else
            {
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(this.Attachment);
                message.Attachments.Add(attachment);
            }

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = this.Host;
            smtp.Port = this.Port;
            smtp.EnableSsl = this.EnableSSL;
                        smtp.Credentials = new System.Net.NetworkCredential(this.UserName,
                this.Password);             smtp.Send(message);
            
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " From:" + this.From + " To:" + this.To + " Body:" + this.Body + " Subject:" + this.Subject + " Attachment:" + this.Attachment + " UserName:" + this.UserName;
        }
    }
}
