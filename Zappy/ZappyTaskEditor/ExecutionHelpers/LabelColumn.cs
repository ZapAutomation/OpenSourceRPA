using System.Windows.Forms;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public sealed class LabelColumn : DataGridViewTextBoxColumn
    {
        public override bool ReadOnly => true;

        public LabelColumn()
        {
            this.CellTemplate = new LabelCell();
            this.SortMode = DataGridViewColumnSortMode.NotSortable;
        }
    }
}