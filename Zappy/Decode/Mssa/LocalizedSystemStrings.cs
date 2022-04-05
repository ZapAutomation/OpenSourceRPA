using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Mssa
{
    public class LocalizedSystemStrings
    {
        private string addToDictionaryText = "Add To Dictionary";
        private string authenticationDialogTitle = "Connect to";
        private const string authUIDllResourceName = "authui.dll.mui";
        private string autoComplete = "AutoComplete";
        private string autoCompletePasswords = "AutoComplete Passwords";
        private const string commonDialogDll = "comdlg32.dll";
        private const string commonDialogResourceDll = "comdlg32.dll.mui";
        private string copyText = "Copy";
        private const string credUIDllResourceName = "credui.dll.mui";
        private static CultureInfo currentOSCulture;
        private string cutText = "Cut";
        private string deleteText = "Delete";
        private const ushort DialogExSignature = 0xffff;
        private const string explorerDllResourceName = "explorer.exe.mui";
        private string fileUploadComboBoxName = "File name:";
        private string firefoxFileUploadDialogTitle = "File Upload";
        private string ie8AddressBarButtonName = "Show Address Bar Autocomplete";
        private string ie9NotificationBar = "Notification Bar";
        private string ieAddressNameText = "Address";
        private string ieAddressNameTextIE9 = "Address and search using %s";
        private string ieBackButtonText = "Back";
        private string ieCloseButtonText = "Close";
        private string ieFileUploadDialogTitle = "Choose file";
        private string ieForwardButtonText = "Forward";
        private const string ieframeDllResourceName = "ieframe.dll.mui";
        private string ieMuteMenuItemText = "Mute";
        private string iePauseMenuItemText = "Pause";
        private string iePlayMenuItemText = "Play";
        private string ieRefreshButtonText = "Refresh (F5)";
        private string ieStopButtonText = "Stop (Esc)";
        private string ieUnmuteMenuItemText = "Unmute";
        private string ignoreText = "Ignore";
        private string informationBar = "Information Bar";
        private static LocalizedSystemStrings instance = new LocalizedSystemStrings();
        private string internetExplorerDialogTitle = "Internet Explorer";
        private static bool? isIE9OrLater;
        private const uint LOAD_LIBRARY_AS_DATAFILE = 2;
        private const uint MaxDialogTitleLen = 0x100;
        private const int MaxStringResourceLength = 0x1000;
        private string microsoftPhishingFilter = "Microsoft Phishing Filter";
        private const string msaaResourceDll = "oleaccrc.dll";
        private const string mshtmlDllResourceName = "mshtml.dll.mui";
        private const ushort NullChar = 0;
        private string pasteText = "Paste";
        private const uint RT_DIALOG = 5;
        private string selectallText = "Select all";
        private string undoText = "Undo";
        private const string user32DllResourceName = "user32.dll.mui";
        private const string wetDllResourceName = "wet.dll.mui";
        private string win7AuthenticationDialogTitle = "Windows Security";
        private string windowMaximizeButtonText = "Maximize";
        private string windowRestoreButtonText = "Restore";
        private string windowsQuickLaunchText = "Quick Launch";
        private string windowsRunningApplicationsText = "Running applications";
        private string winFormsEditableControlNameENU = "Editing Control";
        private string winFormsEditableControlNameLocalized;
        private string WinFormsEditingControlResourceID = "DataGridView_AccEditingControlAccName";
        private const string winFormsResourceDllName = "System.Windows.Forms";

        private LocalizedSystemStrings()
        {
            if (File.Exists(Path.Combine(Environment.SystemDirectory, "oleaccrc.dll")))
            {
                windowMaximizeButtonText = SetStringIfNotNull(GetStringFromResource(0x8f, "oleaccrc.dll"), windowMaximizeButtonText, CurrentOSCulture.Name);
                windowRestoreButtonText = SetStringIfNotNull(GetStringFromResource(0x92, "oleaccrc.dll"), windowRestoreButtonText, CurrentOSCulture.Name);
            }
            object[] args = { MouseButtons.Left };
                        Assembly assembly = null;
            foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Equals(assembly2.GetName().Name, "System.Windows.Forms", StringComparison.Ordinal))
                {
                    assembly = assembly2;
                }
            }
            if (assembly != null)
            {
                winFormsEditableControlNameLocalized = new ResourceManager("System.Windows.Forms", assembly).GetString(WinFormsEditingControlResourceID);
            }
            string str2 = GetFilePathBasedOnLanguage(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe.mui", CurrentOSCulture.Name);
            if (!string.IsNullOrWhiteSpace(str2) && File.Exists(str2))
            {
                windowsRunningApplicationsText = SetStringIfNotNull(GetStringFromResource(0x252, str2), windowsRunningApplicationsText, CurrentOSCulture.Name);
            }
            string str3 = GetFilePathBasedOnLanguage(Path.Combine(Environment.SystemDirectory, "migwiz"), "wet.dll.mui", CurrentOSCulture.Name);
            if (!string.IsNullOrWhiteSpace(str3) && File.Exists(str3))
            {
                windowsQuickLaunchText = SetStringIfNotNull(GetStringFromResource(0x1aa, str3), windowsQuickLaunchText, CurrentOSCulture.Name);
            }
            LoadIEStrings();
        }

        private static void AlignOffsetToNextDWordBoundary(IntPtr resourceStartAddress, ref int currentOffset)
        {
            long num = (resourceStartAddress.ToInt64() + currentOffset) % 4L;
            if (num != 0)
            {
                currentOffset += 4 - (int)num;
            }
        }

        private static void AlignOffsetToNextWordBoundary(IntPtr resourceStartAddress, ref int currentOffset)
        {
            if ((currentOffset + resourceStartAddress.ToInt64()) % 2L != 0)
            {
                currentOffset++;
            }
        }

        private static string GetDialogTitleFromResource(uint resourceId, string resourceFileName) =>
            GetDialogTitleFromResource(resourceId, 0, resourceFileName);

        private static string GetDialogTitleFromResource(uint resourceId, int controlId, string resourceFileName)
        {
            IntPtr ptr;
            uint num;
            string str = string.Empty;
            IntPtr resourceStartAddress = LoadResource(resourceId, resourceFileName, out ptr, out num);
            try
            {
                if (resourceStartAddress == IntPtr.Zero)
                {
                    return str;
                }
                if (ReadWordAtOffset(resourceStartAddress, 2, num) == 0xffff)
                {
                    str = GetTitleFromExtendedDialogTemplate(resourceStartAddress, num, controlId);
                    object[] objArray1 = { str, resourceId, controlId, resourceFileName };
                                        return str;
                }
                str = GetTitleFromDialogTemplate(resourceStartAddress, num, controlId);
                object[] args = { str, resourceId, controlId, resourceFileName };
                            }
            catch (ZappyTaskException)
            {
                object[] objArray3 = { resourceId, controlId, resourceFileName };
                            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    NativeMethods.FreeLibrary(ptr);
                }
            }
            return str;
        }

        private static string GetFilePathBasedOnLanguage(string resourceFileName, string language) =>
            GetFilePathBasedOnLanguage(Environment.SystemDirectory, resourceFileName, language);

        private static string GetFilePathBasedOnLanguage(string directoryName, string resourceFileName, string language)
        {
            string path = Path.Combine(Path.Combine(directoryName, language), resourceFileName);
            if (!File.Exists(path))
            {
                language = CultureInfo.CurrentCulture.Name;
                path = Path.Combine(Path.Combine(directoryName, language), resourceFileName);
            }
            return path;
        }

        internal static string GetMshtmlResourceFilePath()
        {
            string name = CurrentOSCulture.Name;
            return GetFilePathBasedOnLanguage("mshtml.dll.mui", name);
        }

        private static string GetNullTerminatedString(IntPtr resource, uint resourceLength, int titleStartIndex)
        {
            char ch;
            StringBuilder builder = new StringBuilder();
            int num = (int)Math.Min(0x200, resourceLength);
            for (int i = 0; i < num && !Equals(ch = (char)(ushort)Marshal.ReadInt16(resource, titleStartIndex + i), '\0'); i += 2)
            {
                builder.Append(ch);
            }
            return builder.ToString();
        }

        internal static string GetStringFromResource(uint resourceId, string resourceFileName)
        {
            string str;
            IntPtr hInstance = NativeMethods.LoadLibraryEx(resourceFileName, IntPtr.Zero, 2);
            if (hInstance == IntPtr.Zero)
            {
                object[] args = { resourceFileName, Marshal.GetLastWin32Error() };
                                return string.Empty;
            }
            try
            {
                StringBuilder lpBuffer = new StringBuilder(0x1000);
                if (NativeMethods.LoadString(hInstance, resourceId, lpBuffer, 0x1000) < 0)
                {
                    object[] objArray2 = { resourceId, resourceFileName, Marshal.GetLastWin32Error() };
                                        return string.Empty;
                }
                str = lpBuffer.ToString();
            }
            finally
            {
                if (hInstance != IntPtr.Zero)
                {
                    NativeMethods.FreeLibrary(hInstance);
                }
            }
            return str;
        }

        private static string GetTitleFromDialogTemplate(IntPtr resourceHandle, uint size, int controlId)
        {
            if (size < Marshal.SizeOf(typeof(NativeMethods.DLGTEMPLATE)))
            {
                                throw new ZappyTaskException();
            }
            NativeMethods.DLGTEMPLATE dlgtemplate = new NativeMethods.DLGTEMPLATE();
            dlgtemplate = (NativeMethods.DLGTEMPLATE)Marshal.PtrToStructure(resourceHandle, typeof(NativeMethods.DLGTEMPLATE));
            int currentOffset = Marshal.SizeOf(typeof(NativeMethods.DLGTEMPLATE));
            ProcessMenuAndClassItems(resourceHandle, ref currentOffset, size);
            AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
            string str = GetNullTerminatedString(resourceHandle, size, currentOffset);
            if (controlId == 0)
            {
                return str;
            }
            currentOffset += GetWordLength(str.Length + 1);
            if ((dlgtemplate.dwStyle & 0x40) != 0 || (dlgtemplate.dwStyle & 0x48) != 0)
            {
                currentOffset += GetWordLength(1);
                AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
                string str2 = GetNullTerminatedString(resourceHandle, size, currentOffset);
                currentOffset += GetWordLength(str2.Length + 1);
            }
            int num2 = Marshal.SizeOf(typeof(NativeMethods.DLGITEMTEMPLATE));
            for (int i = 0; i < dlgtemplate.cDlgItems; i++)
            {
                AlignOffsetToNextDWordBoundary(resourceHandle, ref currentOffset);
                if (size < currentOffset + num2)
                {
                                        throw new ZappyTaskException();
                }
                NativeMethods.DLGITEMTEMPLATE dlgitemtemplate = new NativeMethods.DLGITEMTEMPLATE();
                dlgitemtemplate = (NativeMethods.DLGITEMTEMPLATE)Marshal.PtrToStructure(new IntPtr(resourceHandle.ToInt64() + currentOffset), typeof(NativeMethods.DLGITEMTEMPLATE));
                currentOffset += num2;
                AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
                if (ReadWordAtOffset(resourceHandle, currentOffset, size) == 0xffff)
                {
                    currentOffset += GetWordLength(2);
                }
                else
                {
                    string str3 = GetNullTerminatedString(resourceHandle, size, currentOffset);
                    currentOffset += GetWordLength(str3.Length + 1);
                }
                AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
                if (ReadWordAtOffset(resourceHandle, currentOffset, size) == 0xffff)
                {
                    currentOffset += GetWordLength(2);
                }
                else
                {
                    string str4 = GetNullTerminatedString(resourceHandle, size, currentOffset);
                    if (dlgitemtemplate.id == controlId)
                    {
                        return str4;
                    }
                    currentOffset += GetWordLength(str4.Length + 1);
                }
                AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
                ushort numWords = ReadWordAtOffset(resourceHandle, currentOffset, size);
                if (numWords != 0)
                {
                    currentOffset += GetWordLength(numWords);
                }
                else
                {
                    currentOffset += GetWordLength(1);
                }
            }
            object[] args = { controlId };
                        throw new ZappyTaskException();
        }

        private static string GetTitleFromExtendedDialogTemplate(IntPtr resourceHandle, uint size, int controlId)
        {
            if (size < Marshal.SizeOf(typeof(NativeMethods.DLGTEMPLATEEX)))
            {
                                throw new ZappyTaskException();
            }
            NativeMethods.DLGTEMPLATEEX dlgtemplateex = new NativeMethods.DLGTEMPLATEEX();
            dlgtemplateex = (NativeMethods.DLGTEMPLATEEX)Marshal.PtrToStructure(resourceHandle, typeof(NativeMethods.DLGTEMPLATEEX));
            int currentOffset = Marshal.SizeOf(typeof(NativeMethods.DLGTEMPLATEEX));
            ProcessMenuAndClassItems(resourceHandle, ref currentOffset, size);
            AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
            string str = GetNullTerminatedString(resourceHandle, size, currentOffset);
            if (controlId == 0)
            {
                return str;
            }
            currentOffset += GetWordLength(str.Length + 1);
            if ((dlgtemplateex.dwStyle & 0x40) != 0 || (dlgtemplateex.dwStyle & 0x48) != 0)
            {
                currentOffset += GetWordLength(1);
                currentOffset += GetWordLength(1);
                currentOffset += GetWordLength(1);
                AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
                string str2 = GetNullTerminatedString(resourceHandle, size, currentOffset);
                currentOffset += GetWordLength(str2.Length + 1);
            }
            int num2 = Marshal.SizeOf(typeof(NativeMethods.DLGITEMTEMPLATEEX));
            for (int i = 0; i < dlgtemplateex.cDlgItems; i++)
            {
                AlignOffsetToNextDWordBoundary(resourceHandle, ref currentOffset);
                if (size < currentOffset + num2)
                {
                                        throw new ZappyTaskException();
                }
                NativeMethods.DLGITEMTEMPLATEEX dlgitemtemplateex = new NativeMethods.DLGITEMTEMPLATEEX();
                dlgitemtemplateex = (NativeMethods.DLGITEMTEMPLATEEX)Marshal.PtrToStructure(new IntPtr(resourceHandle.ToInt64() + currentOffset), typeof(NativeMethods.DLGITEMTEMPLATEEX));
                currentOffset += num2;
                AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
                if (ReadWordAtOffset(resourceHandle, currentOffset, size) == 0xffff)
                {
                    currentOffset += GetWordLength(2);
                }
                else
                {
                    string str3 = GetNullTerminatedString(resourceHandle, size, currentOffset);
                    currentOffset += GetWordLength(str3.Length + 1);
                }
                AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
                if (ReadWordAtOffset(resourceHandle, currentOffset, size) == 0xffff)
                {
                    currentOffset += GetWordLength(2);
                }
                else
                {
                    string str4 = GetNullTerminatedString(resourceHandle, size, currentOffset);
                    if (dlgitemtemplateex.id == controlId)
                    {
                        return str4;
                    }
                    currentOffset += GetWordLength(str4.Length + 1);
                }
                AlignOffsetToNextWordBoundary(resourceHandle, ref currentOffset);
                ushort numWords = ReadWordAtOffset(resourceHandle, currentOffset, size);
                if (numWords != 0)
                {
                    currentOffset += GetWordLength(numWords);
                }
                else
                {
                    currentOffset += GetWordLength(1);
                }
            }
            object[] args = { controlId };
                        throw new ZappyTaskException();
        }

        private static int GetWordLength(int numWords) =>
            numWords * 2;

        private void LoadIEStrings()
        {
            string name = CurrentOSCulture.Name;
            object[] args = { name };
                        string filePathBasedOnLanguage = GetFilePathBasedOnLanguage("ieframe.dll.mui", name);
            string path = null;
            if (IsIE9OrLater)
            {
                path = GetMshtmlResourceFilePath();
                if (File.Exists(path))
                {
                    object[] objArray2 = { path };
                                    }
            }
            IEAlertDialogTitle = "Message from webpage";
            IEGenericTitle = "Windows Internet Explorer";
            IEPromptDialogTitle = "Explorer User Prompt";
            IEDialogCancelButton = "Cancel";
            IEDialogCloseButton = "Close";
            IEDialogOkButton = "OK";
            IEDialogRetryButton = "Retry";
            IESecurityWarningDialogTitle = "Security Warning";
            if (File.Exists(filePathBasedOnLanguage))
            {
                string stringFromResource;
                object[] objArray3 = { filePathBasedOnLanguage };
                                ieBackButtonText = SetStringIfNotNull(GetStringFromResource(0xe541, filePathBasedOnLanguage), ieBackButtonText, name);
                object[] objArray4 = { ieBackButtonText };
                                ieForwardButtonText = SetStringIfNotNull(GetStringFromResource(0xe542, filePathBasedOnLanguage), ieForwardButtonText, name);
                object[] objArray5 = { ieForwardButtonText };
                                ieRefreshButtonText = SetStringIfNotNull(GetStringFromResource(0x3176, filePathBasedOnLanguage), ieRefreshButtonText, name);
                object[] objArray6 = { ieRefreshButtonText };
                                ieStopButtonText = SetStringIfNotNull(GetStringFromResource(0x3175, filePathBasedOnLanguage), ieStopButtonText, name);
                object[] objArray7 = { ieStopButtonText };
                                ieCloseButtonText = SetStringIfNotNull(GetStringFromResource(0x31af, filePathBasedOnLanguage), ieCloseButtonText, name);
                object[] objArray8 = { ieCloseButtonText };
                                ie8AddressBarButtonName = SetStringIfNotNull(GetStringFromResource(0x3184, filePathBasedOnLanguage), ie8AddressBarButtonName, name);
                object[] objArray9 = { ie8AddressBarButtonName };
                                ieAddressNameText = SetStringIfNotNull(GetStringFromResource(0x3ea, filePathBasedOnLanguage), ieAddressNameText, name);
                object[] objArray10 = { ieAddressNameText };
                                if (IsIE9OrLater)
                {
                    stringFromResource = GetStringFromResource(0x1544, path);
                    ieAddressNameTextIE9 = SetStringIfNotNull(GetStringFromResource(0x3262, filePathBasedOnLanguage), ieAddressNameTextIE9, name);
                    ieAddressNameTextIE9 = ieAddressNameTextIE9.Replace("%s", ".+");
                    object[] objArray11 = { ieAddressNameTextIE9 };
                                        iePlayMenuItemText = SetStringIfNotNull(GetStringFromResource(0x350, path), iePlayMenuItemText, name);
                    object[] objArray12 = { iePlayMenuItemText };
                                        iePauseMenuItemText = SetStringIfNotNull(GetStringFromResource(0x351, path), iePauseMenuItemText, name);
                    object[] objArray13 = { iePauseMenuItemText };
                                        ieMuteMenuItemText = SetStringIfNotNull(GetStringFromResource(0x354, path), ieMuteMenuItemText, name);
                    object[] objArray14 = { ieMuteMenuItemText };
                                        ieUnmuteMenuItemText = SetStringIfNotNull(GetStringFromResource(0x355, path), ieUnmuteMenuItemText, name);
                    object[] objArray15 = { ieUnmuteMenuItemText };
                                        string str8 = GetStringFromResource(0x1fc6, path);
                    if (!string.IsNullOrEmpty(str8))
                    {
                        str8 = str8.Replace("%s", ".+");
                    }
                    IEPromptDialogTitle = SetStringIfNotNull(str8, "needs some information", name);
                    object[] objArray16 = { IEPromptDialogTitle };
                                        IEAlertDialogTitle = SetStringIfNotNull(GetStringFromResource(0x92e, path), IEAlertDialogTitle, name);
                    object[] objArray17 = { IEAlertDialogTitle };
                                        IEDialogCancelButton = SetStringIfNotNull(GetStringFromResource(0xd104, path), IEDialogCancelButton, name);
                    object[] objArray18 = { IEDialogCancelButton };
                                        IEDialogOkButton = SetStringIfNotNull(GetStringFromResource(0x20e4, filePathBasedOnLanguage), IEDialogOkButton, name);
                    object[] objArray19 = { IEDialogOkButton };
                                        string str9 = GetStringFromResource(0xd058, filePathBasedOnLanguage);
                    object[] objArray20 = { str9 };
                                        if (!string.IsNullOrEmpty(str9))
                    {
                        str9 = str9.Replace("&", string.Empty).Trim();
                    }
                    IEDialogRetryButton = SetStringIfNotNull(str9, IEDialogRetryButton, name);
                    IEGenericTitle = SetStringIfNotNull(GetStringFromResource(0x3145, filePathBasedOnLanguage), IEGenericTitle, name);
                    object[] objArray21 = { IEGenericTitle };
                                    }
                else
                {
                    stringFromResource = GetStringFromResource(0x1544, filePathBasedOnLanguage);
                    IEPromptDialogTitle = SetStringIfNotNull(GetDialogTitleFromResource(0x1fc3, filePathBasedOnLanguage), IEPromptDialogTitle, name);
                    object[] objArray22 = { IEPromptDialogTitle };
                                        IEAlertDialogTitle = SetStringIfNotNull(GetStringFromResource(0x92e, filePathBasedOnLanguage), IEAlertDialogTitle, name);
                    object[] objArray23 = { IEAlertDialogTitle };
                                        IEDialogCancelButton = SetStringIfNotNull(GetDialogTitleFromResource(0x1fc3, 2, filePathBasedOnLanguage), IEDialogCancelButton, name);
                    object[] objArray24 = { IEDialogCancelButton };
                                        IEDialogOkButton = SetStringIfNotNull(GetDialogTitleFromResource(0x1fc3, 1, filePathBasedOnLanguage), IEDialogOkButton, name);
                    object[] objArray25 = { IEDialogOkButton };
                                        string str10 = GetFilePathBasedOnLanguage("user32.dll.mui", name);
                    if (File.Exists(str10))
                    {
                        IEDialogRetryButton = SetStringIfNotNull(GetDialogTitleFromResource(0x323, str10), IEDialogRetryButton, name);
                        object[] objArray26 = { IEDialogRetryButton };
                                            }
                    else
                    {
                        object[] objArray27 = { CultureInfo.CurrentCulture.Name, CurrentOSCulture.Name };
                                            }
                    IEGenericTitle = SetStringIfNotNull(GetDialogTitleFromResource(0xa300, filePathBasedOnLanguage), IEGenericTitle, name);
                    object[] objArray28 = { IEGenericTitle };
                                    }
                if (!string.IsNullOrEmpty(stringFromResource))
                {
                    int index = stringFromResource.IndexOf('|');
                    if (index > 0)
                    {
                        ieFileUploadDialogTitle = SetStringIfNotNull(stringFromResource.Substring(0, index), ieFileUploadDialogTitle, name);
                    }
                }
                object[] objArray29 = { ieFileUploadDialogTitle };
                                ie9NotificationBar = SetStringIfNotNull(GetStringFromResource(0xa0c0, filePathBasedOnLanguage), ie9NotificationBar, name);
                object[] objArray30 = { ie9NotificationBar };
                                microsoftPhishingFilter = SetStringIfNotNull(GetDialogTitleFromResource(0x9250, filePathBasedOnLanguage), microsoftPhishingFilter, name);
                object[] objArray31 = { microsoftPhishingFilter };
                                autoComplete = SetStringIfNotNull(GetDialogTitleFromResource(0x4223, filePathBasedOnLanguage), autoComplete, name);
                object[] objArray32 = { autoComplete };
                                autoCompletePasswords = SetStringIfNotNull(GetDialogTitleFromResource(0x4220, filePathBasedOnLanguage), autoCompletePasswords, name);
                object[] objArray33 = { autoCompletePasswords };
                                informationBar = SetStringIfNotNull(GetDialogTitleFromResource(0x9206, filePathBasedOnLanguage), informationBar, name);
                object[] objArray34 = { informationBar };
                                internetExplorerDialogTitle = SetStringIfNotNull(GetDialogTitleFromResource(0x1068, filePathBasedOnLanguage), internetExplorerDialogTitle, name);
                object[] objArray35 = { internetExplorerDialogTitle };
                                IEDialogCloseButton = SetStringIfNotNull(GetStringFromResource(0x3280, filePathBasedOnLanguage), IEDialogCloseButton, name);
                object[] objArray36 = { IEDialogCloseButton };
                                IESecurityWarningDialogTitle = SetStringIfNotNull(GetStringFromResource(0x8613, filePathBasedOnLanguage), IESecurityWarningDialogTitle, name);
                object[] objArray37 = { IESecurityWarningDialogTitle };
                            }
            else
            {
                object[] objArray38 = { CultureInfo.CurrentCulture.Name, CurrentOSCulture.Name };
                            }
            string str4 = GetFilePathBasedOnLanguage("comdlg32.dll.mui", name);
            if (!File.Exists(str4))
            {
                str4 = Path.Combine(Environment.SystemDirectory, "comdlg32.dll");
            }
            if (File.Exists(str4))
            {
                string str11 = GetStringFromResource(0x1b1, str4);
                if (!string.IsNullOrEmpty(str11))
                {
                    fileUploadComboBoxName = str11.Replace("&", string.Empty);
                    object[] objArray39 = { fileUploadComboBoxName };
                                    }
            }
            else
            {
                object[] objArray40 = { CultureInfo.CurrentCulture.Name, CurrentOSCulture.Name };
                            }
            string str5 = GetFilePathBasedOnLanguage("authui.dll.mui", name);
            if (File.Exists(str5))
            {
                win7AuthenticationDialogTitle = SetStringIfNotNull(GetStringFromResource(0x2ee8, str5), win7AuthenticationDialogTitle, name);
                object[] objArray41 = { win7AuthenticationDialogTitle };
                            }
            string str6 = GetFilePathBasedOnLanguage("credui.dll.mui", name);
            if (File.Exists(str6))
            {
                string str12 = GetStringFromResource(0x7d8, str6);
                if (!string.IsNullOrEmpty(str12) && str12.IndexOf("%s", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    authenticationDialogTitle = str12.Replace("%s", string.Empty).Trim();
                }
                object[] objArray42 = { authenticationDialogTitle };
                            }
        }

        private static IntPtr LoadResource(uint resourceId, string resourceFileName, out IntPtr hMod, out uint size)
        {
            hMod = NativeMethods.LoadLibraryEx(resourceFileName, IntPtr.Zero, 2);
            size = 0;
            if (hMod == IntPtr.Zero)
            {
                object[] args = { resourceFileName, Marshal.GetLastWin32Error() };
                                return IntPtr.Zero;
            }
            IntPtr hResInfo = NativeMethods.FindResource(hMod, resourceId, 5);
            if (hResInfo == IntPtr.Zero)
            {
                object[] objArray2 = { resourceId, resourceFileName, Marshal.GetLastWin32Error() };
                                return IntPtr.Zero;
            }
            IntPtr ptr2 = NativeMethods.LoadResource(hMod, hResInfo);
            if (ptr2 == IntPtr.Zero)
            {
                object[] objArray3 = { resourceId, resourceFileName, Marshal.GetLastWin32Error() };
                                return IntPtr.Zero;
            }
            size = NativeMethods.SizeofResource(hMod, hResInfo);
            return ptr2;
        }

        public bool MatchIEAddressBoxNameText(string actualName)
        {
            bool flag = false;
            if (!string.IsNullOrWhiteSpace(actualName))
            {
                flag = Regex.IsMatch(actualName, ieAddressNameTextIE9);
                if (!flag)
                {
                    flag = string.Equals(actualName, ieAddressNameText, StringComparison.Ordinal);
                }
            }
            return flag;
        }

        public bool MatchIEDialogTitle(string actualName)
        {
            bool flag = false;
            if (IsIE9OrLater)
            {
                if (!string.IsNullOrWhiteSpace(actualName))
                {
                    flag = Regex.IsMatch(actualName, IEPromptDialogTitle);
                }
                return flag;
            }
            return string.Equals(actualName, IEPromptDialogTitle, StringComparison.Ordinal);
        }

        private static void ProcessMenuAndClassItems(IntPtr resourceStartAddress, ref int currentOffset, uint size)
        {
            for (int i = 0; i < 2; i++)
            {
                AlignOffsetToNextWordBoundary(resourceStartAddress, ref currentOffset);
                switch (ReadWordAtOffset(resourceStartAddress, currentOffset, size))
                {
                    case 0:
                        currentOffset += GetWordLength(1);
                        break;

                    case 0xffff:
                        currentOffset += GetWordLength(2);
                        break;

                    default:
                        {
                            string str = GetNullTerminatedString(resourceStartAddress, size, currentOffset);
                            currentOffset += GetWordLength(str.Length + 1);
                            break;
                        }
                }
            }
        }

        private static ushort ReadWordAtOffset(IntPtr resourceStartAddress, int offset, uint resourceSize)
        {
            if (offset + 2 > resourceSize)
            {
                throw new ZappyTaskException();
            }
            return (ushort)Marshal.ReadInt16(resourceStartAddress, offset);
        }

        private static string SetStringIfNotNull(string source, string defaultValue, string language)
        {
            if (!string.IsNullOrEmpty(source))
            {
                object[] args = { source, defaultValue };
                                return source;
            }
            return defaultValue;
        }

        public string AuthenticationDialogTitle =>
            authenticationDialogTitle;

        public string AutoComplete =>
            autoComplete;

        public string AutoCompletePasswords =>
            autoCompletePasswords;

        internal static CultureInfo CurrentOSCulture
        {
            get
            {
                if (currentOSCulture == null)
                {
                    currentOSCulture = CultureInfo.GetCultureInfo(NativeMethods.GetUserDefaultUILanguage());
                }
                return currentOSCulture;
            }
        }

        public string FileUploadComboBoxName =>
            fileUploadComboBoxName;

        public string FireFoxFileUploadDialogTitle =>
            firefoxFileUploadDialogTitle;

        public string IE8AddressBarButtonName =>
            ie8AddressBarButtonName;

        public string IE9NotificationBar =>
            ie9NotificationBar;

        public string IEAlertDialogTitle { get; private set; }

        public string IEBackButtonText =>
            ieBackButtonText;

        public string IECloseButtonText =>
            ieCloseButtonText;

        public string IEDialogCancelButton { get; private set; }

        public string IEDialogCloseButton { get; private set; }

        public string IEDialogOkButton { get; private set; }

        public string IEDialogRetryButton { get; private set; }

        public string IEFileUploadDialogTitle =>
            ieFileUploadDialogTitle;

        public string IEForwardButtonText =>
            ieForwardButtonText;

        public string IEGenericTitle { get; private set; }

        public string IEMuteMenuItemText =>
            ieMuteMenuItemText;

        public string IEPauseMenuItemText =>
            iePauseMenuItemText;

        public string IEPlayMenuItemText =>
            iePlayMenuItemText;

        public string IEPromptDialogTitle { get; private set; }

        public string IERefreshButtonText =>
            ieRefreshButtonText;

        public string IESecurityWarningDialogTitle { get; private set; }

        public string IESpellCheckAddToDictionaryText =>
            addToDictionaryText;

        public string IESpellCheckCopyText =>
            copyText;

        public string IESpellCheckCutText =>
            cutText;

        public string IESpellCheckDeleteText =>
            deleteText;

        public string IESpellCheckIgnoreText =>
            ignoreText;

        public string IESpellCheckPasteText =>
            pasteText;

        public string IESpellCheckSelectAllText =>
            selectallText;

        public string IESpellCheckUndoText =>
            undoText;

        public string IEStopButtonText =>
            ieStopButtonText;

        public string IEUnmuteMenuItemText =>
            ieUnmuteMenuItemText;

        public string InformationBar =>
            informationBar;

        public static LocalizedSystemStrings Instance =>
            instance;

        public string InternetExplorerDialogTitle =>
            internetExplorerDialogTitle;

        internal static bool IsIE9OrLater
        {
            get
            {
                if (!isIE9OrLater.HasValue || !isIE9OrLater.HasValue)
                {
                    string str = ZappyTaskUtilities.GetRegistryValue(Registry.LocalMachine, @"Software\Microsoft\Internet Explorer", "Version") as string;
                    Version version = new Version(str);
                    isIE9OrLater = version.Major >= 9;
                }
                return isIE9OrLater.Value;
            }
        }



        public string MicrosoftPhishingFilter =>
            microsoftPhishingFilter;

        public string WindowMaximizeButtonText =>
            windowMaximizeButtonText;

        public string WindowRestoreButtonText =>
            windowRestoreButtonText;

        public string Windows7AuthenticationDialogTitle =>
            win7AuthenticationDialogTitle;

        public string WindowsQuickLaunchText =>
            windowsQuickLaunchText;

        public string WindowsRunningApplicationsText =>
            windowsRunningApplicationsText;

        public string WinFormsEditableControlNameENU =>
            winFormsEditableControlNameENU;

        public string WinFormsEditableControlNameLocalized =>
            winFormsEditableControlNameLocalized;
    }
}