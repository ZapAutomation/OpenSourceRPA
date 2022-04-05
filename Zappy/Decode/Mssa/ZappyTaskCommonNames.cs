using System.Text.RegularExpressions;

namespace Zappy.Decode.Mssa
{
    internal static class ZappyTaskCommonNames
    {
        internal const string CharmClassName = "Shell_CharmWindow";
        internal const string CharmsBarClassName = "NativeHWNDHost";
        internal const string CharmsWindowName = "Charm Bar";
        internal const string ContactCardClassName = "ContactCard";
        internal const string DateFormat = "dd-MMM-yyyy";
        internal static Regex DateRangeValueRegex = new Regex("\"(?<mindate>(\\d|\\W|\\w|\\s|\\S^\")+)\"-\"(?<maxdate>(\\d|\\W|\\w|\\s|\\S^\")+)\"", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        internal const string DateTimeFormat = "dd-MMM-yyyy HH:mm:ss";
        internal const string DesktopClassName = "#32769";
        internal const string DirectUIClassName = "DirectUIHWND";
        internal const string FilePickerClassName = "Item Picker Window";
        internal const string FirefoxDialogClassName = "MozillaDialogClass";
        internal const string FirefoxWindowClassName = "MozillaUIWindowClass";
        internal const string IEClientClassName = "Shell DocObject View";
        internal const string IEDialogClassName = "Internet Explorer_TridentDlgFrame";
        internal const string IEProcessName = "iexplore";
        internal const string IEWindowClassName = "IEFrame";
        internal const string ImmersiveAppTitleFrameWindowClassName = "ApplicationFrameWindow";
        internal static readonly string ImmersiveAppWindowClassName = "Windows.UI.Core.CoreWindow";
        internal const string ImmersiveSwitchListClassName = "ImmersiveSwitchList";
        internal const string MfcClassName = "Afx:";
        internal const string SearchPane = "SearchPane";
        internal const string SearchResultsView = "SearchResultsView";
        internal const string SilverlightClassName = "ATL:";
        internal static readonly string StartMenuClassName = "ImmersiveLauncher";
        internal const string UiaWidgetTechnologyName = "UiaWidget";
        internal const string WebViewHostClassName = "XAMLWebViewHostWindowClass";
        internal const string WinformsClassName = "WindowsForms";
        internal const string WpfClassName = "HwndWrapper";
        internal static readonly Regex WpfClassNameRegex = new Regex(@"^HwndWrapper\[.*;.*;.*\]", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal const string WWAFileSearchClassName = "FileSearchAppWindowClass";
        internal const string WWAProcessName = "wwahost";
    }
}