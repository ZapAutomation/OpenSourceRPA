using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Microsoft.Exchange.WebServices.Data;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Item = Microsoft.Exchange.WebServices.Data.Item;

namespace Zappy.ZappyActions.Mail
{

                
        
            
        
                
        
                    
    [Description("Retrieves an email message from Exchange")]
    public class GetsExchangeMailFromInbox : TemplateAction
    {
        public enum Select
        {
            Inbox,
            Outbox,
            Drafts,
            DeletedItems,
            JunkEmail,
            SentItems
        }

        [Category("Input")]
        [Description("select folder which mail user want to read")]
        public Select SelectFolder { get; set; }

        [Category("Input")]
        [Description("User name")]
        public DynamicProperty<string> UserName { get; set; }

        [Category("Input")]
        [Description("Password of the above user name")]
        public DynamicProperty<string> Password { get; set; }

        [Category("Input")]
        [Description("How many mails user want to read")]
        public DynamicProperty<int> Count { get; set; }

        [Category("Input")]
        [Description("Path for save the mail")]
        public DynamicProperty<string> PathToSaveMail { get; set; }

        [Category("Output")]
        [XmlIgnore]
        [Description("Gets last mail details")]
        public FindItemsResults<Item> ListEmails { get; private set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            String MailboxToAccess = UserName;
            ExchangeService service = new Microsoft.Exchange.WebServices.Data.ExchangeService(ExchangeVersion.Exchange2010_SP1);
            SearchFilter sfSearchFilter = new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false);

            service.Credentials = new NetworkCredential(UserName, Password);
            service.AutodiscoverUrl(MailboxToAccess, adAutoDiscoCallBack);

            FolderId FolderToAccess = null;
            if (SelectFolder == Select.Inbox)
                FolderToAccess = new FolderId(WellKnownFolderName.Inbox, MailboxToAccess);
            else if (SelectFolder == Select.Outbox)
                FolderToAccess = new FolderId(WellKnownFolderName.Outbox, MailboxToAccess);
            else if (SelectFolder == Select.Drafts)
                FolderToAccess = new FolderId(WellKnownFolderName.Drafts, MailboxToAccess);
            else if (SelectFolder == Select.SentItems)
                FolderToAccess = new FolderId(WellKnownFolderName.SentItems, MailboxToAccess);
            else if (SelectFolder == Select.JunkEmail)
                FolderToAccess = new FolderId(WellKnownFolderName.JunkEmail, MailboxToAccess);
            else
                FolderToAccess = new FolderId(WellKnownFolderName.DeletedItems, MailboxToAccess);

            ItemView ivItemView = new ItemView(Count);
            FindItemsResults<Item> FindItemResults = service.FindItems(FolderToAccess, sfSearchFilter, ivItemView);
            PropertySet ItemPropertySet = new PropertySet(BasePropertySet.IdOnly);

            ItemPropertySet.Add(ItemSchema.Body);
            ItemPropertySet.Add(ItemSchema.Subject);

            ItemPropertySet.RequestedBodyType = BodyType.Text;
            if (FindItemResults.Items.Count > 0)
            {
                service.LoadPropertiesForItems(FindItemResults.Items, ItemPropertySet);
            }
            foreach (Item item in FindItemResults.Items)
            {
                var Subject = item.Subject;
                var Body = item.Body.Text;
                ListEmails = FindItemResults;

                if (!string.IsNullOrEmpty(PathToSaveMail))
                {

                    item.Load(new PropertySet(ItemSchema.MimeContent));
                    var mimeContent = item.MimeContent;
                    string fileName = Path.Combine(PathToSaveMail, Subject + ".eml");
                    using (var fileStream = new FileStream(PathToSaveMail, FileMode.Create))
                    {
                        fileStream.Write(mimeContent.Content, 0, mimeContent.Content.Length);
                    }
                }
            }
        }
        internal bool adAutoDiscoCallBack(string redirectionUrl)
        {
                        bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

                                                if (redirectionUri.Scheme == "https")
            {
                result = true;
            }

            return result;

        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " UserName:" + this.UserName;
        }
    }
}
