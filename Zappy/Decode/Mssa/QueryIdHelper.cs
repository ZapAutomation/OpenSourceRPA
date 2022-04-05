using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.Decode.Mssa
{
    public static class QueryIdHelper
    {
        private const int InstanceTimeOut = 200;
        private const int MaxSiblingSearch = 10;

        internal static int GenerateInstanceForChild(MsaaElement element, MsaaElement parent, bool useValueInInstanceGeneration, bool ignoreInvisibleChild)
        {
            AccWrapper accessibleWrapper = element.AccessibleWrapper;
            AccChildrenEnumerator enumerator = new AccChildrenEnumerator(parent.AccessibleWrapper);
            AccWrapper nextChild = enumerator.GetNextChild(ignoreInvisibleChild);
            int num = 1;
            bool flag = false;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (nextChild != null && stopwatch.ElapsedMilliseconds <= 200L)
            {
                if (accessibleWrapper.RoleInt == nextChild.RoleInt && string.Equals(accessibleWrapper.Name, nextChild.Name, StringComparison.OrdinalIgnoreCase) && (!useValueInInstanceGeneration || string.Equals(accessibleWrapper.Value, nextChild.Value, StringComparison.OrdinalIgnoreCase)))
                {
                    if (accessibleWrapper.Equals(nextChild))
                    {
                        flag = true;
                        break;
                    }
                    num++;
                }
                nextChild = enumerator.GetNextChild(ignoreInvisibleChild);
            }
            if (!flag)
            {
                num = 0;
            }
            return num;
        }

        internal static int GetInstanceOfNameLessElementUnderParent(MsaaElement element, bool ignoreInvisibleItems)
        {
            if (element == null || !string.IsNullOrEmpty(element.Name) || element.IsDesktop || element.Parent == null || element.Parent.WindowHandle == MsaaUtility.DesktopWindowHandle || element.Parent.IsBoundayForHostedControl)
            {
                return 0;
            }
            MsaaElement parent = element.Parent;
            bool flag = false;
            AccChildrenEnumerator enumerator = new AccChildrenEnumerator(parent.AccessibleWrapper);
            int num = 1;
            Stopwatch stopwatch = Stopwatch.StartNew();
                                    while (stopwatch.ElapsedMilliseconds <= 200L)
            {
                AccWrapper nextChild = enumerator.GetNextChild(ignoreInvisibleItems);
                if (nextChild != null && nextChild.RoleInt == element.RoleInt)
                {
                    if (element.AccessibleWrapper.Equals(nextChild))
                    {
                        flag = true;
                        goto Label_00C7;
                    }
                    num++;
                }
            }
                Label_00C7:
            if (!flag)
            {
                num = 0;
            }
            return num;
        }

        internal static int GetTableCellInstance(MsaaElement element, ref bool shouldUseValueInCell, bool validProperty)
        {
            AccWrapper parent = element.AccessibleWrapper.Parent;
            int num = 1;
            if (parent != null && parent.RoleInt == AccessibleRole.Row)
            {
                AccWrapper wrapper2;
                AccChildrenEnumerator enumerator = new AccChildrenEnumerator(parent);
                shouldUseValueInCell = false;
                if (string.IsNullOrEmpty(element.Value))
                {
                    shouldUseValueInCell = false;
                    return num;
                }
                Stopwatch stopwatch = Stopwatch.StartNew();
                while ((wrapper2 = enumerator.GetNextChild(true)) != null && stopwatch.ElapsedMilliseconds <= 200L)
                {
                    if (wrapper2.RoleInt == AccessibleRole.Cell)
                    {
                        if (wrapper2.Equals(element.AccessibleWrapper))
                        {
                            shouldUseValueInCell = true;
                            return num;
                        }
                        if (!validProperty || string.Equals(wrapper2.Value, element.Value, StringComparison.Ordinal))
                        {
                            num++;
                        }
                    }
                }
            }
            return num;
        }

        internal static int GetWindowControlsInQueryID(MsaaElement element, ITaskActivityElement currentTopMostElement, out MsaaElement mdiWindow, out MsaaElement firstWindow)
        {
            mdiWindow = null;
            firstWindow = null;
            MsaaElement element2 = currentTopMostElement as MsaaElement;
            if (element2 != null && element2.Parent != null)
            {
                IntPtr windowHandle = element2.Parent.WindowHandle;
                bool flag = element2.IsWin32ForSure;
                if (windowHandle != IntPtr.Zero && !windowHandle.Equals(element.TopLevelElement.WindowHandle))
                {
                    int num = 1;
                    MsaaElement element3 = new MsaaElement(windowHandle);
                    while (element3 != null && !element3.IsBoundayForHostedControl && !element3.Equals(element.TopLevelElement))
                    {
                        if (IsMdiWindow(element3))
                        {
                            mdiWindow = element3;
                            return num;
                        }
                        if (!flag && firstWindow == null)
                        {
                            firstWindow = element3;
                        }
                        windowHandle = MsaaNativeMethods.GetParent(element3.WindowHandle);
                        if (windowHandle == IntPtr.Zero)
                        {
                            return num;
                        }
                        element3 = new MsaaElement(windowHandle);
                        num++;
                    }
                    return num;
                }
            }
            return 0;
        }

        private static bool IsBrokenTree(MsaaElement element, MsaaElement parent, MsaaElement nextToElement, int elementInstance, ref int nextToInstance)
        {
            MsaaElement element2 = element.Parent;
            MsaaElement element3 = null;
            MsaaElement element4 = null;
            bool isSecondAlgoUsed = false;
            if (nextToElement == null)
            {
                if (!element2.Equals(parent))
                {
                    isSecondAlgoUsed = true;
                }
                element4 = element;
            }
            else
            {
                element3 = nextToElement;
                element4 = element;
                if (element2 == null || !parent.Equals(element2))
                {
                    isSecondAlgoUsed = true;
                }
            }
            return IsBrokenTreeInternal(element4, element3, parent, elementInstance, isSecondAlgoUsed, ref nextToInstance);
        }


        private static bool IsBrokenTreeInternal(MsaaElement element, MsaaElement nextToElement, MsaaElement parent, int elementInstance, bool isSecondAlgoUsed, ref int nextToInstance)
        {
            object[] args = { isSecondAlgoUsed };
            
            AccChildrenEnumerator enumerator = new AccChildrenEnumerator(parent.AccessibleWrapper);
            bool flag = true;
            AccWrapper nextChild = enumerator.GetNextChild(true);
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (nextToElement == null)
            {
                int num2 = 0;
                while (nextChild != null && (flag = stopwatch.ElapsedMilliseconds <= 200L))
                {
                    if (isSecondAlgoUsed && nextChild != null)
                    {
                        nextChild = nextChild.Navigate(AccessibleNavigation.LastChild);
                    }
                    if (nextChild != null && nextChild.RoleInt == element.RoleInt)
                    {
                        num2++;
                    }
                    if (num2 == elementInstance)
                    {
                        break;
                    }
                    nextChild = enumerator.GetNextChild(true);
                }
            }
            else
            {
                while (nextChild != null && (flag = stopwatch.ElapsedMilliseconds <= 200L))
                {
                    if (isSecondAlgoUsed && nextChild != null)
                    {
                        nextChild = nextChild.Navigate(AccessibleNavigation.LastChild);
                    }
                    if (nextChild != null && nextChild.RoleInt == nextToElement.RoleInt && string.Equals(nextChild.Name, nextToElement.Name))
                    {
                        if (nextChild.Equals(nextToElement.AccessibleWrapper))
                        {
                            break;
                        }
                        nextToInstance++;
                    }
                    nextChild = enumerator.GetNextChild(true);
                }
                if (!flag)
                {
                    return false;
                }
                stopwatch = Stopwatch.StartNew();
                int num = 0;
                while ((flag = stopwatch.ElapsedMilliseconds <= 200L) && num < elementInstance && nextChild != null)
                {
                    if (nextChild != null)
                    {
                        nextChild = enumerator.GetNextChild(true);
                    }
                    if ((nextChild != null) & isSecondAlgoUsed)
                    {
                        nextChild = nextChild.Navigate(AccessibleNavigation.LastChild);
                    }
                    if (nextChild != null && nextChild.RoleInt == element.RoleInt)
                    {
                        num++;
                    }
                }
            }
            if ((nextChild == null || !nextChild.Equals(element.AccessibleWrapper)) & flag)
            {
                
                return true;
            }
            if (!flag)
            {
                nextToInstance = 0;
            }
            return false;
        }

        internal static bool IsDataGridCell(MsaaElement element)
        {
            if (IsTableCell(element))
            {
                MsaaElement parent = element.Parent;
                MsaaElement element3 = null;
                if (parent != null)
                {
                    element3 = parent.Parent;
                }
                if (parent != null && element3 != null && parent.RoleInt == AccessibleRole.Row && element3.RoleInt == AccessibleRole.Table)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsDataGridCellDuplicate(MsaaElement element)
        {
            if (IsTableCell(element))
            {
                MsaaElement parent = element.Parent;
                if (parent != null)
                {
                    string str = parent.Value;
                    string str2 = element.Value;
                    if (parent.RoleInt == AccessibleRole.Row && !string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(str2))
                    {
                        str = ";" + str + ";";
                        MatchCollection matchs = null;
                        try
                        {
                            matchs = Regex.Matches(str, str2);
                        }
                        catch (ArgumentException)
                        {
                            object[] args = { str, str2 };
                            
                            return true;
                        }
                        int count = matchs.Count;
                        if (count != 1)
                        {
                            int num2 = 0;
                            for (int i = 0; i < matchs.Count; i++)
                            {
                                int startIndex = matchs[i].Index - 1;
                                int num5 = matchs[i].Index + matchs[i].Length;
                                if (string.Equals(str.Substring(startIndex, 1), ";", StringComparison.Ordinal) && string.Equals(str.Substring(num5, 1), ";", StringComparison.Ordinal))
                                {
                                    num2++;
                                }
                            }
                            count = num2;
                        }
                        if (count == 1)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static bool IsMdiWindow(MsaaElement element)
        {
            int windowLong = NativeMethods.GetWindowLong(element.WindowHandle, NativeMethods.GWLParameter.GWL_EXSTYLE);
            return (0x40L & windowLong) != 0;
        }

        internal static bool IsTableCell(MsaaElement element) =>
            element.RoleInt == AccessibleRole.Cell && !MonthCalendarUtilities.IsMonthCalendarClassName(element.ClassName);

        internal static bool ShouldGenerateContainsRowValue(MsaaElement element, ref string elementValue)
        {
            if (elementValue.IndexOf(";", StringKeys.Comparison) < 0)
            {
                return false;
            }
            AccWrapper accessibleWrapper = element.AccessibleWrapper;
            bool flag = true;
            Dictionary<long, bool> rowIds = new Dictionary<long, bool>();
            flag = VerifyDuplicateRowId(accessibleWrapper, AccessibleNavigation.Previous, rowIds);
            if (flag)
            {
                flag = VerifyDuplicateRowId(accessibleWrapper.Navigate(AccessibleNavigation.Next), AccessibleNavigation.Next, rowIds);
            }
            rowIds.Clear();
            if (flag)
            {
                elementValue = elementValue.Substring(elementValue.IndexOf(";", StringKeys.Comparison));
            }
            return flag;
        }

        internal static MsaaElement UpdateQueryIDForNamelessControls(MsaaElement element, out MsaaElement nextToElement, ref int elementInstance, bool useValue, bool ignoreInvisibleItems)
        {
            MsaaElement element2 = element;
            MsaaElement parent = element.Parent;
            nextToElement = null;
            if (!useValue && !element.IsTopLevelElement && string.IsNullOrEmpty(element.Name) && (parent == null || !MsaaUtility.HasLanguageNeutralID(parent) || parent.RoleInt != AccessibleRole.Window))
            {
                if (element.IsWin32ForSure)
                {
                    return parent;
                }
                AccessibleRole roleInt = element.RoleInt;
                elementInstance = GetInstanceOfNameLessElementUnderParent(element, ignoreInvisibleItems);
                if (elementInstance <= 0 && element.Parent != null && !element.Parent.IsWin32ForSure)
                {
                    elementInstance = 1;
                    MsaaElement element4 = parent;
                    int num = 1;
                    bool flag = false;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                                                            bool flag2;
                    while (flag2 = stopwatch.ElapsedMilliseconds <= 200L)
                    {
                        MsaaElement previousSiblingFlattened = MsaaUtility.GetPreviousSiblingFlattened(element, ref parent);
                        if (previousSiblingFlattened == null)
                        {
                            break;
                        }
                        if (element4 == null || num < 2 && parent != null && !element4.Equals(parent))
                        {
                            element4 = parent;
                            num++;
                        }
                        if (string.IsNullOrEmpty(previousSiblingFlattened.Name))
                        {
                            if (roleInt == previousSiblingFlattened.RoleInt)
                            {
                                elementInstance++;
                            }
                        }
                        else
                        {
                            nextToElement = previousSiblingFlattened;
                            nextToElement.QueryIdInternal = nextToElement.SingleQueryID(false, false);
                            flag = true;
                            break;
                        }
                        element = previousSiblingFlattened;
                    }
                    if (!flag2)
                    {
                        
                    }
                                        parent = element4;
                    if (!flag)
                    {
                        elementInstance = 0;
                        return element2.Parent;
                    }
                    int nextToInstance = 1;
                    object[] args = { nextToElement, elementInstance };
                    
                    if (IsBrokenTree(element2, parent, nextToElement, elementInstance, ref nextToInstance))
                    {
                        object[] objArray2 = { nextToInstance };
                        
                        parent = element2.Parent;
                        nextToElement = null;
                        elementInstance = 0;
                        return parent;
                    }
                    if (nextToInstance > 1)
                    {
                        nextToElement.QueryId.Condition.Conditions = new List<IQueryCondition>(nextToElement.QueryId.Condition.Conditions) { new PropertyCondition("Instance", nextToInstance) }.ToArray();
                    }
                }
            }
            return parent;
        }

        internal static bool UseValueInDataGridCell(MsaaElement element) =>
            element.RoleInt != AccessibleRole.Cell || !string.Equals(element.Value, bool.TrueString, StringComparison.OrdinalIgnoreCase) && !string.Equals(element.Value, bool.FalseString, StringComparison.OrdinalIgnoreCase) && !string.Equals(element.Value, "System.Drawing.Bitmap", StringComparison.OrdinalIgnoreCase);

        internal static bool UseValueInDataGridCellAndRow(MsaaElement element, ref int instance, ref bool shouldUseValueInCell, ref bool shouldUseValueInRow)
        {
            shouldUseValueInCell = IsTableCell(element);
            bool flag = element.RoleInt == AccessibleRole.RowHeader || IsDataGridCell(element);
            if (flag)
            {
                shouldUseValueInCell = true;
                bool isNewRow = false;
                shouldUseValueInRow = UseValueInDataGridRow(element, ref isNewRow);
                if (IsDataGridCellDuplicate(element))
                {
                    instance = GetTableCellInstance(element, ref shouldUseValueInCell, true);
                    if (instance == 1 && element.RoleInt == AccessibleRole.Cell && string.IsNullOrEmpty(element.Value))
                    {
                        shouldUseValueInCell = true;
                        instance = GetTableCellInstance(element, ref shouldUseValueInCell, false);
                    }
                }
            }
            return flag;
        }

        internal static bool UseValueInDataGridRow(MsaaElement element, ref bool isNewRow)
        {
            MsaaElement parent = element.Parent;
            if (parent != null && parent.RoleInt == AccessibleRole.Row)
            {
                if (string.Equals(parent.Value, "(Create New)", StringComparison.OrdinalIgnoreCase))
                {
                    isNewRow = true;
                    return true;
                }
                AccWrapper wrapper = parent.AccessibleWrapper.Navigate(AccessibleNavigation.Previous);
                AccWrapper wrapper2 = parent.AccessibleWrapper.Navigate(AccessibleNavigation.Next);
                for (int i = 0; (wrapper != null || wrapper2 != null) && i < 10; i++)
                {
                    if (wrapper != null)
                    {
                        if (wrapper.RoleInt == AccessibleRole.Row)
                        {
                            if (string.Equals(wrapper.Value, parent.Value, StringComparison.Ordinal))
                            {
                                return false;
                            }
                            wrapper = wrapper.Navigate(AccessibleNavigation.Previous);
                        }
                        else
                        {
                            wrapper = null;
                        }
                    }
                    if (wrapper2 != null)
                    {
                        if (wrapper2.RoleInt == AccessibleRole.Row)
                        {
                            if (string.Equals(wrapper2.Value, parent.Value, StringComparison.Ordinal))
                            {
                                return false;
                            }
                            wrapper2 = wrapper2.Navigate(AccessibleNavigation.Next);
                        }
                        else
                        {
                            wrapper2 = null;
                        }
                    }
                }
            }
            return true;
        }

        private static bool VerifyDuplicateRowId(AccWrapper startRow, AccessibleNavigation navigationDir, Dictionary<long, bool> rowIds)
        {
            bool flag = true;
            AccWrapper wrapper = startRow;
            for (int i = 0; i < 5 && wrapper != null; i++)
            {
                long num3;
                string str = wrapper.Value;
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                if (string.Equals(str, "Top Row", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "(Create New)", StringComparison.OrdinalIgnoreCase))
                {
                    return flag;
                }
                int index = str.IndexOf(";", StringKeys.Comparison);
                if (index == -1)
                {
                    return false;
                }
                if (!long.TryParse(str.Substring(0, index), out num3))
                {
                    return false;
                }
                if (rowIds.ContainsKey(num3))
                {
                    return false;
                }
                rowIds.Add(num3, true);
                wrapper = wrapper.Navigate(navigationDir);
            }
            return flag;
        }
    }
}