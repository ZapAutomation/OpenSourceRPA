namespace ZappyLogger.ColumnizaeLib
{
    /// <summary>
    /// Service interface implemented by ZappyLogger. This can be used by IFileSystemPlugin implementations to get certain services.
    /// </summary>
    public interface IFileSystemCallback
    {
        #region Public methods

        /// <summary>
        /// Retrieve a logger. The plugin can use the logger to write log messages into ZappyLogger's log file.
        /// </summary>
        /// <returns></returns>
        IZappyLoggerLogger GetLogger();

        #endregion
    }
}