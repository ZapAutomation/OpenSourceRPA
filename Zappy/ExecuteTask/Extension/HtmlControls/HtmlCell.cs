using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlCell : HtmlControl
    {
        public HtmlCell() : this(null)
        {
        }

        public HtmlCell(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Cell.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "TD");
        }

        public virtual int ColumnIndex =>
            (int)GetProperty(PropertyNames.ColumnIndex);

        public virtual int RowIndex =>
            (int)GetProperty(PropertyNames.RowIndex);

        public virtual string Value =>
            (string)GetProperty(PropertyNames.Value);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string ColumnIndex = "ColumnIndex";
            public static readonly string RowIndex = "RowIndex";
            public static readonly string Value = "Value";
        }
    }
}

