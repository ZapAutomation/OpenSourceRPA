using System.ComponentModel;
using System.Data;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
                [Description("Gets last used row and column index of datatable")]
    public class DataTableLastRowColumn : TemplateAction
    {
                
                                [Category("Input")]
        [Description("DataTable")]
        public DynamicProperty<DataTable> DataTable { get; set; }

                                [Category("Output")]
        [Description("Return the last used row index of data table")]
        public int LastRow { get; set; }

                                [Category("Output")]
        [Description("Return the last used column index of data table")]
        public int LastColumn { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        int _RowCount = DataTable.Value.Rows.Count;

            int count = 0;

            for (int i = 0; i < _RowCount - 1; i++)
            {

                if (!(DataTable.Value.Rows[i][0] != null && string.IsNullOrEmpty(DataTable.Value.Rows[i][0].ToString())))
                {
                    count++;
                }
                else if (!(DataTable.Value.Rows[i][1] != null && string.IsNullOrEmpty(DataTable.Value.Rows[i][1].ToString())))
                {
                    count++;
                }
                else if (!(DataTable.Value.Rows[i + 1][0] != null && string.IsNullOrEmpty(DataTable.Value.Rows[i + 1][0].ToString())))
                {
                    count++;
                }
                else if (!(DataTable.Value.Rows[i + 1][1] != null && string.IsNullOrEmpty(DataTable.Value.Rows[i + 1][1].ToString())))
                {
                    count++;
                }

            }
            LastRow = count + 1;



                                                int _columnCount = DataTable.Value.Columns.Count;


            LastColumn = _columnCount;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " LastRow: " + this.LastRow + " LastColumn: " + this.LastColumn;
        }
    }
}
