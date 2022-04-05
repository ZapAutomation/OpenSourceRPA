using NLog;
using ZappyLogger.ColumnizaeLib;

namespace ZappyLogger.Classes
{
    internal class FileSystemCallback : IFileSystemCallback
    {
        #region Public methods

        public IZappyLoggerLogger GetLogger()
        {
            return new NLogZappyLoggerWrapper();
        }

        #endregion

        private class NLogZappyLoggerWrapper : IZappyLoggerLogger
        {
            #region Fields

            private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

            #endregion

            #region Public methods

            public void Info(string msg)
            {
                _logger.Info(msg);
            }

            public void Debug(string msg)
            {
                _logger.Debug(msg);
            }

            public void LogWarn(string msg)
            {
                _logger.Warn(msg);
            }

            public void LogError(string msg)
            {
                _logger.Error(msg);
            }

            #endregion
        }
    }
}