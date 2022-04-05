namespace ZappyLogger.ColumnizaeLib
{
    /// <summary>
    /// Implement this interface to get notified of various global events in ZappyLogger.
    /// The interface can be implemented by all currently known type of ZappyLogger plugins (Columnizers,
    /// keyword plugins, context menu plugins).
    /// </summary>
    public interface IZappyLoggerPlugin
    {
        #region Public methods

        /// <summary>
        /// Called on application exit. May be used for cleanup purposes,
        /// </summary>
        void AppExiting();

        /// <summary>
        /// Called when the plugin is loaded at plugin registration while ZappyLogger startup.
        /// </summary>
        void PluginLoaded();

        #endregion
    }
}