using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlHeaderCell : HtmlControl
    {
        public HtmlHeaderCell() : this(null)
        {
        }

        public HtmlHeaderCell(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Cell.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "TH");
        }

        public virtual int ColumnIndex =>
            (int)GetProperty(HtmlCell.PropertyNames.ColumnIndex);

        public virtual int RowIndex =>
            (int)GetProperty(HtmlCell.PropertyNames.RowIndex);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string ColumnIndex = "ColumnIndex";
            public static readonly string RowIndex = "RowIndex";
        }
    }
}

