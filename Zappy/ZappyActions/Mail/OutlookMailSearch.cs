using System.ComponentModel;
using Zappy.Plugins.Outlook;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.OutlookMessages;

namespace Zappy.ZappyActions.Mail
{
    [Description("Read All Send Mails Of Outlook")]
    public class OutlookMailSearch : TemplateAction
    {
        [Description("Username of the outlook account for searching emails")]
        [Category("Optional")]
        public DynamicProperty<string> OutlookAccountUserName { get; set; }

        [Description("Email address of the email message sender")]
        [Category("Input")]
        public DynamicProperty<string> SenderEmail { get; set; }

        [Description("Name of the email message sender")]
        [Category("Optional")]
        public DynamicProperty<string> SenderName { get; set; }

        [Description("Searches if the email to match contains the subject entered")]
        [Category("Input")]
        public DynamicProperty<string> Subject { get; set; }

        [Category("Optional")]
        [Description("Searches if the email to match contains the body entered")]
        public DynamicProperty<string> Body { get; set; }

        [Description("Only save the leatest email message")]
        [Category("Optional")]
        public DynamicProperty<bool> OnlySaveLatest { get; set; }

                [Description("Saves the matching emails in the given directory path")]
        [Category("Input")]
        public DynamicProperty<string> DirectoryToSaveMatched { get; set; }

        [Description("Saves the matching emails in the given directory path")]
        [Category("Optional")]
        public DynamicProperty<bool> UseInbuiltOutlookSearch { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            OutlookMessageInfo messageInfo = new OutlookMessageInfo();
            messageInfo.OutlookAccountUserName = OutlookAccountUserName;
            messageInfo.OnlySaveLatest = OnlySaveLatest;
            messageInfo.SaveMatchedEmailsDirectory = DirectoryToSaveMatched;
            messageInfo.SenderEmail = SenderEmail;
            messageInfo.SenderName = SenderName;
            messageInfo.Subject = Subject;
            messageInfo.Body = Body;

            if (UseInbuiltOutlookSearch)
                OutlookCommunicator.Instance.SearchMessage_OutlookSearch(messageInfo);
            else
                OutlookCommunicator.Instance.SearchMessage(messageInfo);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " OutlookAccountUserName:" + this.OutlookAccountUserName + " Sender email address:" + this.SenderEmail + " SenderName:" + this.SenderName + " Subject:" + this.Subject;
        }
    }
}