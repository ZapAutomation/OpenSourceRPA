using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;

namespace Zappy.Decode.Helper
{
    [Serializable]
    internal class RecorderOptions : INotifyPropertyChanged
    {
                private Dictionary<string, bool> aggregatorGroupSetting;
        private int aggregatorTimeout;
                                private int delayBetweenActivities;
                                        private List<int> excludeProcessIdList;
        private List<string> excludeProcessNameList;
                        private KeyCombination hoverKey;
                                private List<KeyCombination> ignoreKeys;
        private List<int> imeLanguageList;
                private List<int> includeProcessIdList;
        private List<string> includeProcessNameList;
        private bool isImageActionLogEnabled;
                private bool isRemoteTestingEnabled;
                                        private bool recordThinkTime;
                        private int remoteTestingAggregatorTimeout;
                                private bool showCurrentlyRecording;
                private int thinktimeThreshold;
                private int thinktimeUpperCutoff;
        
                public event PropertyChangedEventHandler PropertyChanged;

        public RecorderOptions()
        {
            imeLanguageList = new List<int>();
            excludeProcessIdList = new List<int>();
            includeProcessIdList = new List<int>();
            excludeProcessNameList = new List<string>();
            includeProcessNameList = new List<string>();
            ignoreKeys = new List<KeyCombination>();
                                                                        
            aggregatorGroupSetting = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            HoverKey = new KeyCombination(ModifierKeys.Shift | ModifierKeys.Control, Keys.R);
            RecordThinkTime = false;
            ShowCurrentlyRecording = true;
            SetTimeSettingsToDefault();
            excludeProcessIdList.Add(ZappyTaskUtilities.CurrentProcessId);
        }

        public RecorderOptions(IDictionary<string, string> appSettings) : this()
        {
            Initialize(appSettings);
        }

        internal RecorderOptions(NameValueCollection appSettings) : this()
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string str in appSettings.AllKeys)
            {
                dictionary[str] = appSettings[str];
            }
            Initialize(dictionary);
        }

        private void Initialize(IDictionary<string, string> appSettings)
        {
            string str;
            string str2;
            bool flag;
            bool flag2;
            bool flag3;
            bool flag4;
            bool flag5;
            bool flag6;
            bool flag7;
            int num;
            int num2;
            bool flag8;
            int num3;
            int num4;
            int num5;
            bool flag9;
            string str3;
            bool flag10;
            if (ZappyTaskUtilities.GetValue(appSettings, "HoverKeyModifier", out str) && ZappyTaskUtilities.GetValue(appSettings, "HoverKey", out str2))
            {
                try
                {
                    HoverKey = new KeyCombination(str, str2);
                }
                catch (ArgumentException exception)
                {
                    CrapyLogger.log.ErrorFormat("Failed to set hover key option.");
                    CrapyLogger.log.Error(exception);
                }
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "SetValueAggregators", out flag))
            {
                aggregatorGroupSetting.Add("SetValueAggregators", flag);
                object[] args = { flag };
                
            }
            flag2 = !ZappyTaskUtilities.GetValue(appSettings, "RecordImplicitHover", out flag2) | flag2;
            RecordImplicitHover = flag2;
            ReliableRecorderStop = true;
            if (ZappyTaskUtilities.GetValue(appSettings, "ReliableRecorderStop", out flag3))
            {
                object[] objArray2 = { flag3 };
                
                ReliableRecorderStop = flag3;
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "AbsorbWindowResizeAndSetFocusAggregators", out flag4))
            {
                aggregatorGroupSetting.Add("AbsorbWindowResizeAndSetFocusAggregators", flag4);
                object[] objArray3 = { flag4 };
                
            }
                                                            
                        if (ZappyTaskUtilities.GetValue(appSettings, "IsRemoteTestingEnabled", out flag6))
            {
                IsRemoteTestingEnabled = flag6;
                object[] objArray5 = { flag6 };
                
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "IsImageActionLogEnabled", out flag7))
            {
                IsImageActionLogEnabled = flag7;
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "AggregatorTimeout", out num))
            {
                if (num > 0)
                {
                    AggregatorTimeout = num;
                }
                else
                {
                    object[] objArray6 = { num };
                    CrapyLogger.log.ErrorFormat("Failed to set aggregator timeout. The value {0} should be greater than 0." + objArray6);
                }
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "RemoteTestingAggregatorTimeout", out num2))
            {
                if (num2 > 0)
                {
                    RemoteTestingAggregatorTimeout = num2;
                }
                else
                {
                    object[] objArray7 = { num };
                    CrapyLogger.log.ErrorFormat("Failed to set remote aggregator timeout. The value {0} should be greater than 0." + objArray7);
                }
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "RecordThinkTime", out flag8))
            {
                RecordThinkTime = flag8;
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "ThinkTimeThreshold", out num3))
            {
                if (num3 > 0)
                {
                    ThinkTimeThreshold = num3;
                }
                else
                {
                    object[] objArray8 = { num3 };
                    CrapyLogger.log.ErrorFormat("Failed to set ThinkTimeThreshold timeout. The value {0} should be greater than 0." + objArray8);
                }
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "ThinkTimeUpperCutoff", out num4))
            {
                if (num4 > 0)
                {
                    ThinkTimeUpperCutoff = num4;
                }
                else
                {
                    object[] objArray9 = { num4 };
                    CrapyLogger.log.ErrorFormat("Failed to set ThinkTimeUpperCutoff. The value {0} should be greater than 0." + objArray9);
                }
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "DelayBetweenActivities", out num5))
            {
                if (num5 > 0)
                {
                    DelayBetweenActivities = num5;
                }
                else
                {
                    object[] objArray10 = { num5 };
                    CrapyLogger.log.ErrorFormat("Failed to set DelayBetweenAction. The value {0} should be greater than 0." + objArray10);
                }
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "ShowCurrentlyRecording", out flag9))
            {
                ShowCurrentlyRecording = flag9;
            }
            if (ZappyTaskUtilities.GetValue(appSettings, "EncryptionKeyLocation", out str3) && string.IsNullOrEmpty(str3))
            {
                
            }
            int num6 = 0x10;
            if (ZappyTaskUtilities.GetValue(appSettings, "EncryptionKeyLength", out num6) && num6 != 0x10 && num6 != 0x18)
            {
                
            }
            if (!string.IsNullOrEmpty(str3))
            {
                object[] objArray11 = { str3, num6 };
                
                EncodeDecode.SetEncryptionKeyLocation(str3, num6);
            }
                        UpdateProcessList(appSettings, "ExcludeProcess", ExcludeProcessName, ExcludeProcess);
            UpdateProcessList(appSettings, "IncludeProcess", IncludeProcessName, IncludeProcess);
            if (ZappyTaskUtilities.GetValue(appSettings, "RecordOnDesktopProcess", out flag10))
            {
                RecordOnDesktopProcess = flag10;
            }
            ValidateTimeSettings();
            foreach (int num8 in ZappyTaskUtilities.GetLocaleIdentifiers(ZappyTaskUtilities.GetAppSettings()))
            {
                imeLanguageList.Add(num8);
            }
        }

        private void SetTimeSettingsToDefault()
        {
            aggregatorTimeout = 10000;
            remoteTestingAggregatorTimeout = 2000;
            thinktimeThreshold = 10000;
            thinktimeUpperCutoff = 120000;
            delayBetweenActivities = 100;
        }

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                                                
                        
                                                                                        
        private static void UpdateProcessList(IDictionary<string, string> appSettings, string keyName, ICollection<string> processNameList, ICollection<int> pidList)
        {
            string str;
            if (ZappyTaskUtilities.GetValue(appSettings, keyName, out str))
            {
                char[] separator = { '\n' };
                foreach (string str2 in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                    int num2;
                    string s = str2.Trim();
                    if (int.TryParse(s, out num2))
                    {
                        pidList.Add(num2);
                    }
                    else if (!string.IsNullOrEmpty(s))
                    {
                        processNameList.Add(ZappyTaskUtilities.GetExpandedLongPath(s));
                    }
                }
            }
        }

        private void ValidateTimeSettings()
        {
            try
            {
                if (thinktimeThreshold < delayBetweenActivities)
                {
                    throw new ArgumentException("ThinkTimeThreshold should to be greater than DelayBetweenActivities");
                }
                if (thinktimeUpperCutoff < thinktimeThreshold)
                {
                    throw new ArgumentException("ThinkTimeUpperCutoff should be greater than ThinkTimeThreshold");
                }
            }
            catch (ArgumentException exception)
            {
                CrapyLogger.log.Error(exception);
                SetTimeSettingsToDefault();
            }
        }

        public IDictionary<string, bool> AggregatorGroupSetting =>
            aggregatorGroupSetting;

        public int AggregatorTimeout
        {
            get =>
                aggregatorTimeout;
            set
            {
                object[] args = { value };
                
                aggregatorTimeout = value;
            }
        }

        public bool DebugModeOn { get; set; }

        public int DelayBetweenActivities
        {
            get =>
                delayBetweenActivities;
            set
            {
                object[] args = { value };
                
                delayBetweenActivities = value;
            }
        }

        public ICollection<int> ExcludeProcess =>
            excludeProcessIdList;

        public ICollection<string> ExcludeProcessName =>
            excludeProcessNameList;

                
        public KeyCombination HoverKey
        {
            get =>
                hoverKey;
            set
            {
                object[] args = { value.Modifier, value.Key };
                
                hoverKey = value;
            }
        }

        public ICollection<KeyCombination> IgnoreKeys =>
            ignoreKeys;

        public ICollection<int> ImeLanguageList =>
            imeLanguageList;

        public ICollection<int> IncludeProcess =>
            includeProcessIdList;

        public ICollection<string> IncludeProcessName =>
            includeProcessNameList;

        public bool IsImageActionLogEnabled
        {
            get =>
                isImageActionLogEnabled;
            set
            {
                isImageActionLogEnabled = value;
            }
        }


        public bool IsRemoteTestingEnabled
        {
            get =>
                isRemoteTestingEnabled;
            set
            {
                isRemoteTestingEnabled = value;
            }
        }

        public bool RecordImplicitHover
        {
            get
            {
                bool flag;
                if (aggregatorGroupSetting.TryGetValue("RecordImplicitHoverAggregator", out flag))
                {
                    return flag;
                }
                return true;
            }
            set
            {
                object[] args = { value };
                
                aggregatorGroupSetting.Add("RecordImplicitHoverAggregator", value);
                aggregatorGroupSetting.Add("DeleteImplicitHoverAggregator", !value);
            }
        }

        
        public bool RecordOnDesktopProcess { get; set; }

        public bool RecordThinkTime
        {
            get =>
                recordThinkTime;
            set
            {
                object[] args = { value };
                
                recordThinkTime = value;
            }
        }

        public bool ReliableRecorderStop { get; private set; }

        public int RemoteTestingAggregatorTimeout
        {
            get =>
                remoteTestingAggregatorTimeout;
            set
            {
                object[] args = { value };
                
                remoteTestingAggregatorTimeout = value;
            }
        }

        public bool ShowCurrentlyRecording
        {
            get =>
                showCurrentlyRecording;
            set
            {
                bool showCurrentlyRecording = this.showCurrentlyRecording;
                this.showCurrentlyRecording = value;
                object[] args = { value };
                
                if (this.showCurrentlyRecording != showCurrentlyRecording && PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ShowCurrentlyRecording"));
                }
            }
        }

        public int ThinkTimeThreshold
        {
            get =>
                thinktimeThreshold;
            set
            {
                object[] args = { value };
                
                thinktimeThreshold = value;
            }
        }

        public int ThinkTimeUpperCutoff
        {
            get =>
                thinktimeUpperCutoff;
            set
            {
                object[] args = { value };
                
                thinktimeUpperCutoff = value;
            }
        }

            }
}