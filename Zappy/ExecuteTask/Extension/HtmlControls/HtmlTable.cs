using System;
using System.Globalization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlTable : HtmlControl
    {
        public HtmlTable() : this(null)
        {
        }

        public HtmlTable(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Table.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "TABLE");
        }

        public HtmlCell FindFirstCellWithValue(string value)
        {
            HtmlCell cell = new HtmlCell(this)
            {
                SearchProperties = { [HtmlControl.PropertyNames.InnerText] = value }
            };
            cell.Find();
            return cell;
        }

        public HtmlControl GetCell(int rowIndex, int columnIndex)
        {
            HtmlControl uiControl = new HtmlControl(this)
            {
                SearchProperties = {
                    [HtmlCell.PropertyNames.RowIndex] = rowIndex.ToString(CultureInfo.InvariantCulture),
                    [HtmlCell.PropertyNames.ColumnIndex] = columnIndex.ToString(CultureInfo.InvariantCulture),
                    [ZappyTaskControl.PropertyNames.MaxDepth] = "3"
                }
            };
            return GetSpecializedControl(uiControl) as HtmlControl;
        }

        public string[] GetColumnNames()
        {
            ZappyTaskControlCollection rows = Rows;
            if (rows != null && rows.Count > 0)
            {
                HtmlControl control = rows[0] as HtmlControl;
                if ((control != null) && (control.ControlType == ControlType.RowHeader))
                {
                    return control.GetChildren().GetValuesOfControls();
                }
            }
            return null;
        }

        public string[] GetContent()
        {
            ZappyTaskControlCollection cells = Cells;
            if (cells != null)
            {
                return cells.GetValuesOfControls();
            }
            return null;
        }

        public HtmlControl GetRow(int rowIndex)
        {
            HtmlControl uiControl = new HtmlControl(this)
            {
                SearchProperties = {
                    [HtmlRow.PropertyNames.RowIndex] = rowIndex.ToString(CultureInfo.InvariantCulture),
                    [ZappyTaskControl.PropertyNames.MaxDepth] = "2"
                }
            };
            return GetSpecializedControl(uiControl) as HtmlControl;
        }

        public virtual int CellCount =>
            (int)GetProperty(PropertyNames.CellCount);

        public virtual ZappyTaskControlCollection Cells =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Cells);

        public virtual int ColumnCount =>
            (int)GetProperty(PropertyNames.ColumnCount);

        public virtual int RowCount =>
            (int)GetProperty(PropertyNames.RowCount);

        public virtual ZappyTaskControlCollection Rows =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Rows);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string CellCount = "CellCount";
            public static readonly string Cells = "Cells";
            public static readonly string ColumnCount = "ColumnCount";
            public static readonly string RowCount = "RowCount";
            public static readonly string Rows = "Rows";
        }
    }
}

