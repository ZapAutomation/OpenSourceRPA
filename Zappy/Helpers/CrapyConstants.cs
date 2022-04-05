using System;
using System.Collections.Generic;
using System.IO;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using Zappy.ZappyTaskEditor.EditorPage.Forms;
using ZappyMessages;
using ZappyMessages.Helpers;

namespace Zappy.Helpers
{
    public static class CrapyConstants
    {
        public static void Init()
        {
            Environment.SetEnvironmentVariable("BaseFolder", BaseFolder);
            CrapyLogger.Init();

            string[] _DirsToCheck = new string[] { BaseFolder,
                ProfileDirectory, DebugFolder, TempFolder, LogFolder,
                LogFolderCopy, AuditDirectory, SavedTaskFolder, imageDirectory,
                DiscoveredProcessFolder, HelperFolder, ProfileDllDirectory
                , MachineLearningFolder };
             
            for (int i = 0; i < _DirsToCheck.Length; i++)
            {
                if (!Directory.Exists(_DirsToCheck[i]))
                    Directory.CreateDirectory(_DirsToCheck[i]);
            }                                                           
        }

        public static string UserID = Environment.UserDomainName + "_" + Environment.UserName + "_" + CrapyConstants.InstID;

        
        public static readonly string PdfToolsFolder = Path.Combine(ZappyMessagingConstants.ExtensionsFolder, "PdfTools");

        public static readonly string TesseractFolder = Path.Combine(ZappyMessagingConstants.ExtensionsFolder, "Tesseract");
                                
        public static readonly string FirefoxExtensionFile = Path.Combine(ZappyMessagingConstants.ExtensionsFolder, "zappy-4.3-fx.xpi");

        public static readonly string BaseFolder = ZappyMessagingConstants.BaseFolder;

        public static readonly string SamplesFolderStartupPath = Path.Combine(ZappyMessagingConstants.StartupPath, "Sample");

        public static readonly string DocumentationFolderPath = Path.Combine(ZappyMessagingConstants.StartupPath, "ActionDocumentation");

        public static readonly string SamplesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ZappySamples");

        public static readonly string DocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static readonly string DebugFolder = Path.Combine(BaseFolder, "Debug");

        public static readonly string TempFolder = Path.Combine(BaseFolder, "Temp");

        public static readonly string LogFolder = Path.Combine(BaseFolder, "Logs");

        public static readonly string LogFolderCopy = Path.Combine(BaseFolder, "LogsCopy");

        public static readonly string AuditDirectory = Path.Combine(BaseFolder, "Audit");
        public static readonly string Logfilename = Path.Combine(AuditDirectory, DateTime.Now.ToString("dd-MM-yyyy") + ".audit");

        public static readonly string ProfileDirectory = Path.Combine(BaseFolder, "Profile");

        public static readonly string InstIdPath = Path.Combine(ProfileDirectory, "InstID.dat");

        public static readonly string RecentFilePath = Path.Combine(ProfileDirectory, "Recent.txt");

        public static readonly string ProfileDllDirectory = Path.Combine(ProfileDirectory, "Dll");
        public static readonly string LicenseFile = Path.Combine(ProfileDirectory, "Zappy.dat");

        public static readonly string SavedTaskFolder = Path.Combine(BaseFolder, Properties.Settings.Default.SavedTask_FolderPath);

        public static readonly string DiscoveredProcessFolder = Path.Combine(BaseFolder, "DiscoveredProcesses");

        public static readonly string HelperFolder = Path.Combine(BaseFolder, "Helper");

        
        public static string imageDirectory = Path.Combine(BaseFolder, "Images");


        public static readonly string MachineLearningFolder = string.IsNullOrEmpty(Properties.Settings.Default.ML_FolderPath) ?
                                                Path.Combine(BaseFolder, "ZappyLearn") : Path.Combine(Properties.Settings.Default.ML_FolderPath, Environment.UserDomainName + "_" + Environment.UserName);


        public static readonly string[] CleanupFoldersOnAppLaunch = { DebugFolder, LogFolderCopy, TempFolder };

        public static readonly string ZipFileLoc = BaseFolder + "Logs.zip";

        public static readonly string InteliticsUserName = Environment.UserDomainName + "_" + Environment.UserName;

                
                        

        public static readonly string CrappyIndex = SavedTaskFolder + "MainWindowIndex.txt";
        
        public static readonly string ZappyShortcuts = Path.Combine(ProfileDirectory, "ZappyShortcuts.dat");
        public static readonly string ZappySettings = Path.Combine(ProfileDirectory, "ZappySettings.dat");

        internal static readonly string TextDataFilePath = SavedTaskFolder + "TempData.txt";
        internal static readonly string pasteChar = "${Paste}";




        public const int MaxFiles = 5,
            TasksForPrediction_Input = 3,
            TasksForPrediction_Output = 5;
            
        public const long MLBatchTicks = 30 * TimeSpan.TicksPerMinute;
                        


        public const int DownloadPredictionDataAfterDays = 7;

        public const int MaxInactivitySecs = 300 * 2;
        public static string InstID { get; internal set; }


        public const string VariableNameEnd = "}";
        public const string ZappyLoopItem = SharedConstants.VariableNameBegin + "LoopItem" + VariableNameEnd;
        public const string ZappyLoopIndex = SharedConstants.VariableNameBegin + "LoopIndex" + VariableNameEnd;
        public const string ZappyLoopData = SharedConstants.VariableNameBegin + "LoopData" + VariableNameEnd;
        public const string ZappyPreviousAction = SharedConstants.VariableNameBegin + "PreviousAction" + VariableNameEnd;
        public const string ZappyCurrentAction = SharedConstants.VariableNameBegin + "CurrentAction" + VariableNameEnd;

        public const string ZappyCurrentLoopStart = SharedConstants.VariableNameBegin + "CurrentLoopStart" + VariableNameEnd;

        
        public const int RemoveTechnologyElementTimeSec = 30;
        public const string TriggerDataKey = SharedConstants.VariableNameBegin + "TriggerData" + VariableNameEnd;

        public const string TriggerKey = SharedConstants.VariableNameBegin + "Trigger" + VariableNameEnd;

        public const string ZappyTaskKey = SharedConstants.VariableNameBegin + "ZappyTask" + VariableNameEnd;

        public const string ExternalActionPublisher = SharedConstants.VariableNameBegin + "PubSubClient" + VariableNameEnd;

        public const string ExecutionSentinel = SharedConstants.VariableNameBegin + "ExecutionSentinel" + VariableNameEnd;

        public const string FullExecutionTrace = SharedConstants.VariableNameBegin + "FullTrace" + VariableNameEnd;

        public const string StringArrayDelemiter = "^^";
        public const string ChromePlaybackSuccessful = "SUCCESS";
        public const string ChromePlaybackFailed = "FAILED";
        public const string AutoLearnActivityName = "AutoLearnedTask.zappy";

        public static Dictionary<Type, ActivityFormDisplayHelper> TypeToActivityFormDisplayHelper;

    }
}
