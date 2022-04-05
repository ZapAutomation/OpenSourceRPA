using System;
using System.Globalization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlRow : HtmlControl
    {
        public HtmlRow() : this(null)
        {
        }

        public HtmlRow(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Row.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "TR");
        }

        public HtmlCell GetCell(int cellIndex)
        {
            HtmlCell cell = new HtmlCell(this)
            {
                SearchProperties = {
                    [HtmlCell.PropertyNames.ColumnIndex] = cellIndex.ToString(CultureInfo.InvariantCulture),
                    [ZappyTaskControl.PropertyNames.MaxDepth] = "2"
                }
            };
            cell.Find();
            return cell;
        }

        public string[] GetContent()
        {
            ZappyTaskControlCollection cells = Cells;
            if (cells != null)
            {
                            }
            return null;
        }

        public virtual int CellCount =>
            (int)GetProperty(PropertyNames.CellCount);

        public virtual ZappyTaskControlCollection Cells =>
            (ZappyTaskControlCollection)GetProperty(PropertyNames.Cells);

        public virtual int RowIndex =>
            (int)GetProperty(PropertyNames.RowIndex);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string CellCount = "CellCount";
            public static readonly string Cells = "Cells";
            public static readonly string RowIndex = "RowIndex";
        }
    }
}

