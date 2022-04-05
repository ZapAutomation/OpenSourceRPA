using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.Helper;
using Zappy.ZappyTaskEditor.Port;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public sealed class ZappyActionUINode : Node
    {
        
        private ZappyActionUINode()
        {
                    }

        internal ZappyActionUINode(IZappyAction activity) : base(activity)
        {
            this.Ports.Add(new NextPort());
            this.Ports.Add(new ErrorPort());

        }



        public override GraphicsPath Render(Graphics g, Rect r, NodeStyle o)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddRectangle(r);
            path.CloseFigure();
                        g.FillPath(o.BackBrush, path);
            g.DrawPath(o.BorderPen, path);
            return path;
        }
    }
}
