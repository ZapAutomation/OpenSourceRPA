using ZappyLogger.Controls.LogWindow;

namespace ZappyLogger.Entities.EventArgs
{
    public class FilterListChangedEventArgs
    {
        #region Fields

        #endregion

        #region cTor

        public FilterListChangedEventArgs(LogWindow logWindow)
        {
            this.LogWindow = logWindow;
        }

        #endregion

        #region Properties

        public LogWindow LogWindow { get; }

        #endregion
    }
}