using System;
using System.ComponentModel;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    [Description("Split Excel Columns")]
    public class ExcelColumnSplitAction : TemplateAction, IExcelAction
    {
        public ExcelColumnSplitAction()
        {
                        Separater = new string[] { Environment.NewLine };
        }

        [Category("Input")]
        [Description("Type of Excel property")]
        public PropertyNames.PropertyNameEnum ExcelPropertyType { get; set; }

        [Category("Input")]
        [Description("RowIndex of Excel")]
        public DynamicProperty<int> RowIndex { get; set; }

        [Category("Input")]
        [Description("ColumnIndex of Excel")]
        public DynamicProperty<int> ColumnIndex { get; set; }

        [Category("Input")]
        [Description("Excel SheetName")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description("Excel WorkbookName")]
        public DynamicProperty<string> WorkbookName { get; set; }

                [Category("Input")]
        [Description("set one or more String Separater but you can split string in enter so type {Enter} ")]
        public string[] Separater { get; set; }

        [Category("Input")]
        [Description("Index")]
        public DynamicProperty<int> Index { get; set; }

        [Category("Output")]
        [Description("PropertyValue of spliting column")]
        public string PropertyValue { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ExcelCellInfo excelCellInfo =
                 new ExcelCellInfo(RowIndex, ColumnIndex, new ExcelWorksheetInfo(SheetName, WorkbookName));
            string value =
                        ExcelCommunicator.Instance.GetCellProperty(excelCellInfo, PropertyNames.PropertyNameEnum.Text.ToString()).ToString();
            string[] lines = value.Split(Separater, StringSplitOptions.None);
            PropertyValue = lines[Index];
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Row Index of excel:" + this.RowIndex + " Column Index of excel:" + ColumnIndex + " SheetName: " + this.SheetName + " WorkbookName:" + this.WorkbookName + " Separater:" + this.Separater + " Index:" + this.Index + " PropertyValue:" + this.PropertyValue;
        }
    }
}
