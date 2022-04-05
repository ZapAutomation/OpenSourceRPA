using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Extension.HtmlControls;
using Zappy.ExecuteTask.Extension.WinControls;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using Brushes = System.Windows.Media.Brushes;

namespace Zappy.ExecuteTask.Helpers
{
    public class ALUtility
    {
        internal const int HIGHLIGHT_TIME = 0x1b58;
        private static readonly RegexOptions options = RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase;
        private static readonly Regex programW6432 = new Regex("%ProgramW6432%", options);
        private static readonly Regex programX86 = new Regex(@"%ProgramFiles\(x86\)%", options);
                private static readonly object staLockObject = new object();
        private static readonly Regex sysWow64;
        private static readonly string windowsFolderPath;
        private static readonly string windowsPathForRegex;
        internal static string WindowsSpecialFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

        static ALUtility()
        {
            object[] args = { windowsPathForRegex };
            sysWow64 = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}", args), options);
            windowsFolderPath = WindowsSpecialFolderPath;
            object[] objArray2 = { windowsFolderPath };
            windowsPathForRegex = Regex.Escape(string.Format(CultureInfo.InvariantCulture, @"{0}\SysWow64", objArray2));
        }

                                                        
        internal static void CleanupBrowserWindow()
        {
                    }

        internal static void CleanupImmersiveSwitcher()
        {
        }

        internal static void CleanupSTAHelperObject()
        {
                                                                    }

        public static ControlStates ConvertState(AccessibleStates accState) =>
            ZappyTaskUtilities.ConvertState(accState);

        internal static bool ConvertStringToDouble(string stringValue, out double doubleValue)
        {
            doubleValue = -1.0;
            if (!double.TryParse(stringValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out doubleValue) && !double.TryParse(stringValue, out doubleValue))
            {
                return false;
            }
            return true;
        }

        internal static ZappyTaskControl CreateNewZappyTaskControlAndCopyFrom(ZappyTaskControl control)
        {
            ZappyTaskControl control2 = new ZappyTaskControl();
            control2.CopyFrom(control);
            return control2;
        }

        internal static ZappyTaskControl CreateZappyTaskControlForBrowserWindow(PropertyExpressionCollection searchProperties, PropertyExpressionCollection filterProperties, string browserWindowTitle, out PropertyExpression nameExpression)
        {
            ZappyTaskControl control = new ZappyTaskControl
            {
                TechnologyName = "MSAA"
            };
            control.SearchProperties.AddRange(searchProperties);
            control.FilterProperties.AddRange(filterProperties);
            nameExpression = control.SearchProperties.Find(ZappyTaskControl.PropertyNames.Name);
            if (nameExpression != null)
            {
                string propertyValue = nameExpression.PropertyValue;
                if (nameExpression.PropertyOperator == PropertyExpressionOperator.EqualTo && propertyValue != null && !propertyValue.EndsWith(browserWindowTitle, StringComparison.Ordinal))
                {
                    propertyValue = propertyValue + browserWindowTitle;
                    nameExpression.PropertyOperator = PropertyExpressionOperator.EqualTo;
                }
                nameExpression.PropertyValue = propertyValue;
                object[] args = { propertyValue };
                
            }
            return control;
        }

        public sealed class Highlighter : IDisposable
        {
            private Popup[] allLines;
            private const int BorderWidth = 4;
            private Popup bottomLine;
            private static int dpiX = -1;
            private static int dpiY = -1;
            private Popup leftLine;
            private const int LOGPIXELSX = 0x58;
            private const int LOGPIXELSY = 90;
            private Popup rightLine;
            private Popup topLine;

            public Highlighter()
            {
                Initialize();
            }

            public void Dispose()
            {
                Hide();
                GC.SuppressFinalize(this);
            }

            [DllImport("user32.dll")]
            internal static extern IntPtr GetDC(IntPtr hwnd);
            [DllImport("gdi32.dll")]
            internal static extern int GetDeviceCaps(IntPtr hdc, int index);
            private Rectangle GetLargestIntersect(Rectangle rect)
            {
                Rectangle empty = Rectangle.Empty;
                foreach (Screen screen in Screen.AllScreens)
                {
                    Rectangle rectangle2 = new Rectangle(screen.Bounds.Location, screen.Bounds.Size);
                    rectangle2.Intersect(rect);
                    if (!rectangle2.IsEmpty)
                    {
                        if (rectangle2.Equals(rect))
                        {
                            return rectangle2;
                        }
                        if (empty == Rectangle.Empty || rectangle2.Width * rectangle2.Height > empty.Width * empty.Height)
                        {
                            empty = rectangle2;
                        }
                    }
                }
                return empty;
            }

            public void Hide()
            {
                if (!leftLine.Dispatcher.CheckAccess())
                {
                    leftLine.Dispatcher.Invoke(new Action(Hide), new object[0]);
                }
                else
                {
                    IsVisible = false;
                }
            }

            public void Highlight(int x, int y, int width, int height)
            {
                Rectangle rect = new Rectangle(x, y, width, height);
                IsVisible = false;
                if (!rect.IsEmpty)
                {
                    rect.Inflate(8, 8);
                    rect.Intersect(SystemInformation.VirtualScreen);
                    if (SystemInformation.MonitorCount > 1)
                    {
                        rect = GetLargestIntersect(rect);
                    }
                    if (!rect.IsEmpty)
                    {
                        leftLine.Width = PixelToPointDimention(4.0, true);
                        leftLine.Height = PixelToPointDimention(rect.Height - 1, false);
                        leftLine.VerticalOffset = PixelToPointDimention(rect.Y, false);
                        leftLine.HorizontalOffset = PixelToPointDimention(rect.X, true);
                        rightLine.Width = PixelToPointDimention(4.0, true);
                        rightLine.Height = PixelToPointDimention(rect.Height - 1, false);
                        rightLine.VerticalOffset = PixelToPointDimention(rect.Y, false);
                        rightLine.HorizontalOffset = PixelToPointDimention(rect.X + rect.Width - 5, true);
                        topLine.Width = PixelToPointDimention(rect.Width - 1, true);
                        topLine.Height = PixelToPointDimention(4.0, false);
                        topLine.VerticalOffset = PixelToPointDimention(rect.Y, false);
                        topLine.HorizontalOffset = PixelToPointDimention(rect.X, true);
                        bottomLine.Width = PixelToPointDimention(rect.Width - 1, true);
                        bottomLine.Height = PixelToPointDimention(4.0, false);
                        bottomLine.VerticalOffset = PixelToPointDimention(rect.Y + rect.Height - 5, false);
                        bottomLine.HorizontalOffset = PixelToPointDimention(rect.X, true);
                        IsVisible = true;
                    }
                }
            }

            private void Initialize()
            {
                leftLine = new Popup();
                leftLine.Name = "leftLine";
                bottomLine = new Popup();
                bottomLine.Name = "bottomLine";
                topLine = new Popup();
                topLine.Name = "topLine";
                rightLine = new Popup();
                rightLine.Name = "rightLine";
                allLines = new[] { leftLine, topLine, rightLine, bottomLine };
                foreach (Popup popup in allLines)
                {
                    popup.AllowsTransparency = true;
                    popup.Focusable = false;
                    popup.IsEnabled = false;
                    popup.IsHitTestVisible = false;
                    popup.Placement = PlacementMode.Absolute;
                    Grid grid = new Grid
                    {
                        Background = Brushes.Blue
                    };
                    popup.Child = grid;
                    popup.IsOpen = false;
                }
            }

            private static double PixelToPointDimention(double pixel, bool xAxis)
            {
                if (dpiX == -1)
                {
                    dpiX = dpiY = 0x60;
                    IntPtr dC = GetDC(IntPtr.Zero);
                    if (IntPtr.Zero != dC)
                    {
                        dpiX = GetDeviceCaps(dC, 0x58);
                        dpiY = GetDeviceCaps(dC, 90);
                        ReleaseDC(IntPtr.Zero, dC);
                    }
                }
                return pixel * 96.0 / (xAxis ? dpiX : dpiY);
            }

            public void ProcessUIEvents()
            {
                DispatcherUtilities.ProcessEventsOnUIThread();
            }

            [DllImport("user32.dll")]
            internal static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

            public bool IsVisible
            {
                set
                {
                    foreach (Popup popup in allLines)
                    {
                        popup.IsOpen = value;
                    }
                }
            }
        }


        internal static void DrawHighlight(int x, int y, int width, int height, int highlightTime = 0x1b58)
        {
            Highlighter highlighter = new Highlighter();
            try
            {
                highlighter.Highlight(x, y, width, height);
                highlighter.ProcessUIEvents();
                Thread.Sleep(highlightTime);
            }
            finally
            {
                highlighter.Hide();
                highlighter.ProcessUIEvents();
                highlighter.Dispose();
            }
        }

        internal static void EnsureWindowForegroundForMenuItem(ITaskActivityElement element)
        {
            if (element != null && ControlType.MenuItem.NameEquals(element.ControlTypeName) && element.QueryId != null)
            {
                ITaskActivityElement ancestor = element.QueryId.Ancestor;
                if (ancestor != null && (ControlType.MenuBar.NameEquals(ancestor.ControlTypeName) || ControlType.ToolBar.NameEquals(ancestor.ControlTypeName) || ControlType.StatusBar.NameEquals(ancestor.ControlTypeName)))
                {
                    ITaskActivityElement element3 = FrameworkUtilities.TopLevelElement(element);
                    if (element3 != null)
                    {
                        IntPtr windowHandle = element3.WindowHandle;
                        if (windowHandle != NativeMethods.GetForegroundWindow())
                        {
                            NativeMethods.SetForegroundWindow(windowHandle);
                        }
                    }
                }
            }
        }

        internal static ControlType GetControlTypeUsingSearchProperties(ZappyTaskControl control)
        {
            ControlType empty = ControlType.Empty;
            if (control != null && control.SearchProperties.Contains(ZappyTaskControl.PropertyNames.ControlType))
            {
                empty = control.SearchProperties[ZappyTaskControl.PropertyNames.ControlType];
            }
            return empty;
        }

        internal static ZappyTaskControlCollection GetDescendantsByControlType(ZappyTaskControl uiControl, string technologyName, ControlType type, int maxDepth)
        {
            ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
            ZappyTaskControl control = new ZappyTaskControl(uiControl)
            {
                TechnologyName = technologyName,
                SearchProperties = {
                    {
                        ZappyTaskControl.PropertyNames.ControlType,
                        type.Name
                    },
                    {
                        ZappyTaskControl.PropertyNames.MaxDepth,
                        maxDepth.ToString(CultureInfo.InvariantCulture)
                    }
                }
            };
            try
            {
                controls = control.FindMatchingControls();
            }
            catch (ZappyTaskException)
            {
            }
            return controls;
        }

        internal static string GetHtmlPropertyForAction(string controlType, ZappyTaskAction action)
        {
            SetValueAction action2 = action as SetValueAction;
            if (action2 == null)
            {
                throw new NotSupportedException();
            }
            if (ControlType.Edit.NameEquals(controlType))
            {
                if (action2.IsActionOnProtectedElement())
                {
                    return HtmlEdit.PropertyNames.Password;
                }
                return HtmlEdit.PropertyNames.Text;
            }
            if (ControlType.List.NameEquals(controlType))
            {
                return HtmlList.PropertyNames.SelectedItemsAsString;
            }
            if (ControlType.ComboBox.NameEquals(controlType))
            {
                return HtmlComboBox.PropertyNames.SelectedItem;
            }
            if (ControlType.FileInput.NameEquals(controlType))
            {
                return HtmlFileInput.PropertyNames.FileName;
            }
            if (ControlType.Slider.NameEquals(controlType))
            {
                return HtmlSlider.PropertyNames.ValueAsNumber;
            }
            return null;
        }

        internal static string GetModifiedInstanceFromIndex(object index, string propertyName)
        {
            int num;
            if (int.TryParse(index.ToString(), out num))
            {
                int num2 = num + 1;
                return num2.ToString(CultureInfo.InvariantCulture);
            }
            object[] args = { index, propertyName };
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidArgumentValue, args));
        }

        internal static Dictionary<string, Type> GetProperties(ZappyTaskPropertyProvider provider, ZappyTaskControl uiControl, ZappyTaskPropertyAttributes attributes, ZappyTaskPropertyAttributes ignoreAttributes)
        {
            Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
            if (provider != null && uiControl != null)
            {
                try
                {
                    ICollection<string> propertyNames = provider.GetPropertyNames(uiControl);
                    if (propertyNames == null || propertyNames.Count <= 0)
                    {
                        return dictionary;
                    }
                    foreach (string str in propertyNames)
                    {
                        ZappyTaskPropertyDescriptor propertyDescriptor = provider.GetPropertyDescriptor(uiControl, str);
                        if (propertyDescriptor != null && (propertyDescriptor.Attributes & attributes) == attributes && (ignoreAttributes == ZappyTaskPropertyAttributes.None || (propertyDescriptor.Attributes & ignoreAttributes) == ZappyTaskPropertyAttributes.None))
                        {
                            dictionary.Add(str, propertyDescriptor.DataType);
                        }
                    }
                }
                catch (NotSupportedException)
                {
                }
                catch (NotImplementedException)
                {
                }
            }
            return dictionary;
        }

        internal static Type GetSpecializedType(ZappyTaskControl uiControl)
        {
            ZappyTaskControl controlWithSearchProperties = uiControl.ControlWithSearchProperties;
            ZappyTaskPropertyProvider propertyProvider = PropertyProviderManager.Instance.GetPropertyProvider(controlWithSearchProperties);
            if (propertyProvider != null)
            {
                try
                {
                    return propertyProvider.GetSpecializedClass(controlWithSearchProperties);
                }
                catch (NotImplementedException)
                {
                }
                catch (NotSupportedException)
                {
                }
            }
            return null;
        }

        internal static TaskActivityElement GetTechElementFromZappyTaskControl(ZappyTaskControl uiTaskControl)
        {
            if (uiTaskControl != null)
            {
                return uiTaskControl.GetProperty(ZappyTaskControl.PropertyNames.UITechnologyElement) as TaskActivityElement;
            }
            return null;
        }

        internal static List<IntPtr> GetWindows(string className)
        {
            List<IntPtr> windowList = new List<IntPtr>();
            IntPtr param2 = new IntPtr();
            NativeMethods.EnumWindows(delegate (IntPtr hwnd, ref IntPtr param)
            {
                if (string.Equals(className, NativeMethods.GetClassName(hwnd), StringComparison.Ordinal))
                {
                    windowList.Add(hwnd);
                }
                return true;
            }, param2);
            return windowList;
        }

                                                                                        
        internal static void InitialiseImmersiveSwitcher()
        {
        }

        internal static void InitializeBrowserWindow()
        {
                    }

        internal static void InitializeSTAHelperObject()
        {
                    }

        internal static bool IsAlwaysSearchFlagSet(ZappyTaskControl uiTaskControl)
        {
            if (uiTaskControl.SearchProperties.Count == 0 && uiTaskControl.FilterProperties.Count == 0)
            {
                return false;
            }
            if (!ExecutionHandler.Settings.AlwaysSearchControls)
            {
                while (uiTaskControl != null)
                {
                    if (SearchConfiguration.ConfigurationExists(uiTaskControl.SearchConfigurations, SearchConfiguration.AlwaysSearch))
                    {
                        return true;
                    }
                    uiTaskControl = uiTaskControl.Container;
                }
                return false;
            }
            return true;
        }

        internal static bool IsMouseButtonPressed(MouseButtons button)
        {
            int num;
            if (button <= MouseButtons.Right)
            {
                if (button == MouseButtons.Left)
                {
                    num = 1;
                    goto Label_004A;
                }
                if (button == MouseButtons.Right)
                {
                    num = 2;
                    goto Label_004A;
                }
            }
            else
            {
                if (button == MouseButtons.Middle)
                {
                    num = 4;
                    goto Label_004A;
                }
                if (button == MouseButtons.XButton1)
                {
                    num = 5;
                    goto Label_004A;
                }
                if (button == MouseButtons.XButton2)
                {
                    num = 6;
                    goto Label_004A;
                }
            }
            return false;
        Label_004A:
            return (NativeMethods.GetAsyncKeyState(num) & 0x8000) == 0x8000;
        }

        internal static bool IsOrderOfInvokeFilterPropertyNotPresent(ZappyTaskControl control) =>
            control.FilterProperties.Find(WinWindow.PropertyNames.OrderOfInvocation) == null;

        internal static bool IsTargetSitePropertyProvider(Exception ex) =>
            ex.TargetSite != null && IsValidTargetType(ex.TargetSite.DeclaringType);

        private static bool IsValidTargetType(Type element)
        {
            if (element == null)
            {
                return false;
            }
            if (!(element == typeof(ZappyTaskControl)))
            {
                while (element != null)
                {
                    if (element.BaseType == typeof(ZappyTaskPropertyProvider))
                    {
                        return true;
                    }
                    element = element.BaseType;
                }
                return false;
            }
            return true;
        }

        internal static bool IsWindowEnabled(ZappyTaskControl uiControl)
        {
            bool flag = true;
            bool flag2 = true;
            IntPtr windowHandle = uiControl.TechnologyElement.WindowHandle;
            if (windowHandle != IntPtr.Zero)
            {
                flag = NativeMethods.IsWindowEnabled(windowHandle);
            }
            if (uiControl.CanTrustState)
            {
                flag2 = !TaskActivityElement.IsState(uiControl.TechnologyElement, AccessibleStates.Unavailable);
            }
            return flag & flag2;
        }

        internal static void PerformRetryOperation(RetryOperation operation)
        {
            PerformRetryOperation(operation, 0x4e20);
        }

        internal static void PerformRetryOperation(RetryOperation operation, int timeout)
        {
            int milliSeconds = 0x7d0;
            int num2 = timeout / milliSeconds;
            while (num2 >= 0)
            {
                if (SearchHelper.Instance.PlaybackCanceled)
                {
                    throw new ExecuteCanceledException();
                }
                if (operation())
                {
                    break;
                }
                num2--;
                ZappyTaskUtilities.Sleep(milliSeconds);
            }
        }

        internal static bool RetryUsingDefaultExecutor(Exception ex)
        {
            bool flag = ex is NotSupportedException || ex is NotImplementedException;
            object[] args = { flag };
            
            return flag;
        }

        internal static bool ShouldErrorEventBeRaisedForException(Exception ex)
        {
            if (ex is ZappyTaskException)
            {
                return true;
                            }
            return ExecutionHandler.ExceptionFromPropertyProvider(ex);
        }

        internal static void ThrowDataGridRelatedException(string errorString, string propertyName)
        {
            object[] args = { WinCell.PropertyNames.ColumnIndex, WinCell.PropertyNames.RowIndex };
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, errorString, args), propertyName);
        }

        internal static object ThrowNotSupportedException(bool isNotSupported)
        {
            throw new RethrowException(new NotSupportedException(), isNotSupported);
        }

        internal static void UpdateSqmData(ZappyTaskControl control)
        {
                                                            
                                                                                                                                                
                                    
                                                
                                                                                            }

                                                                                                                                                        
        internal delegate bool RetryOperation();
    }
}