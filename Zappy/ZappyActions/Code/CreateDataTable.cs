using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
    [Description("Create new data table")]
    public class CreateDataTable : TemplateAction
    {
        [Description("Create new data table")]
        public CreateDataTable()
        {
            DataTableColumnName = new DynamicProperty<string>();
            RowsCount = 100;
        }
        [Category("Input")]
        [Description("Data Table Column Name - multiple search value separated on new lines")]
        public DynamicProperty<string> DataTableColumnName { get; set; }

        [Category("Optional")]
        [Description("Number of rows in data table")]
        public DynamicProperty<int> RowsCount { get; set; }

        [Category("Output")]
        [Description("Create new DataTable according to column and row count")]
        public DataTable DataTable { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            DataTable dt = new DataTable();
            string[] Seperator = new[] { Environment.NewLine, "\n" };
            int i = 0;
            if (!string.IsNullOrEmpty(DataTableColumnName))
            {
                string[] _DataTableColumnName = DataTableColumnName.Value.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
                foreach(string name in _DataTableColumnName)
                {
                    dt.Columns.Add(name);
                    i++;
                }
                for (i = 0; i < RowsCount; i++)
                {
                    dt.Rows.Add();
                } 
            }         
            DataTable = dt;
        }
    }
}
