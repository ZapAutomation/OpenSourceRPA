//#define NLOG

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ZappyMessages
{
    public static class Logger
    {
        #region Internal types
        public enum Rollover
        {
            Day,
            Hour,
            Overwrite
        }

        public enum LogEventType
        {
            Info, Error, Warn, Debug,
            InfoFormat, ErrorFormat, WarnFormat, DebugFormat
        }
        #endregion


        public static void Info(string Message)
        {
            Log(LogEventType.Info, Message, null);
        }

        public static void Info(string Message, params object[] args)
        {
            Log(LogEventType.InfoFormat, Message, args);
        }

        public static void InfoFormat(string Message, params object[] args)
        {
            Log(LogEventType.InfoFormat, Message, args);
        }

        public static void Warn(string Message)
        {
            Log(LogEventType.Warn, Message, null);
        }

        public static void Warn(string Message, params object[] args)
        {
            Log(LogEventType.WarnFormat, Message, args);
        }

        public static void WarnFormat(string Message, params object[] args)
        {
            Log(LogEventType.WarnFormat, Message, args);
        }


        public static void Debug(string Message)
        {
            Log(LogEventType.Debug, Message, null);
        }

        public static void Debug(string Message, params object[] args)
        {
            Log(LogEventType.DebugFormat, Message, args);
        }

        public static void Error(string Message)
        {
            Log(LogEventType.Error, Message, null);
        }
        public static void Error(Exception ex)
        {
            Log(LogEventType.Error, string.Empty, ex);
        }

        public static void Error(string Message, Exception ex)
        {
            Log(LogEventType.Error, Message, ex);
        }

        public static void Error(string Message, params object[] args)
        {
            Log(LogEventType.ErrorFormat, Message, args);
        }

        public static bool Enabled { get; private set; }


#if !NLOG

        struct LogEvent
        {
            public int ThreadID;
            public LogEventType EventType;
            public string Message;
            public object[] Params;
            public DateTime TimeStamp;
        }

        static string[] _LogLevels = { "[Info]", "[ERROR]", "[Warn]", "[Debug]" };

        static StreamWriter sw;
        static Rollover _Rollover;
        static DateTime _NextRollover;

        const string _LogTimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";

        static Dictionary<string, Func<string>> _Macros;

        static string _LogPath, _NewFileName;

        static BlockingCollection<LogEvent> _LogEvents;
        public static void Init(string LogPath, Rollover Rollover)
        {

            Enabled = false;
            if (string.IsNullOrEmpty(LogPath))
                return;

            _Rollover = Rollover;

            _Macros = new Dictionary<string, Func<string>>();
            _Macros["#DATE#"] = GetDate;
            _Macros["#PID#"] = GetPID;
            _Macros["#USER#"] = GetUserName;
            _Macros["#HOUR#"] = GetHour;
            _Macros["#DAY#"] = GetDay;

            if (!Directory.Exists(Path.GetDirectoryName(LogPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath));

            _LogPath = LogPath;
            _LogEvents = new BlockingCollection<LogEvent>();
            Enabled = true;
            Task.Factory.StartNew(WriteLogs);

        }

        static void WriteLogs()
        {
            foreach (LogEvent item in _LogEvents.GetConsumingEnumerable())
            {
                if (item.TimeStamp > _NextRollover)
                    AllocateStreamWriter(item.TimeStamp);

                sw.Write("{0} [{1}] {2} ",
                    item.TimeStamp.ToString(_LogTimeFormat),
                    item.ThreadID, _LogLevels[(int)(item.EventType & LogEventType.Debug)]);

                if (item.EventType <= LogEventType.Debug)
                    sw.WriteLine(item.Message);
                else
                {
                    string _MessageToWrite;
                    try
                    {
                        if (item.EventType == LogEventType.ErrorFormat && item.Params != null && item.Params[0] is Exception)
                        {
                            Exception ex = item.Params[0] as Exception;
                            _MessageToWrite = string.Format("{0}{1}{2}{1}{3}", item.Message, Environment.NewLine, ex, ex.StackTrace);
                        }
                        else
                            _MessageToWrite = string.Format(item.Message, item.Params);

                    }
                    catch
                    {
                        if (item.Params != null)
                            _MessageToWrite = item.Message + "|" + string.Join("|", item.Params);
                        else
                            _MessageToWrite = item.Message;
                    }
                    sw.WriteLine(_MessageToWrite);

                }
                sw.Flush();
            }
        }

        static void AllocateStreamWriter(DateTime Now)
        {
            FileMode _FileMode = FileMode.Append;
            if (_Rollover == Rollover.Overwrite)
            {
                _NextRollover = DateTime.MaxValue;
                _FileMode = FileMode.OpenOrCreate;
            }
            else if (_Rollover == Rollover.Hour)
                _NextRollover = Now.Date.AddHours(Now.Hour + 1);
            else if (_Rollover == Rollover.Day)
                _NextRollover = Now.Date.AddDays(1);

            if (sw != null)
            {

                sw.Close();
                sw.Dispose();
            }

            _NewFileName = _LogPath;
            foreach (KeyValuePair<string, Func<string>> item in _Macros)
                _NewFileName = _NewFileName.Replace(item.Key, item.Value());


            sw = new StreamWriter(File.Open(_NewFileName, _FileMode, FileAccess.Write, FileShare.Read));
        }

        static string GetDate() { return DateTime.UtcNow.ToString("yyyy-MM-dd"); }
        static string GetDay() { return DateTime.UtcNow.DayOfWeek.ToString(); }
        static string GetHour() { return DateTime.UtcNow.ToString("HH"); }
        static string GetPID() { return Process.GetCurrentProcess().Id.ToString(); }
        static string GetUserName() { return Environment.UserName; }


        public static void Log(LogEventType EventType, string Message, params object[] args)
        {
            if (!Enabled)
                return;
            LogEvent le = new LogEvent();
            le.Message = Message;
            le.EventType = EventType;
            le.Params = args;
            le.ThreadID = Thread.CurrentThread.ManagedThreadId;
            le.TimeStamp = DateTime.UtcNow;
            _LogEvents.Add(le);
        }

#else
        static readonly log4net.ILog log
    = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Init(string LogPath, Rollover Rollover)
        {
            Enabled = true;
        }


        public static void Log(LogEventType EventType, string Message, params object[] args)
        {
            if (!Enabled)
                return;
            switch (EventType)
            {
                case LogEventType.Info:
                    log.Info(Message);
                    break;
                case LogEventType.Error:
                    log.Error(Message);
                    break;
                case LogEventType.Warn:
                    log.Warn(Message);
                    break;
                case LogEventType.Debug:
                    log.Debug(Message);
                    break;
                case LogEventType.InfoFormat:
                    log.InfoFormat(Message, args);
                    break;
                case LogEventType.ErrorFormat:
                    if (args != null && args[0] is Exception)
                        log.Error(Message, args[0] as Exception);
                    else
                        log.Error(Message);
                    break;
                case LogEventType.WarnFormat:
                    log.WarnFormat(Message, args);

                    break;
                case LogEventType.DebugFormat:
                    log.DebugFormat(Message, args);

                    break;
                default:
                    break;
            }
        }
#endif
    }
}
