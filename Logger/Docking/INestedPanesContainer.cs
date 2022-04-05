using System.Drawing;

namespace ZappyLogger.Docking
{
    public interface INestedPanesContainer
    {
        #region Properties

        DockState DockState { get; }
        Rectangle DisplayingRectangle { get; }
        NestedPaneCollection NestedPanes { get; }
        VisibleNestedPaneCollection VisibleNestedPanes { get; }
        bool IsFloat { get; }

        #endregion
    }
}