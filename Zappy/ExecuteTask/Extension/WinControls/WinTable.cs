using System;
using System.Globalization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WinControls
{
    [CLSCompliant(true)]
    public class WinTable : WinControl
    {
        public WinTable() : this(null)
        {
        }

        public WinTable(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Table.Name);
        }

        public WinCell FindFirstCellWithValue(string value)
        {
            WinCell cell = new WinCell(this)
            {
                SearchProperties = { [WinCell.PropertyNames.Value] = value }
            };
            cell.Find();
            return cell;
        }

        public WinCell GetCell(int rowIndex, int columnIndex)
        {
            WinCell cell = new WinCell(this)
            {
                SearchProperties = {
                    [WinCell.PropertyNames.RowIndex] = rowIndex.ToString(CultureInfo.InvariantCulture),
                    [WinCell.PropertyNames.ColumnIndex] = columnIndex.ToString(CultureInfo.InvariantCulture)
                }
            };
            cell.Find();
            return cell;
        }

        public string[] GetColumnNames()
        {
            ZappyTaskControlCollection columnHeaders = ColumnHeaders;
            if (columnHeaders != null)
            {
                return columnHeaders.GetNamesOfControls();
            }
            return null;
        }

        public string[] GetContent()
        {
            ZappyTaskControlCollection cells = Cells;
            if (cells != null)
            {
                            }
            return null;
        }

        public WinRow GetRow(int rowIndex)
        {
            WinRow row = new WinRow(this)
            {
                SearchProperties = { [WinRow.PropertyNames.RowIndex] = rowIndex.ToString(CultureInfo.InvariantCulture) }
            };
            row.Find();
            return row;
        }

        public virtual ZappyTaskControlCollection Cells =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Cells);

        public virtual ZappyTaskControlCollection ColumnHeaders =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.ColumnHeaders);

        public virtual ZappyTaskControl HorizontalScrollBar =>
            (ZappyTaskControl)GetProperty(PropertyNames.HorizontalScrollBar);

        public virtual ZappyTaskControlCollection RowHeaders =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.RowHeaders);

        public virtual ZappyTaskControlCollection Rows =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Rows);

        public virtual ZappyTaskControl VerticalScrollBar =>
            (ZappyTaskControl)GetProperty(PropertyNames.VerticalScrollBar);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WinControl.PropertyNames
        {
            public static readonly string Cells = "Cells";
            public static readonly string ColumnHeaders = "ColumnHeaders";
            public static readonly string HorizontalScrollBar = "HorizontalScrollBar";
            public static readonly string RowHeaders = "RowHeaders";
            public static readonly string Rows = "Rows";
            public static readonly string VerticalScrollBar = "VerticalScrollBar";
        }
    }
}

