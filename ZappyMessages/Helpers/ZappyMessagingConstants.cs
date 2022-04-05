using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ZappyMessages.Helpers
{
    public static class ZappyMessagingConstants
    {
        static ZappyMessagingConstants()
        {
            string[] _DirsToCheck = new string[]
            {
                BaseFolder, TriggerFolder, LogFolder, AnalyticsFolder, RobotHubFolder, RobotFolder

            };

            for (int i = 0; i < _DirsToCheck.Length; i++)
            {
                if (!Directory.Exists(_DirsToCheck[i]))
                    Directory.CreateDirectory(_DirsToCheck[i]);
            }
        }

#if DEBUG
        public static readonly string BaseFolder = 
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ZappyAI_DEBUG";
#else
        public static readonly string BaseFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ZappyAI";
#endif

        public static readonly string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static readonly string ExtensionsFolder = Path.Combine(StartupPath, "Extensions");


        public const string TrapyToCrapy_MemoryMapName = "TrapyToCrapy";


        public static readonly string AnalyticsFolder = Path.Combine(BaseFolder, "ZappyAnalytics");

        //Use this for HUB data files
        public static readonly string RobotHubFolder = Path.Combine(BaseFolder, "RobotHubFolder");
        public static readonly string RobotFolder = Path.Combine(BaseFolder, "RobotFolder");


        public const int MemoryMappedFileSize = 1024 * 1024 * 8;

        public const string TrapyStart = "TrapyStart";

        public const string TrapyStop = "TrapyStop";

        public const string ZappyExecutionStarted = "ZappyExecutionStartedZappyExecutionStarted";

        public const string ZappyExecutionStopped = "ZappyExecutionStoppedZappyExecutionStopped";
        public const string ZappyExecutionFailed = "ZappyExecutionFailed23ZappyExecutionFailed23";
        public const string TrapyToCrapy_Message = "TrapyMessage";

        public static readonly string ZappyHubConfigurationFileName = Path.Combine(StartupPath, "HubConfiguration.json");


        public static readonly string TriggerFolder = Path.Combine(BaseFolder, "Trigger");
        public static readonly string LogFolder = Path.Combine(BaseFolder, "Logs");

        public static readonly string ImageFolderCollectionName = "ZappyImagesCollectionForAllUsers";

        public static string UserDomainAndID = new Regex(@"[()<>"";+\n\r`]|^&+|&+$").Replace(Environment.UserDomainName + Environment.UserName, "");
        public static readonly string EndpointLocationZappyService = "net.pipe://localhost/ZappyPubSubServicePipe" + UserDomainAndID;
        //Helper functions
        public static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

    }
}
