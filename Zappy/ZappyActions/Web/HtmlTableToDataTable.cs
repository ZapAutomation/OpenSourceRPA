using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows.Automation;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Web
{
    public class HtmlTableToDataTable : TemplateAction
    {
                                
        
        [Category("Input")]
        [Description("HTML Page Source")]
        public DynamicProperty<string> HTMLString { get; set; }

                                
        [Category("Output")]
        [Description("Gets DataTable from HTML Page")]
        public System.Data.DataTable DataTable { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        
            
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(HTMLString);

            List<List<string>> table = new List<List<string>>();
                                    table = doc.DocumentNode
                       .Descendants("tr")
                       .Skip(1)
                       .Where(tr => tr.Elements("td").Count() > 1)
                       .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                       .ToList();
                                                                                                                                    DataTable dt = new DataTable();
            table.First().ForEach(colname => dt.Columns.Add(colname));
            foreach (List<string> row in table)
                dt.Rows.Add(row.ToArray());

            this.DataTable = dt;

        }
    }
}
