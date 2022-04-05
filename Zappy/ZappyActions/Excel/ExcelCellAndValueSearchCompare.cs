using System;
using System.ComponentModel;
using Zappy.Plugins.Excel;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
                [TypeConverter(typeof(ExpandableObjectConverter))]

    [Description("Compare ExcelCell And SearchValue")]
    public class ExcelCellAndValueSearchCompare
    {
                                [Category("Input")]
        [Description("SheetName of opened workbook for comparing ExcelCell and SearchValue ")]
        public string SheetName { get; set; }

                                [Description("WorkbookName for comparing values")]
        [Category("Input")]
        public string WorkbookName { get; set; }

                                [Description("Search Column Index ")]
        [Category("Input")]
        public int ColumnIndexSearch { get; set; }

                                [Description("Compare Column Index")]
        [Category("Input")]
        public int ColumnIndexCompare { get; set; }

                                [Description("Gets Row Index")]
        [Category("Input")]
        public int Row { get; set; }

                                [Description("Compare Value")]
        [Category("Output")]
        public String ValueCompare { get; set; }

                                [Description("ValueSearch with ExcellCell")]
        [Category("Output")]
        public String ValueSearch { get; set; }


                                                public void UpdateValue()
        {
            ExcelCellInfo excelCellInfo =
                new ExcelCellInfo(Row, ColumnIndexSearch, new ExcelWorksheetInfo(SheetName, WorkbookName));
            ValueSearch = (ExcelCommunicator.Instance.GetCellProperty
                (excelCellInfo, PropertyNames.PropertyNameEnum.Text.ToString()) as string)?.Trim(' ');
            excelCellInfo.ColumnIndex = ColumnIndexCompare;
            ValueCompare = (ExcelCommunicator.Instance.GetCellProperty
                (excelCellInfo, PropertyNames.PropertyNameEnum.Text.ToString()) as string)?.Trim(' ');

        }

        public override string ToString()
        {
            return "SheetName " + SheetName + " WorkbookName " + WorkbookName + " ValueSearch " + ValueSearch
                   + " ValueCompare " + ValueCompare + " ColumnIndexSearch " + ColumnIndexSearch +
            " ColumnIndexCompare " + ColumnIndexCompare + " Row " + Row;
        }
    }
}