using Accessibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;
using AndCondition = System.Windows.Automation.AndCondition;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;
using DataGridUtility = Zappy.Plugins.Uia.Uia.Utilities.DataGridUtility;
using PropertyCondition = Zappy.ActionMap.HelperClasses.PropertyCondition;
using ScrollAmount = System.Windows.Automation.ScrollAmount;

namespace Zappy.Plugins.Uia.Uia
{
    internal static class UiaUtility
    {
        private const string CharmsBar = "Charm Bar";
        private const string DatePickerEditBoxAutomationId = "PART_TextBox";
        internal const string DirectUIFrameworkId = "DirectUI";
        private const string FilePickerClassName = "Item Picker Window";
        internal const string HorizontalScrollBarName = "HorizontalScrollBar";
        internal const string IEProcessName = "iexplore";
        private const string ImmersiveSwitchListClassName = "ImmersiveSwitchList";
        internal const string InternetExplorerFrameworkId = "InternetExplorer";
        internal const string JupiterFrameworkId = "XAML";
        private const string NativeHwndHost = "NativeHWNDHost";
        private static UiaElement paneElement;
        private const int realizeSleepTime = 20;
        private const int realizeTimeOut = 200;
        private static IntPtr s_desktopWindowHandle = IntPtr.Zero;
        private const int scrollAttempts = 20;
        private const double ScrollBarWidth = 17.0;
        private const string ScrollPaneClassName = "ScrollViewer";
        private const int scrollSleepTime = 10;
        private const int timeOut = 20;
        private static Stopwatch timeOutWatch;
        internal const string VerticalScrollBarName = "VerticalScrollBar";
        internal const int WindowsFormsHostSupportLevel = 0x65;
        internal const string WpfClassNameContainsString = "HwndWrapper";
        private static readonly Regex WpfClassNameRegex = new Regex(@"^HwndWrapper\[.*;.*;.*\]", ZappyTaskUtilities.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        internal const string WpfFrameworkId = "WPF";
        internal const string WWAProcessName = "wwahost";

        public static string ArrayToString(int[] arr)
        {
            StringBuilder builder = new StringBuilder();
            foreach (int num2 in arr)
            {
                builder.Append(num2);
                builder.Append(',');
            }
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();
        }

        internal static void BringControlToView(AutomationElement element, bool ensureFullyVisible = false)
        {
            if (element != null)
            {
                AutomationElement element2;
                ScrollPattern firstScrollableParent = GetFirstScrollableParent(element, out element2);
                if (firstScrollableParent != null && IsElementOffScreen(element))
                {
                    ScrollHorizontalHome(firstScrollableParent);
                    for (int i = 0; i < 20 && IsElementOffScreen(element); i++)
                    {
                        if (firstScrollableParent.Current.VerticallyScrollable)
                        {
                            ScrollVerticalHome(firstScrollableParent);
                            for (int j = 0; j < 20 && 100 != (int)firstScrollableParent.Current.VerticalScrollPercent && element.Current.IsOffscreen; j++)
                            {
                                try
                                {
                                    firstScrollableParent.ScrollVertical(ScrollAmount.LargeIncrement);
                                    ZappyTaskUtilities.Sleep(10);
                                }
                                catch (Exception exception)
                                {
                                    object[] args = { exception.Message };
                                    CrapyLogger.log.ErrorFormat("Uia.EnsureVisible: Error when attempting vertical scroll {0}", args);
                                    break;
                                }
                            }
                        }
                        if (!IsElementOffScreen(element) || !firstScrollableParent.Current.HorizontallyScrollable || 100 == (int)firstScrollableParent.Current.HorizontalScrollPercent)
                        {
                            break;
                        }
                        try
                        {
                            firstScrollableParent.ScrollHorizontal(ScrollAmount.LargeIncrement);
                            ZappyTaskUtilities.Sleep(10);
                        }
                        catch (Exception exception2)
                        {
                            object[] objArray2 = { exception2.Message };
                            CrapyLogger.log.ErrorFormat("Uia.EnsureVisible: Error when attempting horizontal scroll {0}", objArray2);
                            break;
                        }
                    }
                }
                if (ensureFullyVisible && !element.Current.IsOffscreen)
                {
                    DoMinorVerticalAdjustments(element, firstScrollableParent, element2);
                    DoMinorHorizontalAdjustments(element, firstScrollableParent, element2);
                }
            }
        }

        public static AutomationElement CheckForHtmlControls(AutomationElement element) =>
            element;

        public static ITaskActivityElement CheckForLegacyHostElements(UiaTechnologyManager manager, ITaskActivityElement elementToConvert, out int supportLevel)
        {
            ITaskActivityElement uiaElementForWindowsFormHost = null;
            supportLevel = 0;
            if (elementToConvert != null && ControlType.Pane.NameEquals(elementToConvert.ControlTypeName))
            {
                uiaElementForWindowsFormHost = GetUiaElementForWindowsFormHost(manager, elementToConvert);
                if (uiaElementForWindowsFormHost != null)
                {
                    supportLevel = 0x65;
                    return uiaElementForWindowsFormHost;
                }
                return uiaElementForWindowsFormHost;
            }
            TaskActivityElement element2 = elementToConvert as TaskActivityElement;
            IAccessible pAcc = null;
            AutomationElement automationElement = null;
            if (element2 != null)
            {
                automationElement = element2.AutomationElement;
            }
            else
            {
                if (string.Equals(elementToConvert.Framework, "MSAA", StringComparison.OrdinalIgnoreCase))
                {
                    pAcc = (elementToConvert.NativeElement as object[])[0] as IAccessible;
                }
                else
                {
                    IServiceProvider serviceProvider = elementToConvert as IServiceProvider;
                    pAcc = ZappyTaskUtilities.GetAccessibleFromServiceProvider(serviceProvider);
                }
                if (pAcc != null)
                {
                    automationElement = AutomationElement.FromHandle(NativeMethods.WindowFromAccessibleObject(pAcc));
                }
            }
            if (automationElement != null && IsWindowOfSupportedTechnology(automationElement, false))
            {
                supportLevel = 100;
                try
                {
                    return UiaElementFactory.GetUiaElement(automationElement, true);
                }
                catch (SystemException exception)
                {
                    MapAndThrowException(exception, null, false);
                    throw;
                }
            }
            return uiaElementForWindowsFormHost;
        }

        internal static bool CheckForMenuItem(UiaElement parent, UiaElement element) =>
            element != null && parent != null && ControlType.MenuItem.NameEquals(element.ControlTypeName) && !ControlType.MenuItem.NameEquals(parent.ControlTypeName) && !ControlType.Menu.NameEquals(parent.ControlTypeName);

        public static bool CheckIfCharmsBar(string className, IntPtr windowHandle) =>
            false;

        internal static void CopyCeilingElement(ITaskActivityElement currentElement, ITaskActivityElement navigatedElement)
        {
            if (navigatedElement != null && currentElement is UiaElement && (currentElement as UiaElement).CeilingElement != null)
            {
                (navigatedElement as UiaElement).CeilingElement = (currentElement as UiaElement).CeilingElement;
            }
        }

        private static void DoMinorHorizontalAdjustments(AutomationElement element, ScrollPattern pattern, AutomationElement containerElement)
        {
            if (pattern != null && element != null && containerElement != null && pattern.Current.HorizontallyScrollable)
            {
                try
                {
                    if (element.Current.BoundingRectangle.Right != containerElement.Current.BoundingRectangle.Right)
                    {
                        for (int i = 0; i < 20 && element.Current.BoundingRectangle.Left == containerElement.Current.BoundingRectangle.Left; i++)
                        {
                            pattern.ScrollHorizontal(ScrollAmount.SmallDecrement);
                            ZappyTaskUtilities.Sleep(10);
                        }
                    }
                    if (element.Current.BoundingRectangle.Left != containerElement.Current.BoundingRectangle.Left)
                    {
                        for (int j = 0; j < 20 && element.Current.BoundingRectangle.Right == containerElement.Current.BoundingRectangle.Right; j++)
                        {
                            pattern.ScrollHorizontal(ScrollAmount.SmallIncrement);
                            ZappyTaskUtilities.Sleep(10);
                        }
                    }
                }
                catch (Exception exception)
                {
                    object[] args = { exception.Message };
                    CrapyLogger.log.ErrorFormat("Uia.EnsureVisible: Error when attempting horizontal scroll {0}", args);
                }
            }
        }

        private static void DoMinorVerticalAdjustments(AutomationElement element, ScrollPattern pattern, AutomationElement containerElement)
        {
            if (pattern != null && element != null && containerElement != null && pattern.Current.VerticallyScrollable)
            {
                try
                {
                    if (element.Current.BoundingRectangle.Top != containerElement.Current.BoundingRectangle.Top)
                    {
                        for (int i = 0; i < 20 && element.Current.BoundingRectangle.Bottom == containerElement.Current.BoundingRectangle.Bottom; i++)
                        {
                            pattern.ScrollVertical(ScrollAmount.SmallIncrement);
                            ZappyTaskUtilities.Sleep(10);
                        }
                    }
                    if (element.Current.BoundingRectangle.Bottom != containerElement.Current.BoundingRectangle.Bottom)
                    {
                        for (int j = 0; j < 20 && element.Current.BoundingRectangle.Top == containerElement.Current.BoundingRectangle.Top; j++)
                        {
                            pattern.ScrollVertical(ScrollAmount.SmallDecrement);
                            ZappyTaskUtilities.Sleep(10);
                        }
                    }
                }
                catch (Exception exception)
                {
                    object[] args = { exception.Message };
                    CrapyLogger.log.ErrorFormat("Uia.EnsureVisible: Error when attempting vertical scroll {0}", args);
                }
            }
        }

        private static bool EnumChildWindows(IntPtr hWnd, ref IntPtr lParam)
        {
            if (timeOutWatch.ElapsedMilliseconds > 20L)
            {
                
                return false;
            }
            try
            {
                AutomationElement element = AutomationElement.FromHandle(hWnd);
                if (element != null && element.Current.ControlType == System.Windows.Automation.ControlType.Window && IsWpfFrameWorkId(element) && (paneElement = GetPaneUnderPopWindowOfComboBox(element)) != null)
                {
                    return false;
                }
            }
            catch (ElementNotAvailableException)
            {
                return true;
            }
            catch (InvalidOperationException)
            {
                return true;
            }
            return true;
        }

        private static AutomationElement FindItemUnderContainer(AutomationElement automationElementContainer, string itemText)
        {
            ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(automationElementContainer) as ItemContainerPattern;
            if (itemContainerPattern != null)
            {
                return itemContainerPattern.FindItemByProperty(null, AutomationElement.NameProperty, itemText);
            }
            return null;
        }

        internal static string GetAutomationId(AutomationElement element)
        {
            string automationId = null;
            try
            {
                automationId = element.Current.AutomationId;
            }
            catch (ElementNotAvailableException)
            {
                RealizeElement(element);
                automationId = element.Current.AutomationId;
            }
            if (string.IsNullOrEmpty(automationId) && System.Windows.Automation.ControlType.TitleBar == element.Current.ControlType)
            {
                automationId = ControlType.TitleBar.Name;
            }
            return automationId;
        }

        internal static T GetAutomationPropertyValue<T>(AutomationElement element, AutomationProperty property)
        {
            T local;
            try
            {
                if (element != null)
                {
                    object currentPropertyValue = element.GetCurrentPropertyValue(property);
                    if (currentPropertyValue != null && currentPropertyValue != AutomationElement.NotSupported)
                    {
                        return (T)currentPropertyValue;
                    }
                }
                local = default(T);
            }
            catch (Exception exception)
            {
                MapAndThrowException(exception, null, false);
                throw;
            }
            return local;
        }

        public static string GetControlTypeName(AutomationElement element, System.Windows.Automation.ControlType uiaControlType, string uiaClassName)
        {
            string programmaticName = uiaControlType.ProgrammaticName;
            if (!string.IsNullOrEmpty(programmaticName) && programmaticName.StartsWith("ControlType.", StringComparison.Ordinal))
            {
                programmaticName = programmaticName.Substring("ControlType.".Length);
            }
            if (uiaControlType == System.Windows.Automation.ControlType.Document && string.Equals(uiaClassName, "RichTextBox", StringComparison.Ordinal))
            {
                return ControlType.Edit.Name;
            }
            if (uiaControlType == System.Windows.Automation.ControlType.Thumb)
            {
                return ControlType.Indicator.Name;
            }
            if (uiaControlType == System.Windows.Automation.ControlType.Group)
            {
                if (string.Equals(uiaClassName, "Expander", StringComparison.Ordinal))
                {
                    return ControlType.Expander.Name;
                }
                if (string.Equals(uiaClassName, "Hub", StringComparison.Ordinal))
                {
                    programmaticName = ControlType.Hub.Name;
                }
                return programmaticName;
            }
            if (uiaControlType == System.Windows.Automation.ControlType.Tab)
            {
                return ControlType.TabList.Name;
            }
            if (uiaControlType == System.Windows.Automation.ControlType.TabItem)
            {
                return ControlType.TabPage.Name;
            }
            if (uiaControlType == System.Windows.Automation.ControlType.List)
            {
                if (string.Equals(uiaClassName, "FlipView", StringComparison.Ordinal))
                {
                    return ControlType.FlipView.Name;
                }
                if (string.Equals(uiaClassName, "Pivot", StringComparison.Ordinal))
                {
                    programmaticName = ControlType.Pivot.Name;
                }
                return programmaticName;
            }
            if (uiaControlType == System.Windows.Automation.ControlType.ListItem)
            {
                string automationPropertyValue = GetAutomationPropertyValue<string>(element, AutomationElement.ClassNameProperty);
                if (string.Equals(automationPropertyValue, "FlipViewItem", StringComparison.Ordinal))
                {
                    return ControlType.FlipViewItem.Name;
                }
                if (string.Equals(automationPropertyValue, "PivotItem", StringComparison.Ordinal))
                {
                    return ControlType.PivotItem.Name;
                }
                if (string.Equals(automationPropertyValue, "HubSection", StringComparison.Ordinal))
                {
                    programmaticName = ControlType.HubSection.Name;
                }
                return programmaticName;
            }
            if (uiaControlType == System.Windows.Automation.ControlType.Button && string.Equals(uiaClassName, ControlType.ToggleSwitch.Name, StringComparison.Ordinal))
            {
                return ControlType.ToggleSwitch.Name;
            }
            if (uiaControlType == System.Windows.Automation.ControlType.Button && PatternHelper.GetTogglePattern(element) != null)
            {
                if (PatternHelper.GetExpandCollapsePattern(TreeWalkerHelper.GetParent(element), false) == null)
                {
                    programmaticName = ControlType.ToggleButton.Name;
                }
                return programmaticName;
            }
            if (DataGridUtility.IsDataGridRow(element, uiaClassName, uiaControlType))
            {
                return ControlType.Row.Name;
            }
            if (DataGridUtility.IsDataGridCell(element, uiaClassName, uiaControlType))
            {
                return ControlType.Cell.Name;
            }
            if (DataGridUtility.IsDataGridTable(element))
            {
                return ControlType.Table.Name;
            }
            if (string.Equals("DataGridRowHeader", uiaClassName, StringComparison.Ordinal))
            {
                return ControlType.RowHeader.Name;
            }
            if (string.Equals("DataGridColumnHeader", uiaClassName, StringComparison.Ordinal))
            {
                return ControlType.ColumnHeader.Name;
            }
            if (uiaControlType == System.Windows.Automation.ControlType.Custom && string.Equals(uiaClassName, "DatePicker", StringComparison.Ordinal))
            {
                programmaticName = ControlType.DatePicker.Name;
            }
            return programmaticName;
        }

        internal static AutomationElement GetDatePickerEditBox(AutomationElement datepicker)
        {
            AutomationElement element = null;
            if (datepicker != null)
            {
                System.Windows.Automation.PropertyCondition condition = new System.Windows.Automation.PropertyCondition(AutomationElement.AutomationIdProperty, "PART_TextBox");
                element = datepicker.FindFirst(TreeScope.Children, condition);
            }
            return element;
        }

        internal static string GetDateStringFromSelectionList(List<string> selectedItems)
        {
            CommaListBuilder builder = new CommaListBuilder();
            if (selectedItems != null)
            {
                foreach (string str in selectedItems)
                {
                    string str2;
                    if (ZappyTaskUtilities.TryGetDateString(str, out str2))
                    {
                        builder.AddValue(str2);
                    }
                }
            }
            return builder.ToString();
        }

        internal static ScrollPattern GetFirstScrollableParent(AutomationElement element, out AutomationElement parent)
        {
            parent = TreeWalker.RawViewWalker.GetParent(element);
            ScrollPattern scrollPattern = null;
            AutomationElement element2 = null;
            while (scrollPattern == null && parent != null)
            {
                element2 = parent;
                scrollPattern = PatternHelper.GetScrollPattern(parent);
                parent = TreeWalker.RawViewWalker.GetParent(parent);
            }
            parent = element2;
            return scrollPattern;
        }

        internal static ITaskActivityElement GetHtmlElementFromPoint(AutomationElement automationElement, int pointX, int pointY) =>
            null;

        internal static int GetNativeWindowHandle(AutomationElement element)
        {
            try
            {
                return element.Current.NativeWindowHandle;
            }
            catch (ElementNotAvailableException)
            {
                RealizeElement(element);
                return element.Current.NativeWindowHandle;
            }
        }

        public static UiaElement GetPaneFromExpandedComboBox(ITaskActivityElement topLevelElement)
        {
            if (topLevelElement != null)
            {
                AutomationElement nativeElement = topLevelElement.NativeElement as AutomationElement;
                System.Windows.Automation.PropertyCondition condition = new System.Windows.Automation.PropertyCondition(AutomationElement.ClassNameProperty, "Popup");
                System.Windows.Automation.PropertyCondition condition2 = new System.Windows.Automation.PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Window);
                Condition[] conditions = { condition, condition2 };
                AndCondition condition3 = new AndCondition(conditions);
                try
                {
                                        {
                        AutomationElementCollection elements = nativeElement.FindAll(TreeScope.Children, condition3);
                        if (elements != null)
                        {
                            foreach (AutomationElement element2 in elements)
                            {
                                UiaElement paneUnderPopWindowOfComboBox = GetPaneUnderPopWindowOfComboBox(element2);
                                if (paneUnderPopWindowOfComboBox != null)
                                {
                                    return paneUnderPopWindowOfComboBox;
                                }
                            }
                        }
                    }
                }
                catch (ElementNotAvailableException)
                {
                }
            }
            
            paneElement = null;
                        {
                IntPtr zero = IntPtr.Zero;
                NativeMethods.EnumWindowsProc lpEnumFunc = EnumChildWindows;
                timeOutWatch = Stopwatch.StartNew();
                timeOutWatch.Reset();
                NativeMethods.EnumChildWindows(zero, lpEnumFunc, ref zero);
                timeOutWatch.Stop();
            }
            if (paneElement == null)
            {
                
            }
            return paneElement;
        }

        private static UiaElement GetPaneUnderPopWindowOfComboBox(AutomationElement window)
        {
            AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(window, TreeWalker.RawViewWalker);
            VirtualizationContext disable = VirtualizationContext.Disable;
            int num = 0;
            while (firstChild != null && num++ < 5)
            {
                if (firstChild.Current.ControlType == System.Windows.Automation.ControlType.Pane && string.Equals("ScrollViewer", firstChild.Current.ClassName, StringComparison.OrdinalIgnoreCase))
                {
                    return UiaElementFactory.GetUiaElement(firstChild, false);
                }
                firstChild = TreeWalkerHelper.GetNextSibling(firstChild, TreeWalker.RawViewWalker, ref disable);
            }
            return null;
        }

        public static AutomationElement GetParentOfIgnoredElement(AutomationElement element)
        {
            if (element != null)
            {
                System.Windows.Automation.ControlType automationPropertyValue = GetAutomationPropertyValue<System.Windows.Automation.ControlType>(element, AutomationElement.ControlTypeProperty);
                if (automationPropertyValue != System.Windows.Automation.ControlType.Text && automationPropertyValue != System.Windows.Automation.ControlType.Image)
                {
                    return element;
                }
                AutomationElement parent = TreeWalkerHelper.GetParent(element);
                if (parent == null || !GetAutomationPropertyValue<bool>(element, AutomationElement.IsContentElementProperty))
                {
                    return element;
                }
                System.Windows.Automation.ControlType type2 = GetAutomationPropertyValue<System.Windows.Automation.ControlType>(parent, AutomationElement.ControlTypeProperty);
                if (type2 != System.Windows.Automation.ControlType.Button && type2 != System.Windows.Automation.ControlType.Calendar && type2 != System.Windows.Automation.ControlType.CheckBox && type2 != System.Windows.Automation.ControlType.ComboBox && type2 != System.Windows.Automation.ControlType.Hyperlink && type2 != System.Windows.Automation.ControlType.List && (type2 != System.Windows.Automation.ControlType.ListItem || automationPropertyValue == System.Windows.Automation.ControlType.Image) && (type2 != System.Windows.Automation.ControlType.Menu && type2 != System.Windows.Automation.ControlType.MenuItem && type2 != System.Windows.Automation.ControlType.RadioButton) && type2 != System.Windows.Automation.ControlType.TabItem && type2 != System.Windows.Automation.ControlType.TreeItem)
                {
                    return element;
                }
                element = parent;
            }
            return element;
        }

        internal static UiaElement GetPreviousSiblingFlattened(UiaElement element, out UiaElement parentPrevious, out bool parentSwitched)
        {
            AutomationElement previousSibling = TreeWalkerHelper.GetPreviousSibling(element.InnerElement);
            parentPrevious = element.Parent;
            parentSwitched = false;
            if (previousSibling == null)
            {
                UiaElement parent = element.Parent;
                if (parent != null && IsElementOfItemType(parent.InnerElement))
                {
                    parentPrevious = null;
                    return null;
                }
                if (parent != null && !QueryIdHelper.ElementHasGoodQueryId(parent))
                {
                    previousSibling = TreeWalkerHelper.GetLastChild(TreeWalkerHelper.GetPreviousSibling(parent.InnerElement));
                    parentPrevious = parent.Parent;
                    if (parentPrevious == null || parent.IsCeilingElement || parentPrevious.IsBoundaryForHostedControl)
                    {
                        parentPrevious = null;
                        return null;
                    }
                    parentSwitched = true;
                }
            }
            return UiaElementFactory.GetUiaElement(previousSibling, false);
        }

        public static ScrollAmount GetScrollAmount(ActionMap.Enums.ScrollAmount amount)
        {
            switch (amount)
            {
                case ActionMap.Enums.ScrollAmount.ForwardByLargeAmount:
                    return ScrollAmount.LargeIncrement;

                case ActionMap.Enums.ScrollAmount.BackByLargeAmount:
                    return ScrollAmount.LargeDecrement;

                case ActionMap.Enums.ScrollAmount.ForwardBySmallAmount:
                    return ScrollAmount.SmallIncrement;

                case ActionMap.Enums.ScrollAmount.BackBySmallAmount:
                    return ScrollAmount.SmallDecrement;
            }
            return ScrollAmount.NoAmount;
        }

        private static bool GetScrollBarsUsingPattern(UiaElement element, ref double verticalScrollBarWidth, ref double horizontalScrollBarHeight)
        {
            ScrollPattern scrollPattern = PatternHelper.GetScrollPattern(element.InnerElement);
            if (scrollPattern == null)
            {
                return false;
            }
            if (scrollPattern.Current.HorizontallyScrollable)
            {
                horizontalScrollBarHeight = 17.0;
            }
            if (scrollPattern.Current.VerticallyScrollable)
            {
                verticalScrollBarWidth = 17.0;
            }
            return true;
        }

        private static bool GetScrollBarsUsingSearch(UiaElement element, ref double verticalScrollBarWidth, ref double horizontalScrollBarHeight)
        {
            if (PatternHelper.GetItemContainerPattern(element.InnerElement) == null && element.ControlTypeName != ControlType.Table)
            {
                try
                {
                    AutomationElement[] elementArray = new AutomationElement[2];
                    elementArray[0] = element.InnerElement.FindFirst(TreeScope.Children, new System.Windows.Automation.PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.ScrollBar));
                    if (elementArray[0] != null)
                    {
                        elementArray[1] = TreeWalkerHelper.GetNextSibling(elementArray[0]);
                    }
                    if (elementArray[1] != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (string.Equals(elementArray[i].Current.AutomationId, "HorizontalScrollBar", StringComparison.Ordinal))
                            {
                                horizontalScrollBarHeight = elementArray[i].Current.BoundingRectangle.Height;
                            }
                            else if (string.Equals(elementArray[i].Current.AutomationId, "VerticalScrollBar", StringComparison.Ordinal))
                            {
                                verticalScrollBarWidth = elementArray[i].Current.BoundingRectangle.Width;
                            }
                        }
                        return true;
                    }
                }
                catch (ElementNotAvailableException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
            return false;
        }

        internal static List<string> GetSelectedItems(AutomationElement element)
        {
            List<AutomationElement> selectedAutomationElements = null;
            return GetSelectedItems(element, ref selectedAutomationElements);
        }

        internal static List<string> GetSelectedItems(AutomationElement element, ref List<AutomationElement> selectedAutomationElements)
        {
            List<string> selectedItemsUsingPattern = null;
            BasePattern itemContainerPattern = PatternHelper.GetItemContainerPattern(element);
            if (itemContainerPattern != null && !string.Equals(element.Current.ClassName, "Pivot", StringComparison.OrdinalIgnoreCase))
            {
                selectedItemsUsingPattern = GetSelectedItemsUsingSearch(itemContainerPattern, element, ref selectedAutomationElements);
            }
            if (selectedItemsUsingPattern == null)
            {
                SelectionPattern selectionPattern = PatternHelper.GetSelectionPattern(element);
                if (selectionPattern != null)
                {
                    selectedItemsUsingPattern = GetSelectedItemsUsingPattern(selectionPattern, ref selectedAutomationElements);
                }
            }
            return selectedItemsUsingPattern;
        }

        internal static List<string> GetSelectedItemsUsingPattern(SelectionPattern selPattern, ref List<AutomationElement> selectedAutomationElements)
        {
            AutomationElement[] selection = PatternHelper.GetSelection(selPattern);
            if (selection == null || selection.Length == 0)
            {
                return null;
            }
            List<string> list = new List<string>();
            for (int i = 0; i < selection.Length; i++)
            {
                if (!IsValidListItemForEvent(selection[i]))
                {
                    return null;
                }
                list.Add(selection[i].Current.Name);
                if (selectedAutomationElements != null)
                {
                    selectedAutomationElements.Add(selection[i]);
                }
            }
            return list;
        }

        internal static List<string> GetSelectedItemsUsingSearch(BasePattern pattern, AutomationElement element, ref List<AutomationElement> selectedAutomationElements)
        {
            if (IsVirtualElement(TreeWalker.ControlViewWalker.GetFirstChild(element)))
            {
                ItemContainerPattern pattern2 = pattern as ItemContainerPattern;
                if (pattern2 != null)
                {
                    List<string> list = new List<string>();
                    for (AutomationElement element3 = pattern2.FindItemByProperty(null, SelectionItemPattern.IsSelectedProperty, true); element3 != null; element3 = pattern2.FindItemByProperty(element3, SelectionItemPattern.IsSelectedProperty, true))
                    {
                        string automationPropertyValue = GetAutomationPropertyValue<string>(element3, AutomationElement.NameProperty);
                        list.Add(automationPropertyValue);
                        if (selectedAutomationElements != null)
                        {
                            selectedAutomationElements.Add(element3);
                        }
                    }
                    return list;
                }
            }
            return null;
        }

        internal static List<int> GetSelectedListItemIndices(AutomationElement element)
        {
            SelectionPattern selectionPattern = PatternHelper.GetSelectionPattern(element);
            if (selectionPattern == null)
            {
                return null;
            }
            List<int> list = new List<int>();
            List<AutomationElement> selectedAutomationElements = new List<AutomationElement>();
            List<string> selectedItemsUsingPattern = null;
            BasePattern itemContainerPattern = PatternHelper.GetItemContainerPattern(element);
            if (itemContainerPattern != null && !string.Equals(element.Current.ClassName, "Pivot", StringComparison.OrdinalIgnoreCase))
            {
                selectedItemsUsingPattern = GetSelectedItemsUsingSearch(itemContainerPattern, element, ref selectedAutomationElements);
            }
            if (selectedItemsUsingPattern != null)
            {
                int num = 0;
                int item = 0;
                ItemContainerPattern pattern3 = itemContainerPattern as ItemContainerPattern;
                for (AutomationElement element2 = pattern3.FindItemByProperty(null, null, 0); element2 != null && num < selectedItemsUsingPattern.Count; element2 = pattern3.FindItemByProperty(element2, null, 0))
                {
                    if (element2.Equals(selectedAutomationElements[num]))
                    {
                        list.Add(item);
                        num++;
                    }
                    item++;
                }
            }
            if (selectedItemsUsingPattern == null)
            {
                selectedItemsUsingPattern = GetSelectedItemsUsingPattern(selectionPattern, ref selectedAutomationElements);
                if (selectedItemsUsingPattern == null)
                {
                    return null;
                }
                int num3 = 0;
                int num4 = 0;
                for (AutomationElement element3 = TreeWalker.ControlViewWalker.GetFirstChild(element); element3 != null && num3 < selectedItemsUsingPattern.Count; element3 = TreeWalker.ControlViewWalker.GetNextSibling(element3))
                {
                    if (element3.Equals(selectedAutomationElements[num3]))
                    {
                        list.Add(num4);
                        num3++;
                    }
                    System.Windows.Automation.ControlType currentPropertyValue = element3.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as System.Windows.Automation.ControlType;
                    if (currentPropertyValue != null && string.Equals(System.Windows.Automation.ControlType.ListItem.LocalizedControlType, currentPropertyValue.LocalizedControlType, StringComparison.OrdinalIgnoreCase))
                    {
                        num4++;
                    }
                }
            }
            return list;
        }

        internal static bool GetSelectionItemState(AutomationElement element)
        {
            SelectionItemPattern selectionItemPattern = PatternHelper.GetSelectionItemPattern(element, true);
            return selectionItemPattern.Current.IsSelected;
        }

        internal static string GetSelectionText(AutomationElement element)
        {
            TextPattern textPattern = PatternHelper.GetTextPattern(element);
            if (textPattern == null)
            {
                throw new NotSupportedException();
            }
            TextPatternRange[] selection = textPattern.GetSelection();
            if (selection != null && selection.Length != 0)
            {
                return selection[0].GetText(-1);
            }
            return null;
        }

        internal static ToggleState GetToggleState(AutomationElement element)
        {
            TogglePattern togglePattern = PatternHelper.GetTogglePattern(element);
            return togglePattern.Current.ToggleState;
        }

        public static UiaElement GetTopLevelElementForPhone(AutomationElement element)
        {
                        {
                AutomationElement automationElement = element;
                AutomationElement rootElement = AutomationElement.RootElement;
                while (element != rootElement)
                {
                    automationElement = element;
                    element = TreeWalkerHelper.GetParent(element);
                    if (element == null)
                    {
                        break;
                    }
                }
                if (automationElement != null)
                {
                    if (automationElement != rootElement && !string.Equals(automationElement.Current.ClassName, ZappyTaskCommonNames.ImmersiveAppWindowClassName, StringComparison.OrdinalIgnoreCase) && (!string.Equals(automationElement.Current.ClassName, "Foreground application", StringComparison.OrdinalIgnoreCase) || automationElement.Current.ProviderDescription.IndexOf("StartHost.exe", StringComparison.OrdinalIgnoreCase) < 0) && !string.Equals(automationElement.Current.AutomationId, "Phone_ChromeRootNode", StringComparison.Ordinal))
                    {
                        throw new Exception(Resources.NotSupportedForPhone);
                    }
                    return UiaElementFactory.GetUiaElement(automationElement, false);
                }
            }
            return null;
        }

        public static UiaElement GetTopLevelElementForPhoneUsingCondition(AndCondition condition)
        {
            try
            {
                AutomationElement rootElement = AutomationElement.RootElement;
                if (rootElement != null)
                {
                    AutomationElement automationElement = rootElement.FindFirst(TreeScope.Subtree, condition);
                    if (automationElement != null)
                    {
                        return UiaElementFactory.GetUiaElement(automationElement, false);
                    }
                }
            }
            catch (Exception exception)
            {
                object[] args = { exception.Message };
                CrapyLogger.log.ErrorFormat("UiaUtility: GetTopLevelElementForPhone {0} ", args);
            }
            return null;
        }

        private static ITaskActivityElement GetUiaElementForWindowsFormHost(UiaTechnologyManager manager, ITaskActivityElement element)
        {
            UiaElement convertedElement;
            Rectangle accBoundingRect;
            if (string.Equals("MSAA", element.Framework, StringComparison.OrdinalIgnoreCase))
            {
                int num2;
                int num3;
                int num4;
                int num5;
                object[] nativeElement = element.NativeElement as object[];
                convertedElement = null;
                if (nativeElement == null || nativeElement.Length < 2 || nativeElement.Length > 3)
                {
                    return null;
                }
                IAccessible accessible = nativeElement[0] as IAccessible;
                if (accessible == null || !(nativeElement[1] is int))
                {
                    return null;
                }
                int varChild = (int)nativeElement[1];
                try
                {
                    accessible.accLocation(out num2, out num3, out num4, out num5, varChild);
                }
                catch (Exception exception)
                {
                    CrapyLogger.log.Error(exception);
                    return null;
                }
                accBoundingRect = new Rectangle(num2, num3, num4, num5);
                NativeMethods.EnumWindowsProc lpEnumFunc = delegate (IntPtr windowHandle, ref IntPtr lParam)
                {
                    try
                    {
                        if (IsWindowsFormsHost(manager, windowHandle, out convertedElement) && accBoundingRect.Equals(convertedElement.GetBoundingRectangle()))
                        {
                            lParam = windowHandle;
                            return false;
                        }
                    }
                    catch (ZappyTaskException)
                    {
                    }
                    return true;
                };
                IntPtr zero = IntPtr.Zero;
                                {
                    NativeMethods.EnumChildWindows(element.WindowHandle, lpEnumFunc, ref zero);
                }
                if (zero != IntPtr.Zero)
                {
                    return convertedElement;
                }
            }
            return null;
        }

        public static AndCondition GetUIAPropertyConditionFromIQueryCondition(IQueryCondition queryCondition)
        {
            List<System.Windows.Automation.PropertyCondition> list = new List<System.Windows.Automation.PropertyCondition>();
            foreach (PropertyCondition condition in queryCondition.Conditions)
            {
                if (string.Equals(condition.PropertyName, "FrameworkId", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(new System.Windows.Automation.PropertyCondition(AutomationElement.FrameworkIdProperty, condition.Value));
                }
                else if (string.Equals(condition.PropertyName, "ClassName", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(new System.Windows.Automation.PropertyCondition(AutomationElement.ClassNameProperty, condition.Value));
                }
                else if (string.Equals(condition.PropertyName, "ControlType", StringComparison.OrdinalIgnoreCase) && ControlType.NameComparer.Equals(condition.Value.ToString(), ControlType.Custom.Name))
                {
                    list.Add(new System.Windows.Automation.PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Custom));
                }
            }
            return new AndCondition(list.ToArray());
        }

        internal static IntPtr GetWindowHandleFromAncestor(AutomationElement automationElement)
        {
            while (automationElement != null && automationElement.Current.NativeWindowHandle == 0)
            {
                automationElement = TreeWalker.ControlViewWalker.GetParent(automationElement);
            }
            if (automationElement != null)
            {
                return (IntPtr)automationElement.Current.NativeWindowHandle;
            }
            return IntPtr.Zero;
        }

        internal static bool IsAccessibleStateRequested(AccessibleStates requestedState, AccessibleStates matchState) =>
            (requestedState & matchState) > AccessibleStates.None;

        public static bool IsBoundaryElement(UiaElement element) =>
            element.IsCeilingElement || element.NativeWindowHandle != IntPtr.Zero && !IsParentOfWpfTechnology(element) || element.IsBoundaryForHostedControl;

        public static bool IsChildOfDesktop(UiaElement element)
        {
            IntPtr ancestor;
            if (element == null)
            {
                return false;
            }
            AutomationElement innerElement = element.InnerElement;
            if (innerElement.Current.NativeWindowHandle != 0)
            {
                ancestor = NativeMethods.GetAncestor((IntPtr)innerElement.Current.NativeWindowHandle, NativeMethods.GetAncestorFlag.GA_PARENT);
            }
            else
            {
                innerElement = TreeWalkerHelper.GetParent(innerElement);
                if (innerElement == null)
                {
                    object[] args = { element };
                    
                    return false;
                }
                ancestor = (IntPtr)innerElement.Current.NativeWindowHandle;
            }
            return NativeMethods.GetDesktopWindow() == ancestor;
        }

        public static bool IsConditionMatchesWpfElement(ITaskActivityElement element, PropertyCondition condition)
        {
            if (string.Equals(condition.PropertyName, "FrameworkId", StringComparison.OrdinalIgnoreCase) && condition.Value != null && string.Equals(condition.Value.ToString(), "WPF", StringComparison.OrdinalIgnoreCase))
            {
                IntPtr windowHandle = element.WindowHandle;
                if (windowHandle != IntPtr.Zero && ZappyTaskUtilities.IsWpfClassName(NativeMethods.GetClassName(windowHandle)))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsDesktop(UiaElement element)
        {
            if (element == null)
            {
                return false;
            }
            if (s_desktopWindowHandle == IntPtr.Zero)
            {
                s_desktopWindowHandle = NativeMethods.GetDesktopWindow();
            }
            return s_desktopWindowHandle == element.NativeWindowHandle;
        }

        public static bool IsDesktopWindowAndNotLegacy(IntPtr windowHandle) =>
            false;

        public static bool IsDesktopWindowAndNotLegacy(AutomationElement element) =>
            false;

        internal static bool IsDirectUIFrameWorkId(AutomationElement element)
        {
            bool flag;
            try
            {
                flag = element != null && string.Equals(element.Current.FrameworkId, "DirectUI", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception exception)
            {
                MapAndThrowException(exception, null, false);
                throw;
            }
            return flag;
        }

        public static bool IsEditableComboBox(UiaElement element, ref AutomationElement editElement)
        {
            if (element != null && ControlType.ComboBox.NameEquals(element.ControlTypeName) && PatternHelper.GetValuePattern(element.InnerElement) != null)
            {
                editElement = TreeWalkerHelper.GetFirstChild(element.InnerElement);
                if (editElement != null)
                {
                    return GetAutomationPropertyValue<System.Windows.Automation.ControlType>(editElement, AutomationElement.ControlTypeProperty) == System.Windows.Automation.ControlType.Edit;
                }
            }
            return false;
        }

        public static int IsElementHost(UiaTechnologyManager manager, IntPtr windowHandle, out UiaElement element)
        {
            element = null;
            if (IsWpfWindow(NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_PARENT)))
            {
                ITaskActivityElement elementFromWindowHandle = manager.GetElementFromWindowHandle(windowHandle);
                element = TransformUiaElement(elementFromWindowHandle);
                if (element != null && IsWpfFrameWorkId(element.InnerElement))
                {
                    return 0x65;
                }
            }
            return 0;
        }

        internal static bool IsElementOffScreen(AutomationElement element)
        {
            bool isOffscreen = element.Current.IsOffscreen;
            if (!isOffscreen && element.Current.ControlType == System.Windows.Automation.ControlType.List)
            {
                Rectangle boundingRectangle = element.Current.BoundingRectangle;
                return element.Current.BoundingRectangle.IsEmpty;
            }
            return isOffscreen;
        }

        internal static bool IsElementOfItemType(AutomationElement element)
        {
            if (element == null)
            {
                return false;
            }
            AutomationElement parent = TreeWalkerHelper.GetParent(element);
            return parent != null && PatternHelper.GetItemContainerPattern(parent) != null;
        }

        private static bool IsHtmlElementInWebView(AutomationElement automationElement, out AutomationElement webViewAutomationElement)
        {
            while (automationElement != null && !string.Equals(automationElement.Current.ClassName, "WebView", StringComparison.OrdinalIgnoreCase))
            {
                automationElement = TreeWalker.RawViewWalker.GetParent(automationElement);
            }
            webViewAutomationElement = automationElement;
            return automationElement != null;
        }

        public static bool IsImmersiveApplicationWindow(UiaElement element) =>
            false;

        internal static bool IsJupiterFrameWorkId(AutomationElement element)
        {
            bool flag;
            try
            {
                flag = element != null && string.Equals(element.Current.FrameworkId, "XAML", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception exception)
            {
                MapAndThrowException(exception, null, false);
                throw;
            }
            return flag;
        }

        internal static bool IsList(AutomationElement elementToValidate) =>
            MatchUiaControlType(System.Windows.Automation.ControlType.List, elementToValidate);

        internal static bool IsParentOfWpfTechnology(UiaElement element) =>
            IsWpfFrameWorkId(TreeWalkerHelper.GetParent(element.InnerElement));

        public static bool IsPointInsideElementRectangle(Rectangle rectangle, int pointX, int pointY)
        {
            rectangle.Inflate(-1, -1);
            return rectangle.Contains(pointX, pointY);
        }

        internal static bool IsPopUpWithTitleBar(IntPtr hwnd) =>
            !(hwnd == IntPtr.Zero) && NativeMethods.IsWindow(hwnd) && (NativeMethods.GetWindowLong(hwnd, NativeMethods.GWLParameter.GWL_STYLE) & 0xc00000) == 0xc00000;

        public static bool IsScrollableContainer(UiaElement element, out Rectangle clientArea)
        {
            double verticalScrollBarWidth = 0.0;
            double horizontalScrollBarHeight = 0.0;
            clientArea = element.GetBoundingRectangle();
            if (clientArea.Width <= 0 || clientArea.Height <= 0)
            {
                return false;
            }
            if (element.ControlTypeName == ControlType.ComboBox && !TaskActivityElement.IsState(element, AccessibleStates.Expanded))
            {
                return false;
            }
            bool flag = GetScrollBarsUsingSearch(element, ref verticalScrollBarWidth, ref horizontalScrollBarHeight) || GetScrollBarsUsingPattern(element, ref verticalScrollBarWidth, ref horizontalScrollBarHeight);
            if (flag)
            {
                clientArea.Height -= (int)horizontalScrollBarHeight;
                if (verticalScrollBarWidth == 0.0)
                {
                    return flag;
                }
                if (element.GetRightToLeftProperty(RightToLeftKind.Layout))
                {
                    clientArea.Width += (int)verticalScrollBarWidth;
                    return flag;
                }
                clientArea.Width -= (int)verticalScrollBarWidth;
            }
            return flag;
        }

        public static bool IsSilverlightWindow(UiaElement element) =>
            ControlType.Window.NameEquals(element.ControlTypeName) && IsSilverlightWindow(element.NativeWindowHandle);

        public static bool IsSilverlightWindow(IntPtr windowHandle) =>
            false;

        internal static bool isTitleBarButton(UiaElement parentElement) =>
            parentElement != null && ControlType.TitleBar.NameEquals(parentElement.ControlTypeName);

        internal static bool IsUiaElement(ITaskActivityElement element) =>
            element is UiaElement;

        private static bool IsUnsupportedClassNameForNonLegacy(string className) =>
            string.Equals(className, "ImmersiveSwitchList", StringComparison.OrdinalIgnoreCase) || string.Equals(className, "Internet Explorer_Server", StringComparison.OrdinalIgnoreCase);

        internal static bool IsValidListItemForEvent(AutomationElement element)
        {
            element = TreeWalkerHelper.GetFirstChild(element);
            int num = 1;
            VirtualizationContext unknown = VirtualizationContext.Unknown;
            while (element != null)
            {
                if (num == 2 || element.Current.ControlType != System.Windows.Automation.ControlType.Text)
                {
                    return false;
                }
                element = TreeWalkerHelper.GetNextSibling(element, ref unknown);
                num++;
            }
            return true;
        }

        internal static bool IsVirtualElement(AutomationElement element)
        {
                        {
                try
                {
                    if (AutomationElement.IsVirtualizedItemPatternAvailableProperty == null)
                    {
                        return false;
                    }
                    if (null == element || element.Current.ControlType == System.Windows.Automation.ControlType.MenuItem)
                    {
                        return false;
                    }
                    if (PatternHelper.GetVirtualizedItemPattern(element) != null)
                    {
                        return true;
                    }
                    AutomationElement parent = TreeWalker.ControlViewWalker.GetParent(element);
                    if (null != parent)
                    {
                        ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(parent) as ItemContainerPattern;
                        return itemContainerPattern != null;
                    }
                }
                catch (ElementNotAvailableException)
                {
                }
                catch (ZappyTaskControlNotAvailableException)
                {
                }
                return false;
            }
        }

        public static bool IsWebView(UiaElement element) =>
            false;

        public static bool IsWindowOfSupportedTechnology(UiaElement element)
        {
            if (ControlType.Window.NameEquals(element.ControlTypeName))
            {
                IntPtr nativeWindowHandle = element.NativeWindowHandle;
                if (nativeWindowHandle != IntPtr.Zero)
                {
                    return IsWindowOfSupportedTechnology(nativeWindowHandle, false);
                }
            }
            return false;
        }

        public static bool IsWindowOfSupportedTechnology(IntPtr windowHandle, bool throwException = false)
        {
            string className = NativeMethods.GetClassName(windowHandle);
            if (!WpfClassNameRegex.IsMatch(className) && !IsSilverlightWindow(windowHandle))
            {
                return false;
            }
            return true;
        }

        internal static bool IsWindowOfSupportedTechnology(AutomationElement element, bool throwException = false)
        {
            string className = element.Current.ClassName;
            IntPtr windowHandleFromAncestor = GetWindowHandleFromAncestor(element);
            if (!IsWpfFrameWorkId(element) && !IsSilverlightWindow(windowHandleFromAncestor))
            {
                return false;
            }
            return true;
        }

        private static bool IsWindowsFormsHost(UiaTechnologyManager manager, IntPtr windowHandle, out UiaElement element)
        {
            element = null;
            return ZappyTaskUtilities.IsWinformsClassName(NativeMethods.GetClassName(windowHandle)) && IsElementHost(manager, windowHandle, out element) == 0x65 && element != null && ControlType.Pane.NameEquals(element.ControlTypeName) && string.Equals(element.ClassName, "Uia.WindowsFormsHost", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsWpfFrameWorkId(AutomationElement element)
        {
            bool flag;
            try
            {
                flag = element != null && string.Equals(element.Current.FrameworkId, "WPF", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception exception)
            {
                MapAndThrowException(exception, null, false);
                throw;
            }
            return flag;
        }

        public static bool IsWpfWindow(UiaElement element) =>
            ControlType.Window.NameEquals(element.ControlTypeName) && IsWpfWindow(element.NativeWindowHandle);

        public static bool IsWpfWindow(IntPtr windowHandle)
        {
            if (windowHandle != IntPtr.Zero)
            {
                string className = NativeMethods.GetClassName(windowHandle);
                return WpfClassNameRegex.IsMatch(className);
            }
            return false;
        }

        public static void MapAndThrowException(Exception e, ITaskActivityElement element, bool useRethrowException = false)
        {
            if (e is InvalidOperationException || e is ElementNotAvailableException || e is COMException)
            {
                CrapyLogger.log.Error(e);
                if (element != null)
                {
                    throw e;
                }
                throw new ZappyTaskControlNotAvailableException(e);
            }
            if (useRethrowException && !(e is Exception))
            {
                throw new RethrowException(e, true);
            }
        }

        internal static bool MatchAutomationElement(AutomationElement element, AutomationElement elementToMatch)
        {
            if (element == null || elementToMatch == null)
            {
                return false;
            }
            int[] runtimeId = element.GetRuntimeId();
            string name = element.Current.Name;
            System.Windows.Automation.ControlType currentPropertyValue = element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as System.Windows.Automation.ControlType;
            return MatchAutomationElement(element, runtimeId, name, currentPropertyValue, elementToMatch);
        }

        internal static bool MatchAutomationElement(AutomationElement element, int[] runtimeId, string name, System.Windows.Automation.ControlType controlType, AutomationElement elementToMatch)
        {
            if (element != null && elementToMatch != null)
            {
                int[] numArray = elementToMatch.GetRuntimeId();
                if (Automation.Compare(runtimeId, numArray))
                {
                    return true;
                }
                string objB = elementToMatch.Current.Name;
                System.Windows.Automation.ControlType currentPropertyValue = elementToMatch.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as System.Windows.Automation.ControlType;
                if (Equals(name, objB) && controlType != null && controlType.Equals(currentPropertyValue))
                {
                    Rectangle? nullable = null;
                    Rectangle? nullable2 = null;
                    try
                    {
                        nullable = element.Current.BoundingRectangle;
                        nullable2 = elementToMatch.Current.BoundingRectangle;
                    }
                    catch (ElementNotAvailableException)
                    {
                    }
                    if (nullable2.HasValue && nullable.HasValue && nullable2.Value.Equals(nullable.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool MatchUiaControlType(System.Windows.Automation.ControlType controlType, AutomationElement elementToMatch)
        {
            if (controlType == null || elementToMatch == null)
            {
                return false;
            }
            System.Windows.Automation.ControlType currentPropertyValue = elementToMatch.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as System.Windows.Automation.ControlType;
            return MatchUiaControlType(controlType, currentPropertyValue);
        }

        internal static bool MatchUiaControlType(System.Windows.Automation.ControlType controlType, System.Windows.Automation.ControlType controlTypeToMatch) =>
            controlType != null && controlTypeToMatch != null && controlType.Equals(controlTypeToMatch);

        internal static void RealizeElement(AutomationElement element)
        {
            VirtualizedItemPattern virtualizedItemPattern = PatternHelper.GetVirtualizedItemPattern(element) as VirtualizedItemPattern;
            if (virtualizedItemPattern != null)
            {
                virtualizedItemPattern.Realize();
                AutomationProperty isOffscreenProperty = null;
                int num = 10;
                for (int i = 0; i <= num; i++)
                {
                    try
                    {
                        isOffscreenProperty = AutomationElement.IsOffscreenProperty;
                        if (!(bool)element.GetCurrentPropertyValue(isOffscreenProperty))
                        {
                            isOffscreenProperty = AutomationElement.BoundingRectangleProperty;
                            Rectangle currentPropertyValue = (Rectangle)element.GetCurrentPropertyValue(isOffscreenProperty);
                            if (!currentPropertyValue.IsEmpty)
                            {
                                break;
                            }
                        }
                    }
                    catch (ElementNotAvailableException)
                    {
                        object[] args = { isOffscreenProperty.ProgrammaticName };
                        CrapyLogger.log.ErrorFormat("RealizeElement - ElementNotAvailableException thrown while fetching property {0}.", args);
                        if (i == num)
                        {
                            throw;
                        }
                    }
                    ZappyTaskUtilities.Sleep(20);
                }
            }
        }

        private static void ScrollHorizontalHome(ScrollPattern pattern)
        {
            if (pattern != null && pattern.Current.HorizontallyScrollable)
            {
                try
                {
                    for (int i = 0; i < 20 && (int)pattern.Current.HorizontalScrollPercent != 0; i++)
                    {
                        pattern.ScrollHorizontal(ScrollAmount.LargeDecrement);
                        ZappyTaskUtilities.Sleep(10);
                    }
                }
                catch (Exception exception)
                {
                    object[] args = { exception.Message };
                    CrapyLogger.log.ErrorFormat("Uia.EnsureVisible: Error when attempting horizontal scroll {0}", args);
                }
            }
        }

        private static void ScrollVerticalHome(ScrollPattern pattern)
        {
            if (pattern != null && pattern.Current.VerticallyScrollable)
            {
                try
                {
                    for (int i = 0; i < 20 && (int)pattern.Current.VerticalScrollPercent != 0; i++)
                    {
                        pattern.ScrollVertical(ScrollAmount.LargeDecrement);
                        ZappyTaskUtilities.Sleep(10);
                    }
                }
                catch (Exception exception)
                {
                    object[] args = { exception.Message };
                    CrapyLogger.log.ErrorFormat("Uia.EnsureVisible: Error when attempting vertical scroll {0}", args);
                }
            }
        }

        public static void SelectContainerItem(AutomationElement automationElementContainer, string controlTypeName, int itemIndex)
        {
            if (automationElementContainer != null)
            {
                ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(automationElementContainer) as ItemContainerPattern;
                if (itemContainerPattern != null)
                {
                    AutomationElement startAfter = null;
                    int num = itemIndex;
                    do
                    {
                        startAfter = itemContainerPattern.FindItemByProperty(startAfter, null, 0);
                    }
                    while (num-- > 0 && startAfter != null);
                    if (itemIndex < 0 || startAfter == null)
                    {
                        object[] args = { itemIndex, controlTypeName, "SelectedIndex" };
                        throw new IndexOutOfRangeException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue, args));
                    }
                    RealizeElement(startAfter);
                    if (!SelectItemPrivate(startAfter))
                    {
                        object[] objArray1 = { itemIndex, controlTypeName, "SelectedIndex" };
                        throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue, objArray1));
                    }
                }
            }
        }

        public static void SelectContainerItem(AutomationElement automationElementContainer, string controlTypeName, string itemText)
        {
            if (automationElementContainer != null)
            {
                bool flag = false;
                AutomationElement element = FindItemUnderContainer(automationElementContainer, itemText);
                if (element != null)
                {
                    RealizeElement(element);
                    flag = SelectItemPrivate(element);
                }
                if (!flag)
                {
                    object[] args = { itemText, controlTypeName, "SelectedItem" };
                    throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue, args));
                }
            }
        }

        private static bool SelectItemPrivate(AutomationElement automationElementItem)
        {
            if (automationElementItem != null)
            {
                try
                {
                    SelectionItemPattern selectionItemPattern = PatternHelper.GetSelectionItemPattern(automationElementItem, true);
                    if (selectionItemPattern != null)
                    {
                        selectionItemPattern.Select();
                        return true;
                    }
                }
                catch (InvalidOperationException)
                {
                    CrapyLogger.log.ErrorFormat("Unable to select the ComboBox item");
                }
            }
            return false;
        }

        private static bool SelectListBoxItemPrivate(AutomationElement itemToSelectInListBox, bool canSelectMultiple)
        {
            SelectionItemPattern selectionItemPattern = PatternHelper.GetSelectionItemPattern(itemToSelectInListBox, true);
            try
            {
                if (selectionItemPattern != null)
                {
                    if (canSelectMultiple)
                    {
                        selectionItemPattern.AddToSelection();
                    }
                    else
                    {
                        selectionItemPattern.Select();
                    }
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
                CrapyLogger.log.ErrorFormat("Could not select the items");
            }
            return false;
        }

        public static void SelectListBoxItems(AutomationElement containerElement, int[] indices)
        {
            if (containerElement != null && indices != null)
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    if (indices[i] < 0)
                    {
                        object[] args = { indices[i], ControlType.List, "SelectedIndices" };
                        throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue, args));
                    }
                }
                UnSelectAllItemsPrivate(containerElement);
                List<int> sortedList = new List<int>(indices);
                sortedList.Sort();
                SelectListItemsPrivate(containerElement, sortedList);
            }
        }

        public static void SelectListBoxItems(AutomationElement containerElement, string[] listItemsToSelect)
        {
            if (containerElement != null)
            {
                UnSelectAllItemsPrivate(containerElement);
                SelectionPattern selectionPattern = PatternHelper.GetSelectionPattern(containerElement);
                bool canSelectMultiple = false;
                if (selectionPattern != null)
                {
                    canSelectMultiple = selectionPattern.Current.CanSelectMultiple;
                }
                ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(containerElement) as ItemContainerPattern;
                if (itemContainerPattern != null)
                {
                    foreach (string str in listItemsToSelect)
                    {
                        bool flag2 = false;
                        AutomationElement element = itemContainerPattern.FindItemByProperty(null, AutomationElement.NameProperty, str);
                        if (element != null)
                        {
                            RealizeElement(element);
                            flag2 = SelectListBoxItemPrivate(element, canSelectMultiple);
                        }
                        if (!flag2)
                        {
                            object[] args = { str, ControlType.List.Name, "SelectedItem" };
                            throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue, args));
                        }
                    }
                }
            }
        }

        public static void SelectListItemsPrivate(AutomationElement automationElementContainer, List<int> sortedList)
        {
            if (automationElementContainer != null)
            {
                ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(automationElementContainer) as ItemContainerPattern;
                if (itemContainerPattern != null)
                {
                    AutomationElement startAfter = null;
                    SelectionPattern selectionPattern = PatternHelper.GetSelectionPattern(automationElementContainer);
                    bool canSelectMultiple = false;
                    if (selectionPattern != null)
                    {
                        canSelectMultiple = selectionPattern.Current.CanSelectMultiple;
                    }
                    int num = 0;
                    for (int i = 0; i < sortedList.Count; i++)
                    {
                        do
                        {
                            startAfter = itemContainerPattern.FindItemByProperty(startAfter, null, 0);
                        }
                        while (num++ < sortedList[i] && startAfter != null);
                        if (num == sortedList[i] + 1 && startAfter != null)
                        {
                            RealizeElement(startAfter);
                            if (!SelectListBoxItemPrivate(startAfter, canSelectMultiple))
                            {
                                object[] args = { sortedList[i], ControlType.List.Name, "SelectedIndex" };
                                throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue, args));
                            }
                        }
                        else
                        {
                            object[] objArray2 = { sortedList[i], ControlType.List.Name, "SelectedIndex" };
                            throw new IndexOutOfRangeException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue, objArray2));
                        }
                    }
                }
            }
        }

        internal static void SetSelectionItemState(AutomationElement element, bool selected)
        {
            SelectionItemPattern selectionItemPattern = PatternHelper.GetSelectionItemPattern(element, true);
            if (selectionItemPattern != null)
            {
                if (selected)
                {
                    selectionItemPattern.Select();
                    return;
                }
                try
                {
                    selectionItemPattern.RemoveFromSelection();
                    return;
                }
                catch (ElementNotAvailableException exception)
                {
                    if (exception.InnerException is InvalidOperationException)
                    {
                        throw new NotSupportedException();
                    }
                    throw;
                }
            }
            throw new NotSupportedException();
        }

        internal static void SetSliderPostion(AutomationElement element, double value)
        {
            RangeValuePattern rangeValuePattern = PatternHelper.GetRangeValuePattern(element);
            if (rangeValuePattern == null)
            {
                throw new NotSupportedException();
            }
            rangeValuePattern.SetValue(value);
        }

        internal static void SetToggleState(AutomationElement element, ToggleState desiredState)
        {
            TogglePattern togglePattern = PatternHelper.GetTogglePattern(element);
            if (togglePattern != null)
            {
                if (togglePattern.Current.ToggleState == desiredState)
                {
                    return;
                }
                for (int i = 0; i < 2; i++)
                {
                    togglePattern.Toggle();
                    if (togglePattern.Current.ToggleState == desiredState)
                    {
                        return;
                    }
                }
            }
            throw new Exception();
        }

        internal static void SwitchToWindow(IntPtr handle)
        {
        }

        internal static UiaElement TransformUiaElement(ITaskActivityElement element)
        {
            if (element is UiaElement)
            {
                return element as UiaElement;
            }
            return null;
        }

        internal static bool TryConvertToType<T>(object value, out T retValue)
        {
            if (value != null)
            {
                try
                {
                    retValue = (T)value;
                }
                catch (InvalidCastException)
                {
                    if (!TryParseToType(value, out retValue))
                    {
                        return false;
                    }
                }
            }
            else
            {
                retValue = default(T);
            }
            return true;
        }

        private static bool TryParseToType<T>(object value, out T retValue)
        {
            Type type = typeof(T);
            retValue = default(T);
            try
            {
                if (type == typeof(int))
                {
                    int num;
                    if (int.TryParse(value.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out num))
                    {
                        object obj2 = num;
                        retValue = (T)obj2;
                    }
                }
                else
                {
                    retValue = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                }
            }
            catch (Exception exception)
            {
                if (!(exception is InvalidCastException) && !(exception is FormatException))
                {
                    throw;
                }
                return false;
            }
            return true;
        }

        private static void UnSelectAllItemsPrivate(AutomationElement containerElement)
        {
            ItemContainerPattern itemContainerPattern = PatternHelper.GetItemContainerPattern(containerElement) as ItemContainerPattern;
            if (itemContainerPattern != null)
            {
                AutomationElement startAfter = null;
                startAfter = itemContainerPattern.FindItemByProperty(startAfter, null, 0);
                if (startAfter != null)
                {
                    SelectItemPrivate(startAfter);
                }
                UnSelectListBoxItemPrivate(startAfter);
            }
        }

        private static void UnSelectListBoxItemPrivate(AutomationElement itemToSelectInListBox)
        {
            if (itemToSelectInListBox != null)
            {
                SelectionItemPattern selectionItemPattern = PatternHelper.GetSelectionItemPattern(itemToSelectInListBox, true);
                try
                {
                    if (selectionItemPattern != null)
                    {
                        selectionItemPattern.RemoveFromSelection();
                    }
                }
                catch (InvalidOperationException)
                {
                    CrapyLogger.log.ErrorFormat("Could not unselect the item");
                }
                catch (ElementNotAvailableException exception)
                {
                    CrapyLogger.log.ErrorFormat("Could not unselect the item");
                    if (!(exception.InnerException is InvalidOperationException))
                    {
                        throw exception;
                    }
                }
            }
        }

        internal static void WaitForReady(UiaElement element)
        {
        }
    }
}

