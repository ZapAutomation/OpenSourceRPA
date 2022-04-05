using OfficeOpenXml;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Excel.ExcelWorkbook
{
                [Description("Create New Workbook ")]
    public class WorkbookCreate : TemplateAction
    {
                                [Description("Enter path and workbookname with extension.")]
        [Category("Input")]
        public DynamicProperty<string> SavePath { get; set; }

                                                [Category("Output")]
        public string WorkbookName { get; set; }
                                [Category("Output")]
        public string SheetName { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string sheetName = "sheet1";
            using (ExcelPackage excelEngine = new ExcelPackage())
            {
                excelEngine.Workbook.Worksheets.Add(sheetName);
                Stream stream = File.Create(SavePath);
                excelEngine.SaveAs(stream);
                stream.Close();

            }
            Process.Start(SavePath);
            WorkbookName = Path.GetFileName(SavePath);
            SheetName = sheetName;

            
                                                                                    
                                                            
                    }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkbookName;
        }
    }
}
