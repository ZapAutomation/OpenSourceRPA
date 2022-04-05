using System.ComponentModel;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Gets Focused of ExcelCell")]
    public class ExcelGetFocusedCell : TemplateAction
    {
                public ExcelGetFocusedCell()
        {
        }

                [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Output")]
        [Description("ExcelCellInfo")]
        public ExcelCellInfo ExcelCellInfo { get; set; }

        [Category("Output")]
        public int FocusedRow { get; set; }

        [Category("Output")]
        public int FocusedColumn { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            ExcelCellInfo = ExcelCommunicator.Instance.GetFocussedElement() as ExcelCellInfo;
            FocusedRow = ExcelCellInfo.RowIndex;
            FocusedColumn = ExcelCellInfo.ColumnIndex;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " ExcelCellInfo " + ExcelCellInfo.ToString();
        }
    }
}