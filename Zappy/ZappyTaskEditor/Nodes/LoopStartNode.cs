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
    public sealed class LoopStartNode : Node
    {
        public Guid LoopEndNodeId
        {
            get { return (Activity as IZappyLoopStartAction).LoopEndGuid; }
        }

        private LoopStartNode()
        {
                    }


        internal LoopStartNode(IZappyAction activity) : base(activity)
        {
            this.Ports.Add(new NextPort());
        }


        public override GraphicsPath Render(Graphics g, Rect r, NodeStyle o)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddPolygon(new Point[]
            {
                new Point(r.X + PageRenderOptions.GridSize, r.Y),
                new Point(r.Right - PageRenderOptions.GridSize, r.Y),
                new Point(r.Right, r.Y + PageRenderOptions.GridSize),
                new Point(r.Right, r.Bottom),
                new Point(r.X, r.Bottom),
                new Point(r.X, r.Y + PageRenderOptions.GridSize)
            });
            path.CloseFigure();
                        g.FillPath(o.BackBrush, path);
            g.DrawPath(o.BorderPen, path);
            return path;
        }
    }
}
