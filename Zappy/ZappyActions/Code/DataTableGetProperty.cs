using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
                [Description("Gets value from Data Table")]
    public class DataTableGetProperty : TemplateAction
    {
                                [Category("Input")]
        [Description("RowIndex of DataTable")]
        public DynamicProperty<int> RowIndex { get; set; }

                                [Category("Input")]
        [Description("ColumnIndex of DataTable")]
        public DynamicProperty<int> ColumnIndex { get; set; }

                                [Category("Input")]
        [Description("DataTable")]
        public DynamicProperty<DataTable> DataTable { get; set; }

                                [Category("Output")]
        [Description("Gets Cell Value from DataTable")]
        public string PropertyValue { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            int _RowIndex = RowIndex - 1;
            int _ColumnIndex = ColumnIndex - 1;
            EnumerableRowCollection<DataRow> tableEnumerable = DataTable.Value.AsEnumerable();
            DataRow[] tableArray = tableEnumerable.ToArray();
            string clipBoardChar = string.Empty;
            string _PropertyValue = tableArray[_RowIndex][_ColumnIndex].ToString();
                        if (checkNumber(_PropertyValue))
            {
                PropertyValue = _PropertyValue;
            }
            else if (CheckDate(_PropertyValue))
            {
                System.DateTime frmdt = Convert.ToDateTime(_PropertyValue);
                string frmdtString = frmdt.ToString("dd-MM-yyyy");
                PropertyValue = frmdtString;
            }
            else
            {
                PropertyValue = _PropertyValue;
            }
        }
        private bool CheckDate(String date)
        {
            try
            {
                System.DateTime dt = System.DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool checkNumber(String num)
        {
            try
            {
                double number = Convert.ToDouble(num);
                double.TryParse("141241", out number);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " RowIndex: " + this.RowIndex + " ColumnIndex: " + this.ColumnIndex + " PropertyValue: " + this.PropertyValue;
        }
    }
}
