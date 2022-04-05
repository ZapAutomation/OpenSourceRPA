using System.Drawing;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.Port
{
    public sealed class FalsePort : Port
    {
        public override Point GetOffset(Rect r) => r.CenterRight;

        public override Brush GetBackBrush() => new SolidBrush(Color.Red);
    }
}