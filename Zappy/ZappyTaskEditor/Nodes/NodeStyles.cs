using Zappy.ZappyTaskEditor.EditorPage;

namespace Zappy.ZappyTaskEditor.Nodes
{
    public static class NodeStyles
    {
        public static readonly NodeStyle General, SelectedNode, BreakPoint, ExecutionPassed, ExecutionFailed, Debug;
        public static readonly NodeStyle[] NodeStyleCache;
        static NodeStyles()
        {
            General = new NodeStyle();

            SelectedNode = new NodeStyle() { BorderPen = PageRenderOptions.SelectedNodeBorderPen };

            BreakPoint = new NodeStyle() { BackBrush = PageRenderOptions.BreakpointBackBrush, BorderPen = PageRenderOptions.BreakpointPen, FontBrush = System.Drawing.Brushes.White };

            ExecutionFailed = new NodeStyle() { BackBrush = PageRenderOptions.ExecutionFailedBackBrush };

            ExecutionPassed = new NodeStyle() { BackBrush = PageRenderOptions.ExecutionPassedBackBrush };

            Debug = new NodeStyle() { BorderPen = PageRenderOptions.SelectedNodeBorderPen, BackBrush = PageRenderOptions.DebugNodeBackBrush };

            NodeStyleCache = new NodeStyle[(int)NodeState.BreakPoint + 1];

            NodeStyleCache[(int)NodeState.General] = General;

            NodeStyleCache[(int)NodeState.Selected] = SelectedNode;

            NodeStyleCache[(int)NodeState.BreakPoint] = BreakPoint;

            NodeStyleCache[(int)NodeState.Debug] = Debug;

            NodeStyleCache[(int)NodeState.ExecutionPassed] = ExecutionPassed;

            NodeStyleCache[(int)NodeState.ExecutionFailed] = ExecutionFailed;



        }
    }
}