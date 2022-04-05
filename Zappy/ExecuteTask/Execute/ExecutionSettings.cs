using System;
using System.Collections.Generic;
using System.Globalization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Execute
{
    public class ExecutionSettings
    {
        private bool alwaysSearchControls;
        private bool autoRefetchEnabled;
        private const int DefaultDelayBetweenActivities = 10;        private const int DefaultEncryptionKeyLength = 0x10;
        private const int DefaultHoverDuration = 300;
        private const int DefaultMaximumPermissibleRetryAttempts = 1;
        private const int DefaultMouseDragSpeed = 500;
        private const int DefaultMouseMoveSpeed = 450;
        private const int DefaultNavigationTimeout = -1;
        private const int DefaultSearchTimeout = 3000;        private const int DefaultSendKeysDelay = 10;
        private const double DefaultThinkTimeMultiplier = 1.0;
        private const int DefaultWaitForReadyTimeout = 5000;        private int delayBetweenActivities;
        internal bool HasAnotherActionStarted;
        private int hoverDuration;
        private string htmlLoggerState;
        private static ExecutionSettings _Default;
        private const int MinValueForDelayBetweenActivities = 100;
        private int mouseDragSpeed;
        private int mouseMoveSpeed;
        private int navigationTimeout;
        private double thinkTimeMultiplier;
        private bool waitForReadyEnabled;

        public ExecutionSettings()
        {
            ResetToDefault();
        }

        private static void AddLocaleIdentifiers(ExecutionSettings settings, int[] identifiers)
        {
            settings.ImeLanguageList.Clear();
            foreach (int num2 in identifiers)
            {
                settings.ImeLanguageList.Add(num2);
            }
        }

        internal void ApplySettings(IDictionary<string, string> appSettings)
        {
            DelayBetweenSendKeys = 0;
            DelayBetweenActivities = 0;
            SearchTimeout = GetConfigOptionValueInt("SearchTimeout", DefaultSearchTimeout, appSettings);
            SearchInMinimizedWindows = GetConfigOptionValue("SearchInMinimizedWindows", true, appSettings);
            MatchExactHierarchy = GetConfigOptionValue("MatchExactHierarchy", false, appSettings);
            SmartMatchOptions = GetSmartMatchOptions(appSettings);
            ThinkTimeMultiplier = GetConfigOptionValueDouble("ThinktimeMultiplier", 1.0, appSettings);
            WaitForReadyTimeout = GetConfigOptionValueInt("WaitForReadyTimeout", DefaultWaitForReadyTimeout, appSettings);
            MaximumRetryCount = GetConfigOptionValueInt("MaximumPermissibleRetryAttempts", 1, appSettings);
            NavigationTimeout = -1;
            AddLocaleIdentifiers(this, ZappyTaskUtilities.GetLocaleIdentifiers(appSettings));
            WaitForReadyEnabled = true;
            AutoRefetchEnabled = true;
            AlwaysSearchControls = false;
            TopLevelWindowSinglePassSearch = false;
            UpdateTitleInWindowSearch = true;
            HoverDuration = 100;
            MouseMoveSpeed = 4500;
            MouseDragSpeed = 5000;
            string keyLocation = GetConfigOptionValue("EncryptionKeyLocation", string.Empty, appSettings);
            int keySize = GetConfigOptionValueInt("EncryptionKeyLength", 0x10, appSettings);
            SetEncryptionKeyLocation(keyLocation, keySize);
            ContinueOnError = false;
            WaitForReadyLevel = WaitForReadyLevel.Disabled;            ALUtility.InitializeBrowserWindow();
        }

        internal static void ApplyTechnologySpecificCurrentSettings()
        {
            ScreenElement.SetPlaybackProperty(ExecuteParameter.SEARCH_TIMEOUT, ScreenElement.SearchTimeout);
            ScreenElement.SetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS, ScreenElement.SmartMatchOptions);
            ScreenElement.SetPlaybackProperty(ExecuteParameter.WFR_TIMEOUT, ScreenElement.WaitForReadyTimeout);
        }

        internal static void CheckForMinimumPermissibleValue(int minimumPermissibleValue, int value, string parameterName)
        {
            if (value < minimumPermissibleValue)
            {
                object[] args = { value, parameterName };
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidArgumentValue, args), parameterName);
            }
        }

        private static T GetConfigOptionValue<T>(string optionName, T defaultValue, IDictionary<string, string> appSettings)
        {
            T local;
            if (!ZappyTaskUtilities.GetValue(appSettings, optionName, out local))
            {
                local = defaultValue;
            }
            return local;
        }

        private static double GetConfigOptionValueDouble(string optionName, double defaultValue, IDictionary<string, string> appSettings)
        {
            double num;
            if (ZappyTaskUtilities.GetValue(appSettings, optionName, out num) && num >= 0.0)
            {
                return num;
            }
            return defaultValue;
        }

        private static int GetConfigOptionValueInt(string optionName, int defaultValue, IDictionary<string, string> appSettings)
        {
            int num;
            if (ZappyTaskUtilities.GetValue(appSettings, optionName, out num) && num >= 0)
            {
                return num;
            }
            return defaultValue;
        }

        private static SmartMatchOptions GetSmartMatchOptions(IDictionary<string, string> appSettings)
        {
            string str;
            SmartMatchOptions options = SmartMatchOptions.Control | SmartMatchOptions.TopLevelWindow;             if (ZappyTaskUtilities.GetValue(appSettings, "SmartMatchOptions", out str) && !string.IsNullOrEmpty(str))
            {
                try
                {
                    options = (SmartMatchOptions)Enum.Parse(typeof(SmartMatchOptions), str, true);
                }
                catch (ArgumentException)
                {
                }
            }
            return options;
        }

        public void ResetToDefault()
        {
            ApplySettings(ZappyTaskUtilities.GetAppSettings());
        }

        public void SetEncryptionKeyLocation(string keyLocation, int keySize)
        {
            object[] args = { keyLocation, keySize };
            
            EncodeDecode.SetEncryptionKeyLocation(keyLocation, keySize);
        }

        internal static void SetSearchTimeOutOrThrowException(int searchTimeOut, string queryId)
        {
            if (searchTimeOut <= 0)
            {
                throw new Exception(queryId);
            }
            ScreenElement.SearchTimeout = searchTimeOut;
        }


        public bool AlwaysSearchControls
        {
            get =>
                alwaysSearchControls;
            set
            {
                object[] args = { value };
                
                alwaysSearchControls = value;
            }
        }


        internal bool AutoRefetchEnabled
        {
            get =>
                autoRefetchEnabled;
            set
            {
                object[] args = { value };
                
                autoRefetchEnabled = value;
            }
        }

        public bool ContinueOnError { get; set; }

        internal bool DebugModeOn
        {
            get =>
                ScreenElement.DebugModeOn;
            set
            {
                object[] args = { value };
                
                ScreenElement.DebugModeOn = value;
            }
        }

        public int DelayBetweenActivities
        {
            get =>
                30;
            set
            {
                object[] args = { value };
                
                CheckForMinimumPermissibleValue(0, value, "DelayBetweenActivities");
                delayBetweenActivities = Math.Max(value, 100);
            }
        }

        internal int DelayBetweenSendKeys
        {
            get => 0;            set
            {
                object[] args = { value };
                
                CheckForMinimumPermissibleValue(0, value, "SendKeysDelay");
                try
                {
                    ScreenElement.SendKeysDelay = value;
                }
                catch (Exception exception)
                {
                    object[] objArray2 = { "DelayBetweenSendKeys" };
                    string actionName = string.Format(CultureInfo.CurrentCulture, Resources.SetPropertyActionName, objArray2);
                    ExecutionHandler.MapAndThrowException(exception, actionName, value, null);
                    throw;
                }
            }
        }

        internal int HoverDuration
        {
            get =>
                hoverDuration;
            set
            {
                object[] args = { value };
                
                CheckForMinimumPermissibleValue(0, value, "HoverDuration");
                hoverDuration = value;
            }
        }

        public ICollection<int> ImeLanguageList =>
            ScreenElement.ImeLanguageList;

        internal static ExecutionSettings Default
        {
            get
            {
                if (_Default == null)
                {
                    _Default = new ExecutionSettings();
                }
                return _Default;
            }
        }

        public string LoggerOverrideState
        {
            get =>
                htmlLoggerState;
            set
            {
                htmlLoggerState = value;
                            }
        }

        public bool MatchExactHierarchy
        {
            get =>
                ScreenElement.MatchExactHierarchy;
            set
            {
                try
                {
                    object[] args = { value };
                    
                    ScreenElement.MatchExactHierarchy = value;
                }
                catch (Exception exception)
                {
                    ExecutionHandler.MapAndThrowException(exception, new ExecuteContext());
                    throw;
                }
            }
        }

        public int MaximumRetryCount { get; set; }

        internal int MouseDragSpeed
        {
            get =>
                mouseDragSpeed;
            set
            {
                object[] args = { value };
                
                CheckForMinimumPermissibleValue(0, value, "MouseDragSpeed");
                mouseDragSpeed = value;
            }
        }

        internal int MouseMoveSpeed
        {
            get =>
                mouseMoveSpeed;
            set
            {
                object[] args = { value };
                
                CheckForMinimumPermissibleValue(0, value, "MouseMoveSpeed");
                mouseMoveSpeed = value;
            }
        }

        internal int NavigationTimeout
        {
            get =>
                navigationTimeout;
            set
            {
                IUITechnologyPluginManager pluginManager = ZappyTaskService.Instance.PluginManager;
                if (pluginManager != null)
                {
                    object[] args = { value };
                    
                    CheckForMinimumPermissibleValue(-1, value, "NavigationTimeout");
                    foreach (IUITechnologyManager manager2 in pluginManager.TechnologyManagers)
                    {
                        manager2.SetTechnologyManagerProperty(UITechnologyManagerProperty.NavigationTimeout, value);
                    }
                    navigationTimeout = value;
                    if (navigationTimeout != -1)
                    {
                        ZappyTaskService.Instance.EatNavigationTimeoutException = false;
                    }
                }
            }
        }

        public bool SearchInMinimizedWindows
        {
            get =>
                ScreenElement.SearchInMinimizedWindows;
            set
            {
                try
                {
                    object[] args = { value };
                    
                    ScreenElement.SearchInMinimizedWindows = value;
                }
                catch (Exception exception)
                {
                    ExecutionHandler.MapAndThrowException(exception, new ExecuteContext());
                    throw;
                }
            }
        }

        public int SearchTimeout
        {
            get =>
                ScreenElement.SearchTimeout;
            set
            {
                object[] args = { value };
                                try
                {
                    CheckForMinimumPermissibleValue(0, value, "SearchTimeout");
                }
                catch
                {
                    value = 0;
                }

                try
                {
                    ScreenElement.SearchTimeout = value;
                }
                catch (Exception exception)
                {
                    ExecutionHandler.MapAndThrowException(exception, new ExecuteContext());
                    throw;
                }
            }
        }

        public bool SendKeysAsScanCode
        {
            get =>
                ScreenElement.SendKeysAsScanCode;
            set
            {
                object[] args = { value };
                
                ScreenElement.SendKeysAsScanCode = value;
            }
        }

                                                                                                                
        public bool SkipSetPropertyVerification
        {
            get =>
                ScreenElement.IgnoreVerification;
            set
            {
                object[] args = { value };
                
                ScreenElement.IgnoreVerification = value;
            }
        }

        public SmartMatchOptions SmartMatchOptions
        {
            get =>
                ScreenElement.SmartMatchOptions;
            set
            {
                try
                {
                    object[] args = { value };
                    
                    ScreenElement.SmartMatchOptions = value;
                }
                catch (Exception exception)
                {
                    ExecutionHandler.MapAndThrowException(exception, new ExecuteContext());
                    throw;
                }
            }
        }

        public double ThinkTimeMultiplier
        {
            get =>
                thinkTimeMultiplier;
            set
            {
                object[] args = { value };
                
                if (value < 0.0)
                {
                    object[] objArray2 = { value, "ThinkTimeMultiplier" };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidArgumentValue, objArray2));
                }
                thinkTimeMultiplier = value;
            }
        }

        internal bool TopLevelWindowSinglePassSearch
        {
            get =>
                ScreenElement.TopLevelWindowSinglePassSearch;
            set
            {
                object[] args = { value };
                
                ScreenElement.TopLevelWindowSinglePassSearch = value;
            }
        }

        internal bool UpdateTitleInWindowSearch { get; set; }

        internal bool WaitForReadyEnabled
        {
            get =>
                waitForReadyEnabled;
            set
            {
                object[] args = { value };
                
                waitForReadyEnabled = value;
            }
        }

        public WaitForReadyLevel WaitForReadyLevel
        {
            get =>
                ScreenElement.WaitForReadyLevel;
            set
            {
                ScreenElement.WaitForReadyLevel = value;
            }
        }

        public int WaitForReadyTimeout
        {
            get =>
                ScreenElement.WaitForReadyTimeout;
            set
            {
                object[] args = { value };
                
                CheckForMinimumPermissibleValue(0, value, "WaitForReadyTimeout");
                try
                {
                    ScreenElement.WaitForReadyTimeout = value;
                }
                catch (Exception exception)
                {
                    ExecutionHandler.MapAndThrowException(exception, new ExecuteContext());
                    throw;
                }
            }
        }

    }
}