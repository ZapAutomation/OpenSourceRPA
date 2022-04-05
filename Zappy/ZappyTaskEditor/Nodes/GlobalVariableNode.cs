using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.EditorPage;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public sealed class GlobalVariableNode : Node
    {
        public const string StartToken = "[";

        public const string EndToken = "]";

        public override bool CanEndLink => false;

        public override bool CanStartLink => false;

        private GlobalVariableNode()
        {
                    }

        internal GlobalVariableNode(IZappyAction activity, int VariableIndex) : base(activity)
        {
            Name += (" " + VariableIndex.ToString());
        }

        public override GraphicsPath Render(Graphics g, Rect r, NodeStyle o)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddPolygon(new Point[]
            {
                new Point(r.X + PageRenderOptions.GridSize, r.Y),
                new Point(r.Right, r.Y),
                new Point(r.Right - PageRenderOptions.GridSize, r.Bottom),
                new Point(r.X, r.Bottom),
            });
            path.CloseFigure();
                        g.FillPath(o.BackBrush, path);
            g.DrawPath(o.BorderPen, path);
            return path;
        }

                                    }
}
