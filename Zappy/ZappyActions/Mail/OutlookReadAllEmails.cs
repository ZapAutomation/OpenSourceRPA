using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Mail
{
    [Description("Read all outlook mails")]
    public class OutlookReadAllEmails : TemplateAction
    {
        [Category("Output")]
        [Description("Gets list of mail details")]
        public List<OutlookEmails> ListEmailDetails { get; set; }

        public static List<OutlookEmails> ReadMailItems()
        {
            Application outlookApplication = null;
            NameSpace outlookNameSpace = null;
            MAPIFolder inboxFolder = null;

            Items mailItems = null;
            List<OutlookEmails> listEmailDetails = new List<OutlookEmails>();
            OutlookEmails emailDetails;

            try
            {
                outlookApplication = new Application();
                outlookNameSpace = outlookApplication.GetNamespace("MAPI");
                inboxFolder = outlookNameSpace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
                mailItems = inboxFolder.Items;
                foreach (MailItem item in mailItems)
                {
                    emailDetails = new OutlookEmails();
                    emailDetails.EmailFrom = item.SenderEmailAddress;
                    emailDetails.EmailSub = item.Subject;
                    emailDetails.EmailBody = item.Body;
                                        listEmailDetails.Add(emailDetails);
                    ReleaseComObject(item);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ReleaseComObject(mailItems);
                ReleaseComObject(inboxFolder);
                ReleaseComObject(outlookNameSpace);
                ReleaseComObject(outlookApplication);
            }

            return listEmailDetails;
        }

        private static void ReleaseComObject(Object obj)
        {
            if (obj != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            ListEmailDetails = ReadMailItems();
        }

        public override string AuditInfo()
        {

            return base.AuditInfo() + " ListEmailDetails :" + this.ListEmailDetails;
        }
    }
}