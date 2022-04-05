using System;
using System.IO;
using ZappyMessages.Helpers;

namespace ZappyMessages.Logger
{
    public static class ConsoleLogger
    {
        private static StreamWriter sw;
        public static void Init(string _logfilename)
        {
            try
            {

                FileMode _Mode = FileMode.OpenOrCreate;
                string logfilename = string.Empty;
                if (!_logfilename.Contains(":"))
                {

                    logfilename = Path.Combine(ZappyMessagingConstants.LogFolder, _logfilename);

                    if (!Directory.Exists(Path.GetDirectoryName(logfilename)))
                        Directory.CreateDirectory(Path.GetDirectoryName(logfilename));
                    if (File.Exists(logfilename))
                        _Mode |= FileMode.Truncate;

                }
                else
                {
                    logfilename = _logfilename;
                }

                FileStream fs = File.Open(logfilename, _Mode, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);
            }
            catch
            { }
        }



        public static void writeLogToFile(string message)
        {
            try
            {
                if (sw != null)
                {
                    sw.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + message);
                    sw.Flush();
                }
            }
            catch { }

        }
        public static void Info(string message)
        { writeLogToFile(message); }

        public static void Error(Exception ex)
        {
            writeLogToFile(ex.ToString() + ex.StackTrace);
        }

        public static void Close()
        {
            try
            {
                sw.Dispose();
            }
            catch
            {

            }
        }


    }
}