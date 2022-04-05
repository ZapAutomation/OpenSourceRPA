using System.Drawing;

namespace ZappyLogger.Docking
{
    internal interface ISplitterDragSource : IDragSource
    {
        #region Properties

        bool IsVertical { get; }
        Rectangle DragLimitBounds { get; }

        #endregion

        #region Public methods

        void BeginDrag(Rectangle rectSplitter);
        void EndDrag();
        void MoveSplitter(int offset);

        #endregion
    }
}