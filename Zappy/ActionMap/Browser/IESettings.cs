using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Crapy.ActionMap.ZappyTaskUtil;
using Crapy.Decode.LogManager;
using Crapy.Properties;
using Microsoft.Win32;

namespace Crapy.ActionMap.Browser
{
    public static class IESettings
    {
        private static Version currentBrowserVersion = new Version(0, 0);
        private static Version invalidVersion = new Version(0, 0);
        private static bool? isBrowserSupported;
        private static bool? isNotificationToolBarSupported;
        private static Version MinimumRequired64BitBrowserVersion = new Version(9, 0);
        private static Version MinimumRequiredBrowserVersion = new Version(7, 0);
        private static Version MinimumRequiredNotificationToolBrowserVersion = new Version(9, 0);

        internal static SettingGroup CaptureAllSettings() =>
            new SettingGroup
            {
                GroupName = "IE",
                Setting = {
                    Version,
                    InformationBar,
                    AutoCompletePassword,
                    AutoCompleteForm,
                    DefaultBrowser,
                    PopupBlocker,
                    TabbedBrowsing,
                    InternetZoneSecurity,
                    IntranetZoneSecurity,
                    TrustedZoneSecurity,
                    RestrictedZoneSecurity,
                    PhishingFilter,
                    EnhancedSecurityConfiguration
                }
            };

        public static bool Check64BitBrowserVersionSupport()
        {
            bool flag = false;
            if (CurrentBrowserVersion != null && CurrentBrowserVersion >= MinimumRequired64BitBrowserVersion && ZappyTaskUtilities.IsWin8OrGreaterOs())
            {
                flag = true;
            }
            return flag;
        }

        private static bool CheckBrowserVersion()
        {
            bool flag = false;
            if (CurrentBrowserVersion != null && CurrentBrowserVersion >= MinimumRequiredBrowserVersion)
            {
                flag = true;
            }
            return flag;
        }

        public static void ThrowIfBrowserVersionNotSupported()
        {
            if (!CheckBrowserVersion())
            {
                Version currentBrowserVersion = CurrentBrowserVersion;
                if (currentBrowserVersion != null)
                {
                    object[] args = { currentBrowserVersion.Major, currentBrowserVersion.Minor };
                    throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.BrowserVersionNotSupported, args));
                }
                throw new Exception(Resources.BrowserVersionNotDetected);
            }
        }

        public static Setting AutoCompleteForm =>
            new Setting("AutoCompleteForm", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\Main", "Use FormSuggest"), 1);

        public static Setting AutoCompletePassword =>
            new Setting("AutoCompletePassword", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\Main", "FormSuggest PW Ask"), 1);

        internal static Version CurrentBrowserVersion
        {
            get
            {
                if (currentBrowserVersion != null && currentBrowserVersion.Equals(invalidVersion))
                {
                    currentBrowserVersion = null;
                    try
                    {
                        string str = ZappyTaskUtilities.GetRegistryValue<string>(Registry.LocalMachine, @"Software\Microsoft\Internet Explorer", "Version", false);
                        if (!string.IsNullOrEmpty(str))
                        {
                            currentBrowserVersion = new Version(str);
                        }
                    }
                    catch (SystemException exception)
                    {
                        CrapyLogger.log.Error(exception);
                    }
                }
                return currentBrowserVersion;
            }
            set
            {
                currentBrowserVersion = value;
            }
        }

        public static Setting DefaultBrowser =>
            new Setting("DefaultBrowser", ZappyTaskUtilities.GetRegistryValue(Registry.LocalMachine, @"SOFTWARE\Clients\StartMenuInternet", string.Empty), 1);

        public static Setting EnhancedSecurityConfiguration
        {
            get
            {
                object obj2 = null;
                if (OSSettings.IsCurrentUserAdministrator())
                {
                    obj2 = ZappyTaskUtilities.GetRegistryValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Active Setup\Installed Components\{A509B1A8-37EF-4b3f-8CFC-4F3A74704073}", "IsInstalled");
                }
                else
                {
                    obj2 = ZappyTaskUtilities.GetRegistryValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Active Setup\Installed Components\{A509B1A7-37EF-4b3f-8CFC-4F3A74704073}", "IsInstalled");
                }
                return new Setting("EnhancedSecurityConfiguration", obj2, 1);
            }
        }

        public static Setting InformationBar =>
            new Setting("InformationBar", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\InformationBar", "FirstTime"), 1);

        public static Setting InternetZoneSecurity =>
            new Setting("InternetZoneSecurity", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\1", "CurrentLevel"), 2);

        public static Setting IntranetZoneSecurity =>
            new Setting("IntranetZoneSecurity", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\2", "CurrentLevel"), 2);

        internal static bool IsBrowserSupported
        {
            get
            {
                if (!isBrowserSupported.HasValue)
                {
                    isBrowserSupported = CheckBrowserVersion();
                }
                return isBrowserSupported.Value;
            }
        }

        internal static bool IsNotificationToolBarSupported
        {
            get
            {
                if (!isNotificationToolBarSupported.HasValue)
                {
                    isNotificationToolBarSupported = CurrentBrowserVersion != null && CurrentBrowserVersion >= MinimumRequiredNotificationToolBrowserVersion;
                }
                return isNotificationToolBarSupported.Value;
            }
        }


        public static Setting PhishingFilter =>
            new Setting("PhishingFilter", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"SOFTWARE\Microsoft\Internet Explorer\PhishingFilter", "Enabled"), 1);

        public static Setting PopupBlocker =>
            new Setting("PopupBlocker", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\New Windows", "PopupMgr"), 1);

        public static Setting RestrictedZoneSecurity =>
            new Setting("RestrictedZoneSecurity", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\4", "CurrentLevel"), 2);

        public static Setting TabbedBrowsing =>
            new Setting("TabbedBrowsing", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Internet Explorer\TabbedBrowsing", "Enabled"), 2);

        public static Setting TrustedZoneSecurity =>
            new Setting("TrustedZoneSecurity", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3", "CurrentLevel"), 2);

        public static Setting Version =>
            new Setting("Version", ZappyTaskUtilities.GetRegistryValue(Registry.LocalMachine, @"Software\Microsoft\Internet Explorer", "Version"), 1);
    }
}