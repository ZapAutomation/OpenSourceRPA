using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.Helper;
using Zappy.ZappyTaskEditor.Port;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public sealed class StartNode : Node
    {
        
        private StartNode()
        {
                    }

        public StartNode(IZappyAction activity) : base(activity)
        {
            this.Ports.Add(new NextPort());
        }

        public override bool CanEndLink => false;

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
