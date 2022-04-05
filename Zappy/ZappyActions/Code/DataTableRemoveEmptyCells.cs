using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
    [Description("Remove all empty cell in Datatable")]
    public class DataTableRemoveEmptyCells : TemplateAction
    {
        [Category("Input")]
        [Description("DataTable")]
        public DynamicProperty<DataTable> DataTable { get; set; }

        [Category("Output")]
        [Description("Remove all empty cells and return the data table")]
        public DataTable OutputDataTable { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            OutputDataTable = ClearSpace(DataTable);
        }
        public DataTable ClearSpace(DataTable datable)
        {
            List<List<object>> listTemp = new List<List<object>>();

            for (int i = 0; i < datable.Columns.Count; i++)
            {
                List<object> stack = new List<object>();
                for (int j = 0; j < datable.Rows.Count; j++)
                {
                    object obj = datable.Rows[j][i];
                    if (obj.ToString() != "")
                    {
                        stack.Add(obj);
                    }
                }
                listTemp.Add(stack);
            }

            datable.Clear();

            while (hasData(listTemp) == true)
            {
                for (int i = 0; i < datable.Columns.Count; i++)
                {
                    DataRow dr = datable.NewRow();

                    for (int j = 0; j < listTemp.Count; j++)
                    {
                        if (listTemp[j].Count != 0)
                        {
                            dr[j] = listTemp[j][0];

                            listTemp[j].RemoveAt(0);
                        }
                    }

                    datable.Rows.Add(dr);
                }

            }
            return datable;

        }
        private bool hasData(List<List<object>> listTemp)
        {
            foreach (var item in listTemp)
            {
                if (item.Count != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo();
        }
    }
}
