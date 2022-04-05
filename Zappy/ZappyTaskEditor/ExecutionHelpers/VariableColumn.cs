using System.Windows.Forms;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public sealed class VariableColumn : DataGridViewComboBoxColumn
    {
        public override bool ReadOnly
        {
            get => base.ReadOnly;
            set
            {
                if (!value) this.HeaderCell.Style.ForeColor = System.Drawing.Color.Blue;
                base.ReadOnly = value;
            }
        }

        public VariableColumn()
        {
            this.CellTemplate = new VariableCell();
            this.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.DisplayStyleForCurrentCellOnly = true;
        }
    }
}