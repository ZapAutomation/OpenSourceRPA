using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
    [Description("Get Json File from data table")]
    public class DataTableToJsonFile : TemplateAction
    {
        [Category("Input")]
        [Description("DataTable")]
        public DynamicProperty<DataTable> DataTable { get; set; }

        [Description("The Json file path to convert the data table to Json file")]
        [Category("Input")]
        public DynamicProperty<string> JsonFilePath { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        DataTable = RemoveUnusedColumnsAndRows(DataTable);
            string data=DataTableToJsonObj(DataTable);           
            File.WriteAllText(JsonFilePath, data);
        }
       
        DataTable RemoveUnusedColumnsAndRows(DataTable table)
        {
            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                if (table.Rows[i][1] == DBNull.Value)
                    table.Rows[i].Delete();
            }
            table.AcceptChanges();
            foreach (var column in table.Columns.Cast<DataColumn>().ToArray())
            {
                if (table.AsEnumerable().All(dr => dr.IsNull(column)))
                    table.Columns.Remove(column);
            }
            table.AcceptChanges();
            return table;
        }
        public string DataTableToJsonObj(DataTable dt)
        {
            DataSet ds = new DataSet();
            ds.Merge(dt);
            StringBuilder JsonString = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                JsonString.Append("["+ Environment.NewLine);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    
                    JsonString.Append("{"+ Environment.NewLine);
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][j].ToString()))
                        {
                                                        if (j < ds.Tables[0].Columns.Count - 1)
                            {
                                JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\"," + Environment.NewLine);
                            }
                            else if (j == ds.Tables[0].Columns.Count - 1)
                            {
                                JsonString.Append("\"" + ds.Tables[0].Columns[j].ColumnName.ToString() + "\":" + "\"" + ds.Tables[0].Rows[i][j].ToString() + "\"" + Environment.NewLine);
                            }
                        }
                        else
                        {

                        }
                    }
                    if (i == ds.Tables[0].Rows.Count - 1)
                    {
                        JsonString.Append("}"+ Environment.NewLine);
                    }
                    else
                    {
                        JsonString.Append("},"+ Environment.NewLine);
                    }
                }
                JsonString.Append("]"+ Environment.NewLine);
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
