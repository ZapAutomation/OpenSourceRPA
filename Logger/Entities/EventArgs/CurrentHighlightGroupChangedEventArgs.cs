using ZappyLogger.Controls.LogWindow;

namespace ZappyLogger.Entities.EventArgs
{
    public class CurrentHighlightGroupChangedEventArgs
    {
        #region Fields

        #endregion

        #region cTor

        public CurrentHighlightGroupChangedEventArgs(LogWindow logWindow, HilightGroup currentGroup)
        {
            this.LogWindow = logWindow;
            this.CurrentGroup = currentGroup;
        }

        #endregion

        #region Properties

        public LogWindow LogWindow { get; }

        public HilightGroup CurrentGroup { get; }

        #endregion
    }
}