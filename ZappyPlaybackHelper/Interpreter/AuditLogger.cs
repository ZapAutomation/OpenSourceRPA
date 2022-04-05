using System;
using System.IO;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using ZappyMessages;
using ZappyMessages.PubSub;
using ZappyMessages.PubSubHelper;

namespace ZappyPlaybackHelper.Interpreter
{
    public static class AuditLogger
    {
        private static StreamWriter sw;
        public static void Init()
        {
            try
            {
                FileMode _Mode = FileMode.Append;
                //if (File.Exists(logfilename))
                //    _Mode |= FileMode.Truncate;
                FileStream fs = File.Open(CrapyConstants.Logfilename, _Mode, FileAccess.Write, FileShare.ReadWrite);

                sw = new StreamWriter(fs);
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        public static void writeLogToFile(string message)
        {
            try
            {
                {
                    //Can have two channels depending on what is required
                    string LogLine = DateTime.Now.ToString("HH:mm:ss") + " " + message;
                    //So that date time is unaffected
                    PlaybackHelperService._Client.Publish(PubSubTopicRegister.AuditLogsChannel, LogLine);
                    //send string to hub
                    sw.WriteLine(LogLine);
                    sw.Flush();
                }
            }
            catch (Exception e)
            {
                CrapyLogger.log.Error(e);
            }
        }
        public static void Info(string message)
        { writeLogToFile(message); }

        public static void Close()
        {
            sw.Dispose();
            //pubSubClient.Dispose();
        }

        public static void SaveSuccessfulExecutedTaskLog(ZappyTask ExecutedTask, string TaskFileName)
        {       
        }

        public static void SaveFailedExecutedTaskLog(ZappyTask ExecutedTask, string TaskFileName)
        {
           
        }

    }
}
