using OfficeOpenXml;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Zappy.Helpers;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Excel
{
                                    [Description("Gets Excel File From CSVFile")]
    public class CSVFileToExcel : TemplateAction
    {
        public CSVFileToExcel()
        {
            OpenFileInExcel = true;
            Delimiter = ',';
            TextQualifier = '"';
        }

                                [Category("Input")]
        [Description("CSVFile name")]
        public DynamicTextProperty FileName { get; set; }

                        
                                [Category("Optional")]
        public DynamicProperty<bool> OpenFileInExcel { get; set; }

        [Category("Optional")]
        public DynamicProperty<char> Delimiter { get; set; }

        [Category("Optional")]
        public DynamicProperty<char> TextQualifier { get; set; }


                                [Category("Output")]
        [Description("Gets WorkbookName from CSVFile ")]
        public string WorkbookName { get; set; }

                                [Category("Output")]
        [Description("Gets SheetName from Workbook ")]
        public string SheetName { get; set; }

                                [Category("Output")]
        [Description("Path of the temporary created excel file")]
        public string TempExcelFile { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string file = FileName;
            string sheetname = "Sheet1";
            string OutputPath = OpenFileInExcel.ToString();
            string filename = Path.GetExtension(OutputPath);
            if (string.IsNullOrEmpty(filename))
            {
                TempExcelFile = Path.Combine(CrapyConstants.TempFolder, Path.GetFileNameWithoutExtension(FileName) + ".xlsx");
            }
            else
            {
                TempExcelFile = OutputPath;
            }
            ConvertWithEPPlus(FileName, TempExcelFile, sheetname);
            Process.Start(TempExcelFile);

            WorkbookName = Path.GetFileName(TempExcelFile);
            SheetName = sheetname;
        }

        private void ConvertWithEPPlus(string csvFileName, string excelFileName, string worksheetName, char delimiter = ',')
        {
            
            var format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = Delimiter;            format.TextQualifier = TextQualifier;                        format.DataTypes = new[]
            {
                eDataTypes.String, eDataTypes.String,
                eDataTypes.String, eDataTypes.String, eDataTypes.String,
                eDataTypes.String, eDataTypes.String, eDataTypes.String,
                eDataTypes.String, eDataTypes.String, eDataTypes.String,
                eDataTypes.String, eDataTypes.String, eDataTypes.String
            };

            using (ExcelPackage package = new ExcelPackage(new FileInfo(excelFileName)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);
                worksheet.Cells["A1"].LoadFromText(new FileInfo(csvFileName), format);                worksheet.Cells.AutoFitColumns();
                package.Save();
            }
        }

                                                                public override string AuditInfo()
        {
            return base.AuditInfo() + " File name:" + this.FileName;
        }
    }
}