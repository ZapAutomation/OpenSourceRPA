using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using Crapy.ActionMap.UITest;

namespace Crapy.ActionMap.HelperClasses
{
    internal class ExtensionResources
    {
        private static CultureInfo resourceCulture = Thread.CurrentThread.CurrentCulture;
        private static System.Resources.ResourceManager resourceMan;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExtensionResources()
        {
        }

        internal static string Audio =>
            "Audio";

        internal static string Button =>
            "Button";

        internal static string Calendar =>
            "Calendar";

        internal static string Cell =>
            "Cell";

        internal static string CheckBox =>
            "CheckBox";

        internal static string CheckBoxTreeItem =>
            "CheckBoxTreeItem";

        internal static string Client =>
            "Client";

        internal static string ColumnHeader =>
            "ColumnHeader";

        internal static string ComboBox =>
            "ComboBox";

        internal static string ContextMenu =>
            "ContextMenu";

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get =>
                resourceCulture;
            set
            {
                resourceCulture = value;
            }
        }

        internal static string Custom =>
            "Custom";

        internal static string DatePicker =>
            "DatePicker";

        internal static string DateTimePicker =>
            "DateTimePicker";

        internal static string Document =>
            "Document";

        internal static string Edit =>
            "Edit";

        internal static string Ellipsis =>
            "Ellipsis";

        internal static string ErrorElementNotAvailable =>
            "ErrorElementNotAvailable";

        internal static string Expander =>
            "Expander";

        internal static string FileInput =>
            "FileInput";

        internal static string FlipView =>
            "FlipView";

        internal static string FlipViewItem =>
            "FlipViewItem";

        internal static string Frame =>
            "Frame";

        internal static string Group =>
            "Group";

        internal static string Hub =>
            "Hub";

        internal static string HubSection =>
            "HubSection";

        internal static string Hyperlink =>
            "Hyperlink";

        internal static string Image =>
            "Image";

        internal static string In =>
            "In";

        internal static string InvalidParameterValue =>
            "InvalidParameterValue";

        internal static string InvalidQueryString =>
            "InvalidQueryString";

        internal static string Label =>
            "Label";

        internal static string List =>
            "List";

        internal static string ListItem =>
            "ListItem";

        internal static string Media =>
            "Media";

        internal static string MenuBar =>
            "MenuBar";

        internal static string MenuItem =>
            "MenuItem";

        internal static string MicrosoftCorporation =>
            "MicrosoftCorporation";

        internal static string OddNumberOfArguments =>
            "OddNumberOfArguments";

        internal static string Pane =>
            "Pane";

        internal static string Pivot =>
            "Pivot";

        internal static string PivotItem =>
            "PivotItem";

        internal static string PopupMenu =>
            "PopupMenu";

        internal static string ProgressBar =>
            "ProgressBar";

        internal static string PropertyExpressionNotFound =>
            "PropertyExpressionNotFound";

        internal static string RadioButton =>
            "RadioButton";

        internal static string Rating =>
            "Rating";

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("ExtensionResources", UITestUtilities.GetTypeAssembly(typeof(ExtensionResources)));
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static string Row =>
            "Row";

        internal static string RowHeader =>
            "RowHeader";

        internal static string ScrollBar =>
            "ScrollBar";

        internal static string SemanticZoom =>
            "SemanticZoom";

        internal static string Separator =>
            "Separator";

        internal static string SetStateException =>
            "SetStateException";

        internal static string SFIInitError =>
            "SFIInitError";

        internal static string SFIRequestError =>
            "SFIRequestError";

        internal static string SingleWarningString =>
            "SingleWarningString";

        internal static string Slider =>
            "Slider";

        internal static string Spinner =>
            "Spinner";

        internal static string SplitButton =>
            "SplitButton";

        internal static string StateNotSupported =>
            "StateNotSupported";

        internal static string StatusBar =>
            "StatusBar";

        internal static string Tab =>
            "Tab";

        internal static string TabItem =>
            "TabItem";

        internal static string Table =>
            "Table";

        internal static string TabList =>
            "TabList";

        internal static string TabPage =>
            "TabPage";

        internal static string TitleBar =>
            "TitleBar";

        internal static string ToggleButton =>
            "ToggleButton";

        internal static string ToggleSwitch =>
            "ToggleSwitch";

        internal static string ToolBar =>
            "ToolBar";

        internal static string ToolTip =>
            "ToolTip";

        internal static string TotalTimeString =>
            "TotalTimeString";

        internal static string Tree =>
            "Tree";

        internal static string TreeItem =>
            "TreeItem";

        internal static string TruncatedStringWithEllipsis =>
            "TruncatedStringWithEllipsis";

        internal static string Video =>
            "Video";

        internal static string WarningsString =>
            "WarningsString";

        internal static string Window =>
            "Window";
    }
}
