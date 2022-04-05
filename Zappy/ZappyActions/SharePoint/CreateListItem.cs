using Microsoft.SharePoint.Client;
using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.SharePoint
{
    [Description("Create ListItem In List On SharePoint")]
    public class CreateListItem : TemplateAction
    {
        [Category("Input")]
        [Description("SiteUrl for SharePoint")]
        public string SiteUrl { get; set; }

        [Category("Input")]
        [Description("Title for created list on sharepoint")]
        public string ListTitle { get; set; }

        [Category("Input")]
        [Description("Title for created listItem")]
        public string ItemTitle { get; set; }

        [Category("Input")]
        public string FilePath { get; set; }

        [Category("Input")]
        [Description("Ask for Overwrite")]
        public Boolean Overwrite { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        ClientContext clientContext = new ClientContext(this.SiteUrl);

            List list = clientContext.Web.Lists.GetByTitle(this.ListTitle);
            FileCreationInformation fci = new FileCreationInformation();
            fci.Content = System.IO.File.ReadAllBytes(FilePath);
            fci.Overwrite = Overwrite;

            clientContext.ExecuteQuery();
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath + " ItemTitle:" + this.ItemTitle + " ListTitle:" + this.ListTitle + " SiteUrl:" + this.SiteUrl;
        }
    }
}
