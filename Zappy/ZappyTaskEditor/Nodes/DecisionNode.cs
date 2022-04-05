using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.Helper;
using Zappy.ZappyTaskEditor.Port;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public sealed class DecisionNode : Node
    {
        
        

        internal DecisionNode(IZappyAction activity) : base(activity)
        {
            this.Ports.Add(new TruePort());
            this.Ports.Add(new FalsePort());
            this.Ports.Add(new ErrorPort());
        }


        public override GraphicsPath Render(Graphics g, Rect r, NodeStyle o)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddPolygon(new Point[]
            {
                r.CenterTop,
                r.CenterRight,
                r.CenterBottom,
                r.CenterLeft
            });
            path.CloseFigure();
                        g.FillPath(o.BackBrush, path);
            g.DrawPath(o.BorderPen, path);
            return path;
        }
    }
}
