using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public sealed class GroupNode : INode
    {
                        
        
        internal GroupNode(IZappyAction activity)
        {
            var r = this.Bounds;
            this.Bounds = new Rect(r.X, r.Y, Convert.ToInt32(r.Width * 1.3), r.Height * 5);

        }
        public bool CanStartLink => false;

        public bool CanEndLink => false;

        public IZappyAction Activity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Rect Bounds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ExecutionResult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Guid Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public NodeState NodeState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<Port.Port> Ports { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

                                        public GraphicsPath Render(Graphics g, Rect r, NodeStyle o)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddRectangle(r);
            path.CloseFigure();
                        g.FillPath(o.BackBrush, path);
            g.DrawPath(o.BorderPen, path);
            return path;
        }
                public void RenderText(Graphics g, Rect r, NodeStyle o)
        {
                                                                                                                                                                                                                                            }

        public Port.Port GetPortById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Port.Port GetPortFromPoint(Point pt)
        {
            throw new NotImplementedException();
        }
            }
}
