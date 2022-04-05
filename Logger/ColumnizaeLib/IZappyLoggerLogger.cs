namespace ZappyLogger.ColumnizaeLib
{
    /// <summary>
    /// Simple Logger interface to let plugins log into ZappyLogger's application log file.
    /// </summary>
    public interface IZappyLoggerLogger
    {
        #region Public methods

        /// <summary>
        /// Logs a message on INFO level to ZappyLogger#s log file. The logfile is only active in debug builds.
        /// The logger in ZappyLogger will automatically add the class and the method name of the caller.
        /// </summary>
        /// <param name="msg">A message to be logged.</param>
        void Info(string msg);

        /// <summary>
        /// Logs a message on DEBUG level to ZappyLogger#s log file. The logfile is only active in debug builds.
        /// The logger in ZappyLogger will automatically add the class and the method name of the caller.
        /// </summary>
        /// <param name="msg">A message to be logged.</param>
        void Debug(string msg);

        /// <summary>
        /// Logs a message on WARN level to ZappyLogger#s log file. The logfile is only active in debug builds.
        /// The logger in ZappyLogger will automatically add the class and the method name of the caller.
        /// </summary>
        /// <param name="msg">A message to be logged.</param>
        void LogWarn(string msg);

        /// <summary>
        /// Logs a message on ERROR level to ZappyLogger#s log file. The logfile is only active in debug builds.
        /// The logger in ZappyLogger will automatically add the class and the method name of the caller.
        /// </summary>
        /// <param name="msg">A message to be logged.</param>
        void LogError(string msg);

        #endregion
    }
}