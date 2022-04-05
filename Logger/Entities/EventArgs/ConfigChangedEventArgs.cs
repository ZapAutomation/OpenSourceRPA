using ZappyLogger.Config;

namespace ZappyLogger.Entities.EventArgs
{
    internal class ConfigChangedEventArgs : System.EventArgs
    {
        #region Fields

        #endregion

        #region cTor

        internal ConfigChangedEventArgs(SettingsFlags changeFlags)
        {
            this.Flags = changeFlags;
        }

        #endregion

        #region Properties

        public SettingsFlags Flags { get; }

        #endregion
    }
}