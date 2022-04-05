namespace ZappyLogger.Docking
{
    public partial class DockWindow
    {
        private class SplitterControl : SplitterBase
        {
            #region Properties

            protected override int SplitterSize
            {
                get { return Measures.SplitterSize; }
            }

            #endregion

            #region Overrides

            protected override void StartDrag()
            {
                DockWindow window = Parent as DockWindow;
                if (window == null)
                {
                    return;
                }

                window.DockPanel.BeginDrag(window, window.RectangleToScreen(Bounds));
            }

            #endregion
        }
    }
}