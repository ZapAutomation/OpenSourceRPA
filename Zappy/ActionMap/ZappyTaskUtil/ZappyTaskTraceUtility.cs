using System;
using System.IO;
using System.Security;
using System.Windows.Forms;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    public static class ZappyTaskTraceUtility
    {
        private const string LastRunDirectory = "LastRun";
        private static string logFileDirectory;
        private const int MaxLogDirectories = 0x19;
        private const string PreviousRunDirectory = "PreviousRun";

        internal static string CreateLogFolder(string traceLogFolder)
        {
            traceLogFolder = Environment.ExpandEnvironmentVariables(traceLogFolder);
            if (!string.IsNullOrEmpty(traceLogFolder))
            {
                if (!Directory.Exists(traceLogFolder))
                {
                    Directory.CreateDirectory(traceLogFolder);
                }
                logFileDirectory = Path.Combine(traceLogFolder, "LastRun");
                if (Directory.Exists(logFileDirectory))
                {
                    string[] directories = Directory.GetDirectories(traceLogFolder, "PreviousRun*");
                    int num = 1;
                    if (directories != null && directories.Length != 0)
                    {
                        string oldest = string.Empty;
                        string str = string.Empty;
                        int num2 = 0x7fffffff;
                        int num3 = -2147483648;
                        int num4 = 0;
                        foreach (string str2 in directories)
                        {
                            int num6;
                            string str3 = str2.Substring(Path.Combine(traceLogFolder, "PreviousRun").Length);
                            if (!string.IsNullOrEmpty(str3) && int.TryParse(str3, out num6))
                            {
                                if (num6 < num2)
                                {
                                    num2 = num6;
                                    oldest = str2;
                                }
                                if (num6 > num3)
                                {
                                    num3 = num6;
                                    str = str2;
                                }
                                num4++;
                            }
                        }
                        if (!string.IsNullOrEmpty(oldest) && num4 >= 0x19)
                        {
                            SafeIOCall(delegate
                            {
                                Directory.Delete(Path.Combine(traceLogFolder, oldest), true);
                            });
                        }
                        if (!string.IsNullOrEmpty(str))
                        {
                            num = num3 + 1;
                        }
                    }
                    string previousLogDirectory = Path.Combine(traceLogFolder, "PreviousRun" + num);
                    SafeIOCall(delegate
                    {
                        Directory.Move(logFileDirectory, previousLogDirectory);
                    });
                }
                SafeIOCall(delegate
                {
                    Directory.CreateDirectory(logFileDirectory);
                });
            }
            return logFileDirectory;
        }

        private static void SafeIOCall(MethodInvoker ioCall)
        {
            try
            {
                ioCall();
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (SecurityException)
            {
            }
        }

                                        
                                        
                                                                                                                        
                                                                                                                                        
        public static string LogFileDirectory =>
            logFileDirectory;

                                        
                                    }

}
