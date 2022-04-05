using Accessibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;
using PropertyCondition = Zappy.ActionMap.HelperClasses.PropertyCondition;
using ScrollAmount = Zappy.ActionMap.Enums.ScrollAmount;

namespace Zappy.Decode.Mssa
{
    [Serializable,]
    internal sealed class MsaaElement : TaskActivityElement
    {
        private string accesskey;
        [NonSerialized]
        private AccWrapper accWrapper;
        private IntPtr backupWindowHandle;
        private string className;
        private int? controlId;
        private string controlText;
        private string controlTypeName;
        private bool distinctCheckDone;
        private const string EditableControlName = "Editing Control";
        private string friendlyName;
        private string id;
        private const string IE9AddressBarParentClassName = "Address Band Root";
        private static readonly char[] InvalidXMLCharacters = new char[1];
        private bool isCacheMode;
        private bool? isCeilingElement;
        private bool? isElementDataGridCell;
        private bool isElementNonRootTreeItem;
        private bool? isIE9AddressEditControl;
        private bool? isPassword;
        private bool? isSimpleComboBox;
        private bool? isTopLevelElement;
        private bool? isWin32ForSure;
        private Point midPoint;
        private string name;
        private string nativeControlType;
        private IntPtr nativeWindowHandle;
        private string ownerWindowClassName;
        private IntPtr ownerWindowHandle;
        private string ownerWindowText;
        private MsaaElement parent;
        private int? processId;
        private IQueryElement queryID;
        private string runtimeId;
        private ITaskActivityElement simpleComboBoxSourceElement;
        private IQueryElement singleQueryID;
        private int? supportLevel;
        [NonSerialized]
        private TaskActivityElement topLevelElement;
        private const string UnitializedValue = "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878";

        internal MsaaElement(AccWrapper accessibleWrapper)
        {
            name = "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878";
            friendlyName = "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878";
            id = "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878";
            controlText = "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878";
            if (accessibleWrapper == null)
            {
                throw new ZappyTaskControlNotAvailableException();
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            accWrapper = accessibleWrapper;
            nativeWindowHandle = accWrapper.WindowHandle;
            InitializeConstructorProperties();
            stopwatch.Stop();
            object[] args = { stopwatch.ElapsedMilliseconds, runtimeId };
            
        }

        internal MsaaElement(IntPtr win32Handle)
        {
            name = "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878";
            friendlyName = "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878";
            id = "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878";
            controlText = "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878";
            Stopwatch stopwatch = Stopwatch.StartNew();
            isWin32ForSure = true;
            nativeWindowHandle = win32Handle;
            accWrapper = AccWrapper.GetAccWrapperFromWindow(win32Handle);
            InitializeConstructorProperties();
            stopwatch.Stop();
            object[] args = { stopwatch.ElapsedMilliseconds, runtimeId };
            
        }

        internal MsaaElement(IAccessible accessibleObject, int childId) : this(new AccWrapper(accessibleObject, childId))
        {
        }

        internal MsaaElement(IntPtr windowHandle, IAccessible accessibleObject, int childId) : this(new AccWrapper(windowHandle, accessibleObject, childId))
        {
        }

        private void AddSearchConfigurations()
        {
            List<string> configList = new List<string>();
            if (queryID.SearchConfigurations != null)
            {
                configList.AddRange(queryID.SearchConfigurations);
            }
            if (!distinctCheckDone && queryID.Condition != null)
            {
                distinctCheckDone = true;
                if (queryID.Ancestor != null)
                {
                    if (ExtensionUtilities.IsQueryConditionSubSetOfParent(Framework, queryID.Condition.Conditions, queryID.Ancestor) && !SearchConfiguration.ConfigurationExists(configList, SearchConfiguration.DisambiguateChild))
                    {
                        configList.Add(SearchConfiguration.DisambiguateChild);
                    }
                }
                else if (!IsTopLevelElement && TopLevelElement.Equals(Parent))
                {
                    if (ExtensionUtilities.IsQueryConditionSubSetOfParent(Framework, queryID.Condition.Conditions, TopLevelElement) && !SearchConfiguration.ConfigurationExists(configList, SearchConfiguration.DisambiguateChild))
                    {
                        configList.Add(SearchConfiguration.DisambiguateChild);
                    }
                }
                else
                {
                    distinctCheckDone = false;
                }
            }
            if ((ControlType.MenuItem.NameEquals(ControlTypeName) || ControlType.TreeItem.NameEquals(ControlTypeName) || ControlType.CheckBoxTreeItem.NameEquals(ControlTypeName) || ControlType.Menu.NameEquals(ControlTypeName)) && !SearchConfiguration.ConfigurationExists(configList, SearchConfiguration.ExpandWhileSearching))
            {
                configList.Add(SearchConfiguration.ExpandWhileSearching);
            }
            if (ControlType.Row.NameEquals(ControlTypeName) && !SearchConfiguration.ConfigurationExists(configList, SearchConfiguration.AlwaysSearch))
            {
                configList.Add(SearchConfiguration.AlwaysSearch);
            }
            if (configList.Count > 0)
            {
                queryID.SearchConfigurations = configList.ToArray();
            }
            else
            {
                queryID.SearchConfigurations = null;
            }
        }

        public override void CacheProperties()
        {
            isCacheMode = true;
            object topLevelElement = TopLevelElement;
            topLevelElement = Parent;
            topLevelElement = QueryId;
            topLevelElement = ControlId;
            topLevelElement = Name;
            topLevelElement = IsPassword;
            topLevelElement = FriendlyName;
            topLevelElement = IsTopLevelElement;
            if (IsTopLevelElement)
            {
                OwnerWindowHandle = NativeMethods.GetParent(WindowHandle);
                OwnerWindowClassName = NativeMethods.GetClassName(OwnerWindowHandle);
                OwnerWindowText = NativeMethods.GetWindowText(OwnerWindowHandle);
            }
        }

        private bool CheckIfParentPartOfTreeView()
        {
            if (AccessibleWrapper.Parent != null)
            {
                AccessibleRole roleInt = AccessibleWrapper.Parent.RoleInt;
                if (roleInt == AccessibleRole.Outline)
                {
                    
                    return true;
                }
                AccessibleStates state = AccessibleWrapper.Parent.State;
                if ((roleInt == AccessibleRole.CheckButton || roleInt == AccessibleRole.OutlineItem) && (state & AccessibleStates.Expanded) != AccessibleStates.None || (state & AccessibleStates.Collapsed) != AccessibleStates.None)
                {
                    
                    return true;
                }
            }
            return false;
        }


        private bool CheckIfTreeItem()
        {
            int result = 0;
            try
            {
                if ((RoleInt == AccessibleRole.OutlineItem || RoleInt == AccessibleRole.CheckButton) && (int.TryParse(Value, out result) && result >= 0) && CheckIfParentPartOfTreeView() && RoleInt == AccessibleRole.CheckButton)
                {
                    controlTypeName = ControlType.CheckBoxTreeItem.Name;
                }
            }
            catch (ZappyTaskException)
            {
            }
            catch (NotSupportedException)
            {
            }
            return result > 0;
        }

        public override void EnsureVisibleByScrolling(int pointX, int pointY, ref int outPointX, ref int outPointY)
        {
            throw new NotSupportedException();
        }



        public override bool Equals(ITaskActivityElement element)
        {
            if (this == element)
            {
                return true;
            }
            MsaaElement element2 = element as MsaaElement;
            if (element2 == null)
            {
                return false;
            }
            if (IsWin32ForSure && element2.IsWin32ForSure)
            {
                return WindowHandle == element2.WindowHandle;
            }
            bool flag = string.Equals(runtimeId, element2.runtimeId, StringComparison.Ordinal) && Equals(SwitchingElement, element2.SwitchingElement);
            if (flag)
            {
                if (IsIE9AddressEditControl)
                {
                    return true;
                }
                flag = AccessibleWrapper.Equals(element2.AccessibleWrapper);
                object[] args = { this, element2 };
                
            }
            return flag;
        }

        public override bool Equals(object obj)
        {
            ITaskActivityElement element = obj as ITaskActivityElement;
            return Equals(element);
        }

        internal bool EqualsIgnoreContainer(ITaskActivityElement element)
        {
            if (this == element)
            {
                return true;
            }
            MsaaElement element2 = element as MsaaElement;
            if (element2 == null)
            {
                return false;
            }
            if (runtimeId != element2.runtimeId)
            {
                return false;
            }
            return IsIE9AddressEditControl || AccessibleWrapper.Equals(element2.AccessibleWrapper);
        }

        private void GenerateInstanceForWindowElement(MsaaElement currentMsaaElement, MsaaElement currentAncestor)
        {
            IntPtr windowHandle;
            if (currentAncestor == null)
            {
                if (SwitchingElement != null)
                {
                    windowHandle = SwitchingElement.WindowHandle;
                }
                else
                {
                    windowHandle = TopLevelElement.WindowHandle;
                }
            }
            else
            {
                windowHandle = currentAncestor.WindowHandle;
            }
            int num = new WindowInstanceGetter().GetInstance(windowHandle, currentMsaaElement.accWrapper, currentMsaaElement.singleQueryID.Condition.Conditions, MsaaUtility.HasSearchConfiguration(currentMsaaElement.singleQueryID.SearchConfigurations, SearchConfiguration.VisibleOnly));
            if (num > 1)
            {
                currentMsaaElement.singleQueryID.Condition.Conditions = new List<IQueryCondition>(currentMsaaElement.singleQueryID.Condition.Conditions) { new PropertyCondition("Instance", num) }.ToArray();
            }
        }

        private string GetAccessKey()
        {
            try
            {
                return AccessibleWrapper.KeyboardShortcut;
            }
            catch (NotSupportedException)
            {
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
            return null;
        }

        public override void GetBoundingRectangle(out int left, out int top, out int width, out int height)
        {
            int num;
            Rectangle invalidRectangle = MsaaUtility.InvalidRectangle;
            height = num = -1;
            width = num;
            left = top = num;
            try
            {
                invalidRectangle = accWrapper.GetBoundingRectangle();
            }
            catch (ZappyTaskException)
            {
                if (!TryRefreshAccWrapper())
                {
                    throw;
                }
                invalidRectangle = accWrapper.GetBoundingRectangle();
            }
            left = invalidRectangle.Left;
            top = invalidRectangle.Top;
            width = invalidRectangle.Width;
            height = invalidRectangle.Height;
            midPoint.X = left + width / 2;
            midPoint.Y = top + height / 2;
                    }

        public override void GetClickablePoint(out int pointX, out int pointY)
        {
            int num;
            int num2;
            int num3;
            int num4;
            GetBoundingRectangle(out num, out num2, out num3, out num4);
            pointX = num + num3 / 2;
            pointY = num2 + num4 / 2;
        }

        public override int GetHashCode() =>
            runtimeId.GetHashCode();

        public override object GetNativeControlType(NativeControlTypeKind nativeControlTypeKind)
        {
            if (nativeControlTypeKind == NativeControlTypeKind.AsString)
            {
                return nativeControlType;
            }
            if (nativeControlTypeKind == NativeControlTypeKind.AsInteger)
            {
                return RoleInt;
            }
            return null;
        }

        public override object GetPropertyValue(string propertyName)
        {
            propertyName = MsaaUtility.GetPropertyNameInCorrectCase(propertyName);
            switch (propertyName)
            {
                case "Name":
                    {
                        return Name;
                    }

                case "_isWin32ForSure":
                    {
                        return IsWin32ForSure;
                    }

                case "HelpText":
                    {
                        return AccessibleWrapper.HelpText;
                    }

                case "OwnerWindowHandle":
                    {
                        return OwnerWindowHandle;
                    }

                case "ControlId":
                    {
                        return ControlId;
                    }

                case "ClassName":
                    {
                        return ClassName;
                    }

                case "AccessibleName":
                    {
                        if (!IsWin32ForSure)
                        {
                            throw new NotSupportedException();
                        }
                        return AccessibleWrapper.Name;
                    }

                case "ControlType":
                    {
                        return ControlTypeName;
                    }

                case "AccessibleDescription":
                    {
                        return AccessibleWrapper.AccessibleDescription;
                    }

                case "CachedAccessKey":
                    {
                        return accesskey;
                    }

                case "_isNumericUpDownControl":
                    {
                        return MsaaUtility.IsNumericUpDownControl(this);
                    }

                case "_isManagedMsaaElement":
                    {
                        return true;
                    }

                case "AccessKey":
                    {
                        return AccessibleWrapper.KeyboardShortcut;
                    }

                case "Role":
                    {
                        return RoleInt;
                    }

                case "OwnerWindowText":
                    {
                        return OwnerWindowText;
                    }

                case "Value":
                    {
                        return Value;
                    }

                case "ControlName":
                    {
                        return Id;
                    }

                case "OwnerWindowClassName":
                    {
                        return OwnerWindowClassName;
                    }

                case "IsSimpleComboBoxType":
                    {
                        return IsSimpleComboBox;
                    }

                case "_LightWeightInstance":
                    {
                        return MsaaUtility.GetLightWeightInstance(AccessibleWrapper);
                    }

                case "NativeControlType":
                    {
                        return nativeControlType;
                    }
            }
            throw new NotSupportedException();
        }

        public override string GetQueryIdForRelatedElement(ZappyTaskElementKind relatedElement, object additionalInfo, out int maxDepth)
        {
            if (relatedElement != ZappyTaskElementKind.Child)
            {
                throw new NotSupportedException();
            }
            if (additionalInfo is string)
            {
                maxDepth = -1;
                return GetQueryIdOfChild(additionalInfo as string);
            }
            CrapyLogger.log.ErrorFormat("MSAA::GetQueryIdForRelatedElement unexpected type for additionalInfo. Expected type is string.");
            throw new ArgumentException("MSAA::GetQueryIdForRelatedElement unexpected type for additionalInfo. Expected type is string.");
        }

        private string GetQueryIdOfChild(string childname)
        {
            string str = PropertyCondition.Escape(childname);
            object[] args = { str };
            return string.Format(CultureInfo.InvariantCulture, ";[MSAA]Name='{0}'", args);
        }

        public override AccessibleStates GetRequestedState(AccessibleStates requestedState)
        {
            AccessibleStates unavailable = AccessibleStates.Unavailable;
            if (NativeMethods.IsWindow(WindowHandle))
            {
                unavailable = accWrapper.State;
            }
            return unavailable;
        }

        public override bool GetRightToLeftProperty(RightToLeftKind rightToLeftKind)
        {
            int windowLong = NativeMethods.GetWindowLong(WindowHandle, NativeMethods.GWLParameter.GWL_EXSTYLE);
            return rightToLeftKind == RightToLeftKind.Text && (0x2000 & windowLong) != 0 || RightToLeftKind.Layout == rightToLeftKind && (0x400000 & windowLong) != 0;
        }

        private string GetRuntimeId()
        {
            if (WindowHandle == MsaaUtility.DesktopWindowHandle)
            {
                object[] objArray1 = { WindowHandle, "_", nativeControlType };
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", objArray1);
            }
            if (RoleInt == AccessibleRole.Window && WindowHandle != IntPtr.Zero || MsaaUtility.IsIEServerControl(ClassName, nativeControlType))
            {
                return WindowHandle.ToString();
            }
            if (RoleInt == AccessibleRole.MenuItem || RoleInt == AccessibleRole.MenuPopup)
            {
                object[] objArray2 = { BackupWindowHandle == IntPtr.Zero ? WindowHandle : BackupWindowHandle, "_", ChildId, "_", GetSingleQueryIDString() };
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", objArray2);
            }
            object[] args = { WindowHandle, "_", ChildId, "_", GetSingleQueryIDString() };
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", args);
        }

        public override int GetScrolledPercentage(ScrollDirection scrollDirection, ITaskActivityElement scrollElement)
        {
            throw new NotSupportedException();
        }

        private int GetSelectionFlag(ProgrammaticActionOption programmaticActionOption)
        {
            if (programmaticActionOption == ProgrammaticActionOption.TakeSelection)
            {
                return 2;
            }
            if (programmaticActionOption == ProgrammaticActionOption.RemoveSelection)
            {
                return 0x10;
            }
            if (programmaticActionOption != ProgrammaticActionOption.TakeFocus)
            {
                throw new NotSupportedException();
            }
            return 1;
        }


        private string GetSingleQueryIDString()
        {
            StringBuilder builder = new StringBuilder(MsaaUtility.QueryFramework);
            if (!ControlType.MenuItem.NameEquals(ControlTypeName))
            {
                string id = Id;
                if (!string.IsNullOrEmpty(id))
                {
                    object[] args = { "ControlName", id };
                    builder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}'", args);
                    builder.Append(" && ");
                }
            }
            if (RoleInt == AccessibleRole.Cell && !MonthCalendarUtilities.IsMonthCalendarClassName(ClassName) || RoleInt == AccessibleRole.Row)
            {
                string a = null;
                try
                {
                    a = Value;
                }
                catch (ZappyTaskException)
                {
                }
                if (string.Equals(a, string.Empty, StringComparison.Ordinal) && RoleInt == AccessibleRole.Cell && Parent != null && Parent.Parent != null && Parent.Parent.RoleInt == AccessibleRole.Table && !string.Equals(Parent.Parent.Name, "DataGridView", StringComparison.OrdinalIgnoreCase))
                {
                    AccWrapper wrapper = Parent.AccessibleWrapper.Navigate(AccessibleNavigation.Next);
                    if (wrapper != null && string.Equals(wrapper.Name, "(Create New)", StringComparison.OrdinalIgnoreCase))
                    {
                        a = null;
                    }
                }
                if (a != null)
                {
                    object[] objArray2 = { "Value", a };
                    builder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}'", objArray2);
                    builder.Append(" && ");
                }
            }
            if (RoleInt == AccessibleRole.RowHeader)
            {
                string str4 = null;
                AccWrapper parentWrapper = ParentWrapper;
                if (parentWrapper != null && parentWrapper.RoleInt == AccessibleRole.Row)
                {
                    try
                    {
                        str4 = parentWrapper.Value;
                    }
                    catch (ZappyTaskException)
                    {
                    }
                    if (str4 != null)
                    {
                        object[] objArray3 = { "Value", str4 };
                        builder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}'", objArray3);
                        builder.Append(" && ");
                    }
                }
            }
            int controlId = ControlId;
            if (controlId > 0 && controlId != WindowHandle.ToInt32())
            {
                object[] objArray4 = { "ControlId", controlId };
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}'", objArray4);
                builder.Append(" && ");
            }
            else
            {
                string name = Name;
                bool flag = false;
                bool flag2 = false;
                if (!string.IsNullOrEmpty(name) && MsaaUtility.CanUseNameProperty(this))
                {
                    flag = true;
                    name = MsaaUtility.NormalizeQueryPropertyValue(name);
                    object[] objArray5 = { "Name", name };
                    builder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}'", objArray5);
                    builder.Append(" && ");
                }
                string controlTypeName = ControlTypeName;
                if (!string.IsNullOrEmpty(controlTypeName))
                {
                    flag2 = true;
                    object[] objArray6 = { "ControlType", controlTypeName };
                    builder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}'", objArray6);
                    builder.Append(" && ");
                }
                if (!flag || !flag2)
                {
                    string className = ClassName;
                    if (!string.IsNullOrEmpty(className))
                    {
                        className = ZappyTaskUtilities.NormalizeDynamicClassName(className);
                        object[] objArray7 = { "ClassName", className };
                        builder.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}'", objArray7);
                        builder.Append(" && ");
                    }
                }
            }
            string str = builder.ToString();
            if (str.EndsWith(" && ", StringComparison.OrdinalIgnoreCase))
            {
                str = str.Substring(0, str.Length - " && ".Length);
            }
            return str;
        }

        private static MsaaElement GetTreeParentByValue(MsaaElement element, out int instance)
        {
            int num;
            instance = 1;
            if (int.TryParse(element.Value, out num))
            {
                AccWrapper accessibleWrapper = element.AccessibleWrapper;
                AccWrapper wrapper2 = accessibleWrapper;
                int num2 = num;
                while (num2 != num - 1)
                {
                    int num3;
                    accessibleWrapper = accessibleWrapper.Navigate(AccessibleNavigation.Previous);
                    if (accessibleWrapper == null || accessibleWrapper.Equals(wrapper2))
                    {
                        return null;
                    }
                    if (int.TryParse(accessibleWrapper.Value, out num3))
                    {
                        num2 = num3;
                    }
                    if (num2 == num)
                    {
                        instance++;
                    }
                    wrapper2 = accessibleWrapper;
                }
                if (!string.IsNullOrEmpty(element.Name))
                {
                    instance = 1;
                }
                element = new MsaaElement(accessibleWrapper);
            }
            return element;
        }

        private void InitializeConstructorProperties()
        {
            SetOption(UITechnologyElementOption.WaitForReadyOptions, WaitForReadyOptions.EnablePlaybackWaitForReady);
            if (RoleInt == AccessibleRole.MenuItem || RoleInt == AccessibleRole.MenuPopup)
            {
                backupWindowHandle = WindowHandleGetter.GetWindowHandleUsingPoint(AccessibleWrapper);
            }
            className = NativeMethods.GetClassName(WindowHandle);
            nativeControlType = MsaaUtility.GetRoleText(RoleInt);
            controlTypeName = MsaaUtility.GetControlTypeName(RoleInt);
            if (RoleInt == AccessibleRole.MenuItem)
            {
                accesskey = GetAccessKey();
            }
            UpdateControlTypeForDateTimePicker();
            UpdateControlTypeForMonthCalendar();
            runtimeId = GetRuntimeId();
            isElementNonRootTreeItem = CheckIfTreeItem();
            midPoint.X = -1;
            midPoint.Y = -1;
        }

        public override bool InitializeProgrammaticScroll()
        {
            throw new NotSupportedException();
        }


        private void InitializeQueryId()
        {
            this.parent = null;
            bool shouldUseValueInCell = false;
            int instance = 1;
            bool shouldUseValueInRow = false;
            IsElementDataGridCell = QueryIdHelper.UseValueInDataGridCellAndRow(this, ref instance, ref shouldUseValueInCell, ref shouldUseValueInRow);
            IQueryElement element = SingleQueryID(false, shouldUseValueInCell || IsElementTreeItem(), instance);
            MsaaElement nextToElement = null;
            MsaaElement ancestor = null;
            int elementInstance = 1;
            string name = Name;
            MsaaElement element4 = QueryIdHelper.UpdateQueryIDForNamelessControls(this, out nextToElement, ref elementInstance, shouldUseValueInCell || isElementNonRootTreeItem, MsaaUtility.HasSearchConfiguration(element.SearchConfigurations, SearchConfiguration.VisibleOnly));
            MsaaElement element5 = element4;
            if (elementInstance > 1)
            {
                element.Condition.Conditions = new List<IQueryCondition>(element.Condition.Conditions) { new PropertyCondition("Instance", elementInstance) }.ToArray();
            }
            MsaaElement parent = null;
            bool useLanguageNeutralId = false;
            bool flag4 = RoleInt == AccessibleRole.MenuItem;
            bool flag5 = false;
            while (element4 != null && !element4.IsBoundayForHostedControl)
            {
                parent = element4.Parent;
                if (element4.RoleInt > AccessibleRole.None)
                {
                    if (flag4)
                    {
                        if (element4.RoleInt == AccessibleRole.Window)
                        {
                            goto Label_02D4;
                        }
                        if (MsaaUtility.ValidMenuParent(element4.RoleInt))
                        {
                            if (element4.RoleInt == AccessibleRole.MenuPopup)
                            {
                                if (!MsaaUtility.IsTopLevelPopUpMenu(element4))
                                {
                                    goto Label_02D4;
                                }
                                ancestor = element4;
                                break;
                            }
                            if (element.Ancestor == null)
                            {
                                element.Ancestor = element4;
                            }
                            else
                            {
                                ancestor.QueryId.Ancestor = element4;
                            }
                            ancestor = element4;
                            ancestor.QueryIdInternal = ancestor.SingleQueryID(false, false);
                            goto Label_02D4;
                        }
                        if (MsaaUtility.ElementBreaksMenuTree(element4))
                        {
                            
                            ancestor = null;
                        }
                        else
                        {
                            ancestor = element4;
                        }
                    }
                    else if (isElementNonRootTreeItem)
                    {
                        if (element5 != null && ControlType.NameComparer.Equals(ControlTypeName, element5.ControlTypeName))
                        {
                            ancestor = Parent;
                        }
                        else
                        {
                            MsaaElement element8 = this;
                            flag5 = true;
                            ancestor = GetTreeParentByValue(element8, out elementInstance);
                        }
                    }
                    else if (element4.WindowHandle != MsaaUtility.DesktopWindowHandle && !element4.IsBoundayForHostedControl && !element4.IsTopLevelElement)
                    {
                        bool flag6 = false;
                        useLanguageNeutralId = MsaaUtility.HasLanguageNeutralID(element4);
                        if (!useLanguageNeutralId)
                        {
                            if (element4 == element5 && element4.IsWin32ForSure)
                            {
                                if (!string.IsNullOrEmpty(Name))
                                {
                                    goto Label_02D4;
                                }
                                ancestor = element4;
                                ancestor.QueryIdInternal = ancestor.SingleQueryID(useLanguageNeutralId, shouldUseValueInRow);
                                break;
                            }
                            if (!MsaaUtility.IsElementUniqueAmongSiblings(ref element4, parent, ref useLanguageNeutralId))
                            {
                                flag6 = true;
                            }
                        }
                        if (!flag6 && !element4.IsTopLevelElement)
                        {
                            ancestor = element4;
                            ancestor.QueryIdInternal = ancestor.SingleQueryID(useLanguageNeutralId, shouldUseValueInRow);
                        }
                        if (IsElementDataGridCell || string.Equals("Editing Control", Name, StringComparison.Ordinal) && RoleInt != AccessibleRole.StaticText || RoleInt == AccessibleRole.RowHeader || RoleInt == AccessibleRole.ColumnHeader)
                        {
                            SetThirdLevelInQID(ancestor, parent, true, true);
                        }
                        else if (RoleInt == AccessibleRole.Row)
                        {
                            SetThirdLevelInQID(ancestor, parent, false, false);
                        }
                        ancestor = element4;
                    }
                    break;
                }
            Label_02D4:
                element4 = parent;
            }
            if (ancestor != null && !ancestor.IsWin32ForSure && string.IsNullOrEmpty(ancestor.Name) && ancestor.Parent != null && (ancestor.Parent.IsWin32ForSure && !ancestor.Parent.IsBoundayForHostedControl) && !ancestor.Parent.Equals(TopLevelElement) && !flag4)
            {
                SetThirdLevelInQID(ancestor, ancestor.Parent, false, false);
            }
            if (ancestor != null)
            {
                MsaaElement mdiWindow = null;
                MsaaElement firstWindow = null;
                ITaskActivityElement currentTopMostElement = ancestor;
                while (currentTopMostElement.QueryId != null && currentTopMostElement.QueryId.Ancestor != null)
                {
                    currentTopMostElement = currentTopMostElement.QueryId.Ancestor;
                }
                if (QueryIdHelper.GetWindowControlsInQueryID(this, currentTopMostElement, out mdiWindow, out firstWindow) > 5 && firstWindow != null)
                {
                    MsaaElement element12 = firstWindow;
                    element12.QueryIdInternal = firstWindow.SingleQueryID(MsaaUtility.HasLanguageNeutralID(firstWindow), false);
                    currentTopMostElement.QueryId.Ancestor = element12;
                    currentTopMostElement = element12;
                }
                if (mdiWindow != null)
                {
                    MsaaElement element13 = mdiWindow;
                    element13.QueryIdInternal = mdiWindow.SingleQueryID(MsaaUtility.HasLanguageNeutralID(mdiWindow), false);
                    currentTopMostElement.QueryId.Ancestor = element13;
                }
            }
            for (MsaaElement element7 = this; element7 != null && !element7.IsCeilingElement; element7 = element4)
            {
                if (element7.Equals(this))
                {
                    if (nextToElement != null)
                    {
                        element7 = nextToElement;
                    }
                    element4 = ancestor;
                }
                else
                {
                    if (element7.QueryId == null)
                    {
                        break;
                    }
                    element4 = element7.QueryId.Ancestor as MsaaElement;
                }
                if (element7.IsWin32ForSure)
                {
                    GenerateInstanceForWindowElement(element7, element4);
                }
            }
            List<string> list = null;
            if (element.SearchConfigurations != null && element.SearchConfigurations.Length != 0)
            {
                list = new List<string>(element.SearchConfigurations);
            }
            else
            {
                list = new List<string>();
            }
            if (nextToElement != null)
            {
                nextToElement.QueryId.Ancestor = ancestor;
                list.Add(SearchConfiguration.NextSibling);
                element.Ancestor = nextToElement;
            }
            else if (element.Ancestor == null && !MsaaUtility.IsTopLevelPopUpMenu(this))
            {
                if (!isElementNonRootTreeItem || !flag5)
                {
                    element.Ancestor = ancestor;
                }
                else if (ancestor != null)
                {
                    list.Add(SearchConfiguration.NextSibling);
                    element.Ancestor = ancestor;
                }
                if (isElementNonRootTreeItem && elementInstance >= 1 && string.IsNullOrEmpty(Name))
                {
                    element.Condition.Conditions = new List<IQueryCondition>(element.Condition.Conditions) { new PropertyCondition("Instance", elementInstance) }.ToArray();
                }
            }
            this.parent = null;
            queryID = element;
            if (list != null && list.Count > 0)
            {
                queryID.SearchConfigurations = list.ToArray();
            }
            else
            {
                queryID.SearchConfigurations = null;
            }
            if (!IsElementDataGridCell && !flag4 && !isElementNonRootTreeItem && !string.IsNullOrEmpty(Name) && !IsWin32ForSure && element5 != null && element5.Equals(ancestor))
            {
                bool useValueInInstanceGeneration = singleQueryID.Condition.GetPropertyValue("Value") != null;
                elementInstance = QueryIdHelper.GenerateInstanceForChild(this, ancestor, useValueInInstanceGeneration, MsaaUtility.HasSearchConfiguration(queryID.SearchConfigurations, SearchConfiguration.VisibleOnly));
                if (elementInstance > 1)
                {
                    queryID.Condition.Conditions = new List<IQueryCondition>(queryID.Condition.Conditions) { new PropertyCondition("Instance", elementInstance) }.ToArray();
                }
            }
        }

        public override void InvokeProgrammaticAction(ProgrammaticActionOption programaticOption)
        {
            if (programaticOption - 1 > ProgrammaticActionOption.TakeFocus && programaticOption != ProgrammaticActionOption.RemoveSelection)
            {
                if (programaticOption != ProgrammaticActionOption.DefaultAction)
                {
                    throw new NotSupportedException();
                }
                AccessibleWrapper.DoDefaultAction();
            }
            else
            {
                int selectionFlag = GetSelectionFlag(programaticOption);
                AccessibleWrapper.Select(selectionFlag);
            }
        }

        private bool IsBoundaryOrIE()
        {
            if (!IsBoundayForHostedControl && (Parent == null || !Parent.IsBoundayForHostedControl) && !MsaaUtility.IsIEServerControl(ClassName, nativeControlType) && !ZappyTaskUtilities.IsWpfClassName(ClassName))
            {
                return ZappyTaskUtilities.IsSilverlightClassName(ClassName);
            }
            return true;
        }

        private bool IsElementTreeItem()
        {
            if (!ControlType.TreeItem.NameEquals(ControlTypeName))
            {
                return ControlType.CheckBoxTreeItem.NameEquals(ControlTypeName);
            }
            return true;
        }

        public override void ScrollProgrammatically(ScrollDirection srollDirection, ScrollAmount scrollAmount)
        {
            throw new NotSupportedException();
        }

        public override void SetFocus()
        {
        }

        private static void SetThirdLevelInQID(MsaaElement ancestor, MsaaElement ancestorParent, bool skipWindow, bool generateWin32Parent)
        {
            if (ancestor != null && ancestor.QueryIdInternal != null)
            {
                if ((ancestorParent == ancestor.Parent && ancestorParent != null && ancestorParent.RoleInt == AccessibleRole.Window) & skipWindow && ancestorParent.Parent != null && !ancestorParent.Parent.IsBoundayForHostedControl)
                {
                    ancestorParent = ancestorParent.Parent;
                }
                if (ancestorParent != null)
                {
                    MsaaElement element = ancestorParent;
                    element.QueryIdInternal = ancestorParent.SingleQueryID(MsaaUtility.HasLanguageNeutralID(ancestorParent), false);
                    if (generateWin32Parent && ancestorParent != null && ancestorParent.Parent != null && ancestorParent.Parent.IsWin32ForSure && !ancestorParent.Parent.IsBoundayForHostedControl)
                    {
                        MsaaElement parent = ancestorParent.Parent;
                        parent.QueryIdInternal = ancestorParent.Parent.SingleQueryID(MsaaUtility.HasLanguageNeutralID(parent), false);
                        element.QueryId.Ancestor = parent;
                    }
                    if (!element.IsBoundayForHostedControl)
                    {
                        ancestor.QueryId.Ancestor = element;
                    }
                }
            }
        }

        private bool ShouldRefresh<T>(T? propertyValue) where T : struct
        {
            if (isCacheMode)
            {
                return !propertyValue.HasValue;
            }
            return true;
        }

        private bool ShouldRefresh(string propertyValue)
        {
            if (isCacheMode)
            {
                return string.Equals(propertyValue, "UnitializedBB839B89-49D2-4923-9F10-3C00A9902878", StringComparison.Ordinal);
            }
            return true;
        }

        internal IQueryElement SingleQueryID(bool useLanguageNeutralId, bool useValue) =>
            SingleQueryID(useLanguageNeutralId, useValue, 1);

        internal IQueryElement SingleQueryID(bool useLanguageNeutralId, bool useValue, int instance)
        {
            bool generateAccessibleName = false;
            if (singleQueryID == null)
            {
                QueryElementProperty none = QueryElementProperty.None;
                if (!ControlType.Custom.NameEquals(ControlTypeName))
                {
                    none |= QueryElementProperty.ControlTypeName;
                }
                if ((IsWin32ForSure || IsDesktop) && !useLanguageNeutralId && !MsaaUtility.IsIEServerControl(ClassName, nativeControlType))
                {
                    none |= QueryElementProperty.ClassName | QueryElementProperty.Name;
                    generateAccessibleName = true;
                }
                else if (RoleInt == AccessibleRole.MenuPopup)
                {
                    none |= QueryElementProperty.Name;
                }
                else if (useValue)
                {
                    none |= QueryElementProperty.Value;
                    if (IsElementTreeItem())
                    {
                        none |= QueryElementProperty.Name;
                    }
                }
                else
                {
                    if (IsWin32ForSure)
                    {
                        none |= QueryElementProperty.ClassName;
                    }
                    if (useLanguageNeutralId)
                    {
                        none |= QueryElementProperty.ControlId | QueryElementProperty.ControlName;
                    }
                    if (MsaaUtility.CanUseNameProperty(this) && !MsaaUtility.IsIEServerControl(ClassName, nativeControlType))
                    {
                        none |= QueryElementProperty.Name;
                    }
                }
                if (IsTopLevelElement && RoleInt != AccessibleRole.MenuPopup && !IsDesktop)
                {
                    none |= QueryElementProperty.OrderOfInvocation;
                }
                singleQueryID = MsaaUtility.GenerateSingleQueryElement(this, none, instance, generateAccessibleName);
                if (SwitchingElement == null && !ControlType.ListItem.NameEquals(ControlTypeName) && !ControlType.TreeItem.NameEquals(ControlTypeName) && !ControlType.CheckBoxTreeItem.NameEquals(ControlTypeName) && !ControlType.MenuItem.NameEquals(ControlTypeName) && !MsaaUtility.IsProgrammaticallyInvisible(this) && (!IsWin32ForSure || NativeMethods.IsWindowVisible(WindowHandle)) && !MsaaUtility.IsIEServerControlNotInsideIEFrame(this))
                {
                    singleQueryID.SearchConfigurations = new[] { SearchConfiguration.VisibleOnly };
                }
            }
            return singleQueryID;
        }

        public override string ToString()
        {
            object[] args = { name, controlTypeName, nativeControlType, className, runtimeId };
            return string.Format(CultureInfo.InvariantCulture, "Name [{0}], ControlType [{1}], NativeControlType [{2}], ClassName [{3}], RuntimeId [{4}]", args);
        }

        private bool TryRefreshAccWrapper()
        {
            bool flag = false;
            if (midPoint.X != -1 && midPoint.Y != -1)
            {
                MsaaElement elementFromPoint = MsaaZappyPlugin.Instance.GetElementFromPoint(midPoint.X, midPoint.Y) as MsaaElement;
                if (elementFromPoint != null && elementFromPoint.Equals(this))
                {
                    accWrapper = elementFromPoint.AccessibleWrapper;
                    flag = true;
                }
            }
            return flag;
        }

        private void UpdateControlTypeForDateTimePicker()
        {
            if (RoleInt == AccessibleRole.DropList && DateTimePickerUtilities.IsDateTimePickerClassName(ClassName))
            {
                controlTypeName = ControlType.DateTimePicker.Name;
            }
        }

        private void UpdateControlTypeForMonthCalendar()
        {
            if (RoleInt == AccessibleRole.Client && MonthCalendarUtilities.IsMonthCalendarClassName(ClassName))
            {
                controlTypeName = ControlType.Calendar.Name;
            }
        }

        public override void WaitForReady()
        {
        }

        internal IAccessible AccessibleObject =>
            accWrapper.AccessibleObject;

        internal AccWrapper AccessibleWrapper =>
            accWrapper;

        public override AutomationElement AutomationElement
        {
            get
            {
                AutomationElement element = null;
                try
                {
                    element = AutomationElement.FromIAccessible(AccessibleObject, ChildId);
                    if (!ControlType.Window.NameEquals(ControlTypeName) || element.Current.NativeWindowHandle == (int)WindowHandle)
                    {
                        return element;
                    }
                    IAccessible acc = NativeMethods.AccessibleClientObjectFromWindow(WindowHandle);
                    if (acc != null)
                    {
                        element = AutomationElement.FromIAccessible(acc, 0);
                    }
                    element.IsMSAAWindow = true;
                }
                catch (Exception)
                {
                }
                return element;
            }
        }

        public override AutomationElement AutomationElementForScreenshot
        {
            get
            {
                try
                {
                    return AutomationElement.FromIAccessible(AccessibleObject, ChildId);
                }
                catch
                {
                    return AutomationElement.FromHandle(WindowHandle);
                }
            }
        }

        internal IntPtr BackupWindowHandle =>
            backupWindowHandle;

        internal AccWrapper CeilingElement { get; set; }

        internal int ChildId =>
            accWrapper.ChildId;

        public override int ChildIndex
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override string ClassName =>
            className;

        internal int ControlId
        {
            get
            {
                if (ShouldRefresh(controlId))
                {
                    controlId = 0;
                    if (RoleInt == AccessibleRole.Window && !IsTopLevelElement)
                    {
                        controlId = MsaaUtility.GetControlID(WindowHandle);
                    }
                }
                return controlId.Value;
            }
        }

        internal string ControlText
        {
            get
            {
                if (ShouldRefresh(controlText))
                {
                    controlText = NativeMethods.GetWindowText(WindowHandle);
                }
                return controlText;
            }
        }

        public override string ControlTypeName =>
            controlTypeName;

        public override string FriendlyName
        {
            get
            {
                if (ShouldRefresh(friendlyName))
                {
                    friendlyName = string.Empty;
                    if (MsaaUtility.CanUseControlTextProperty(ControlTypeName))
                    {
                        if (!ControlType.MenuItem.NameEquals(ControlTypeName) && (Parent == null || WindowHandle != Parent.WindowHandle || ControlType.Window.NameEquals(Parent.ControlTypeName)))
                        {
                            friendlyName = ControlText;
                        }
                        if (string.IsNullOrEmpty(friendlyName))
                        {
                            if (MsaaUtility.CanUseNameProperty(this))
                            {
                                friendlyName = Name;
                            }
                            if (string.IsNullOrEmpty(friendlyName) && (Parent == null || WindowHandle != Parent.WindowHandle || ControlType.Window.NameEquals(Parent.ControlTypeName)))
                            {
                                friendlyName = Id;
                            }
                        }
                    }
                    else if (QueryIdHelper.IsTableCell(this))
                    {
                        friendlyName = Value;
                        if (string.IsNullOrEmpty(friendlyName))
                        {
                            friendlyName = Name;
                        }
                    }
                    else
                    {
                        friendlyName = Id;
                        if (string.IsNullOrEmpty(friendlyName) && MsaaUtility.CanUseNameProperty(this))
                        {
                            friendlyName = Name;
                        }
                    }
                    if (string.IsNullOrEmpty(friendlyName) && RoleInt == AccessibleRole.Client && ParentWrapper != null && ParentWrapper.Parent != null)
                    {
                        MsaaElement element = new MsaaElement(ParentWrapper.Parent);
                        friendlyName = element.FriendlyName;
                    }
                    friendlyName = ExtensionUtilities.SanitizeFriendlyName(friendlyName);
                }
                return friendlyName;
            }
        }

        internal string Id
        {
            get
            {
                if (ShouldRefresh(id))
                {
                    id = string.Empty;
                    id = MsaaUtility.GetControlName(WindowHandle, ProcessId);
                }
                return id;
            }
        }

        internal bool IsBoundayForHostedControl
        {
            get
            {
                bool flag = false;
                if (SwitchingElement != null)
                {
                    if (string.Equals(Framework, SwitchingElement.Framework, StringComparison.OrdinalIgnoreCase))
                    {
                        flag = EqualsIgnoreContainer(SwitchingElement);
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    return IsCeilingElement;
                }
                return true;
            }
        }

        internal bool IsCeilingElement
        {
            get
            {
                if (!isCeilingElement.HasValue || !isCeilingElement.HasValue)
                {
                    isCeilingElement = false;
                    if (AccessibleWrapper != null && AccessibleWrapper.Equals(CeilingElement))
                    {
                        isCeilingElement = true;
                    }
                }
                return isCeilingElement.Value;
            }
        }

        internal bool IsChildOfDesktop
        {
            get
            {
                IntPtr desktopWindowHandle = MsaaUtility.DesktopWindowHandle;
                AccWrapper parentWrapper = ParentWrapper;
                if (parentWrapper != null)
                {
                    desktopWindowHandle = parentWrapper.WindowHandle;
                }
                return desktopWindowHandle == MsaaUtility.DesktopWindowHandle;
            }
        }

        internal bool IsDesktop =>
            WindowHandle == MsaaUtility.DesktopWindowHandle && RoleInt == AccessibleRole.Window;

        internal bool IsElementDataGridCell
        {
            get
            {
                if (!isElementDataGridCell.HasValue)
                {
                    bool shouldUseValueInCell = false;
                    int instance = 0;
                    isElementDataGridCell = QueryIdHelper.UseValueInDataGridCellAndRow(this, ref instance, ref shouldUseValueInCell, ref shouldUseValueInCell);
                }
                return isElementDataGridCell.Value;
            }
            set
            {
                isElementDataGridCell = value;
            }
        }

        internal bool IsIE9AddressEditControl
        {
            get
            {
                if (!isIE9AddressEditControl.HasValue)
                {
                    isIE9AddressEditControl = false;
                    if (ControlType.Edit.NameEquals(ControlTypeName))
                    {
                        string className = NativeMethods.GetClassName(NativeMethods.GetAncestor(WindowHandle, NativeMethods.GetAncestorFlag.GA_ROOTOWNER));
                        isIE9AddressEditControl = string.Equals("IEFrame", className, StringComparison.Ordinal) && Parent != null && Parent.Parent != null && string.Equals("Address Band Root", Parent.Parent.ClassName, StringComparison.Ordinal);
                    }
                }
                return isIE9AddressEditControl.Value;
            }
        }

        public override bool IsLeafNode
        {
            get
            {
                ITaskActivityElement element = MsaaZappyPlugin.Navigate(this, AccessibleNavigation.LastChild);
                if (element != null)
                {
                    return Equals(element);
                }
                return true;
            }
        }

        public override bool IsPassword
        {
            get
            {
                if (ShouldRefresh(isPassword))
                {
                    AccessibleStates unavailable = AccessibleStates.Unavailable;
                    try
                    {
                        unavailable = GetRequestedState(~AccessibleStates.None);
                    }
                    catch (ZappyTaskException)
                    {
                    }
                    isPassword = ControlType.Edit.NameEquals(ControlTypeName) || (unavailable & AccessibleStates.Protected) > AccessibleStates.None;
                    if ((unavailable & AccessibleStates.Protected) == AccessibleStates.None)
                    {
                        try
                        {
                            string str = Value;
                            isPassword = false;
                        }
                        catch (ZappyTaskException exception)
                        {
                            if (!isPassword.Value && exception.HResult == -2147024891)
                            {
                                isPassword = true;
                            }
                        }
                        catch (NotSupportedException)
                        {
                        }
                    }
                }
                return isPassword.Value;
            }
        }

        internal bool IsSimpleComboBox
        {
            get
            {
                if (!isSimpleComboBox.HasValue)
                {
                    isSimpleComboBox = MsaaUtility.IsSimpleComboBox(this, out simpleComboBoxSourceElement);
                }
                return isSimpleComboBox.Value;
            }
        }

        internal bool IsTopLevelElement
        {
            get
            {
                if (ShouldRefresh(isTopLevelElement))
                {
                    isTopLevelElement = false;
                    if (SwitchingElement == null)
                    {
                        if (RoleInt == AccessibleRole.MenuItem || RoleInt == AccessibleRole.MenuPopup)
                        {
                            isTopLevelElement = false;
                        }
                        else if (topLevelElement == null)
                        {
                            isTopLevelElement = Equals(TopLevelElement);
                        }
                        else
                        {
                            isTopLevelElement = Equals(topLevelElement);
                        }
                    }
                }
                return isTopLevelElement.Value;
            }
        }

        public override bool IsTreeSwitchingRequired =>
            false;

        internal bool IsWin32ForSure
        {
            get
            {
                if (!isWin32ForSure.HasValue)
                {
                    isWin32ForSure = false;
                    if (WindowHandle != IntPtr.Zero && NativeMethods.IsWindow(WindowHandle) && (SwitchingElement == null || !NativeMethods.IsWindow(SwitchingElement.WindowHandle) || WindowHandle != SwitchingElement.WindowHandle))
                    {
                        try
                        {
                            AccWrapper accWrapperFromWindow = AccWrapper.GetAccWrapperFromWindow(WindowHandle);
                            isWin32ForSure = accWrapperFromWindow != null && AccessibleWrapper.Equals(accWrapperFromWindow);
                        }
                        catch (Exception ex)
                        {
                            CrapyLogger.log.Error(ex);

                        }
                    }
                    object[] args = { isWin32ForSure, this };
                    
                }
                return isWin32ForSure.Value;
            }
        }

        public override string Name
        {
            get
            {
                if (ShouldRefresh(name))
                {
                    string controlText;
                    if (IsWin32ForSure)
                    {
                        controlText = ControlText;
                    }
                    else
                    {
                        controlText = accWrapper.Name;
                    }
                    if (PluginUtilities.CheckForValueAndNameEquals(ControlTypeName))
                    {
                        string valueForIdentification = ValueForIdentification;
                        if (string.Equals(controlText, valueForIdentification, StringComparison.Ordinal))
                        {
                            controlText = null;
                        }
                    }
                    name = controlText;
                }
                return name;
            }
        }

        public override object NativeElement =>
            new object[] { AccessibleObject, ChildId };

        internal string OwnerWindowClassName
        {
            get =>
                ownerWindowClassName;
            set
            {
                ownerWindowClassName = value;
            }
        }

        internal IntPtr OwnerWindowHandle
        {
            get =>
                ownerWindowHandle;
            set
            {
                ownerWindowHandle = value;
            }
        }

        internal string OwnerWindowText
        {
            get =>
                ownerWindowText;
            set
            {
                ownerWindowText = value;
            }
        }

        internal MsaaElement Parent
        {
            get
            {
                if (this.parent == null)
                {
                    AccWrapper parent = AccessibleWrapper.Parent;
                    if (parent != null)
                    {
                        this.parent = new MsaaElement(parent);
                    }
                }
                return this.parent;
            }
            set
            {
                parent = value;
            }
        }

        internal AccWrapper ParentWrapper =>
            parent?.AccessibleWrapper;

        public int ProcessId
        {
            get
            {
                if (!processId.HasValue)
                {
                    uint windowProcessId = 0;
                    IntPtr windowHandle = WindowHandle;
                    
                    if (windowHandle != IntPtr.Zero)
                    {
                        windowProcessId = NativeMethods.GetWindowProcessId(windowHandle);
                    }
                    processId = (int)windowProcessId;
                }
                return processId.Value;
            }
        }

        public override IQueryElement QueryId
        {
            get
            {
                if (queryID == null)
                {
                    if (IsTopLevelElement)
                    {
                        queryID = SingleQueryID(false, false, 1);
                    }
                    else if (IsBoundaryOrIE())
                    {
                        queryID = SingleQueryID(false, false, 1);
                        GenerateInstanceForWindowElement(this, null);
                    }
                    else
                    {
                        InitializeQueryId();
                    }
                }
                if (queryID != null)
                {
                    AddSearchConfigurations();
                }
                if (MsaaUtility.IsIEServerControl(ClassName, nativeControlType) && !MsaaUtility.IsIEServerControlNotInsideIEFrame(this))
                {
                    queryID.Ancestor = topLevelElement;
                }
                return queryID;
            }
        }

        internal IQueryElement QueryIdInternal
        {
            get =>
                QueryId;
            set
            {
                queryID = value;
            }
        }

        internal AccessibleRole RoleInt =>
            accWrapper.RoleInt;

        internal ITaskActivityElement SimpleComboBoxSourceElement =>
            simpleComboBoxSourceElement;

        public override int SupportLevel
        {
            get
            {
                if (ShouldRefresh(supportLevel))
                {
                    try
                    {
                        supportLevel = MsaaUtility.GetControlSupportLevel(WindowHandle);
                    }
                    catch (Exception)
                    {
                        supportLevel = 0;
                    }
                }
                return supportLevel.Value;
            }
        }

        public override ITaskActivityElement SwitchingElement
        {
            get =>
                accWrapper.ContainerElement;
            set
            {
                accWrapper.ContainerElement = value;
            }
        }

        public override UITechnologyManager TechnologyManager =>
            MsaaZappyPlugin.Instance;

        public override string Framework =>
            "MSAA";

        public override TaskActivityElement TopLevelElement
        {
            get
            {
                if (topLevelElement == null)
                {
                    bool isBrokenTree = false;
                    if (MsaaUtility.ValidMenuParent(RoleInt))
                    {
                        MsaaElement msaaTopElement = MsaaUtility.GetMsaaTopElement(this, out isBrokenTree);
                        if (isBrokenTree && msaaTopElement != null)
                        {
                            topLevelElement = MsaaUtility.GetWin32TopElement(msaaTopElement);
                        }
                        else
                        {
                            topLevelElement = msaaTopElement;
                        }
                    }
                    if (topLevelElement == null)
                    {
                        topLevelElement = MsaaUtility.GetWin32TopElement(this);
                    }
                    if (Equals(topLevelElement))
                    {
                        topLevelElement = this;
                    }
                }
                return topLevelElement;
            }
            set
            {
                topLevelElement = value;
            }
        }

        public override string Value
        {
            get
            {
                if (ControlType.Slider.NameEquals(ControlTypeName))
                {
                    return MsaaUtility.GetAbsoluteValueForSlider(this);
                }

                string str;
                try
                {
                    str = accWrapper.Value;
                }
                catch
                {
                    str = null;
                }

                if (!string.IsNullOrEmpty(str) && str.Length > 0xfff)
                {
                    IntPtr ptr;
                    NativeMethods.SendMessageTimeout(WindowHandle, 14, IntPtr.Zero, IntPtr.Zero, NativeMethods.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 0x3e8, out ptr);
                    if (ptr != IntPtr.Zero)
                    {
                        int capacity = ptr.ToInt32() + 1;
                        StringBuilder builder = new StringBuilder(capacity);
                        if (NativeMethods.SendMessage(WindowHandle, 13, (IntPtr)capacity, builder) != IntPtr.Zero)
                        {
                            str = builder.ToString();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(str))
                {
                    str = str.TrimEnd(InvalidXMLCharacters);
                }
                return str;
            }
            set
            {
                accWrapper.Value = value;
            }
        }

        internal string ValueForIdentification
        {
            get
            {
                string str = null;
                try
                {
                    str = Value;
                }
                catch (NotSupportedException)
                {
                }
                catch (ZappyTaskControlNotAvailableException exception)
                {
                    if (exception.InnerException == null || !(exception.InnerException is NotImplementedException) && !(exception.InnerException is COMException))
                    {
                        throw;
                    }
                    return str;
                }
                return str;
            }
        }

        public override IntPtr WindowHandle =>
            nativeWindowHandle;

        private enum SelectionFlags
        {
            RemoveSelection = 0x10,
            TakeFocus = 1,
            TakeSelection = 2
        }
    }
}