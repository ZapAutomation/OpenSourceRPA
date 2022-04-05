using System.Drawing;
using System.Windows.Forms;

namespace ZappyLogger.Docking
{
    internal interface IDockDragSource : IDragSource
    {
        #region Public methods

        Rectangle BeginDrag(Point ptMouse);
        bool IsDockStateValid(DockState dockState);
        bool CanDockTo(DockPane pane);
        void FloatAt(Rectangle floatWindowBounds);
        void DockTo(DockPane pane, DockStyle dockStyle, int contentIndex);
        void DockTo(DockPanel panel, DockStyle dockStyle);

        #endregion
    }
}