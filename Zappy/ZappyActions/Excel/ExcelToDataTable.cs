using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using ExcelDataReader;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Excel
{
                    
    [Description("Gets DataTable From Excel File")]
    public class ExcelToDataTable : TemplateAction
    {
        [Category("Input")]
        [Description("Excel file path")]
        public DynamicProperty<string> ExcelFilePath { get; set; }

        [Category("Input")]
        [Description("Sheet name")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Optional")]
        [Description("Use ExcelDataReader")]
        public DynamicProperty<bool> UseExcelDataReader { get; set; }

                                [Category("Output")]
        [Description("Return DataTable from the Excel file")]
        public DataTable DataTable { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (UseExcelDataReader)
                DataTable = ExtractDataUsingExcelDataReader(SheetName, ExcelFilePath);
            else
                DataTable = ReadExcelFileWithMSOLEDB(SheetName, ExcelFilePath);

        }

        private DataTable ExtractDataUsingExcelDataReader(string sheetName, string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;

                reader = ExcelReaderFactory.CreateReader(stream);
                var conf = new ExcelDataSetConfiguration
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };
                                DataSet dataSet = reader.AsDataSet(conf);
                                if(string.IsNullOrEmpty(sheetName))
                {
                    return dataSet.Tables[0];
                }
                else
                    return dataSet.Tables[sheetName];
            }
        }

                                                                        private DataTable ReadExcelFileWithMSOLEDB(string sheetName, string path)
        {
                        using (OleDbConnection conn = new OleDbConnection())
            {
                DataTable dt = new DataTable();
                string Import_FileName = path;
                string fileExtension = Path.GetExtension(Import_FileName);
                                                                conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                using (OleDbCommand comm = new OleDbCommand())
                {
                    comm.CommandText = "Select * from [" + sheetName + "$]";
                    comm.Connection = conn;
                    using (OleDbDataAdapter da = new OleDbDataAdapter())
                    {
                        da.SelectCommand = comm;
                        da.Fill(dt);
                        return dt;                       }
                }
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Excel File Path:" + this.ExcelFilePath + " Output" + this.DataTable;
        }
     
    }
}