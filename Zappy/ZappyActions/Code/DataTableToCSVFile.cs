using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Zappy.Helpers;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
                                
    [Description("Gets CSV File From DataTable")]
    public class DataTableToCSVFile : TemplateAction
    {

        public DataTableToCSVFile()
        {
            SaveToFile = true;
        }

                                [Category("Optional")]
        [Description("The CSV file path to convert the data table to CSV file")]
        public DynamicTextProperty FileName { get; set; }

                                [Category("Optional")]
        [Description("True, if user wants to save csv file; otherwise,false")]
        public DynamicProperty<bool> SaveToFile { get; set; }

                                [Category("Optional")]
        [Description("Is First Row Header")]
        public DynamicProperty<bool> IsFirstRowHeader { get; set; }

                                [Category("Optional")]
        [Description("Append text to CSV file")]
        public DynamicProperty<bool> Append { get; set; }

                                [Category("Input")]
        [Description("DataTable")]
        public DynamicProperty<DataTable> DataTable { get; set; }

                                [Category("Output")]
        [Description("Return the CSV file from the DataTable")]
        public string DataTableInCSVString { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            DataTableInCSVString = WriteDataTableToCsv(DataTable);
            if (SaveToFile)
            {
                if (string.IsNullOrEmpty(FileName))
                    FileName = Path.Combine(CrapyConstants.TempFolder, "temp.csv");
                string __FileName = FileName;
                string _DirPath = Path.GetDirectoryName(__FileName);

                if (!Directory.Exists(_DirPath))
                    Directory.CreateDirectory(_DirPath);

                if (DataTable.Value != null && !string.IsNullOrEmpty(__FileName) && DataTable.Value.Columns.Count > 0)
                {
                    if (Append)
                        File.AppendAllText(__FileName, DataTableInCSVString);
                    else
                    {
                        File.WriteAllText(__FileName, DataTableInCSVString);
                    }
                }
            }
        }

                                string WriteDataTableToCsv(DataTable dtDataTable)
        {
            StringBuilder sb = new StringBuilder();
                        {                  int _ColumnCount = dtDataTable.Columns.Count;
                int __ColumnCountMinus_1 = _ColumnCount - 1;

                if (IsFirstRowHeader)
                {
                    for (int i = 0; i < _ColumnCount; i++)
                    {
                        sb.Append(dtDataTable.Columns[i]);
                        if (i < __ColumnCountMinus_1)
                            sb.Append(",");
                    }
                    sb.Append(Environment.NewLine);
                }

                foreach (DataRow dr in dtDataTable.Rows)
                {
                    for (int i = 0; i < __ColumnCountMinus_1; i++)
                    {
                        WriteColumnValue(sb, dr, i);
                        sb.Append(",");
                    }
                                        WriteColumnValue(sb, dr, __ColumnCountMinus_1);
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

                                                        void WriteColumnValue(StringBuilder sb, DataRow dr, int ColumnIndex)
        {
            if (!Convert.IsDBNull(dr[ColumnIndex]))
            {
                string value = dr[ColumnIndex].ToString();
                if (value.Contains(','))
                    value = string.Format("\"{0}\"", value);
                sb.Append(value);
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Input FileName" + this.FileName + " Input Append boolean value:" + this.Append + " Input Save file in boolean value" + this.SaveToFile;
        }
    }

}
