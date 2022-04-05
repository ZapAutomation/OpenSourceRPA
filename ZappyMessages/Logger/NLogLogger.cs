
namespace ZappyMessages.Logger
{
    public static class NLogLogger
    {
        public static void Init(string _logfilename)
        {
            try
            {
                var config = new NLog.Config.LoggingConfiguration();

                // Targets where to log to: File and Console
                var logfile = new NLog.Targets.FileTarget("logfile") { FileName = _logfilename };
                var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

                // Rules for mapping loggers to targets            
                //config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
                //config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

                // Apply config           
                NLog.LogManager.Configuration = config;
                NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

                log.Info("sdfsd");
            }
            catch
            { }
        }
     
    }
}
