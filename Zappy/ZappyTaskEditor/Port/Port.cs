using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.ZappyTaskEditor.EditorPage;
using Zappy.ZappyTaskEditor.Helper;
using Zappy.ZappyTaskEditor.Nodes;

namespace Zappy.ZappyTaskEditor.Port
{
    public abstract class Port
    {
        public Guid Id { get; }

        public Guid NextNodeId { get; set; }

        public Rect Bounds { get; set; }

        public abstract Point GetOffset(Rect r);

        public abstract Brush GetBackBrush();

        public Port()
        {
            this.Id = Guid.NewGuid();
        }

        internal virtual void UpdateBounds(Rectangle r)
        {
            var portPoint = this.GetOffset(r);
            var portSize = new Size(PageRenderOptions.GridSize, PageRenderOptions.GridSize);
            portPoint.Offset(-portSize.Width / 2, -portSize.Height / 2);
            var portBounds = new Rectangle(portPoint, portSize);
            this.Bounds = portBounds;
        }

        public GraphicsPath Render(Graphics g, Rectangle r, NodeStyle o)
        {
            this.UpdateBounds(r);
            g.FillEllipse(this.GetBackBrush(), this.Bounds);
            var portPath = new GraphicsPath();
            portPath.StartFigure();
            portPath.AddEllipse(this.Bounds);
            portPath.CloseFigure();
            return portPath;
        }
    }
}
