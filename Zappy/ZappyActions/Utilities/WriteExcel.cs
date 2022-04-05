using System;
using System.Activities;
using System.ComponentModel;
using System.Linq;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Utilities
{

    public class WriteExcel : TemplateAction
    {
        [RequiredArgument]
        [Category("Input")]
        public DynamicProperty<string> Filename { get; set; }

        [RequiredArgument, OverloadGroup("DataSet")]
        [Category("Optional")]
        public DynamicProperty<System.Data.DataSet> DataSet { get; set; }

        [RequiredArgument, OverloadGroup("DataTable")]
        [Category("Input")]
        public DynamicProperty<System.Data.DataTable> DataTable { get
                ; set; }

        [RequiredArgument]
        [Category("Optional")]
        public DynamicProperty<bool> includeHeader { get; set; } = true;

        [RequiredArgument][Category("Optional")]
        public DynamicProperty<string> Theme { get; set; } = "None";

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();
            var ds = DataSet;
            var dt = DataTable;
            if (dt != null)
            {
                System.Data.DataTable table = dt;
                var name = table.TableName;
                if (string.IsNullOrEmpty(name)) name = "Sheet1";
                if (!includeHeader)
                {
                    System.Data.DataTable t = new System.Data.DataTable() { TableName = name };
                    var sheet = wb.Worksheets.Add(t, name);
                    sheet.FirstRow().FirstCell().InsertData(table.Rows);
                    var table2 = sheet.Tables.FirstOrDefault();
                    if (table2 != null)
                    {
                        table2.ShowAutoFilter = false;
                        table2.Theme = ClosedXML.Excel.XLTableTheme.FromName(Theme);
                    }

                }
                else
                {
                    var sheet = wb.Worksheets.Add(table, name);
                    var table2 = sheet.Tables.First();
                    table2.ShowAutoFilter = false;
                    table2.Theme = ClosedXML.Excel.XLTableTheme.FromName(Theme);
                }
            } 
            else
            {
                int idx = 0;
                foreach (System.Data.DataTable table in ds.Value.Tables)
                {
                    ++idx;
                    var name = table.TableName;
                    if (string.IsNullOrEmpty(name)) name = "Sheet" + idx.ToString();

                    if (!includeHeader)
                    {
                        System.Data.DataTable t = new System.Data.DataTable() { TableName = name };
                        var sheet = wb.Worksheets.Add(t, name);
                        sheet.FirstRow().FirstCell().InsertData(table.Rows);
                        var table2 = sheet.Tables.FirstOrDefault();
                        if (table2 != null)
                        {
                            table2.ShowAutoFilter = false;
                            table2.Theme = ClosedXML.Excel.XLTableTheme.FromName(Theme);
                        }

                    }
                    else
                    {
                        var sheet = wb.Worksheets.Add(table, name);
                        var table2 = sheet.Tables.First();
                        table2.ShowAutoFilter = false;
                        table2.Theme = ClosedXML.Excel.XLTableTheme.FromName(Theme);
                    }
                }

            }
            var filename = Filename;
            filename = Environment.ExpandEnvironmentVariables(filename);
            wb.SaveAs(filename);
        }
        

        
    }
}