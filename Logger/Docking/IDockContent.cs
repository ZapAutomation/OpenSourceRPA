using System;

namespace ZappyLogger.Docking
{
    public interface IDockContent
    {
        #region Properties

        DockContentHandler DockHandler { get; }

        #endregion

        #region Public methods

        void OnActivated(EventArgs e);
        void OnDeactivate(EventArgs e);

        #endregion
    }
}