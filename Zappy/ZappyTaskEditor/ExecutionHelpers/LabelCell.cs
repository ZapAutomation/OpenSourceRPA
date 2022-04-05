using System.Drawing;
using System.Windows.Forms;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public sealed class LabelCell : DataGridViewTextBoxCell
    {
        public override bool ReadOnly => true;

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            formattedValue = formattedValue.ToString().Humanize();
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }
    }
}