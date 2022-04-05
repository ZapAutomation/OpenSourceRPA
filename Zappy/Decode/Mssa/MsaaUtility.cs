using Accessibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers.Interface;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;
using PropertyCondition = Zappy.ActionMap.HelperClasses.PropertyCondition;

namespace Zappy.Decode.Mssa
{
    internal static class MsaaUtility
    {
        internal const string _isManagedMsaaElement = "_isManagedMsaaElement";
        internal const string _isNumericUpDownControl = "_isNumericUpDownControl";
        internal const string _isWin32ForSure = "_isWin32ForSure";
        internal const string AccessibleDescription = "AccessibleDescription";
        internal const string AutoSuggestDropDownClass = "Auto-Suggest Dropdown";
        internal static string classNameOfSiblingWindow;
        internal const string ClientCaption = "Client Caption";
        private static uint controlNameMessage = NativeMethods.RegisterWindowMessage("WM_GETCONTROLNAME");
        internal const string DesktopOwnerClassName = "SHELLDLL_DefView";

        private static IntPtr desktopWindowHandle = NativeMethods.GetDesktopWindow();
        internal static Dictionary<string, int> doNotUseTextControlList;
        internal const int E_ACCESSDENIED = -2147024891;
        private static Dictionary<string, string> elementProperties;
        private const string FirefoxClassName = "MozillaUIWindowClass";
        private const string FirefoxTechnologyName = "Mozilla Firefox";
        internal const string HelpText = "HelpText";
        private static readonly IntPtr HTCAPTION;
        private static readonly IntPtr HTCLIENT;
        private static readonly IntPtr HTHSCROLL;
        private static readonly IntPtr HTVSCROLL;
        internal const string IEControlRolename = "client";
        internal const string IEFrameClassname = "IEFrame";
        internal static Rectangle InvalidRectangle;
        internal const string IsSimpleComboBoxType = "IsSimpleComboBoxType";
        internal const string LightWeightInstance = "_LightWeightInstance";
        private const string MfcClassName = "Afx:";
        internal const string MicrosoftTestManagerProcess = "mtm";
        internal static Dictionary<string, int> notAcceptableTopLevelClassesExact;
        internal static List<string> notAcceptableTopLevelClassesPartial;
        internal static int numberOfFoundSiblingWindows;
        private const int PrecisionValue = 2;
        internal const string QueryConditionName = "SearchCondition";
        internal const string QueryElementPropertyFormat = "{0}='{1}'";
        internal const string QueryElementSeparator = ";";
        internal static readonly string QueryFramework;
        internal const string QueryPropertyAccessibleName = "AccessibleName";
        internal const string QueryPropertyAccessKey = "AccessKey";
        internal const string QueryPropertyCachedAccessKey = "CachedAccessKey";
        internal const string QueryPropertyClassName = "ClassName";
        internal const string QueryPropertyControlID = "ControlId";
        internal const string QueryPropertyControlName = "ControlName";
        internal const string QueryPropertyControlTypeName = "ControlType";
        internal const string QueryPropertyDescription = "Description";
        internal const string QueryPropertyInstance = "Instance";
        internal const string QueryPropertyName = "Name";
        internal const string QueryPropertyNativeControlType = "NativeControlType";
        internal const string QueryPropertyOrderOfInvoke = "OrderOfInvocation";
        internal const string QueryPropertyOwnerWindowClassName = "OwnerWindowClassName";
        internal const string QueryPropertyOwnerWindowHandle = "OwnerWindowHandle";
        internal const string QueryPropertyOwnerWindowText = "OwnerWindowText";
        internal const string QueryPropertyRole = "Role";
        internal const string QueryPropertySeparator = " && ";
        internal const string QueryPropertyValue = "Value";
        internal const string RichTextBoxClassName = "RICHEDIT";
        internal static Dictionary<AccessibleRole, ControlType> roleControlTypeMapping;
        internal static Dictionary<AccessibleRole, string> roleConversionTable;
        internal static Dictionary<AccessibleRole, string> roleStringMapping;
        internal const string ShellTrayClassName = "Shell_TrayWnd";
        private const int SliderMaximum = 0x402;
        private const int SliderMinimum = 0x401;
        internal const string Win32ListClassName = "SysListView32";
        internal const string WinformsDataGridCellCheckBoxHelpText = "DataGridViewCheckBoxCell(DataGridViewCell)";
        private const uint WM_NCHITTEST = 0x84;
        private const long WS_CHILD = 0x40000000L;
        private const long WS_EX_MDICHILD = 0x40L;

        static MsaaUtility()
        {
            object[] args = { "MSAA" };
            QueryFramework = string.Format(CultureInfo.InvariantCulture, "[{0}]", args);
            HTCLIENT = (IntPtr)1;
            HTCAPTION = (IntPtr)2;
            HTHSCROLL = (IntPtr)6;
            HTVSCROLL = (IntPtr)7;
            InvalidRectangle = new Rectangle(-1, -1, -1, -1);
            roleControlTypeMapping = InitializeRoleControlTypeMapping();
            roleStringMapping = InitializeRoleStringMapping();
            roleConversionTable = InitializeRoleConversionTable();
            doNotUseTextControlList = InitializeDoNotUseTextControlList();
            notAcceptableTopLevelClassesExact = InitializeNotAcceptableTopLevelClassesExact();
            notAcceptableTopLevelClassesPartial = InitializeNotAcceptableTopLevelClassesPartial();
            elementProperties = InitializeElementProperties();
        }

        internal static bool CanUseControlTextProperty(string controlTypeName) =>
            !doNotUseTextControlList.ContainsKey(controlTypeName);

        internal static bool CanUseNameProperty(MsaaElement element) =>
            !DateTimePickerUtilities.IsDateTimePickerClassName(element.ClassName) && !ControlType.Calendar.NameEquals(element.ControlTypeName) && !MonthCalendarUtilities.IsMonthCalendarButton(element) && !ControlType.RowHeader.NameEquals(element.ControlTypeName);

        private static bool CheckStyle(int dwStyles, int dwExStyles) =>
            (dwStyles & 0x40000000L) != 0 && (dwExStyles & 0x40L) == 0L;

        internal static AccessibleRole ConvertRoleToInteger(string role)
        {
            foreach (AccessibleRole role2 in roleConversionTable.Keys)
            {
                if (string.Equals(roleConversionTable[role2], role, StringComparison.OrdinalIgnoreCase))
                {
                    return role2;
                }
            }
            return AccessibleRole.None;
        }

        internal static bool ElementBreaksMenuTree(MsaaElement element) =>
            element.RoleInt == AccessibleRole.PushButton;

        internal static AccWrapper FastAccessibleObjectFromPoint(NativeMethods.POINT ptScreen)
        {
            IntPtr windowHandle = WindowFromPoint(ptScreen);
            if (windowHandle == IntPtr.Zero)
            {
                return null;
            }
            AccWrapper accWrapperFromWindow = null;
            try
            {
                accWrapperFromWindow = AccWrapper.GetAccWrapperFromWindow(windowHandle);
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
            if (accWrapperFromWindow == null)
            {
                return null;
            }
        Label_0029:;
            try
            {
                if (accWrapperFromWindow.ChildCount > 0)
                {
                    object obj2 = accWrapperFromWindow.AccessibleObject.accHitTest(ptScreen.x, ptScreen.y);
                    if (obj2 is IAccessible)
                    {
                        accWrapperFromWindow = new AccWrapper(obj2 as IAccessible, 0);
                    }
                    else
                    {
                        if (obj2 is int)
                        {
                            accWrapperFromWindow = AccWrapper.GetAccessibleFromObject(accWrapperFromWindow.AccessibleObject, (int)obj2);
                            goto Label_008F;
                        }
                        if (obj2 == null)
                        {
                            goto Label_008F;
                        }
                    }
                    goto Label_0029;
                }
            }
            catch (SystemException)
            {
            }
        Label_008F:
            if (!LocalizedSystemStrings.IsIE9OrLater || !string.Equals(NativeMethods.GetClassName(accWrapperFromWindow.WindowHandle), "Client Caption", StringComparison.OrdinalIgnoreCase))
            {
                return accWrapperFromWindow;
            }
            string className = NativeMethods.GetClassName(windowHandle);
            if (!string.Equals("IEFrame", className, StringComparison.OrdinalIgnoreCase) && !string.Equals("Client Caption", className, StringComparison.OrdinalIgnoreCase))
            {
                return accWrapperFromWindow;
            }
            return null;
        }

        internal static bool FindChildWithClass(IntPtr windowHandle, ref IntPtr lParam)
        {
            if (NativeMethods.GetClassName(windowHandle) == classNameOfSiblingWindow && ++numberOfFoundSiblingWindows > 1)
            {
                return false;
            }
            return true;
        }

        internal static QueryElement GenerateSingleQueryElement(MsaaElement element, QueryElementProperty properties, int instance, bool generateAccessibleName)
        {
            QueryElement element2 = new QueryElement();
            AndConditionBuilder andConditionBuilder = new AndConditionBuilder();
            if (!GenerateSingleQueryForLanguageNeutralControls(element, properties, andConditionBuilder))
            {
                GenerateSingleQueryForNonLanguageNeutralControls(element, properties, andConditionBuilder, generateAccessibleName);
            }
            if ((properties & QueryElementProperty.ControlTypeName) != QueryElementProperty.None)
            {
                andConditionBuilder.Append("ControlType", element.ControlTypeName);
            }
            if (instance > 1)
            {
                andConditionBuilder.Append("Instance", instance);
            }
            if (andConditionBuilder.Count == 0)
            {
                andConditionBuilder.Append("ControlType", element.ControlTypeName);
            }
            if ((properties & QueryElementProperty.OrderOfInvocation) != QueryElementProperty.None)
            {
                int orderOfInvocation = OrderOfInvoke.GetOrderOfInvocation(element, andConditionBuilder.Build());
                if (orderOfInvocation > 1)
                {
                    PropertyCondition condition = new PropertyCondition("OrderOfInvocation", orderOfInvocation);
                    IQueryCondition[] conditions = { condition };
                    andConditionBuilder.Append(new FilterCondition(conditions));
                }
            }
            element2.Condition = andConditionBuilder.Build();
            element2.Condition.Name = "SearchCondition";
            return element2;
        }

        internal static bool GenerateSingleQueryForLanguageNeutralControls(MsaaElement element, QueryElementProperty properties, AndConditionBuilder andConditionBuilder)
        {
            bool flag = false;
            if ((properties & QueryElementProperty.ControlName) != QueryElementProperty.None)
            {
                string id = element.Id;
                if (!string.IsNullOrEmpty(id))
                {
                    andConditionBuilder.Append("ControlName", id);
                    flag = true;
                }
            }
            if (!flag && (properties & QueryElementProperty.ControlId) != QueryElementProperty.None)
            {
                int controlId = element.ControlId;
                if (controlId > 0 && controlId != element.WindowHandle.ToInt32())
                {
                    andConditionBuilder.Append("ControlId", controlId);
                    flag = true;
                }
            }
            return flag;
        }

        internal static void GenerateSingleQueryForNonLanguageNeutralControls(MsaaElement element, QueryElementProperty properties, AndConditionBuilder andConditionBuilder, bool generateAccessibleName)
        {
            string propertyValue = string.Empty;
            if (generateAccessibleName)
            {
                if ((properties & QueryElementProperty.Name) != QueryElementProperty.None && !string.IsNullOrEmpty(element.ControlText))
                {
                    propertyValue = NormalizeQueryPropertyValue(element.ControlText);
                    andConditionBuilder.Append("Name", propertyValue);
                }
                else
                {
                    string name = element.AccessibleWrapper.Name;
                    if ((properties & QueryElementProperty.Name) != QueryElementProperty.None && !string.IsNullOrEmpty(name))
                    {
                        propertyValue = NormalizeQueryPropertyValue(name);
                        andConditionBuilder.Append("AccessibleName", propertyValue);
                    }
                }
            }
            else if (string.IsNullOrEmpty(propertyValue) && (properties & QueryElementProperty.Name) != QueryElementProperty.None)
            {
                propertyValue = element.Name;
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    propertyValue = NormalizeQueryPropertyValue(propertyValue);
                    andConditionBuilder.Append("Name", propertyValue);
                }
            }
            if ((properties & QueryElementProperty.Value) != QueryElementProperty.None)
            {
                string valueForIdentification = element.ValueForIdentification;
                if (!string.IsNullOrEmpty(valueForIdentification))
                {
                    if (ControlType.Row.NameEquals(element.ControlTypeName) && QueryIdHelper.ShouldGenerateContainsRowValue(element, ref valueForIdentification))
                    {
                        andConditionBuilder.Append(new PropertyCondition("Value", valueForIdentification, PropertyConditionOperator.Contains));
                    }
                    else
                    {
                        andConditionBuilder.Append("Value", valueForIdentification);
                    }
                }
            }
            if ((properties & QueryElementProperty.ClassName) != QueryElementProperty.None)
            {
                string className = element.ClassName;
                if (!string.IsNullOrEmpty(className))
                {
                    PropertyConditionOperator equalTo = PropertyConditionOperator.EqualTo;
                    string b = ZappyTaskUtilities.NormalizeDynamicClassName(className);
                    if (!string.Equals(className, b, StringComparison.Ordinal))
                    {
                        equalTo = PropertyConditionOperator.Contains;
                    }
                    andConditionBuilder.Append(new PropertyCondition("ClassName", b, equalTo));
                }
            }
        }

        internal static string GetAbsoluteValueForSlider(MsaaElement element)
        {
            double num3;
            int num = MsaaNativeMethods.SendMessage(element.WindowHandle, 0x401, IntPtr.Zero, IntPtr.Zero).ToInt32();
            int num2 = MsaaNativeMethods.SendMessage(element.WindowHandle, 0x402, IntPtr.Zero, IntPtr.Zero).ToInt32();
            string s = element.AccessibleWrapper.Value;
            if (!double.TryParse(s, out num3))
            {
                return s;
            }
            if (num2 != num)
            {
                double num4 = num + num3 * (num2 - num) / 100.0;
                return Math.Round(num4, 2).ToString(CultureInfo.InvariantCulture);
            }
            return Math.Round(num3, 2).ToString(CultureInfo.InvariantCulture);
        }

        internal static AccWrapper GetAccessibleObjectFromAE(AutomationElement element)
        {
            if (element != null)
            {
                object obj2;
                IAccessible accessibleObject = null;
                if (element.TryGetCurrentPattern(AutomationPattern.LookupById(LegacyIAccessiblePattern.Pattern.Id), out obj2))
                {
                    try
                    {
                        accessibleObject = (obj2 as LegacyIAccessiblePattern).GetIAccessible();
                    }
                    catch (ElementNotAvailableException)
                    {
                    }
                    if (accessibleObject != null)
                    {
                        return new AccWrapper(accessibleObject, (obj2 as LegacyIAccessiblePattern).Current.ChildId);
                    }
                    if (element.Current.NativeWindowHandle != 0)
                    {
                        return AccWrapper.GetAccWrapperFromWindow(new IntPtr(element.Current.NativeWindowHandle));
                    }
                }
            }
            return null;
        }


        internal static IAccessible GetChildIAccessible(IAccessible accessibleObject, object childId)
        {
            try
            {
                return (IAccessible)accessibleObject.get_accChild(childId);
            }
            catch (SystemException)
            {
                return null;
            }
        }

        internal static ITaskActivityElement GetChildIfOnlyChild(ITaskActivityElement element)
        {
            ITaskActivityElement element2 = MsaaZappyPlugin.Navigate(element, AccessibleNavigation.FirstChild);
            if (element2 != null && MsaaZappyPlugin.Navigate(element2, AccessibleNavigation.Next) == null)
            {
                return element2;
            }
            return null;
        }

        internal static int GetControlID(IntPtr windowHandle) =>
            NativeMethods.GetWindowLong(windowHandle, NativeMethods.GWLParameter.GWL_ID);

        internal static string GetControlName(IntPtr windowHandle, int processId)
        {
            string str = string.Empty;
            if (windowHandle != IntPtr.Zero && NativeMethods.IsWindowResponding(windowHandle))
            {
                IntPtr ptr2;
                IntPtr ptr3;
                IntPtr wParam = MsaaNativeMethods.SendMessage(windowHandle, controlNameMessage, IntPtr.Zero, IntPtr.Zero);
                if (wParam == IntPtr.Zero)
                {
                    return str;
                }
                if (!MsaaNativeMethods.AllocateInProcMemory((uint)processId, wParam.ToInt32(), out ptr3, out ptr2))
                {
                    return str;
                }
                try
                {
                    if (MsaaNativeMethods.SendMessage(windowHandle, controlNameMessage, wParam, ptr3) != IntPtr.Zero)
                    {
                        byte[] buffer = new byte[wParam.ToInt32()];
                        if (MsaaNativeMethods.ReadProcessMemory(ptr2, ptr3, buffer))
                        {
                            str = Encoding.Unicode.GetString(buffer).TrimEnd(new char[1]);
                        }
                    }
                }
                finally
                {
                    MsaaNativeMethods.FreeInProcAllocatedMemory(ptr3, ptr2);
                }
            }
            return str;
        }

        internal static int GetControlSupportLevel(IntPtr windowHandle) =>
            GetControlSupportLevelInternal(string.Empty, windowHandle);

        internal static int GetControlSupportLevel(AutomationElement element) =>
            GetControlSupportLevelInternal(element.Current.ClassName, GetWindowHandleFromAncestor(element));

        private static int GetControlSupportLevelInternal(string className, IntPtr windowHandle)
        {
            if (string.IsNullOrEmpty(className))
            {
                className = NativeMethods.GetClassName(windowHandle);
            }
                        
            if (ZappyTaskUtilities.IsWinformsClassName(className) || IsMfcClassName(windowHandle))
            {
                return 100;
            }
            if (ZappyTaskUtilities.IsImmersiveBrowserWindow(windowHandle))
            {
                throw new Exception("TechnologyNotSupportedException(string.Format(CultureInfo.CurrentCulture, Resource.ImmersiveBrowserNotSupportedMessage, args))");
            }
            if (!string.IsNullOrEmpty(className) && !ZappyTaskUtilities.IsWindowsStartScreen(windowHandle) && (className.Contains(ZappyTaskCommonNames.ImmersiveAppWindowClassName) || ZappyTaskUtilities.IsRootParentSupportedControl(windowHandle, className)))
            {
                if (!string.Equals(Process.GetCurrentProcess().ProcessName, "mtm", StringComparison.OrdinalIgnoreCase))
                {
                    if (ZappyTaskUtilities.IsWWAWindow(windowHandle))
                    {
                        throw new Exception("TechnologyNotSupportedException(Resource.WWANotSupported");
                    }
                    throw new Exception("TechnologyNotSupportedException(string.Format(CultureInfo.CurrentCulture, Resource.WindowStoreAppsNotSupportedMessage, new object[0])");
                }
                throw new Exception("TechnologyNotSupportedException(string.Format(CultureInfo.CurrentCulture, Resource.MTMWindowsStoreAppsNotSupported, objArray2)");
            }
                                                                        return 1;
        }

        internal static string GetControlTypeName(AccessibleRole role)
        {
            if (roleControlTypeMapping.ContainsKey(role))
            {
                return roleControlTypeMapping[role].Name;
            }
            if (roleStringMapping.ContainsKey(role))
            {
                return roleStringMapping[role];
            }
            object[] args = { role };
            
            int num = (int)role;
            if (num > 0 && num <= 0x40)
            {
                return role.ToString();
            }
            return ControlType.Custom.Name;
        }

        internal static int GetLightWeightInstance(AccWrapper element)
        {
            if (element == null)
            {
                return 0;
            }
            AccWrapper wrapper = element.Navigate(AccessibleNavigation.Previous);
            int num = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (wrapper != null && stopwatch.ElapsedMilliseconds < 0x7d0L)
            {
                if (wrapper.RoleInt == element.RoleInt)
                {
                    num++;
                }
                wrapper = wrapper.Navigate(AccessibleNavigation.Previous);
            }
            if (wrapper != null)
            {
                stopwatch.Stop();
                
                AccWrapper parent = element.Parent;
                if (parent == null)
                {
                    return num;
                }
                AccChildrenEnumerator enumerator = new AccChildrenEnumerator(parent);
                if (enumerator == null)
                {
                    return num;
                }
                AccWrapper nextChild = enumerator.GetNextChild(false);
                num = 0;
                while (nextChild != null && !nextChild.Equals(element))
                {
                    if (nextChild.RoleInt == element.RoleInt)
                    {
                        num++;
                    }
                    nextChild = enumerator.GetNextChild(false);
                }
            }
            return num;
        }

        internal static MsaaElement GetMsaaTopElement(MsaaElement element, out bool isBrokenTree)
        {
            isBrokenTree = false;
            bool flag = false;
            MsaaElement element2 = element;
            bool flag2 = element.RoleInt == AccessibleRole.MenuItem;
            if (element.SwitchingElement != null)
            {
                return null;
            }
            if (element.WindowHandle != DesktopWindowHandle)
            {
                int num = 0;
                while (element2 != null)
                {
                    if (element2.RoleInt == AccessibleRole.MenuPopup)
                    {
                        num++;
                        if (IsTopLevelPopUpMenu(element2))
                        {
                            flag = true;
                        }
                    }
                    MsaaElement parent = element2.Parent;
                    if (parent == null || parent.WindowHandle == DesktopWindowHandle)
                    {
                        break;
                    }
                    if (flag2 && ElementBreaksMenuTree(parent))
                    {
                        isBrokenTree = true;
                        break;
                    }
                    element2 = parent;
                }
                if (element2 != null && !isBrokenTree && (!IsWindowAcceptableAsTop(element2.ClassName) || !element2.IsDesktop && element2.Parent == null))
                {
                    if (flag && num > 1)
                    {
                        isBrokenTree = true;
                    }
                    else
                    {
                        element2 = null;
                    }
                }
            }
            object[] args = { element2 };
            
            return element2;
        }

        internal static MsaaElement GetPreviousSiblingFlattened(MsaaElement element, ref MsaaElement parent)
        {
            MsaaElement element2 = MsaaZappyPlugin.Navigate(element, AccessibleNavigation.Previous);
            if (element2 == null)
            {
                element2 = element.Parent;
                if (element2 == null)
                {
                    return element2;
                }
                MsaaElement element3 = element2.Parent;
                if (element3 == null || element3.WindowHandle != DesktopWindowHandle)
                {
                    element2 = MsaaZappyPlugin.Navigate(element2, AccessibleNavigation.Previous);
                    if (element2 == null)
                    {
                        return element2;
                    }
                    parent = element2.Parent;
                    element2 = MsaaZappyPlugin.Navigate(element2, AccessibleNavigation.LastChild);
                    if (parent == null)
                    {
                        return null;
                    }
                    if (!parent.IsTopLevelElement && !parent.IsBoundayForHostedControl)
                    {
                        return element2;
                    }
                }
                return null;
            }
            parent = element2.Parent;
            if (parent == null || !parent.IsTopLevelElement && !parent.IsBoundayForHostedControl)
            {
                return element2;
            }
            return null;
        }

        internal static string GetPropertyNameInCorrectCase(string propertyName)
        {
            if (elementProperties.ContainsKey(propertyName))
            {
                return elementProperties[propertyName];
            }
            return propertyName;
        }

        internal static string GetRoleText(AccessibleRole role)
        {
            string str = string.Empty;
            if (role != AccessibleRole.None && roleConversionTable.ContainsKey(role))
            {
                return roleConversionTable[role];
            }
            StringBuilder lpszRole = new StringBuilder(0x100);
            if (MsaaNativeMethods.GetRoleText((int)role, lpszRole, (uint)lpszRole.Capacity) > 0)
            {
                str = lpszRole.ToString();
            }
            return str;
        }

        internal static MsaaElement GetWin32TopElement(MsaaElement element)
        {
            IntPtr zero = IntPtr.Zero;
            bool flag = false;
            IntPtr windowHandle = element.WindowHandle;
            IntPtr desktopWindowHandle = DesktopWindowHandle;
            bool isMenu = element.RoleInt == AccessibleRole.MenuItem;
            MsaaElement element2 = null;
            if (windowHandle == IntPtr.Zero)
            {
                windowHandle = desktopWindowHandle;
            }
            MsaaElement parent = element.Parent;
            if (isMenu && (parent == null || !ValidMenuParent(parent.RoleInt)))
            {
                AccWrapper accWrapperFromWindow = null;
                try
                {
                    accWrapperFromWindow = AccWrapper.GetAccWrapperFromWindow(windowHandle);
                }
                catch (ZappyTaskControlNotAvailableException)
                {
                    if (!NativeMethods.IsWindow(windowHandle))
                    {
                        throw;
                    }
                }
                if (accWrapperFromWindow != null && accWrapperFromWindow.RoleInt != AccessibleRole.MenuItem)
                {
                    windowHandle = element.BackupWindowHandle;
                }
            }
            IntPtr ancestor = NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_ROOT);
            if (!IsFairWin32Top(ancestor, isMenu))
            {
                while (windowHandle != IntPtr.Zero)
                {
                    if (IsFairWin32Top(windowHandle, isMenu))
                    {
                        flag = true;
                        zero = windowHandle;
                    }
                    windowHandle = NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_PARENT);
                    if ((windowHandle == desktopWindowHandle || windowHandle == IntPtr.Zero) && flag)
                    {
                        element2 = new MsaaElement(zero);
                        break;
                    }
                }
            }
            else
            {
                element2 = new MsaaElement(ancestor);
            }
            if (element2 == null)
            {
                element2 = new MsaaElement(desktopWindowHandle);
            }
            return element2;
        }

        private static IntPtr GetWindowHandleFromAncestor(AutomationElement automationElement)
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

        internal static bool HasLanguageNeutralID(MsaaElement element)
        {
            bool flag = false;
            if (element == null || !element.IsWin32ForSure || !(NativeMethods.GetAncestor(element.WindowHandle, NativeMethods.GetAncestorFlag.GA_PARENT) != DesktopWindowHandle))
            {
                return flag;
            }
            if ((element.ControlId <= 0 || element.ControlId == element.WindowHandle.ToInt32()) && string.IsNullOrEmpty(element.Id))
            {
                return flag;
            }
            return true;
        }

        internal static bool HasSearchConfiguration(string[] searchConfigurations, string testSearchConfiguration)
        {
            if (searchConfigurations != null && searchConfigurations.Length != 0)
            {
                foreach (string str in searchConfigurations)
                {
                    if (string.Equals(str, testSearchConfiguration, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool HasState(AccessibleStates state, AccessibleStates testState) =>
            (state & testState) == testState;

        private static Dictionary<string, int> InitializeDoNotUseTextControlList() =>
            new Dictionary<string, int>(ControlType.NameComparer) {
                {
                    ControlType.Edit.Name,
                    1
                },
                {
                    ControlType.MenuBar.Name,
                    1
                },
                {
                    ControlType.StatusBar.Name,
                    1
                },
                {
                    ControlType.ToolBar.Name,
                    1
                },
                {
                    ControlType.List.Name,
                    1
                },
                {
                    ControlType.DateTimePicker.Name,
                    1
                },
                {
                    ControlType.Calendar.Name,
                    1
                },
                {
                    ControlType.Cell.Name,
                    1
                }
            };

        private static Dictionary<string, string> InitializeElementProperties() =>
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {
                    "AccessibleName",
                    "AccessibleName"
                },
                {
                    "ClassName",
                    "ClassName"
                },
                {
                    "ControlId",
                    "ControlId"
                },
                {
                    "ControlName",
                    "ControlName"
                },
                {
                    "Description",
                    "Description"
                },
                {
                    "Name",
                    "Name"
                },
                {
                    "Value",
                    "Value"
                },
                {
                    "Instance",
                    "Instance"
                },
                {
                    "OrderOfInvocation",
                    "OrderOfInvocation"
                },
                {
                    "AccessKey",
                    "AccessKey"
                },
                {
                    "OwnerWindowHandle",
                    "OwnerWindowHandle"
                },
                {
                    "OwnerWindowClassName",
                    "OwnerWindowClassName"
                },
                {
                    "OwnerWindowText",
                    "OwnerWindowText"
                },
                {
                    "ControlType",
                    "ControlType"
                },
                {
                    "NativeControlType",
                    "NativeControlType"
                },
                {
                    "IsSimpleComboBoxType",
                    "IsSimpleComboBoxType"
                },
                {
                    "HelpText",
                    "HelpText"
                },
                {
                    "AccessibleDescription",
                    "AccessibleDescription"
                }
            };

        private static Dictionary<string, int> InitializeNotAcceptableTopLevelClassesExact() =>
            new Dictionary<string, int> {
                {
                    "MsoCommandBar",
                    1
                },
                {
                    "MsoCommandBarDock",
                    1
                },
                {
                    "WorkerW",
                    1
                },
                {
                    "MMCChildFrm",
                    1
                },
                {
                    "Shell_TrayWnd",
                    1
                }
            };

        private static List<string> InitializeNotAcceptableTopLevelClassesPartial() =>
            new List<string> {
                "List",
                "Combo"
            };

        private static Dictionary<AccessibleRole, ControlType> InitializeRoleControlTypeMapping() =>
            new Dictionary<AccessibleRole, ControlType> {
                {
                    AccessibleRole.None,
                    ControlType.Custom
                },
                {
                    AccessibleRole.TitleBar,
                    ControlType.TitleBar
                },
                {
                    AccessibleRole.MenuBar,
                    ControlType.MenuBar
                },
                {
                    AccessibleRole.ScrollBar,
                    ControlType.ScrollBar
                },
                {
                    AccessibleRole.Window,
                    ControlType.Window
                },
                {
                    AccessibleRole.Client,
                    ControlType.Client
                },
                {
                    AccessibleRole.MenuPopup,
                    ControlType.Menu
                },
                {
                    AccessibleRole.MenuItem,
                    ControlType.MenuItem
                },
                {
                    AccessibleRole.ToolTip,
                    ControlType.ToolTip
                },
                {
                    AccessibleRole.Document,
                    ControlType.Document
                },
                {
                    AccessibleRole.Pane,
                    ControlType.Pane
                },
                {
                    AccessibleRole.Grouping,
                    ControlType.Group
                },
                {
                    AccessibleRole.Separator,
                    ControlType.Separator
                },
                {
                    AccessibleRole.ToolBar,
                    ControlType.ToolBar
                },
                {
                    AccessibleRole.StatusBar,
                    ControlType.StatusBar
                },
                {
                    AccessibleRole.Table,
                    ControlType.Table
                },
                {
                    AccessibleRole.ColumnHeader,
                    ControlType.ColumnHeader
                },
                {
                    AccessibleRole.RowHeader,
                    ControlType.RowHeader
                },
                {
                    AccessibleRole.Row,
                    ControlType.Row
                },
                {
                    AccessibleRole.Cell,
                    ControlType.Cell
                },
                {
                    AccessibleRole.Link,
                    ControlType.Hyperlink
                },
                {
                    AccessibleRole.List,
                    ControlType.List
                },
                {
                    AccessibleRole.ListItem,
                    ControlType.ListItem
                },
                {
                    AccessibleRole.Outline,
                    ControlType.Tree
                },
                {
                    AccessibleRole.OutlineItem,
                    ControlType.TreeItem
                },
                {
                    AccessibleRole.PageTab,
                    ControlType.TabPage
                },
                {
                    AccessibleRole.Graphic,
                    ControlType.Image
                },
                {
                    AccessibleRole.StaticText,
                    ControlType.Text
                },
                {
                    AccessibleRole.Text,
                    ControlType.Edit
                },
                {
                    AccessibleRole.PushButton,
                    ControlType.Button
                },
                {
                    AccessibleRole.CheckButton,
                    ControlType.CheckBox
                },
                {
                    AccessibleRole.RadioButton,
                    ControlType.RadioButton
                },
                {
                    AccessibleRole.ComboBox,
                    ControlType.ComboBox
                },
                {
                    AccessibleRole.ProgressBar,
                    ControlType.ProgressBar
                },
                {
                    AccessibleRole.Slider,
                    ControlType.Slider
                },
                {
                    AccessibleRole.SpinButton,
                    ControlType.Spinner
                },
                {
                    AccessibleRole.PageTabList,
                    ControlType.TabList
                },
                {
                    AccessibleRole.SplitButton,
                    ControlType.SplitButton
                }
            };

        private static Dictionary<AccessibleRole, string> InitializeRoleConversionTable() =>
            new Dictionary<AccessibleRole, string> {
                {
                    AccessibleRole.None,
                    "unknown"
                },
                {
                    AccessibleRole.TitleBar,
                    "title bar"
                },
                {
                    AccessibleRole.MenuBar,
                    "menu bar"
                },
                {
                    AccessibleRole.ScrollBar,
                    "scroll bar"
                },
                {
                    AccessibleRole.Grip,
                    "grip"
                },
                {
                    AccessibleRole.Sound,
                    "sound"
                },
                {
                    AccessibleRole.Cursor,
                    "cursor"
                },
                {
                    AccessibleRole.Caret,
                    "caret"
                },
                {
                    AccessibleRole.Alert,
                    "alert"
                },
                {
                    AccessibleRole.Window,
                    "window"
                },
                {
                    AccessibleRole.Client,
                    "client"
                },
                {
                    AccessibleRole.MenuPopup,
                    "popup menu"
                },
                {
                    AccessibleRole.MenuItem,
                    "menu item"
                },
                {
                    AccessibleRole.ToolTip,
                    "tool tip"
                },
                {
                    AccessibleRole.Application,
                    "application"
                },
                {
                    AccessibleRole.Document,
                    "document"
                },
                {
                    AccessibleRole.Pane,
                    "pane"
                },
                {
                    AccessibleRole.Chart,
                    "chart"
                },
                {
                    AccessibleRole.Dialog,
                    "dialog"
                },
                {
                    AccessibleRole.Border,
                    "border"
                },
                {
                    AccessibleRole.Grouping,
                    "grouping"
                },
                {
                    AccessibleRole.Separator,
                    "separator"
                },
                {
                    AccessibleRole.ToolBar,
                    "tool bar"
                },
                {
                    AccessibleRole.StatusBar,
                    "status bar"
                },
                {
                    AccessibleRole.Table,
                    "table"
                },
                {
                    AccessibleRole.ColumnHeader,
                    "column header"
                },
                {
                    AccessibleRole.RowHeader,
                    "row header"
                },
                {
                    AccessibleRole.Column,
                    "column"
                },
                {
                    AccessibleRole.Row,
                    "row"
                },
                {
                    AccessibleRole.Cell,
                    "cell"
                },
                {
                    AccessibleRole.Link,
                    "link"
                },
                {
                    AccessibleRole.HelpBalloon,
                    "help balloon"
                },
                {
                    AccessibleRole.Character,
                    "character"
                },
                {
                    AccessibleRole.List,
                    "list"
                },
                {
                    AccessibleRole.ListItem,
                    "list item"
                },
                {
                    AccessibleRole.Outline,
                    "outline"
                },
                {
                    AccessibleRole.OutlineItem,
                    "outline item"
                },
                {
                    AccessibleRole.PageTab,
                    "page tab"
                },
                {
                    AccessibleRole.PropertyPage,
                    "property page"
                },
                {
                    AccessibleRole.Indicator,
                    "indicator"
                },
                {
                    AccessibleRole.Graphic,
                    "graphic"
                },
                {
                    AccessibleRole.StaticText,
                    "text"
                },
                {
                    AccessibleRole.Text,
                    "editable text"
                },
                {
                    AccessibleRole.PushButton,
                    "push button"
                },
                {
                    AccessibleRole.CheckButton,
                    "check box"
                },
                {
                    AccessibleRole.RadioButton,
                    "radio button"
                },
                {
                    AccessibleRole.ComboBox,
                    "combo box"
                },
                {
                    AccessibleRole.DropList,
                    "drop down"
                },
                {
                    AccessibleRole.ProgressBar,
                    "progress bar"
                },
                {
                    AccessibleRole.Dial,
                    "dial"
                },
                {
                    AccessibleRole.HotkeyField,
                    "hot key field"
                },
                {
                    AccessibleRole.Slider,
                    "slider"
                },
                {
                    AccessibleRole.SpinButton,
                    "spin box"
                },
                {
                    AccessibleRole.Diagram,
                    "diagram"
                },
                {
                    AccessibleRole.Animation,
                    "animation"
                },
                {
                    AccessibleRole.Equation,
                    "equation"
                },
                {
                    AccessibleRole.ButtonDropDown,
                    "drop down button"
                },
                {
                    AccessibleRole.ButtonMenu,
                    "menu button"
                },
                {
                    AccessibleRole.ButtonDropDownGrid,
                    "grid drop down button"
                },
                {
                    AccessibleRole.WhiteSpace,
                    "white space"
                },
                {
                    AccessibleRole.PageTabList,
                    "page tab list"
                },
                {
                    AccessibleRole.Clock,
                    "clock"
                },
                {
                    AccessibleRole.SplitButton,
                    "split button"
                },
                {
                    AccessibleRole.IpAddress,
                    "IP address"
                },
                {
                    AccessibleRole.OutlineButton,
                    "outline button"
                }
            };

        private static Dictionary<AccessibleRole, string> InitializeRoleStringMapping() =>
            new Dictionary<AccessibleRole, string> {
                {
                    AccessibleRole.ButtonMenu,
                    "MenuButton"
                },
                {
                    AccessibleRole.OutlineButton,
                    "TreeButton"
                },
                {
                    AccessibleRole.DropList,
                    "DropDown"
                },
                {
                    AccessibleRole.ButtonDropDown,
                    "DropDownButton"
                },
                {
                    AccessibleRole.ButtonDropDownGrid,
                    "DropDownGridButton"
                }
            };

        internal static bool IsDesktopListOrListItem(MsaaElement element)
        {
            if (element == null || !ControlType.List.NameEquals(element.ControlTypeName) && !ControlType.ListItem.NameEquals(element.ControlTypeName))
            {
                return false;
            }
            IntPtr parent = NativeMethods.GetParent(element.WindowHandle);
            return string.Equals(element.ClassName, "SysListView32", StringComparison.Ordinal) && string.Equals("SHELLDLL_DefView", NativeMethods.GetClassName(parent), StringComparison.Ordinal);
        }

        internal static bool IsElementDataGridCheckBox(AccWrapper element) =>
            element?.RoleInt == AccessibleRole.Cell && string.Equals(element.HelpText, "DataGridViewCheckBoxCell(DataGridViewCell)");

        internal static bool IsElementDataGridCheckBox(MsaaElement element)
        {
            if (element == null)
            {
                return false;
            }
            return IsElementDataGridCheckBox(element.AccessibleWrapper);
        }

        internal static bool IsElementUniqueAmongSiblings(ref MsaaElement element, MsaaElement parent, ref bool hasLanguageNeutralID)
        {
            bool flag = true;
            if (string.IsNullOrEmpty(element.Name) && parent != null && parent.IsWin32ForSure && !string.IsNullOrEmpty(element.ClassName))
            {
                if (HasLanguageNeutralID(parent))
                {
                    hasLanguageNeutralID = true;
                    element = parent;
                    return flag;
                }
                IntPtr hwndParent = MsaaNativeMethods.GetParent(element.WindowHandle);
                if (hwndParent != IntPtr.Zero)
                {
                    IntPtr zero = IntPtr.Zero;
                    classNameOfSiblingWindow = element.ClassName;
                    numberOfFoundSiblingWindows = 0;
                    NativeMethods.EnumWindowsProc lpEnumFunc = FindChildWithClass;
                    NativeMethods.EnumChildWindows(hwndParent, lpEnumFunc, ref zero);
                    if (numberOfFoundSiblingWindows == 2)
                    {
                        flag = false;
                    }
                }
            }
            return flag;
        }

        private static bool IsFairWin32Top(IntPtr win32handle, bool isMenu)
        {
            string className = NativeMethods.GetClassName(win32handle);
            string windowText = NativeMethods.GetWindowText(win32handle);
            if (!string.IsNullOrEmpty(className))
            {
                bool flag = IsWindowAcceptableAsTop(className);
                AccWrapper accWrapperFromWindow = null;
                if (flag)
                {
                    try
                    {
                        accWrapperFromWindow = AccWrapper.GetAccWrapperFromWindow(win32handle);
                    }
                    catch (ZappyTaskControlNotAvailableException)
                    {
                    }
                    if (accWrapperFromWindow != null)
                    {
                        if (!string.IsNullOrEmpty(windowText))
                        {
                            if (isMenu || accWrapperFromWindow.RoleInt == AccessibleRole.Window)
                            {
                                return true;
                            }
                            if (!string.IsNullOrEmpty(accWrapperFromWindow.Name))
                            {
                                return true;
                            }
                        }
                        else if (!string.IsNullOrEmpty(accWrapperFromWindow.Name))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool IsFirefoxClassName(IntPtr windowHandle)
        {
            string className = NativeMethods.GetClassName(NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_ROOT));
            return string.Equals("MozillaUIWindowClass", className, StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsIEServerControl(string elementClassName, string elementRole) =>
            "Internet Explorer_Server".Equals(elementClassName, StringComparison.Ordinal) && string.Equals(elementRole, "client", StringComparison.Ordinal);

        internal static bool IsIEServerControlNotInsideIEFrame(MsaaElement element)
        {
            string nativeControlType = element.GetNativeControlType(NativeControlTypeKind.AsString) as string;
            if (!IsIEServerControl(element.ClassName, nativeControlType))
            {
                return false;
            }
            if (element.TopLevelElement != null)
            {
                return !"IEFrame".Equals(element.TopLevelElement.ClassName, StringComparison.Ordinal);
            }
            return true;
        }

        private static bool IsMfcClassName(IntPtr windowHandle)
        {
            string className = NativeMethods.GetClassName(NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_ROOT));
            return !string.IsNullOrEmpty(className) && className.StartsWith("Afx:", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsNumericUpDownControl(ITaskActivityElement uiControl)
        {
            ITaskActivityElement childIfOnlyChild = GetChildIfOnlyChild(uiControl);
            if (childIfOnlyChild != null)
            {
                ITaskActivityElement element = null;
                try
                {
                    element = MsaaZappyPlugin.Navigate(childIfOnlyChild, AccessibleNavigation.Previous);
                }
                catch (ZappyTaskException exception)
                {
                    object[] args = { exception };
                    
                    return false;
                }
                if (element != null)
                {
                    ITaskActivityElement element3 = GetChildIfOnlyChild(element);
                    if (element3 != null && element3.ControlTypeName != null && ControlType.Spinner.NameEquals(element3.ControlTypeName))
                    {
                        ITaskActivityElement element4 = GetChildIfOnlyChild(childIfOnlyChild);
                        if (element4 != null)
                        {
                            ITaskActivityElement element5 = element4;
                            if (element5 != null && element5.ControlTypeName != null && ControlType.Edit.NameEquals(element5.ControlTypeName))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal static bool IsPointInWindow(IntPtr hWnd, NativeMethods.POINT point)
        {
            NativeMethods.RECT rect;
            NativeMethods.GetWindowRect(hWnd, out rect);
            bool flag = true;
            flag = new Rectangle(rect.left - 1, rect.top - 1, rect.right - rect.left + 2, rect.bottom - rect.top + 2).Contains(point.x, point.y);
            if (flag)
            {
                IntPtr hRegion = MsaaNativeMethods.CreateRectRgn(0, 0, 0, 0);
                if (!(hRegion != IntPtr.Zero))
                {
                    return flag;
                }
                if (MsaaNativeMethods.GetWindowRgn(hWnd, hRegion) != 0)
                {
                    flag = MsaaNativeMethods.PtInRegion(hRegion, point.x - rect.left, point.y - rect.top);
                }
                MsaaNativeMethods.DeleteObject(hRegion);
            }
            return flag;
        }

        internal static bool IsProgrammaticallyInvisible(MsaaElement element)
        {
            if (element == null)
            {
                return false;
            }
            try
            {
                AccessibleStates requestedState = element.GetRequestedState(AccessibleStates.Offscreen | AccessibleStates.Invisible);
                return HasState(AccessibleStates.Invisible, requestedState) && !HasState(AccessibleStates.Offscreen, requestedState);
            }
            catch (ZappyTaskControlNotAvailableException)
            {
                return false;
            }
        }

        internal static bool IsRichTextBoxClassName(string className) =>
            !string.IsNullOrEmpty(className) && className.ToUpperInvariant().Contains("RICHEDIT");

        internal static bool IsSimpleComboBox(MsaaElement element, out ITaskActivityElement sourceElement)
        {
            bool flag = false;
            bool flag2 = false;
            sourceElement = null;
            if (element == null || AccessibleRole.ComboBox != element.RoleInt)
            {
                return false;
            }
            AccChildrenEnumerator enumerator = new AccChildrenEnumerator(element.AccessibleWrapper);
            AccWrapper nextChild = enumerator.GetNextChild(true);
            int num = 0;
            while (num < 3 && nextChild != null)
            {
                if (AccessibleRole.Window == nextChild.RoleInt)
                {
                    AccWrapper accessibleWrapper = new AccChildrenEnumerator(nextChild).GetNextChild(true);
                    if (accessibleWrapper != null)
                    {
                        if (AccessibleRole.List == accessibleWrapper.RoleInt)
                        {
                            flag = true;
                        }
                        else if (AccessibleRole.Text == accessibleWrapper.RoleInt)
                        {
                            flag2 = true;
                            sourceElement = new MsaaElement(accessibleWrapper);
                        }
                    }
                }
                nextChild = enumerator.GetNextChild(true);
                num++;
            }
            if (!((num == 2) & flag & flag2))
            {
                return (num == 1) & flag2;
            }
            return true;
        }

        internal static bool IsTopLevelPopUpMenu(MsaaElement element)
        {
            bool flag = false;
            if (element == null || element.RoleInt != AccessibleRole.MenuPopup)
            {
                return flag;
            }
            AccWrapper parentWrapper = element.ParentWrapper;
            if (parentWrapper != null && parentWrapper.WindowHandle != DesktopWindowHandle)
            {
                if (parentWrapper.RoleInt != AccessibleRole.Window)
                {
                    return flag;
                }
                AccWrapper parent = parentWrapper.Parent;
                if (parent != null && !(parent.WindowHandle == DesktopWindowHandle) && !"Shell_TrayWnd".Equals(NativeMethods.GetClassName(parent.WindowHandle), StringComparison.Ordinal))
                {
                    return flag;
                }
            }
            return true;
        }

        private static bool IsWindowAcceptableAsTop(string className)
        {
            bool flag = true;
            if (ZappyTaskUtilities.IsWpfClassName(className))
            {
                return true;
            }
            foreach (string str in notAcceptableTopLevelClassesPartial)
            {
                if (className != null && -1 != className.IndexOf(str, StringComparison.OrdinalIgnoreCase))
                {
                    flag = false;
                    break;
                }
            }
            if (flag && notAcceptableTopLevelClassesExact.ContainsKey(className))
            {
                flag = false;
            }
            return flag;
        }

        internal static bool IsWindowsVistaOrAbove() =>
            Environment.OSVersion.Version.CompareTo(new Version(6, 0, 0, 0)) >= 0;

        internal static void MapAndThrowException(SystemException e, bool throwNotSupported)
        {
            if (!MsaaZappyPlugin.Instance.IsRecording & throwNotSupported && e is COMException && (e as COMException).ErrorCode == -2147352573)
            {
                throw new NotSupportedException();
            }
            if (e is ArgumentException || e is COMException || e is InvalidCastException || e is InvalidOperationException || (e is NotImplementedException || e is NullReferenceException || e is OutOfMemoryException || e is RemotingException) || e is InvalidComObjectException || e is UnauthorizedAccessException)
            {
                
                            }
        }

        internal static string NormalizeQueryPropertyValue(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("'", "'");
            }
            return value;
        }

        internal static void SetCeilingElement(MsaaElement technologyElement, AutomationElement ceilingElement)
        {
            object obj2;
            if (technologyElement != null && ceilingElement != null && ceilingElement.TryGetCurrentPattern(AutomationPattern.LookupById(LegacyIAccessiblePattern.Pattern.Id), out obj2))
            {
                IAccessible iAccessible = (obj2 as LegacyIAccessiblePattern).GetIAccessible();
                int childId = (obj2 as LegacyIAccessiblePattern).Current.ChildId;
                technologyElement.CeilingElement = new AccWrapper(iAccessible, childId);
            }
        }

        internal static IntPtr TopChildWindowFromPoint(IntPtr hwnd, NativeMethods.POINT point)
        {
            bool flag;
            if (hwnd == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            return TopSiblingWindowFromPoint(MsaaNativeMethods.GetWindow(hwnd, MsaaNativeMethods.GWParameter.GW_CHILD), point, out flag);
        }

        internal static ITaskActivityElement TopLevelElement(ITaskActivityElement element)
        {
            TaskActivityElement element2 = element as TaskActivityElement;
            if (element2 != null)
            {
                return element2.TopLevelElement;
            }
            return null;
        }

        internal static IntPtr TopSiblingWindowFromPoint(IntPtr hwnd, NativeMethods.POINT point, out bool isUniqueSibling)
        {
            if (hwnd == IntPtr.Zero)
            {
                isUniqueSibling = false;
                return IntPtr.Zero;
            }
            int num = 0;
            IntPtr zero = IntPtr.Zero;
            for (IntPtr ptr2 = MsaaNativeMethods.GetWindow(hwnd, MsaaNativeMethods.GWParameter.GW_HWNDFIRST); IntPtr.Zero != ptr2; ptr2 = MsaaNativeMethods.GetWindow(ptr2, MsaaNativeMethods.GWParameter.GW_HWNDNEXT))
            {
                if (NativeMethods.IsWindowVisible(ptr2) && IsPointInWindow(ptr2, point))
                {
                    zero = ptr2;
                    if (++num == 2)
                    {
                        break;
                    }
                }
            }
            isUniqueSibling = num == 1;
            return zero;
        }

        internal static bool ValidMenuParent(AccessibleRole role)
        {
            if (role != AccessibleRole.MenuItem && role != AccessibleRole.MenuBar && role != AccessibleRole.StatusBar && role != AccessibleRole.ToolBar)
            {
                return role == AccessibleRole.MenuPopup;
            }
            return true;
        }

        internal static IntPtr WindowFromPoint(NativeMethods.POINT ptScreen)
        {
            bool flag;
            IntPtr hwnd = NativeMethods.WindowFromPoint(ptScreen);
            if (!(NativeMethods.GetDesktopWindow() != NativeMethods.GetAncestor(hwnd, NativeMethods.GetAncestorFlag.GA_PARENT)))
            {
                return hwnd;
            }
            IntPtr lParam = new IntPtr(Util.MAKELONG(ptScreen.x, ptScreen.y));
            IntPtr ptr3 = MsaaNativeMethods.SendMessage(hwnd, 0x84, IntPtr.Zero, lParam);
            if (ptr3 == HTHSCROLL || ptr3 == HTVSCROLL)
            {
                return hwnd;
            }
            for (IntPtr ptr4 = TopChildWindowFromPoint(hwnd, ptScreen); ptr4 != IntPtr.Zero && ptr4 != hwnd; ptr4 = TopChildWindowFromPoint(hwnd, ptScreen))
            {
                hwnd = TopSiblingWindowFromPoint(ptr4, ptScreen, out flag);
            }
            int windowLong = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWLParameter.GWL_STYLE);
            int dwExStyles = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWLParameter.GWL_EXSTYLE);
            if (!CheckStyle(windowLong, dwExStyles))
            {
                return hwnd;
            }
            IntPtr ptr5 = TopSiblingWindowFromPoint(hwnd, ptScreen, out flag);
            if (ptr5 != IntPtr.Zero)
            {
                hwnd = ptr5;
            }
            IntPtr ancestor = NativeMethods.GetAncestor(hwnd, NativeMethods.GetAncestorFlag.GA_PARENT);
            ptr3 = MsaaNativeMethods.SendMessage(ancestor, 0x84, IntPtr.Zero, lParam);
            if (flag && (!(HTCLIENT != ptr3) || !(HTCAPTION != ptr3)))
            {
                return hwnd;
            }
            return ancestor;
        }

        internal static IntPtr DesktopWindowHandle =>
            desktopWindowHandle;
    }
}