using System.ComponentModel;
using System.Data;
using System.Linq;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
                [Description("Set value in DataTable")]
    public class DataTableSetProperty : TemplateAction
    {
        public DataTableSetProperty()
        {
            RowIndex = null;
            ColumnIndex = null;

        }
                                [Category("Input")]
        [Description("RowIndex of DataTable")]
        public DynamicProperty<int> RowIndex { get; set; }

                                [Category("Input")]
        [Description("ColumnIndex of DataTable")]
        public DynamicProperty<int> ColumnIndex { get; set; }

                                        [Category("Input")]
        [Description("DataTable")]
        public DynamicProperty<DataTable> DataTable { get; set; }

                                [Category("Input")]
        [Description("The value to set in the datatable")]
        public DynamicProperty<string> PropertyValue { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string _Result = string.Empty;
            int _RowIndex = RowIndex - 1;
            int _ColumnIndex = ColumnIndex - 1;
            int count=DataTable.Value.Rows.Count;
            if(count< RowIndex)
            {
                for (int i = count; i < RowIndex; i++)
                {
                    DataTable.Value.Rows.Add();
                }
            }
   
            EnumerableRowCollection<DataRow> tableEnumerable = DataTable.Value.AsEnumerable();
            DataRow[] tableArray = tableEnumerable.ToArray();
            tableArray[_RowIndex][_ColumnIndex] = PropertyValue;
            _Result = "Cannot read Task";
        }       

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Row index of DataTable: " + this.RowIndex + " Column index of DataTable: " + this.ColumnIndex + " PropertyValue: " + this.PropertyValue;
        }

    }
}
