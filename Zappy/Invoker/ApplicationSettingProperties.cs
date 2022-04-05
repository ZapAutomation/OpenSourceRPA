using System;
using System.IO;
using System.Windows;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.ZappyTaskEditor;
using ZappyMessages;
using ZappyMessages.Helpers;

namespace Zappy.Invoker
{
    public class ApplicationSettingProperties
    {
        public static ApplicationSettingProperties Instance = new ApplicationSettingProperties();
        private bool enableComPlayback;
                private bool enableMLLearnSaveTask;

        static ApplicationSettingProperties()
        {
            if (File.Exists(CrapyConstants.ZappySettings))
            {
                try
                {
                    Instance =
                       ZappySerializer.DeserializeObject<ApplicationSettingProperties>(File.ReadAllText(CrapyConstants.ZappySettings));
                    if (Instance.AddedSamples)
                    {
                        MessageBox.Show("Check Zappy Samples at " + CrapyConstants.SamplesFolderStartupPath);
                        Instance.AddedSamples = false;
                        File.WriteAllText(CrapyConstants.ZappySettings, ZappySerializer.SerializeObject(ApplicationSettingProperties.Instance));
                    }
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.Error(ex);
                                    }
            }
                                                        }

        public bool ExcelRecordMouseAction { get; set; }
        public bool ChromeXPAthOnlyRecord { get; set; }
        public bool ChromeInsertPause { get; set; }
        public bool EnableTrapy { get; set; }

        public bool EnableLaunchActivityRecording { get; set; }

        public bool EnableJava64Bit { get; set; }
        public bool EnableJava32Bit { get; set; }

        public bool SaveMLLearningFile
        {
            get { return enableMLLearnSaveTask; }
            set
            {
                enableMLLearnSaveTask = value;
                if (InternalNodeGenerator.NodeGraphBuilder != null)
                {
                  
                }
            }
        }

        public bool AddedSamples { get; set; }
        public bool EnableChromeNativeRecording { get; set; }
                                                                                                                                                                                                                                                                            
        public string[] mlLearningProcesses { get; set; }

        public System.Collections.Generic.List<string> screenShotRecordingProcesses { get; set; }

        public bool EnableTraceLog { get; set; }

        public bool EnableFullAuditLog { get; set; }

        public bool EnableZappyShortcuts { get; set; }

        public LanguageZappy ZappyUILanguage { get; set; }

        public bool EnableRecordScreenshots { get; set; }

        public bool EnableComPlayback = false;


        public ApplicationSettingProperties()
        {
            mlLearningProcesses = new string[0];
            EnableTrapy = true;
            ZappyUILanguage = LanguageZappy.general;
            screenShotRecordingProcesses = new System.Collections.Generic.List<string>();

        }


    }
}
