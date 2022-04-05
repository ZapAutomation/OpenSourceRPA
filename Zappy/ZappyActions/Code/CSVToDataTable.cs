using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
                                
    [Description("Convert CSV file to Data Table")]
    public class CSVToDataTable : TemplateAction
    {
                                [Category("Input")]
        [Description("CSV FilePath")]
        public DynamicTextProperty FilePath { get; set; }

                                [Category("Optional")]
        public bool IsFirstRowHeader { get; set; }

                                [Category("Output"), XmlIgnore]
        [Description("Stores values into DataTable from CSVFile")]
        public DynamicProperty<DataTable> DataTable { get; set; }


                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string __FileName = FilePath;
            DataTable = GetDataTableFromCsv(__FileName, IsFirstRowHeader);
        }

                                                                DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }


        public override string AuditInfo()
        {
            return base.AuditInfo() + " File Name:" + this.FilePath + " Input Row Header in boolean value:" + this.IsFirstRowHeader + " Output Data table:" + this.DataTable;
        }
    }

}
