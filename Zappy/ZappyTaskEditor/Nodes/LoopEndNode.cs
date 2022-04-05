using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Loops;
using Zappy.ZappyTaskEditor.EditorPage;
using Zappy.ZappyTaskEditor.Helper;
using Zappy.ZappyTaskEditor.Port;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public sealed class LoopEndNode : Node
    {
        
        public Guid LoopStartNodeId
        {
            get { return (Activity as IZappyLoopEndAction).LoopStartGuid; }

        }

        private LoopEndNode()
        {
                    }

        internal LoopEndNode(IZappyAction activity) : base(activity)
        {
            this.Ports.Add(new NextPort());
        }



        public override GraphicsPath Render(Graphics g, Rect r, NodeStyle o)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddPolygon(new Point[]
            {
                new Point(r.X, r.Y),
                new Point(r.Right, r.Y),
                new Point(r.Right, r.Bottom - PageRenderOptions.GridSize),
                new Point(r.Right - PageRenderOptions.GridSize, r.Bottom),
                new Point(r.X + PageRenderOptions.GridSize, r.Bottom),
                new Point(r.X, r.Bottom - PageRenderOptions.GridSize)
            });
            path.CloseFigure();
                        g.FillPath(o.BackBrush, path);
            g.DrawPath(o.BorderPen, path);
            return path;
        }
    }
}
