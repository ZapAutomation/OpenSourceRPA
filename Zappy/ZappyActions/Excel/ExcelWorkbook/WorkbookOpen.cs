using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel.ExcelWorkbook
{
                [Description("Open Existing Workbook")]
    public class WorkbookOpen : TemplateAction
    {
        public WorkbookOpen()
        {
            WorkbookOpenDelay = 8000;
        }
                                [Category("Input")]
        [Description("Enter the workbook path with extension")]
        public DynamicProperty<string> WorkbookPath { get; set; }

                                [Description("Enter time to open an workbook")]
        [Category("Input")]
        [DefaultValue("8000")]
        public DynamicProperty<int> WorkbookOpenDelay { get; set; }

        [Category("Output")]
        [Description("It will return an open workbook name")]
        public string WorkbookName { get; set; }

        [Category("Output")]
        [Description("It will return an open sheet name")]
        public string SheetName { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string path = WorkbookPath;
            Process.Start(path);
            Thread.Sleep(WorkbookOpenDelay);
                                    WorkbookName = Path.GetFileName(path);
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(1, 1, new ExcelWorksheetInfo("", WorkbookName));
            ExcelCellInfo temp =
                ExcelCommunicator.Instance.RunExcelCustomAction(excelCellInfo, ExcelCustomPropertyNames.FindSheetName.ToString(), "") as ExcelCellInfo;
            SheetName = temp.SheetName;


            
                                                            
            
            
                                                
        }

                                                                                        
        public override string AuditInfo()
        {
            return base.AuditInfo() + " WorkbookPath:" + this.WorkbookPath;
        }
    }
}