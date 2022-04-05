namespace ZappyLogger.Interface
{
    public interface IZappyLoggerClient
    {
        #region Properties

        int Id { get; }

        IZappyLoggerProxy Proxy { get; }

        #endregion

        #region Public methods

        void NotifySettingsChanged(IZappyLoggerProxy server, object cookie);

        void OnSettingsChanged(object cookie);

        #endregion
    }
}