using Microsoft.SharePoint.Client;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.SharePoint
{
    [Description("Update ListItem On SharePoint")]
    public class UpdateListItem : TemplateAction
    {
        [Category("Input")]
        [Description("SiteUrl for SharePoint")]
        public string SiteUrl { get; set; }


        [Category("Input")]
        [Description("Title for created list on sharepoint")]
        public string ListTitle { get; set; }


        [Category("Input")]
        [Description("ItemId of itemlist")]
        public string ItemId { get; set; }

        [Category("Input")]
        [Description("ColumnValue")]
        public string ColumnValue { get; set; }

        [Category("Output")]
        [Description("Gets updated ColumnName")]
        public string ColumnName { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ClientContext clientContext = new ClientContext(this.SiteUrl);

            List list = clientContext.Web.Lists.GetByTitle(this.ListTitle);

            ListItem item = list.GetItemById(this.ItemId);
            clientContext.Load(item);
            clientContext.ExecuteQuery();

            item[this.ColumnName] = this.ColumnValue;
            item.Update();

            clientContext.ExecuteQuery();
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " ColumnName:" + this.ColumnName + " ColumnValue:" + this.ColumnValue + " ListTitle:" + this.ListTitle + " SiteUrl:" + this.SiteUrl;
        }
    }
}
