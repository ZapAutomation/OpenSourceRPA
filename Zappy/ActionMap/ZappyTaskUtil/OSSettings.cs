using Microsoft.Win32;
using System;
using System.Drawing;
using System.Globalization;
using System.Security.Principal;
using System.Windows.Forms;
using Zappy.Decode.Helper;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal class OSSettings
    {
        private static string name;

        internal static SettingGroup CaptureAllSettings() =>
            new SettingGroup
            {
                GroupName = "OS",
                Setting = {
                    Name,
                    Version,
                    IsUserAdmin,
                    Is64BitOperatingSystem,
                    IsTerminalServerSession,
                    OSLanguage,
                    UserLocale,
                    DragFullWindows,
                    ScreenResolutionWidth,
                    ScreenResolutionHeight,
                    SystemDPIX,
                    SystemDPIY,
                    Aero,
                    UACEnabled,
                    UACPromptEnabled
                }
            };

        internal static bool IsCurrentUserAdministrator()
        {
            try
            {
                WindowsIdentity current = WindowsIdentity.GetCurrent();
                if (current != null)
                {
                    return new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public static Setting Aero =>
            new Setting("Aero", ZappyTaskUtilities.GetRegistryValue(Registry.CurrentUser, @"Software\Microsoft\Windows\DWM", "Composition"), 1);

        public static Setting DragFullWindows =>
            new Setting("DragFullWindows", SystemInformation.DragFullWindows, 2);

        public static Setting Is64BitOperatingSystem =>
            new Setting("Is64BitOperatingSystem", NativeMethods.Is64BitOperatingSystem, 2);

        public static Setting IsTerminalServerSession =>
            new Setting("IsTerminalServerSession", SystemInformation.TerminalServerSession, 2);

        public static Setting IsUserAdmin =>
            new Setting("IsUserAdmin", IsCurrentUserAdministrator(), 2);

        public static Setting Name
        {
            get
            {
                if (name == null)
                {
                    name = ZappyTaskUtilities.GetOSInfo("Caption");
                }
                return new Setting("Name", name, 2);
            }
        }

        public static Setting OSLanguage =>
            new Setting("OSLanguage", CultureInfo.InstalledUICulture.LCID, 1);

        public static Setting ScreenResolutionHeight =>
            new Setting("ScreenResolutionHeight", Screen.PrimaryScreen.Bounds.Height, 2);

        public static Setting ScreenResolutionWidth =>
            new Setting("ScreenResolutionWidth", Screen.PrimaryScreen.Bounds.Width, 2);


        public static Setting SystemDPIX
        {
            get
            {
                using (Graphics graphics = Graphics.FromHwnd(NativeMethods.GetDesktopWindow()))
                {
                    return new Setting("SystemDPIX", graphics.DpiX, 2);
                }
            }
        }


        public static Setting SystemDPIY
        {
            get
            {
                using (Graphics graphics = Graphics.FromHwnd(NativeMethods.GetDesktopWindow()))
                {
                    return new Setting("SystemDPIY", graphics.DpiY, 2);
                }
            }
        }

        public static Setting UACEnabled =>
            new Setting("UACEnabled", ZappyTaskUtilities.GetRegistryValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA"), 2);

        public static Setting UACPromptEnabled =>
            new Setting("UACPromptEnabled", ZappyTaskUtilities.GetRegistryValue(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin"), 1);

        public static Setting UserLocale =>
            new Setting("UserLocale", CultureInfo.CurrentCulture.LCID, 1);

        public static Setting Version =>
            new Setting("Version", Environment.OSVersion.VersionString, 2);
    }
}