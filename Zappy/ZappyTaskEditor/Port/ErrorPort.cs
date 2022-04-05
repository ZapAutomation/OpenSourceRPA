using System.Drawing;
using Zappy.ZappyTaskEditor.EditorPage;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.Port
{
    public sealed class ErrorPort : Port
    {
        public override Point GetOffset(Rect r) => r.CenterLeft;

        public override Brush GetBackBrush() => new SolidBrush(Color.FromArgb(100, Color.Gray));

        internal override void UpdateBounds(Rectangle r)
        {
            var portPoint = this.GetOffset(r);
            var portSize = new Size(PageRenderOptions.GridSize / 2, PageRenderOptions.GridSize / 2);
            portPoint.Offset(-portSize.Width / 2, -portSize.Height / 2);
            var portBounds = new Rectangle(portPoint, portSize);
            this.Bounds = portBounds;
        }
    }
}