using System.Drawing;

namespace Zappy.ZappyTaskEditor.EditorPage
{
    public sealed class PageRenderOptions
    {
        public static int GridSize = 18;

        public static Pen GridPen = new Pen(Color.FromArgb(240, 240, 240), 1);

                        
        public static Color BackColor = Color.White;

        public static Brush SelectionBackBrush = new SolidBrush(Color.FromArgb(50, 150, 150, 150));

        public static Brush BreakpointBackBrush = Brushes.Maroon;

        public static Pen BreakpointPen = Pens.Gray;

        public static Brush ExecutionPassedBackBrush = Brushes.GreenYellow;
        public static Brush ExecutionFailedBackBrush = Brushes.OrangeRed;

        public static Brush DebugNodeBackBrush = new SolidBrush(Color.FromArgb(255, 225, 125));

        public static Pen SelectedNodeBorderPen = new Pen(Color.DarkOrange, 2);
    }
}