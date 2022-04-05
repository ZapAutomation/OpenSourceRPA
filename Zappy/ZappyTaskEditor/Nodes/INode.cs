using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.Nodes
{
    internal interface INode
    {
        IZappyAction Activity { get; set; }
        Rect Bounds { get; set; }
        bool CanEndLink { get; }
        bool CanStartLink { get; }
        string ExecutionResult { get; set; }
        Guid Id { get; }
        string Name { get; }
        NodeState NodeState { get; set; }
        List<Port.Port> Ports { get; set; }

        Port.Port GetPortById(Guid id);
        Port.Port GetPortFromPoint(Point pt);
        GraphicsPath Render(Graphics g, Rect r, NodeStyle o);
        void RenderText(Graphics g, Rect r, NodeStyle o);
    }
}