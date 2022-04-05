using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
    [Description("Perform Copy and Paste action on DataTable")]
    public class DataTableSendKeysAction : TemplateAction
    {
        [Category("Input")]
        [Description("Enter 'copy' to copy value from datatable or Enter 'paste' to paste value in datatable")]
        public DynamicProperty<string> Text { get; set; }

                                [Category("Input")]
        [Description("Row index of DataTable")]
        public DynamicProperty<int> RowIndex { get; set; }

                                [Category("Input")]
        [Description("Column index of DataTable")]
        public DynamicProperty<int> ColumnIndex { get; set; }

                                [Category("Input")]
        [Description("DataTable")]
        public DynamicProperty<DataTable> DataTable { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            EnumerableRowCollection<DataRow> tableEnumerable = DataTable.Value.AsEnumerable();
            DataRow[] tableArray = tableEnumerable.ToArray();
            string clipBoardChar = string.Empty;

            if (Text == "copy")
            {
                int _RowIndex = RowIndex - 1;
                int _ColumnIndex = ColumnIndex - 1;
                string _PropertyValue = tableArray[_RowIndex][_ColumnIndex].ToString();
                if (checkNumber(_PropertyValue))
                {
                    if (_PropertyValue != null)
                    {
                        CommonProgram.SetTextInClipboard(_PropertyValue);
                    }
                }
                else if (CheckDate(_PropertyValue))
                {
                    System.DateTime frmdt = Convert.ToDateTime(_PropertyValue);
                    string frmdtString = frmdt.ToString("dd-MM-yyyy");
                    if (frmdtString != null)
                    {
                        CommonProgram.SetTextInClipboard(frmdtString);
                    }
                }
                else
                {
                    if (_PropertyValue != null) CommonProgram.SetTextInClipboard(_PropertyValue);
                }
            }
            else if (Text == "paste")
            {
                string _Result = string.Empty;
                int _RowIndex = RowIndex - 1;
                int _ColumnIndex = ColumnIndex - 1;
                int count = DataTable.Value.Rows.Count;
                if (count < RowIndex)
                {
                    for (int i = count; i < RowIndex; i++)
                    {
                        DataTable.Value.Rows.Add();
                    }
                }
                if (!CommonProgram.GetTextFromClipboard(out clipBoardChar))
                    throw new Exception("Unable to read clipboard contents!");
                tableArray[_RowIndex][_ColumnIndex] = clipBoardChar;
                _Result = "Cannot read Task";
            }
            else
            {
                throw new Exception("Not Supported Command");
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
            return base.AuditInfo() + " Text: " + this.Text + " Row Index:" + this.RowIndex + " Column Index: " + this.ColumnIndex;
        }
    }
}

