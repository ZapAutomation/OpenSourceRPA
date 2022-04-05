using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public sealed class EndNode : Node
    {
        private EndNode()
        {
                    }

        internal EndNode(IZappyAction activity) : base(activity)
        {
            this.Ports.Clear();
        }

        public override bool CanStartLink => false;

        public override GraphicsPath Render(Graphics g, Rect r, NodeStyle o)
        {
            var leftRect = new Rectangle(r.X, r.Y, r.Height, r.Height);
            var rightRect = new Rectangle(r.Right - r.Height, r.Y, r.Height, r.Height);
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(leftRect, 90, 180);
            path.AddArc(rightRect, -90, 180);
            path.CloseFigure();
                        g.FillPath(o.BackBrush, path);
            g.DrawPath(o.BorderPen, path);
            return path;
        }
    }
}
