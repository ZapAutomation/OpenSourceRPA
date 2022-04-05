using System;
using System.ComponentModel;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Gets The Excel Focused Cell")]
    public class ExcelSetFocusedCell : TemplateAction
    {
                public ExcelSetFocusedCell()
        {
            ExcelCellInfo = new ExcelCellInfo();
        }

                
        [Category("Input")]
        [Description("RowOffset of Excel")]
        public int RowOffset { get; set; }

        [Category("Input")]
        [Description("ColumnOffset of Excel")]
        public int ColumnOffset { get; set; }

        [Category("Input")]
        [Description("Check about UseFixedRow")]
        public bool UseFixedRow { get; set; }

        [Category("Input")]
        [Description("Information of ExcelCell")]
        public ExcelCellInfo ExcelCellInfo { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            if (!UseFixedRow)
            {
                int i = 0;
                        Retry:
                ExcelCellInfo focusedExcelCellInfo = ExcelCommunicator.Instance.GetFocussedElement() as ExcelCellInfo;
                if (focusedExcelCellInfo != null && focusedExcelCellInfo.EqualSheets(ExcelCellInfo))
                {
                    ExcelCellInfo.RowIndex = focusedExcelCellInfo.RowIndex;
                }
                else
                {
                    if (i < 5)
                    {
                        i++;
                        goto Retry;
                    }
                    throw new Exception("ERR: E335 Set Focus Unable to get focused cell for loop iteration");
                }
            }

            ExcelCellInfo.RowIndex += RowOffset;
            ExcelCellInfo.ColumnIndex += ColumnOffset;

            ExcelCommunicator.Instance.SetFocus(ExcelCellInfo);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " ExcelCellInfo:" + this.ExcelCellInfo;
        }
    }
}