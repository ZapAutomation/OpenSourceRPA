using OfficeOpenXml;
using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Excel.ExcelWorkbook
{

                [Description("Create New Worksheet")]
    public class WorksheetCreate : TemplateAction
    {
                                [Category("Input")]
        [Description("Enter WorkbookName")]
        public DynamicProperty<string> WorkbookPath { get; set; }

                                [Category("Input")]
        [Description("Worksheet name")]
        public DynamicProperty<string> WorksheetName { get; set; }

                
                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

                                                
                        var workbookFileInfo = new FileInfo(WorkbookPath);
            using (var excelPackage = new ExcelPackage(workbookFileInfo))
            {
                excelPackage.Workbook.Worksheets.Add(WorksheetName);
                excelPackage.Save();
            }


                        
                                                
                    }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkbookPath + " WorksheetName:" + this.WorksheetName;
        }
    }

}