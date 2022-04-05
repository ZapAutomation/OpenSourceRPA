using System;

namespace ZappyLogger.Docking
{
    public class DockContentEventArgs : EventArgs
    {
        #region Fields

        #endregion

        #region cTor

        public DockContentEventArgs(IDockContent content)
        {
            Content = content;
        }

        #endregion

        #region Properties

        public IDockContent Content { get; }

        #endregion
    }
}