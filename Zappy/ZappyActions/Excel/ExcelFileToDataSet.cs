using ExcelDataReader;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Xml.Serialization;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;


namespace Zappy.ZappyActions.Excel
{
    [Description("Gets DataSet From ExcelFile")]
    public class ExcelFileToDataSet : TemplateAction
    {
        [Category("Input")]
        [Description("Excel FileName")]
        public DynamicTextProperty FileName { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Gets DataSet")]
        public DynamicProperty<DataSet> DataSet { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            using (var stream = System.IO.File.Open(FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                                                                var conf = new ExcelDataReader.ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataReader.ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };
                using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                {
                                                                                                                                                                                                                            DataSet = reader.AsDataSet(conf);
                }
            }
                                                
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FileName:" + this.FileName + " DataSet:" + this.DataSet;
        }
    }
}
