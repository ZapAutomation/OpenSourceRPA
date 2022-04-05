using Microsoft.SharePoint.Client;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.SharePoint
{
    [Description("Create List On SharePoint")]
    public class CreateList : TemplateAction
    {
        [Category("Input")]
        [Description("SiteUrl for SharePoint")]
        public string SiteUrl { get; set; }

        [Category("Input")]
        [Description("Title for created list on sharepoint")]
        public string ListTitle { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            ClientContext spContext = new ClientContext(this.SiteUrl);
            Microsoft.SharePoint.Client.Web spWeb = spContext.Web;

            ListCreationInformation lci = new ListCreationInformation();
            lci.Title = this.ListTitle;
            lci.TemplateType = (int)ListTemplateType.DocumentLibrary;

            List newList = spWeb.Lists.Add(lci);

            try
            {
                spContext.ExecuteQuery();
            }
            catch
            {
                            }

        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " ListTitle:" + this.ListTitle + " SiteUrl:" + this.SiteUrl;
        }
    }
}
