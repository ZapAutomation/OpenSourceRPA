using Microsoft.Office.Interop.Excel;
using System.ComponentModel;
using System.Threading;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Run Excel Macro")]
    public class MacroRun : ExcelBaseAction
    {

        [Category("Input")]
        public DynamicProperty<string> MacroName { get; set; }

                
                
                
                
                
        [Category("Output")]
        public string Result { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                                                                                                                                                                                                                                                            
                        ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(1, 1, new ExcelWorksheetInfo(SheetName, WorkbookName));
            ExcelCellInfo TempResult = ExcelCommunicator.Instance.RunExcelCustomAction(excelCellInfo,
                ExcelCustomPropertyNames.RunMacro.ToString(), MacroName.ToString()) as ExcelCellInfo;
            Result = TempResult.SheetName;
        }

        

        public override string AuditInfo()
        {
            return base.AuditInfo() + " MacroName: " + this.MacroName;        }
    }
}
