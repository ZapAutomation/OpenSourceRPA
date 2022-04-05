using System;
using System.Globalization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.WpfControls
{
    [CLSCompliant(true)]
    public class WpfTable : WpfControl
    {
        public WpfTable() : this(null)
        {
        }

        public WpfTable(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Table.Name);
        }

        public WpfCell FindFirstCellWithValue(string value)
        {
            if (RowCount != 0)
            {
                for (ZappyTaskControl control = GetRow(0); control != null; control = control.NextSibling)
                {
                    WpfCell cell2 = new WpfCell(control)
                    {
                        SearchProperties = { [WpfCell.PropertyNames.Value] = value }
                    };
                    if (cell2.TryFind())
                    {
                        return cell2;
                    }
                }
            }
            WpfCell cell = new WpfCell
            {
                SearchProperties = { {
                    ZappyTaskControl.PropertyNames.Value,
                    value
                } }
            };
            throw new Exception();        }

        public WpfCell GetCell(int rowIndex, int columnIndex)
        {
            WpfCell cell = new WpfCell(this)
            {
                SearchProperties = {
                    [WpfCell.PropertyNames.RowIndex] = rowIndex.ToString(CultureInfo.InvariantCulture),
                    [WpfCell.PropertyNames.ColumnIndex] = columnIndex.ToString(CultureInfo.InvariantCulture)
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
                return cells.GetValuesOfControls();
            }
            return null;
        }

        public WpfRow GetRow(int rowIndex)
        {
            WpfRow row = new WpfRow(this)
            {
                SearchProperties = { [WpfRow.PropertyNames.RowIndex] = rowIndex.ToString(CultureInfo.InvariantCulture) }
            };
            row.Find();
            return row;
        }

        public virtual bool CanSelectMultiple =>
            (bool)GetProperty(PropertyNames.CanSelectMultiple);

        public virtual ZappyTaskControlCollection Cells =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Cells);

        public virtual int ColumnCount =>
            (int)GetProperty(PropertyNames.ColumnCount);

        public virtual ZappyTaskControlCollection ColumnHeaders =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.ColumnHeaders);

        public virtual int RowCount =>
            (int)GetProperty(PropertyNames.RowCount);

        public virtual ZappyTaskControlCollection RowHeaders =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.RowHeaders);

        public virtual ZappyTaskControlCollection Rows =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Rows);

        [CLSCompliant(true)]
        public abstract class PropertyNames : WpfControl.PropertyNames
        {
            public static readonly string CanSelectMultiple = "CanSelectMultiple";
            public static readonly string Cells = "Cells";
            public static readonly string ColumnCount = "ColumnCount";
            public static readonly string ColumnHeaders = "ColumnHeaders";
            internal static readonly string IsGroupedTable = "IsGroupedTable";
            public static readonly string RowCount = "RowCount";
            public static readonly string RowHeaders = "RowHeaders";
            public static readonly string Rows = "Rows";
        }
    }
}

