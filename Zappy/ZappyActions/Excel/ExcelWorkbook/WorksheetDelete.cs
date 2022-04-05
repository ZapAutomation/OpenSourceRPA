using OfficeOpenXml;
using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
namespace Zappy.ZappyActions.Excel.ExcelWorkbook
{
                [Description("Delete Worksheet")]
    public class WorksheetDelete : TemplateAction
    {
                                                        [Category("Input")]
        [Description("Workbook file path")]
        public DynamicProperty<string> WorkBookPath { get; set; }
                                [Category("Input")]
        [Description("Enter worksheet name to delete")]
        public DynamicProperty<string> WorksheetName { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            var workbookFileInfo = new FileInfo(WorkBookPath);
            using (var excelPackage = new ExcelPackage(workbookFileInfo))
            {
                excelPackage.Workbook.Worksheets.Delete(WorksheetName);
                excelPackage.Save();
            }

                    }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookName:" + this.WorkBookPath + " WorksheetName:" + this.WorksheetName;
        }
    }
}
