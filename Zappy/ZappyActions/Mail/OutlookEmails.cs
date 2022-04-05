using System.ComponentModel;

namespace Zappy.ZappyActions.Mail
{
    [Description("Outlook Mails")]
    public class OutlookEmails
    {
        [Category("Output")]
        [Description("Sender of email")]
        public string EmailFrom { get; set; }

        [Category("Output")]
        [Description("Mail Subject")]
        public string EmailSub { get; set; }

        [Category("Output")]
        [Description("Mail Body")]
        public string EmailBody { get; set; }
    }
}