using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Hooks.Mouse;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Execute;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Helpers
{

    internal class ScreenElement
    {
        private static bool addedTechnologyManagers;
        private const int CLICKABLE_POINT_OFFSET = 5;
        private const string ContextMenuClassName = "ContextMenu";
        private const int defaultPressure = 0x7d00;
        private const ScrollOptions DefaultScrollOptions = ScrollOptions.UseKeyboard | ScrollOptions.UseClickOnScrollBar | ScrollOptions.UseScrollBar | ScrollOptions.UseMouseWheel | ScrollOptions.UseProgrammatic;
        private static ScreenElement desktop;
        private const uint EventModifyState = 2;
        private static int findAllTimeout = -1;
        private const string IETitleBarClassName = "IEFrame";
        private static List<int> imeLanguageList = new List<int>();
        private static bool isDebugModeOn;
        private bool? isPopUpWindow;
        private bool? isTopLevelWindow;
        private static ILastInvocationInfo lastSearchInfo;
        private static bool matchExactHierarchy;
        private const uint MONITOR_DEFAULTTOPRIMARY = 1;
        private static IRPFPlayback playback;
        internal const int PlaybackSearchRetryCountDefault = 3;
        private const string QueryIdSeparator = ";";
        private static bool searchInMinimizedWindows;
        private static int searchTimeout = -1;
        private static int sendKeysDelay = -1;
        private static readonly string SkipPlayBackEventName = "TASK_SKIP_STEP" + Guid.NewGuid();
        private static SmartMatchOptions smartMatchOptions;
        private const int SmartMatchTimeOut = 0xbb8;
        private static bool startedSession;
        internal const int SWP_NOACTIVATE = 0x10;
        internal const int SWP_NOCOPYBITS = 0x100;
        internal const int SWP_NOMOVE = 2;
        internal const int SWP_NOOWNERZORDER = 0x200;
        internal const int SWP_NOREDRAW = 8;
        internal const int SWP_NOREPOSITION = 0x200;
        internal const int SWP_NOSENDCHANGING = 0x400;
        internal const int SWP_NOSIZE = 1;
        internal const int SWP_NOZORDER = 4;
        internal const int SWP_SHOWWINDOW = 0x40;
        private ITaskActivityElement technologyElement;
        internal const int TopLevelWindowExactMatchTimeoutPercentage = 15;
        private IScreenElement uiElement;
        private static int waitForReadyTimeout = -1;

                                                        
        private bool BringUpIfTopWindow()
        {
            if (!isTopLevelWindow.HasValue)
            {
                isTopLevelWindow = false;
                if (ControlType.Window.NameEquals(TechnologyElement.ControlTypeName))
                {
                    TaskActivityElement element = ZappyTaskService.Instance.ConvertTechnologyElement(TechnologyElement);
                    if (FrameworkUtilities.IsTopLevelElement(element) && !IsPopUpMenu(element))
                    {
                        isTopLevelWindow = true;
                    }
                }
            }
            if (isTopLevelWindow.Value)
            {
                ShowIfMinimized(WindowHandle);
                NativeMethods.SetForegroundWindow(WindowHandle);
                return true;
            }
            return false;
        }

        public void Check()
        {
            SetScrollOptionsForAction();
            int nCheckUncheckFlag = GetCheckOptionsCheckedListBox() | 2;
            UIElement.Check(nCheckUncheckFlag);
        }

        public void CheckIndeterminate()
        {
            SetScrollOptionsForAction();
            int nCheckUncheckFlag = 10;
            UIElement.Check(nCheckUncheckFlag);
        }

        private static void CheckPlaybackCancelled()
        {
            if (IsSkipStepOn)
            {
                throw new ExecuteCanceledException();
            }
        }

                                
        public void CheckTreeItem()
        {
            SetScrollOptionsForAction();
            UIElement.Select(2, !IgnoreVerification);
            UIElement.SendKeys("{Space}", 3, 0);
        }

        public void Collapse(ExpandCollapseOptions[] expandCollapseFlags)
        {
            Array playbackOptionAsArray;
            SetScrollOptionsForAction();
            if (expandCollapseFlags == null)
            {
                playbackOptionAsArray = GetPlaybackOptionAsArray(UITechnologyElementOption.ExpandCollapseOptions);
            }
            else
            {
                playbackOptionAsArray = expandCollapseFlags;
            }
            for (int i = 0; i < playbackOptionAsArray.Length; i++)
            {
                try
                {
                    ExpandCollapseOptions options = (ExpandCollapseOptions)playbackOptionAsArray.GetValue(i);
                    if (IgnoreVerification)
                    {
                        options |= ExpandCollapseOptions.DoNotVerify;
                    }
                    UIElement.Collapse((int)options);
                    break;
                }
                catch (COMException exception)
                {
                    object[] args = { playbackOptionAsArray.GetValue(i), exception };
                    
                    if (i == playbackOptionAsArray.Length - 1)
                    {
                        
                        throw;
                    }
                }
            }
        }

        public static void Delay(int duration)
        {
            ZappyTaskUtilities.Sleep(duration);
        }

        private void DeleteContent()
        {
            bool option = true;
            try
            {
                option = (bool)UIElement.GetOption(4);
            }
            catch (COMException)
            {
            }
            catch (ZappyTaskException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (NotImplementedException)
            {
            }
            if (!option || TechnologyElement.IsPassword || !string.IsNullOrEmpty(TechnologyElement.Value))
            {
                UIElement.SendKeys("^{HOME}", 3, 1);
                if (!IsFocusedElement())
                {
                    SingleLineDeleteContent();
                }
                else
                {
                    UIElement.SendKeys("^+{END}", 3, 1);
                    if (!IsFocusedElement())
                    {
                        SingleLineDeleteContent();
                    }
                    else
                    {
                        UIElement.SendKeys("{DELETE}", 3, 1);
                    }
                }
                if (!TechnologyElement.IsPassword && !string.IsNullOrEmpty(TechnologyElement.Value))
                {
                    TechnologyElement.Value = string.Empty;
                    if (!string.IsNullOrEmpty(TechnologyElement.Value))
                    {
                        
                        SelectContent();
                    }
                }
            }
        }

        private static void DisposeSkipEventObject()
        {
            ZappyTaskUtilities.DisposeSkipEventObject();
        }

                                
                                
        public void DoubleClick(int x, int y, MouseButtons button, ModifierKeys modifierKeys)
        {
            DoubleClick(x, y, button, modifierKeys, 1);
        }

        public void DoubleClick(int x, int y, MouseButtons button, ModifierKeys modifierKeys, int ensureVisible)
        {
            if (IsIETitleBarwithInvalidRect)
            {
                ScreenElementUtility.PerformActionOnIETitleBar(this, MouseActionType.DoubleClick, button, modifierKeys, WindowHandle, 0, -1);
            }
            else
            {
                SetScrollOptionsForAction();
                MouseButtonsPlayback buttons = Utility.ConvertToPlaybackMouseButton(button);
                UIElement.DoubleClick(x, y, (int)buttons, ensureVisible, modifierKeys.ToString());
            }
        }

        public void DoubleTap(int pointX, int pointY, bool ensureVisible)
        {
            ExecuteTouchAction(() => UIElement.DoubleTap(pointX, pointY, ensureVisible, 0x7d00), ensureVisible);
        }

        public void EnsureVisible(int offsetX, int offsetY)
        {
            EnsureVisible(offsetX, offsetY, GetOption(UITechnologyElementOption.ScrollOptions));
        }

        public void EnsureVisible(int offsetX, int offsetY, int scrollFlag)
        {
            UIElement.EnsureVisible(1, offsetX, offsetY, null, scrollFlag, 2);
        }

        private void ExecuteTouchAction(Action touchAction, bool ensureVisible)
        {
            if (ensureVisible)
            {
                SetScrollOptionsForAction();
            }
            touchAction();
        }

        public void Expand(ExpandCollapseOptions[] expandCollapseFlags)
        {
            Expand(expandCollapseFlags, false);
        }

        public void Expand(ExpandCollapseOptions[] expandCollapseFlags, bool isSearch)
        {
            Array playbackOptionAsArray;
            SetScrollOptionsForAction();
            if (expandCollapseFlags == null)
            {
                playbackOptionAsArray = GetPlaybackOptionAsArray(UITechnologyElementOption.ExpandCollapseOptions);
            }
            else
            {
                playbackOptionAsArray = expandCollapseFlags;
            }
            for (int i = 0; i < playbackOptionAsArray.Length; i++)
            {
                int fEnable = 0;
                if (isSearch)
                {
                    fEnable = Playback.EnableEnsureVisibleForPrimitive(0);
                }
                try
                {
                    ExpandCollapseOptions options = (ExpandCollapseOptions)playbackOptionAsArray.GetValue(i);
                    if (IgnoreVerification)
                    {
                        options |= ExpandCollapseOptions.DoNotVerify;
                    }
                    UIElement.Expand((int)options);
                    break;
                }
                catch (COMException exception)
                {
                    object[] args = { playbackOptionAsArray.GetValue(i), exception };
                    
                    if (i == playbackOptionAsArray.Length - 1)
                    {
                        
                        throw;
                    }
                }
                finally
                {
                    if (isSearch)
                    {
                        Playback.EnableEnsureVisibleForPrimitive(fEnable);
                    }
                }
            }
        }

        public bool ExpandCollapseComboBox(string buttonQueryId)
        {
            object pvarResKeys = null;
            int num;
            int num2;
            IScreenElement uIElement = null;
            try
            {
                object[] objArray = UIElement.FindAllDescendants(buttonQueryId, ref pvarResKeys, 0, 1);
                if (objArray.Length == 0)
                {
                    return false;
                }
                uIElement = objArray[0] as IScreenElement;
                num = num2 = -1;
            }
            catch (COMException)
            {
                int num3;
                int num4;
                int num5;
                int num6;
                GetBoundingRectangle(out num3, out num4, out num5, out num6);
                num = num5 - 5;
                num2 = 5;
                if (TechnologyElement.GetRightToLeftProperty(RightToLeftKind.Layout) || TechnologyElement.GetRightToLeftProperty(RightToLeftKind.Text))
                {
                    num = num2 = 5;
                }
                uIElement = UIElement;
            }
            try
            {
                uIElement.MouseButtonClick(num, num2, 0, 1, ModifierKeys.None.ToString());
                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }

        private static void ExpandScreenElement(ScreenElement element, ScreenElement parentElement, SingleQueryId singleQid, bool firstExpansion, out bool expanded)
        {
            expanded = false;
            string expandableControlType = string.Empty;
            string valueWrapper = string.Empty;
            if (singleQid == null && !Utility.IsExpandable(element, out expandableControlType))
            {
                return;
            }
            if (singleQid != null)
            {
                foreach (PropertyCondition condition in singleQid.SearchProperties)
                {
                    if (condition.PropertyName.Equals("Role"))
                    {
                        valueWrapper = condition.ValueWrapper;
                    }
                }
                if (string.IsNullOrEmpty(valueWrapper))
                {
                    foreach (PropertyCondition condition2 in singleQid.SearchProperties)
                    {
                        if (condition2.PropertyName.Equals("ControlType"))
                        {
                            expandableControlType = condition2.ValueWrapper;
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(valueWrapper) && string.IsNullOrEmpty(expandableControlType))
            {
                return;
            }
            if (!string.Equals(valueWrapper, "menu item") && !ControlType.MenuItem.NameEquals(valueWrapper) && !ControlType.MenuItem.NameEquals(expandableControlType))
            {
                
                if (TaskActivityElement.IsState(element.TechnologyElement, AccessibleStates.Collapsed))
                {
                    element.Expand(null, true);
                    expanded = true;
                }
                return;
            }
            if (TaskActivityElement.IsState(element.TechnologyElement, AccessibleStates.Expanded))
            {
                
                return;
            }
            
            if (!firstExpansion)
            {
                goto Label_01EA;
            }
            if (parentElement == null)
            {
                TaskActivityElement element2 = ZappyTaskService.Instance.ConvertTechnologyElement(element.TechnologyElement);
                parentElement = FromTechnologyElement(ZappyTaskService.Instance.GetParent(element2));
            }
            if (parentElement != null)
            {
                if (Utility.IsMenuContainer(parentElement))
                {
                    try
                    {
                        parentElement.LeftButtonClick();
                        goto Label_017D;
                    }
                    catch (COMException)
                    {
                        
                        goto Label_017D;
                    }
                }
                WPFCollapseMenu(element, parentElement);
            }
        Label_017D:;
            try
            {
                element.LeftButtonClick();
                expanded = true;
                return;
            }
            catch (COMException exception)
            {
                switch ((uint)Marshal.GetHRForException(exception))
                {
                    case 0xf004f002:
                    case 0xf004f003:
                        try
                        {
                            
                            element.TechnologyElement.InvokeProgrammaticAction(ProgrammaticActionOption.DefaultAction);
                            return;
                        }
                        catch (COMException)
                        {
                            throw new Exception();
                        }
                        catch (ZappyTaskException)
                        {
                            throw new Exception();
                        }
                        catch (NotSupportedException)
                        {
                            throw new Exception();
                        }
                        catch (NotImplementedException)
                        {
                            throw new Exception();
                        }
                }
                throw;
            }
        Label_01EA:
            element.LeftButtonClick();
            expanded = true;
        }

        public ScreenElement[] FindAllScreenElement(string queryId, int depth, bool singleQueryId, bool throwException) =>
            FindAllScreenElement(queryId, depth, singleQueryId, throwException, true);

        private ScreenElement[] FindAllScreenElement(string queryId, int depth, bool singleQueryId, bool throwException, bool resetSkipStep)
        {
            List<ScreenElement> list = new List<ScreenElement>();
            if (depth == 0 || depth < -1)
            {
                object[] objArray1 = { depth };
                            }
            if (singleQueryId)
            {
                QueryId id = new QueryId(queryId);
                if (id.SingleQueryIds.Count > 1 || id.SingleQueryIds.Count == 0)
                {
                                    }
                id.SingleQueryIds[0].AddTechnologyAttribute("FindAll");
                queryId = id.ToString();
            }
            Utility.RestoreMinimizedWindow(TechnologyElement);
            object[] args = { queryId };
            
            LastSearchInfo = null;
            ResetSkipStep();
            object[] objArray = null;
            object pvarResKeys = null;
            try
            {
                queryId = GetModifiedQueryId(queryId, false);
                objArray = UIElement.FindAllDescendants(queryId, ref pvarResKeys, 0, depth);
            }
            catch (COMException)
            {
                if (throwException)
                {
                    throw;
                }
            }
            finally
            {
                LastSearchInfo = ScreenElementUtility.GetStepInfo(Playback);
            }
            if (objArray != null)
            {
                foreach (IScreenElement element in objArray)
                {
                    ScreenElement item = new ScreenElement
                    {
                        UIElement = element
                    };
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        private static ScreenElement FindFromMultipleWindows(string[] queryIds, int orderVal)
        {
            object[] objArray;
            string topQueryId = ";" + queryIds[0];
            ScreenElement element = new ScreenElement();
            Stopwatch stopWatch = Stopwatch.StartNew();
            int num = 1;
            int num2 = 1;
            IScreenElement element2 = null;
            IScreenElement element3 = null;
            IScreenElement element4 = null;
            List<IntPtr> list = new List<IntPtr>();
            CheckPlaybackCancelled();
            if (!FindNTopWindows(topQueryId, stopWatch, 1, out objArray))
            {
                return element;
            }
            foreach (object obj2 in objArray)
            {
                int num4;
                IScreenElement element5 = obj2 as IScreenElement;
                IntPtr item = element5.TechnologyElement.WindowHandle;
                list.Add(item);
                if (OrderOfInvoke.OrderMapPlayback.TryGetValue(item, out num4))
                {
                    if (num4 > num2)
                    {
                        num2 = num4;
                        element4 = element5;
                    }
                    if (num4 == orderVal)
                    {
                        element.UIElement = element5;
                        break;
                    }
                    if (num4 < orderVal && num4 > num)
                    {
                        num = num4;
                        element3 = element5;
                    }
                }
                else if (element2 == null)
                {
                    element2 = element5;
                }
            }
            if (element.UIElement == null)
            {
                if (element2 != null)
                {
                    element.UIElement = element2;
                }
                else if (orderVal > num2)
                {
                    CheckPlaybackCancelled();
                    if (FindNTopWindows(topQueryId, stopWatch, list.Count + 1, out objArray))
                    {
                        foreach (object obj3 in objArray)
                        {
                            IScreenElement element6 = obj3 as IScreenElement;
                            IntPtr ptr2 = element6.TechnologyElement.WindowHandle;
                            if (!list.Contains(ptr2))
                            {
                                element.UIElement = element6;
                                break;
                            }
                        }
                    }
                    else
                    {
                        element.UIElement = element4;
                    }
                }
                else
                {
                    element.UIElement = element3;
                }
            }
            if (element.UIElement == null)
            {
                return element;
            }
            IntPtr windowHandle = element.UIElement.TechnologyElement.WindowHandle;
            OrderOfInvoke.SetOrderMapPlayback(windowHandle, orderVal);
            ShowIfMinimized(windowHandle);
            if (queryIds.Length <= 1)
            {
                return element;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < queryIds.Length; i++)
            {
                builder.Append(";" + queryIds[i]);
            }
            ScreenElement element7 = new ScreenElement
            {
                UIElement = element.UIElement
            };
            return element7.FindScreenElement(builder.ToString(), false);
        }

        public static ScreenElement FindFromPartialQueryId(string queryId) =>
            FindFromPartialQueryId(queryId, true);

        private static ScreenElement FindFromPartialQueryId(string queryId, bool resetSkipStep)
        {
            object[] args = { queryId };
            
            LastSearchInfo = null;
            if (resetSkipStep)
            {
                ResetSkipStep();
            }
            ScreenElement element = new ScreenElement();
            int orderVal = 1;
            int searchTimeout = SearchTimeout;
            try
            {
                string modifiedQueryId;
                int num4;
                string[] queryIds = QueryId.ParseQueryId(queryId).ToArray();
                if (queryIds.Length == 0)
                {
                    object[] objArray2 = { queryId };
                                    }
                orderVal = OrderOfInvoke.ParseQueryId(queryId, out modifiedQueryId);
                if (orderVal > 0)
                {
                    element = FindFromMultipleWindows(queryIds, orderVal);
                    if (element.UIElement != null)
                    {
                        return element;
                    }
                    SearchTimeout = 0xbb8;
                                        {
                        modifiedQueryId = GetModifiedQueryId(modifiedQueryId, false);
                        element.UIElement = Playback.FindScreenElement(modifiedQueryId);
                        return element;
                    }
                }
                ScreenElement element2 = FindTopLevelWindowHelper(";" + queryIds[0]);
                if (element2 != null)
                {
                    element = element2;
                    if (queryIds.Length > 1)
                    {
                        StringBuilder builder = new StringBuilder();
                        for (int i = 1; i < queryIds.Length; i++)
                        {
                            builder.Append(";" + queryIds[i]);
                        }
                        element = element2.FindScreenElement(builder.ToString(), false);
                    }
                }
                else
                {
                    CrapyLogger.log.ErrorFormat("FindTopLevelWindowHelper return null, should have thrown exception");
                }
                if (!OrderOfInvoke.IsEnabled)
                {
                    return element;
                }
                IntPtr ancestor = NativeMethods.GetAncestor(element.UIElement.TechnologyElement.WindowHandle, NativeMethods.GetAncestorFlag.GA_ROOT);
                if (!OrderOfInvoke.OrderMapPlayback.TryGetValue(ancestor, out num4))
                {
                    OrderOfInvoke.SetOrderMapPlayback(ancestor, 1);
                    return element;
                }
                if (num4 == 1)
                {
                    return element;
                }
                element = FindFromMultipleWindows(queryIds, 1);
                if (element.UIElement != null)
                {
                    return element;
                }
                SearchTimeout = 0xbb8;
                                {
                    modifiedQueryId = GetModifiedQueryId(modifiedQueryId, false);
                    element.UIElement = Playback.FindScreenElement(modifiedQueryId);
                    return element;
                }
            }
            finally
            {
                SearchTimeout = searchTimeout;
                LastSearchInfo = ScreenElementUtility.GetStepInfo(Playback);
            }
        }

        public static ScreenElement FindFromWindowHandle(IntPtr windowHandle) =>
            new ScreenElement { UIElement = Playback.ScreenElementFromWindow(windowHandle.ToInt32()) };

        internal static bool FindNTopWindows(string topQueryId, Stopwatch stopWatch, int winNumber, out object[] retVal)
        {
            bool flag = true;
            retVal = null;
            object[] args = { ScreenElement.findAllTimeout };
            
            int searchTimeout = SearchTimeout;
            int findAllTimeout = ScreenElement.findAllTimeout;
            try
            {
                SearchTimeout = 100;
            Label_0037:
                CheckPlaybackCancelled();
                if (stopWatch.ElapsedMilliseconds > findAllTimeout)
                {
                    flag = false;
                }
                else
                {
                    try
                    {
                                                {
                            topQueryId = GetModifiedQueryId(topQueryId, true);
#if COMENABLED
                            Playback.FindAllScreenElements(null, topQueryId, null, 0, 0, out retVal);
#endif
                        }
                    }
                    catch (COMException exception)
                    {
                        if (IsPlaybackCancelledComException(exception))
                        {
                            
                            throw;
                        }
                        goto Label_0037;
                    }
                    if (retVal.Length < winNumber)
                    {
                        ZappyTaskUtilities.Sleep(100);
                        goto Label_0037;
                    }
                }
            }
            finally
            {
                SearchTimeout = searchTimeout;
            }
            if (retVal != null)
            {
                object[] objArray2 = { retVal.Length };
                
            }
            return flag;
        }

        public ScreenElement FindScreenElement(string queryId) =>
            FindScreenElement(queryId, -1, true);

        private ScreenElement FindScreenElement(string queryId, bool resetSkipStep) =>
            FindScreenElement(queryId, -1, resetSkipStep);

        public ScreenElement FindScreenElement(string queryId, int depth) =>
            FindScreenElement(queryId, depth, true);

        private ScreenElement FindScreenElement(string queryId, int depth, bool resetSkipStep)
        {
            object[] args = { queryId };
            
            Utility.RestoreMinimizedWindow(TechnologyElement);
            ScreenElement[] elementArray = FindAllScreenElement(queryId, depth, false, true, resetSkipStep);
            ScreenElement element = null;
            if (elementArray.Length != 0)
            {
                element = elementArray[0];
            }
            if (OrderOfInvoke.IsEnabled)
            {
                int num;
                IntPtr ancestor = NativeMethods.GetAncestor(element.TechnologyElement.WindowHandle, NativeMethods.GetAncestorFlag.GA_ROOT);
                if (!OrderOfInvoke.OrderMapPlayback.TryGetValue(ancestor, out num))
                {
                    OrderOfInvoke.SetOrderMapPlayback(ancestor, 1);
                }
            }
            return element;
        }

        internal static ScreenElement FindScreenElementByExpandingUI(ScreenElement parentElement, string queryIdString)
        {
            ScreenElement element;
            object[] args = { queryIdString };
            
            ResetSkipStep();
            QueryId id = new QueryId(queryIdString);
            int num = -1;
            int startIndex = 0;
            if (parentElement == null)
            {
                startIndex = id.GetIndexOfExpandableElement(1);
            }
            else
            {
                startIndex = id.GetIndexOfExpandableElement(0);
            }
            string expandableParentQueryIdString = null;
            if (startIndex != 0 && id.GetIndexOfExpandableElement(startIndex) != startIndex)
            {
                expandableParentQueryIdString = id.GetQueryString(0, startIndex);
            }
            if (!TryFindScreenElement(parentElement, queryIdString, expandableParentQueryIdString, out element))
            {
                if (string.IsNullOrEmpty(expandableParentQueryIdString))
                {
                    expandableParentQueryIdString = id.GetQueryString(0, startIndex);
                }
                int num3 = 0;
                ScreenElement element2 = parentElement;
                ScreenElement element3 = null;
                if (startIndex > 0 && parentElement != null)
                {
                    element2 = FindScreenElementInSinglePass(parentElement, id.GetQueryString(0, startIndex - 1));
                    if (element2 != null)
                    {
                        num3 = startIndex;
                        string queryString = id.GetQueryString(num3, startIndex);
                        element3 = FindScreenElementInSinglePass(element2, queryString);
                        if (element3 == null)
                        {
                            num3 = 1;
                        }
                        else
                        {
                            expandableParentQueryIdString = queryString;
                            parentElement = element2;
                        }
                    }
                    else
                    {
                        element2 = parentElement;
                    }
                }
                bool firstExpansion = true;
                bool expanded = false;
                while (startIndex < id.SingleQueryIds.Count - 1)
                {
                    ScreenElement element4;
                    if (!firstExpansion || element3 == null)
                    {
                        if (parentElement == null)
                        {
                            element4 = FindFromPartialQueryId(expandableParentQueryIdString, false);
                        }
                        else
                        {
                            element4 = parentElement.FindScreenElement(expandableParentQueryIdString, false);
                        }
                        if (startIndex != num + 1)
                        {
                            element2 = null;
                        }
                    }
                    else
                    {
                        element4 = element3;
                    }
                    ExpandScreenElement(element4, element2, id.SingleQueryIds[startIndex], firstExpansion, out expanded);
                    firstExpansion = false;
                    element2 = element4;
                    num = startIndex;
                    startIndex = id.GetIndexOfExpandableElement(num + 1);
                    expandableParentQueryIdString = id.GetQueryString(num3, startIndex);
                }
                if (parentElement == null)
                {
                    element = FindFromPartialQueryId(expandableParentQueryIdString, false);
                }
                else
                {
                    element = parentElement.FindScreenElement(expandableParentQueryIdString, false);
                }
                if (expanded && ControlType.TreeItem.NameEquals(element.TechnologyElement.ControlTypeName) && UITechnologyManager.AreEqual("MSAA", element.TechnologyElement.Framework))
                {
                    try
                    {
                        element.EnsureVisible(-1, -1, 0x1a);
                    }
                    catch (COMException)
                    {
                        object[] objArray2 = { element.TechnologyElement.ToString() };
                        
                    }
                }
            }
            return element;
        }

        private static ScreenElement FindScreenElementInSinglePass(ScreenElement parentElement, string queryId)
        {
            if (string.IsNullOrEmpty(queryId))
            {
                return null;
            }
            ScreenElement element = null;
#if COMENABLED
            int playbackProperty = (int)GetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT);
            SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, 1);
#endif
            try
            {
                
                if (parentElement == null)
                {
                    return FindFromPartialQueryId(queryId, false);
                }
                element = parentElement.FindScreenElement(queryId, false);
            }
            catch (COMException exception)
            {
                if (IsPlaybackCancelledComException(exception))
                {
                    throw;
                }
            }
            finally
            {
#if COMENABLED
                SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, playbackProperty);
#endif
            }
            return element;
        }

        private static ScreenElement FindTopLevelWindowHelper(string queryId)
        {
#if COMENABLED
            bool playbackProperty = (bool)GetPlaybackProperty(ExecuteParameter.SEARCH_IN_MINIMIZED_WINDOWS);
            SmartMatchOptions options = (SmartMatchOptions)GetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS);
            int num = (int)GetPlaybackProperty(ExecuteParameter.SEARCH_TIMEOUT);
            int num2 = (int)GetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT);
            object[] foundDescendants = null;
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    CheckPlaybackCancelled();
                    try
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    SetPlaybackProperty(ExecuteParameter.SEARCH_IN_MINIMIZED_WINDOWS, true);
                                    SetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS, SmartMatchOptions.None);
                                    SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, 1);
                                    object[] args = { 1 };
                                    
                                    break;
                                }
                            case 1:
                                {
                                    SetPlaybackProperty(ExecuteParameter.SEARCH_IN_MINIMIZED_WINDOWS, playbackProperty);
                                    int num4 = -1;
                                    if (num > 0 && !TopLevelWindowSinglePassSearch)
                                    {
                                        num4 = num * 15 / 100;
                                    }
                                    SetPlaybackProperty(ExecuteParameter.SEARCH_TIMEOUT, num4);
                                    if (TopLevelWindowSinglePassSearch)
                                    {
                                        SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, 1);
                                    }
                                    else
                                    {
                                        SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, -1);
                                    }
                                    object[] objArray2 = { num4 };
                                    
                                    break;
                                }
                            default:
                                {
                                    SetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS, options);
                                    int num5 = num - num * 15 / 100;
                                    if (num > 0 && num2 > 0)
                                    {
                                        num5 = 0xbb8;
                                    }
                                    SetPlaybackProperty(ExecuteParameter.SEARCH_TIMEOUT, num5);
                                    object[] objArray3 = { num5 };
                                    
                                    break;
                                }
                        }
                                                {
                            queryId = GetModifiedQueryId(queryId, false);
                            
                            Playback.FindAllScreenElements(null, queryId, null, 0, 0, out foundDescendants);

                                                                                                                                            
                        }
                    }
                    catch (COMException exception)
                    {
                        if (IsPlaybackCancelledComException(exception))
                        {
                            
                            throw;
                        }
                        if (i == 0 || i == 1)
                        {
                            object[] objArray4 = { i, exception.ToString() };
                            
                        }
                        else
                        {
                            CrapyLogger.log.Error(exception);
                            throw;
                        }
                    }
                    if (foundDescendants != null && foundDescendants.Length != 0)
                    {
                        break;
                    }
                }
                if (foundDescendants != null && foundDescendants.Length != 0)
                {
                    IScreenElement element = foundDescendants[0] as IScreenElement;
                    ShowIfMinimized(element.TechnologyElement.WindowHandle);
                    ScreenElement element2 = new ScreenElement
                    {
                        UIElement = element
                    };
                    if ((SmartMatchOptions)GetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS) != SmartMatchOptions.None)
                    {
                                            }
                    return element2;
                }
            }
            finally
            {
                SetPlaybackProperty(ExecuteParameter.SEARCH_TIMEOUT, num);
                SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, num2);
                SetPlaybackProperty(ExecuteParameter.SEARCH_IN_MINIMIZED_WINDOWS, playbackProperty);
                SetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS, options);
            }
#endif
            return null;
        }

        internal static void FinishPlayback()
        {
            try
            {
                IRPFPlayback rpfPlayback = null;
                if (ThreadUtility.DangerousFixPlaybackThreadAccess(playback, out rpfPlayback))
                {
                    Playback = rpfPlayback;
                }
                StopSession();
                if (Playback != null)
                {
                    Playback.FinishPlayBack();
                }
                if (ThreadUtility.ThreadInfoObject != null)
                {
                    GC.KeepAlive(ThreadUtility.ThreadInfoObject);
                }
            }
            finally
            {
                startedSession = false;
                Playback = null;
                desktop = null;
                DisposeSkipEventObject();
            }
        }

        public void Flick(int pointX, int pointY, double directionInDegrees, uint flickLength, uint duration, bool ensureVisible)
        {
            ExecuteTouchAction(() => UIElement.Flick(pointX, pointY, directionInDegrees, flickLength, duration, ensureVisible, 0x7d00), ensureVisible);
        }

        internal static ScreenElement FromNativeElement(object nativeElement, string technologyName)
        {
            IScreenElement element = Playback.ScreenElementFromNativeElement(nativeElement, technologyName);
            return new ScreenElement { UIElement = element };
        }

        public static ScreenElement FromTechnologyElement(ITaskActivityElement element)
        {
            ZappyTaskUtilities.CheckForNull(element, "TechnologyElement");
            ScreenElement element2 = FromTechnologyElementInternal(element);
            ITaskActivityElement switchingElement = element.SwitchingElement;
            for (ITaskActivityElement element5 = element2.TechnologyElement; switchingElement != null; element5 = element5.SwitchingElement)
            {
                ScreenElement element3 = FromTechnologyElementInternal(switchingElement);
                element5.SwitchingElement = element3.TechnologyElement;
                switchingElement = switchingElement.SwitchingElement;
            }
            return element2;
        }

        private static ScreenElement FromTechnologyElementInternal(ITaskActivityElement technologyElement)
        {
            ScreenElement element = null;
            if (ZappyTaskUtilities.IsManagedMsaaElement(technologyElement) && !ZappyTaskUtilities.UseManagedMSAA)
            {
                element = FromNativeElement(technologyElement.NativeElement, technologyElement.Framework);
                ((TaskActivityElement)technologyElement).CopyOptionsTo(element.TechnologyElement);
                return element;
            }
            IScreenElement element2 = Playback.ScreenElementFromUITechnologyElement(technologyElement);
            return new ScreenElement { UIElement = element2 };
        }

        public void GetBoundingRectangle(out int x, out int y, out int width, out int height)
        {
            UIElement.GetBoundingRectangle(out x, out y, out width, out height);
        }

        private int GetCheckOptionsCheckedListBox()
        {
            bool flag = false;
            try
            {
                flag = ControlType.CheckBox.NameEquals(TechnologyElement.ControlTypeName) && UIElement.Parent != null && ControlType.List.NameEquals(UIElement.Parent.TechnologyElement.ControlTypeName);
            }
            catch (COMException)
            {
            }
            int num = 0;
            if (flag)
            {
                num |= 4;
                if (!IgnoreVerification)
                {
                    num |= 1;
                }
            }
            return num;
        }

        public void GetClickablePoint(out int pointX, out int pointY)
        {
            UIElement.GetClickablePoint(out pointX, out pointY);
        }

        private int GetDefaultOption(UITechnologyElementOption playbackOption)
        {
            switch (playbackOption)
            {
                case UITechnologyElementOption.SetValueAsComboBoxOptions:
                    return 0x1106;

                case UITechnologyElementOption.SetValueAsEditBoxOptions:
                    return 0x1100;

                case UITechnologyElementOption.ScrollOptions:
                    return 0x1f;
            }
            throw new NotSupportedException();
        }

        private int[] GetDefaultOptionArray(UITechnologyElementOption playbackOption)
        {
            if (playbackOption - 0x33 > UITechnologyElementOption.UseWaitForReadyLevelForElementReady)
            {
                if (playbackOption != UITechnologyElementOption.ExpandCollapseOptions)
                {
                    throw new NotSupportedException();
                }
                return new[] { 4, 0x10, 1 };
            }
            return new[] { GetDefaultOption(playbackOption) };
        }

        private static string GetModifiedQueryId(string queryId, bool addFindAll)
        {
            if (string.IsNullOrEmpty(queryId))
            {
                return queryId;
            }
            QueryId id = new QueryId(queryId);
            bool flag = false;
            IList<SingleQueryId> singleQueryIds = id.SingleQueryIds;
            foreach (SingleQueryId id2 in singleQueryIds)
            {
                if (!id2.TechnologyAttributes.Contains("Window") && (id2.TechnologyAttributes.Contains("MSAA") || IsCoreTechnologyMsaa(id2.TechnologyAttributes)))
                {
                    foreach (PropertyCondition condition in id2.SearchProperties)
                    {
                        if (condition.PropertyName.Equals("AccessibleName") || condition.PropertyName.Equals("ClassName"))
                        {
                            id2.AddTechnologyAttribute("Window");
                            flag = true;
                        }
                    }
                }
            }
            if ((singleQueryIds.Count == 1) & addFindAll)
            {
                SingleQueryId id3 = singleQueryIds[0];
                if (!id3.TechnologyAttributes.Contains("FindAll"))
                {
                    id3.AddTechnologyAttribute("FindAll");
                    flag = true;
                }
            }
            if (!flag)
            {
                return queryId;
            }
            return id.ToString();
        }

        private int GetOption(UITechnologyElementOption playbackOption)
        {
            int defaultOption = 0;
            object technologyElementOption = TaskActivityElement.GetTechnologyElementOption(TechnologyElement, playbackOption);
            if (technologyElementOption != null)
            {
                try
                {
                    defaultOption = (int)technologyElementOption;
                }
                catch (InvalidCastException)
                {
                                    }
            }
            if (defaultOption == 0)
            {
                defaultOption = GetDefaultOption(playbackOption);
            }
            return defaultOption;
        }

        private int[] GetPlaybackOptionAsArray(UITechnologyElementOption playbackOption)
        {
            int[] defaultOptionArray = null;
            object technologyElementOption = TaskActivityElement.GetTechnologyElementOption(TechnologyElement, playbackOption);
            if (technologyElementOption != null)
            {
                if (technologyElementOption is int[])
                {
                    defaultOptionArray = (int[])technologyElementOption;
                }
                else
                {
                    try
                    {
                        int num = (int)technologyElementOption;
                        if (num != 0)
                        {
                            defaultOptionArray = new[] { num };
                        }
                    }
                    catch (InvalidCastException)
                    {
                                            }
                }
            }
            if (defaultOptionArray == null)
            {
                defaultOptionArray = GetDefaultOptionArray(playbackOption);
            }
            return defaultOptionArray;
        }
#if COMENABLED
        internal static object GetPlaybackProperty(ExecuteParameter parameter) =>
            Playback.GetPlaybackProperty((int)parameter);
#endif
        public static ControlStates GetWindowState(IntPtr windowHandle)
        {
            if (NativeMethods.IsZoomed(windowHandle))
            {
                return ControlStates.Maximized;
            }
            if (NativeMethods.IsIconic(windowHandle))
            {
                return ControlStates.Minimized;
            }
            return ControlStates.None | ControlStates.Restored;
        }

        internal static void InitPlayback()
        {
            if (Playback == null)
            {
                try
                {

                                                            try
                    {
                        Playback = ScreenElementUtility.CreatePlaybackInstance();

                        
#if COMENABLED
                        ThreadUtility.SetThreadInfo(playback);
                        SetPlaybackProperty(ExecuteParameter.CONTAINER_BASED_SWITCH_SUPPORT, true);
                        SetPlaybackProperty(ExecuteParameter.ENSURE_ENABLED, false);
                        SetPlaybackProperty(ExecuteParameter.IS_PHONE, ZappyTaskUtilities.IsPhone);
                        SetPlaybackProperty(ExecuteParameter.SEARCH_RETRY_COUNT, 3);
                        SetPlaybackProperty(ExecuteParameter.SCROLL_FLAG,
                            ScrollOptions.UseKeyboard | ScrollOptions.UseClickOnScrollBar | ScrollOptions.UseScrollBar |
                            ScrollOptions.UseMouseWheel | ScrollOptions.UseProgrammatic);
                        SetPlaybackProperty(ExecuteParameter.SCROLL_MAX_CONTAINERS, 10);
                        SetPlaybackProperty(ExecuteParameter.INPUT_LOCALE_IDENTIFIER,
                            ScreenElementUtility.GetKeyboardLayout());
                        int nLoggingFlag = -1;
                                                                                                                        Playback.SetLoggingFlag(nLoggingFlag);
                        InitSkipEventObject();
#endif
                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.Error(ex);
                    }


                    startedSession = true;

                }
                catch (COMException exception)
                {
                    Playback = null;
                    startedSession = false;
                    CrapyLogger.log.Error(exception);

                    throw;
                }
            }
        }

        private static void InitSkipEventObject()
        {
            ZappyTaskUtilities.InitSkipEventObject(SkipPlayBackEventName);
        }

        private static bool IsCoreTechnologyMsaa(IList<string> technologyAttributes)
        {
            if (technologyAttributes != null && technologyAttributes.Count > 0)
            {
                foreach (string str in technologyAttributes)
                {
                    string coreTechnologyName = ZappyTaskService.Instance.GetCoreTechnologyName(str);
                    if (!string.IsNullOrEmpty(coreTechnologyName) && string.Equals("MSAA", coreTechnologyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsFocusedElement()
        {
            try
            {
                ITaskActivityElement focusedElement = ZappyTaskService.Instance.GetFocusedElement();
                ITaskActivityElement element = ZappyTaskService.Instance.ConvertTechnologyElement(TechnologyElement);
                return focusedElement.Equals(element);
            }
            catch (ZappyTaskException exception)
            {
                object[] args = { exception.Message };
                CrapyLogger.log.ErrorFormat("IsFocussedElement Failed: {0}", args);
            }
            return false;
        }

        private static bool IsPlaybackCancelledComException(COMException exception) =>
            Marshal.GetHRForException(exception) == -2147023673;

        private bool IsPopUpMenu(TaskActivityElement element)
        {
            if (!isPopUpWindow.HasValue)
            {
                isPopUpWindow = false;
                if (element != null && (ControlType.Menu.NameEquals(element.ControlTypeName) || ControlType.MenuItem.NameEquals(element.ControlTypeName)))
                {
                    isPopUpWindow = true;
                    return true;
                }
                if (element != null && ControlType.Window.NameEquals(element.ControlTypeName))
                {
                    TaskActivityElement firstChild = ZappyTaskService.Instance.GetFirstChild(element);
                    if (firstChild != null && ControlType.Menu.NameEquals(firstChild.ControlTypeName))
                    {
                        isPopUpWindow = true;
                        return true;
                    }
                }
            }
            return isPopUpWindow.Value;
        }

        public void LeftButtonClick()
        {
            LeftButtonClick(ModifierKeys.None);
        }

        public void LeftButtonClick(ModifierKeys modifierKeys)
        {
            LeftButtonClick(-1, -1, modifierKeys);
        }

        public void LeftButtonClick(int x, int y, ModifierKeys modifierKeys)
        {
            SetScrollOptionsForAction();
            UIElement.LeftButtonClick(x, y, 1, modifierKeys.ToString());
        }

                                
                                
                                
        public void MouseButtonClick(MouseButtons button)
        {
            MouseButtonClick(button, ModifierKeys.None);
        }

        public void MouseButtonClick(MouseButtons button, ModifierKeys modifierKeys)
        {
            MouseButtonClick(-1, -1, button, modifierKeys);
        }

        public void MouseButtonClick(int x, int y, MouseButtons button, ModifierKeys modifierKeys)
        {
            MouseButtonClick(x, y, button, modifierKeys, 1);
        }

        public void MouseButtonClick(int x, int y, MouseButtons button, ModifierKeys modifierKeys, int ensureVisible)
        {
            if (IsIETitleBarwithInvalidRect)
            {
                ScreenElementUtility.PerformActionOnIETitleBar(this, MouseActionType.Click, button, modifierKeys, WindowHandle, 0, -1);
            }
            else
            {
                SetScrollOptionsForAction();
                MouseButtonsPlayback buttons = Utility.ConvertToPlaybackMouseButton(button);
                UIElement.MouseButtonClick(x, y, (int)buttons, ensureVisible, modifierKeys.ToString());
            }
        }

        public void MouseHover(int x, int y, int ensureVisible, int speed, int duration)
        {
            if (IsIETitleBarwithInvalidRect)
            {
                ScreenElementUtility.PerformActionOnIETitleBar(this, MouseActionType.Hover, MouseButtons.None, ModifierKeys.None, WindowHandle, duration, speed);
            }
            else
            {
                SetScrollOptionsForAction();
                UIElement.MoveMouse(x, y, ensureVisible, speed);
                Delay(duration);
            }
        }

                                
        public void MouseWheel(int delta, ModifierKeys modifierKeys)
        {
            MouseWheel(delta, modifierKeys, true);
        }

        public void MouseWheel(int delta, ModifierKeys modifierKeys, bool setMousePosition)
        {
            int fSetMousePos = 0;
            if (setMousePosition)
            {
                fSetMousePos = 1;
            }
            SetScrollOptionsForAction();
            UIElement.MouseWheel(delta, modifierKeys.ToString(), fSetMousePos);
        }

        internal void PressAndHold(int pointX, int pointY, int duration, bool ensureVisible)
        {
            ExecuteTouchAction(() => UIElement.PressAndHold(pointX, pointY, duration, ensureVisible, 0x7d00), ensureVisible);
        }

        public void PressModifierKeys(ModifierKeys keys)
        {
            string bstrKeys = Utility.ConvertModiferKeysToString(keys);
            if (BringUpIfTopWindow())
            {
                Playback.TypeString(bstrKeys, SendKeysDelay, 0, 1);
            }
            else
            {
                SetScrollOptionsForAction();
                UIElement.SendKeys(bstrKeys, 1, 0);
            }
        }

        public static void PressModifierKeysStatic(ModifierKeys keys)
        {
            string bstrKeys = Utility.ConvertModiferKeysToString(keys);
            Playback.TypeString(bstrKeys, SendKeysDelay, 0, 1);
        }

        public void ReleaseModifierKeys(ModifierKeys keys)
        {
            string bstrKeys = Utility.ConvertModiferKeysToString(keys);
            if (BringUpIfTopWindow())
            {
                Playback.TypeString(bstrKeys, SendKeysDelay, 0, 2);
            }
            else
            {
                SetScrollOptionsForAction();
                UIElement.SendKeys(bstrKeys, 2, 0);
            }
        }

        public static void ReleaseModifierKeysStatic(ModifierKeys keys)
        {
            string bstrKeys = Utility.ConvertModiferKeysToString(keys);
            Playback.TypeString(bstrKeys, SendKeysDelay, 0, 2);
        }

                                                        
        internal static void ResetSkipStep()
        {
            if (ZappyTaskUtilities.CanResetSkipStep())
            {
                Playback.ResetSkipStep();
            }
        }

                                
                                
                                                                                                
        public void Select()
        {
            SetScrollOptionsForAction();
            UIElement.Select(1, !IgnoreVerification);
        }

        private void SelectContent()
        {
            UIElement.SendKeys("^{HOME}", 3, 1);
            if (!IsFocusedElement())
            {
                SingleLineSelectContent();
            }
            else
            {
                UIElement.SendKeys("^+{END}", 3, 1);
                if (!IsFocusedElement())
                {
                    SingleLineSelectContent();
                }
            }
        }

        public void SendKeys(string keys)
        {
            SendKeys(keys, true);
        }

        public void SendKeys(string keys, bool isUnicode)
        {
#if COMENABLED
            object playbackProperty = (TypeUnicodeConstants)GetPlaybackProperty(ExecuteParameter.TYPE_UNICODE);
            SetPlaybackProperty(ExecuteParameter.TYPE_UNICODE, isUnicode ? TypeUnicodeConstants.TYPE_UNICODE_AUTOMATIC : TypeUnicodeConstants.TYPE_UNICODE_DISABLE);
            try
            {
                if (BringUpIfTopWindow())
                {
                    Playback.TypeString(keys, SendKeysDelay, 0, 3);
                }
                else
                {
                    SetScrollOptionsForAction();
                                        UIElement.SendKeys(keys, 3, 0);
                }
            }
            finally
            {
                SetPlaybackProperty(ExecuteParameter.TYPE_UNICODE, playbackProperty);
            }
#endif
        }

        public void SendKeys(string keys, ModifierKeys modifierKeys, bool isUnicode)
        {
            string str = Utility.ConvertModiferKeysToString(modifierKeys, keys);
            SendKeys(str, isUnicode);
        }

        public void SendKeys(string keys, ModifierKeys modifierKeys, bool isEncoded, bool isUnicode)
        {
            if (isEncoded)
            {
                if (UIElement == null || !UIElement.TechnologyElement.IsPassword)
                {
                    throw new ArgumentException(Resources.EncryptedText);
                }
                int nLoggingFlag = Utility.DisablePlaybackLoggingContent(Playback);
                try
                {
                    keys = EncodeDecode.DecodeString(keys);
                    SendKeys(keys, modifierKeys, isUnicode);
                }
                finally
                {
                    Playback.SetLoggingFlag(nLoggingFlag);
                }
            }
            else
            {
                SendKeys(keys, modifierKeys, isUnicode);
            }
        }

        public void SendKeysDeleteContent(string keysToSend)
        {
            DeleteContent();
            SendKeys(keysToSend);
        }

        public void SetFocus()
        {
            if (!BringUpIfTopWindow())
            {
                UIElement.SetFocus();
            }
        }

        internal static void SetPlaybackProperty(ExecuteParameter parameter, object value)
        {
#if COMENABLED
            if (Playback != null)
            {
                Playback.SetPlaybackProperty((int)parameter, value);
            }
#endif
        }

        private void SetScrollOptionsForAction()
        {
            SetPlaybackProperty(ExecuteParameter.SCROLL_FLAG, GetOption(UITechnologyElementOption.ScrollOptions));
        }

        public void SetValueAsComboBox(string value, bool useEdit)
        {
            SetScrollOptionsForAction();
            if (value == null)
            {
                value = string.Empty;
            }
            ResetSkipStep();
            if (useEdit)
            {
                try
                {
                    if (ScreenElementUtility.ControlWindowUsesConfiguredIMELanguage(UIElement))
                    {
                        SetValueAsComboBoxInternal(value, SetValueAsComboBoxType.UseCopyPaste);
                    }
                    else
                    {
                        SetValueAsComboBoxInternal(value, SetValueAsComboBoxType.UseEditOnly);
                    }
                }
                catch (COMException exception)
                {
                    uint hRForException = 0;
                    hRForException = (uint)Marshal.GetHRForException(exception);
                    if (hRForException != 0xf004f005 && hRForException != 0xf004f006)
                    {
                        throw;
                    }
                    object[] args = { exception };
                    
                    SetValueAsComboBoxInternal(value, SetValueAsComboBoxType.UseSelectOnly);
                }
            }
            else if (ScreenElementUtility.ControlWindowUsesConfiguredIMELanguage(UIElement))
            {
                try
                {
                    SetValueAsComboBoxInternal(value, SetValueAsComboBoxType.UseSelectOnly);
                }
                catch (COMException exception2)
                {
                    uint num2 = 0;
                    num2 = (uint)Marshal.GetHRForException(exception2);
                    object[] objArray2 = { num2 };
                    
                    SetValueAsComboBoxInternal(value, SetValueAsComboBoxType.UseCopyPaste);
                }
            }
            else
            {
                SetValueAsComboBoxInternal(value, SetValueAsComboBoxType.UseSelectOnly);
            }
        }

        private void SetValueAsComboBoxInternal(string value, SetValueAsComboBoxType setValueAsComboBoxType)
        {
            int option = GetOption(UITechnologyElementOption.SetValueAsComboBoxOptions);
            switch (setValueAsComboBoxType)
            {
                case SetValueAsComboBoxType.UseSelectOnly:
                    option &= -16129;
                    option &= -5;
                    option |= 2;
                    break;

                case SetValueAsComboBoxType.UseEditOnly:
                    option &= -3;
                    option |= 4;
                    break;

                case SetValueAsComboBoxType.UseQueryId:
                    option &= -16129;
                    option &= -5;
                    option |= 0x20;
                    break;

                case SetValueAsComboBoxType.UseCopyPaste:
                    option &= -16129;
                    option &= -3;
                    option |= 4;
                    option |= 0x900;
                    break;
            }
            if (IgnoreVerification)
            {
                option |= 8;
            }
            UIElement.SetValueAsComboBox(value, option);
        }

        public void SetValueAsComboBoxUsingQueryId(string value)
        {
            SetScrollOptionsForAction();
            if (value == null)
            {
                value = string.Empty;
            }
            SetValueAsComboBoxInternal(value, SetValueAsComboBoxType.UseQueryId);
        }

        public void SetValueAsEditBox(string value)
        {
            SetValueAsEditBox(value, false);
        }

        public void SetValueAsEditBox(string value, bool isEncoded)
        {
            SetValueAsEditBoxOptions option = (SetValueAsEditBoxOptions)GetOption(UITechnologyElementOption.SetValueAsEditBoxOptions);
            if (!isEncoded && ScreenElementUtility.ControlWindowUsesConfiguredIMELanguage(UIElement))
            {
                
                option = SetValueAsEditBoxOptions.UseCopyPaste | SetValueAsEditBoxOptions.DeleteContent;
            }
            UIElement.SetFocus();
            if ((option & SetValueAsEditBoxOptions.DeleteContent) == SetValueAsEditBoxOptions.DeleteContent && (TechnologyElement.IsPassword || !string.IsNullOrEmpty(TechnologyElement.Value)))
            {
                DeleteContent();
            }
            else
            {
                
            }
            option &= ~SetValueAsEditBoxOptions.DeleteContent;
            SetValueAsEditBox(value, isEncoded, (int)option);
        }

        public void SetValueAsEditBox(string value, int flag)
        {
            SetValueAsEditBox(value, false, flag);
        }

        public void SetValueAsEditBox(string value, bool isEncoded, int flag)
        {
            SetScrollOptionsForAction();
            if (value == null)
            {
                value = string.Empty;
            }
            if (IgnoreVerification)
            {
                flag |= 0x2000;
            }
            if (isEncoded)
            {
                if (UIElement == null || !UIElement.TechnologyElement.IsPassword)
                {
                    throw new ArgumentException(Resources.EncryptedText);
                }
                int nLoggingFlag = Utility.DisablePlaybackLoggingContent(Playback);
                try
                {
                    value = EncodeDecode.DecodeString(value);
                    UIElement.SetValueAsEditBox(value, flag);
                }
                finally
                {
                    Playback.SetLoggingFlag(nLoggingFlag);
                }
            }
            else
            {
                UIElement.SetValueAsEditBox(value, flag);
            }
        }

        public void SetValueAsListBox(string value)
        {
            SetValueAsListBox(CommaListBuilder.GetCommaSeparatedValues(value).ToArray());
        }

        public void SetValueAsListBox(string[] values)
        {
            SetValueAsListBox(values, false);
        }

        public void SetValueAsListBox(string[] values, bool isQueryIdArray)
        {
            SetScrollOptionsForAction();
            ResetSkipStep();
            List<string> list = new List<string>();
            ITaskActivityElement technologyElement = TechnologyElement;
            if (values != null)
            {
                int length = Array.FindAll(values, str => str == null).Length;
                if (length > 0 && length != values.Length)
                {
                                    }
                if (length != values.Length)
                {
                    if (isQueryIdArray)
                    {
                        list = new List<string>(values);
                    }
                    else
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            int num2;
                            list.Add(technologyElement.GetQueryIdForRelatedElement(ZappyTaskElementKind.Child, values[i], out num2));
                        }
                    }
                }
            }
            List<object> list2 = new List<object>(list.ToArray());
            UIElement.DoSelectByMouseClick(list2.ToArray(), "{ControlKey}");
        }

        public void SetValueAsScrollBar(string value, ZappyTaskElementKind thumbType)
        {
            ResetSkipStep();
            if (value == null)
            {
                value = string.Empty;
            }
            if (!string.Equals(TechnologyElement.Value, value, StringComparison.OrdinalIgnoreCase))
            {
                TechnologyElement.Value = value;
                string queryId = string.Empty;
                try
                {
                    int num;
                    queryId = TechnologyElement.GetQueryIdForRelatedElement(thumbType, null, out num);
                }
                catch (Exception exception)
                {
                    if (!(exception is NotSupportedException) && !(exception is NotImplementedException))
                    {
                        throw;
                    }
                    object[] args = { TechnologyElement.Framework, ControlType.Indicator.Name };
                    queryId = string.Format(CultureInfo.InvariantCulture, ";[{0}]ControlType='{1}'", args);
                }
                ScreenElement element = FindScreenElementInSinglePass(this, queryId);
                if (element == null)
                {
                    CrapyLogger.log.ErrorFormat("Couldn't find indicator, value is set via put_accValue");
                    throw new Exception();
                }
                try
                {
                    element.LeftButtonClick();
                }
                catch (COMException exception2)
                {
                    CrapyLogger.log.ErrorFormat("Couldn't click on the indicator as it is obscured by another element");
                    throw exception2;
                }
            }
        }

        public void SetValueAsSlider(string value)
        {
            SetValueAsSlider(value, 0);
        }

        public void SetValueAsSlider(string value, int orientation)
        {
            SetScrollOptionsForAction();
            if (value == null)
            {
                value = string.Empty;
            }
            UIElement.SetValueAsSlider(value, orientation);
        }

        public void SetWindowState(ControlStates stateToSet)
        {
            object[] args = { stateToSet.ToString(), UIElement == null ? string.Empty : TechnologyElement.Name };
            string message = string.Format(CultureInfo.CurrentCulture, Resources.SetStateException, args);
            NativeMethods.SetForegroundWindow(WindowHandle);
            if (stateToSet != (ControlStates.None | ControlStates.Restored))
            {
                if (stateToSet != ControlStates.Maximized)
                {
                    if (stateToSet != ControlStates.Minimized)
                    {
                        object[] objArray2 = { stateToSet.ToString(), "SetState" };
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.StateNotSupported, objArray2));
                    }
                    NativeMethods.ShowWindow(WindowHandle, NativeMethods.WindowShowStyle.ShowMinimized);
                }
                else
                {
                    NativeMethods.ShowWindow(WindowHandle, NativeMethods.WindowShowStyle.ShowMaximized);
                }
            }
            else
            {
                NativeMethods.ShowWindow(WindowHandle, NativeMethods.WindowShowStyle.Restore);
            }
            ControlStates windowState = GetWindowState(WindowHandle);
            if (windowState != stateToSet && (stateToSet != (ControlStates.None | ControlStates.Restored) || ControlStates.Minimized == windowState))
            {
                throw new NotSupportedException(message);
            }
        }

        private static void ShowIfMinimized(IntPtr windowHandle)
        {
            if (NativeMethods.IsIconic(windowHandle) && NativeMethods.IsWindowVisible(windowHandle))
            {
                object[] args = { windowHandle };
                
                NativeMethods.ShowWindow(windowHandle, NativeMethods.WindowShowStyle.Restore);
            }
        }

        private void SingleLineDeleteContent()
        {
            UIElement.SendKeys("{HOME}", 3, 0);
            UIElement.SendKeys("+{END}", 3, 1);
            UIElement.SendKeys("{DELETE}", 3, 1);
        }

        private void SingleLineSelectContent()
        {
            UIElement.SendKeys("{HOME}", 3, 0);
            UIElement.SendKeys("+{END}", 3, 1);
        }

        internal static void SkipStep()
        {
            ZappyTaskUtilities.SkipStep();
        }

        public void Slide(int pointX, int pointY, double directionInDegrees, uint slideLength, uint duration, bool ensureVisible)
        {
            ExecuteTouchAction(() => UIElement.Slide(pointX, pointY, directionInDegrees, slideLength, duration, ensureVisible, 0x7d00), ensureVisible);
        }

        public void StartDragging(int offsetX, int offsetY, MouseButtons button, ModifierKeys modifierKeys, bool ensureVisible)
        {
            int nEnsureVisibleFlag = 0;
            if (ensureVisible)
            {
                nEnsureVisibleFlag = 1;
                UIElement.EnsureVisible(nEnsureVisibleFlag, offsetX, offsetY, null, GetOption(UITechnologyElementOption.ScrollOptions), 2);
            }
            MouseButtonsPlayback buttons = Utility.ConvertToPlaybackMouseButton(button);
            UIElement.StartDragging(offsetX, offsetY, (int)buttons, modifierKeys.ToString(), nEnsureVisibleFlag);
        }

        internal static void StartSession()
        {
            if (Playback != null)
            {
                if (!startedSession)
                {
#if COMENABLED
                    ResetSkipStep();
                    Playback.StartSession();
#endif

                    startedSession = true;
                }
#if COMENABLED
                KeyboardInputUtility.SetKeyState();
                if (!addedTechnologyManagers)
                {
                    addedTechnologyManagers = true;
                    foreach (UITechnologyManager manager in ZappyTaskService.Instance.PluginManager.TechnologyManagers)
                    {
                        if (!string.Equals(manager.TechnologyName, "MSAA", StringComparison.OrdinalIgnoreCase))
                        {
                            Playback.AddTechnologyManager(manager);
                        }
                    }
                }
#endif
            }
        }

        public void StopDragging(int offsetX, int offsetY, int mouseDragSpeed)
        {
            SetScrollOptionsForAction();
            UIElement.StopDragging(offsetX, offsetY, mouseDragSpeed);
        }

        internal static void StopSession()
        {
            
            if (startedSession)
            {
#if COMENABLED

                IRPFPlayback rpfPlayback = null;
                if (ThreadUtility.DangerousFixPlaybackThreadAccess(playback, out rpfPlayback))
                {
                    Playback = rpfPlayback;
                }
#endif
                if (Playback != null)
                {
                    KeyboardInputUtility.RestoreKeyState();
#if COMENABLED
                    Playback.StopSession();
#endif
                    startedSession = false;
                    if (addedTechnologyManagers)
                    {
                        foreach (UITechnologyManager manager in ZappyTaskService.Instance.PluginManager.TechnologyManagers)
                        {
                            if (!string.Equals(manager.TechnologyName, "MSAA", StringComparison.OrdinalIgnoreCase))
                            {
                                Playback.RemoveTechnologyManager(manager);
                            }
                        }
                        addedTechnologyManagers = false;
                    }
                }
                startedSession = false;
            }
            
        }

        public void Swipe(int pointX, int pointY, double directionInDegrees, uint swipeLength, bool ensureVisible)
        {
            ExecuteTouchAction(() => UIElement.Swipe(pointX, pointY, directionInDegrees, swipeLength, ensureVisible, 0x7d00), ensureVisible);
        }

        public void Tap(int pointX, int pointY, bool ensureVisible)
        {
            ExecuteTouchAction(() => UIElement.Tap(pointX, pointY, ensureVisible, 0x7d00), ensureVisible);
        }

        private static bool TryFindScreenElement(ScreenElement parentElement, string queryIdString, string expandableParentQueryIdString, out ScreenElement element)
        {
            element = null;
            bool flag = false;
#if COMENABLED
            try
            {
                int num;
                int num2;
                object[] args = { queryIdString };
                
                bool playbackProperty = (bool)GetPlaybackProperty(ExecuteParameter.EXACT_QID_MATCH);
                try
                {
                    SetPlaybackProperty(ExecuteParameter.EXACT_QID_MATCH, true);
                    element = FindScreenElementInSinglePass(parentElement, queryIdString);
                }
                finally
                {
                    SetPlaybackProperty(ExecuteParameter.EXACT_QID_MATCH, playbackProperty);
                }
                if (element == null)
                {
                    return flag;
                }
                object[] objArray2 = { queryIdString };
                
                
                element.GetClickablePoint(out num, out num2);
                
                object[] objArray3 = { expandableParentQueryIdString };
                
                ScreenElement element2 = FindScreenElementInSinglePass(parentElement, expandableParentQueryIdString);
                Point point = new Point(num, num2);
                if (element2 != null && ControlType.MenuItem.NameEquals(element2.TechnologyElement.ControlTypeName))
                {
                    int num3;
                    int num4;
                    object[] objArray4 = { expandableParentQueryIdString };
                    
                    
                    element2.GetBoundingRectangle(out num, out num2, out num3, out num4);
                    Rectangle rectangle = new Rectangle(num, num2, num3, num4);
                    if (!rectangle.Contains((int)point.X, (int)point.Y))
                    {
                        
                        return true;
                    }
                    
                    try
                    {
                        
                        element2.TechnologyElement.InvokeProgrammaticAction(ProgrammaticActionOption.DefaultAction);
                        return flag;
                    }
                    catch (COMException exception)
                    {
                        if (IsPlaybackCancelledComException(exception))
                        {
                            throw;
                        }
                        return flag;
                    }
                    catch (ExecuteCanceledException)
                    {
                        throw;
                    }
                    catch (ZappyTaskException)
                    {
                        return flag;
                    }
                    catch (NotSupportedException)
                    {
                        return flag;
                    }
                    catch (NotImplementedException)
                    {
                        return flag;
                    }
                }
                flag = true;
            }
            catch (COMException exception2)
            {
                if (IsPlaybackCancelledComException(exception2))
                {
                    throw;
                }
            }
            catch (ExecuteCanceledException)
            {
                throw;
            }
            catch (ZappyTaskException)
            {
            }
#endif
            return flag;
        }

        public void Turn(int point1X, int point1Y, int point2X, int point2Y, double directionInDegrees, bool ensureVisible)
        {
            ExecuteTouchAction(() => UIElement.Turn(point1X, point1Y, point2X, point2Y, directionInDegrees, ensureVisible, 0x7d00, 0x2d), ensureVisible);
        }

        public static void TypeString(string text, ModifierKeys modifierKeys, bool isEncoded, bool isUnicode)
        {
            string dataToDecode = text;
#if COMENABLED
            TypeUnicodeConstants playbackProperty = (TypeUnicodeConstants)GetPlaybackProperty(ExecuteParameter.TYPE_UNICODE);
#endif
            SetPlaybackProperty(ExecuteParameter.TYPE_UNICODE, isUnicode ? TypeUnicodeConstants.TYPE_UNICODE_AUTOMATIC : TypeUnicodeConstants.TYPE_UNICODE_DISABLE);
            try
            {
                if (isEncoded)
                {
                    int nLoggingFlag = Utility.DisablePlaybackLoggingContent(Playback);
                    try
                    {
                        dataToDecode = EncodeDecode.DecodeString(dataToDecode);
                        dataToDecode = Utility.ConvertModiferKeysToString(modifierKeys, dataToDecode);
                        Playback.TypeString(dataToDecode, SendKeysDelay, 0, 3);
                        return;
                    }
                    finally
                    {
                        Playback.SetLoggingFlag(nLoggingFlag);
                    }
                }
                dataToDecode = Utility.ConvertModiferKeysToString(modifierKeys, text);
                Playback.TypeString(dataToDecode, SendKeysDelay, 0, 3);
            }
            finally
            {
#if COMENABLED
                SetPlaybackProperty(ExecuteParameter.TYPE_UNICODE, playbackProperty);
#endif
            }
        }

        public void Uncheck()
        {
            SetScrollOptionsForAction();
            int nCheckUncheckFlag = GetCheckOptionsCheckedListBox() | 2;
            UIElement.Uncheck(nCheckUncheckFlag);
        }

        public void UncheckTreeItem()
        {
            SetScrollOptionsForAction();
            UIElement.Select(2, !IgnoreVerification);
            UIElement.SendKeys("{Space}", 3, 0);
        }

        public void WaitForReady(int millisecondsTimeout)
        {
            int waitForReadyTimeout = WaitForReadyTimeout;
            WaitForReadyTimeout = millisecondsTimeout;
            try
            {
                if (UIElement != null)
                {
                    UIElement.WaitForReady();
                }
            }
            finally
            {
                WaitForReadyTimeout = waitForReadyTimeout;
            }
        }

        private static void WPFCollapseMenu(ScreenElement element, ScreenElement parentElement)
        {
            ITaskActivityElement technologyElement = parentElement.TechnologyElement;
            technologyElement = ZappyTaskService.Instance.ConvertTechnologyElement(technologyElement);
            if (ControlType.Menu.NameEquals(technologyElement.ControlTypeName))
            {
                string str = technologyElement.ClassName != null ? technologyElement.ClassName : string.Empty;
                if (!str.Contains("ContextMenu"))
                {
                    try
                    {
                        element.MouseHover(5, 5, 1, -1, 0);
                    }
                    catch (COMException)
                    {
                        
                    }
                    if (TaskActivityElement.IsState(element.TechnologyElement, AccessibleStates.Expanded))
                    {
                        
                        try
                        {
                            element.LeftButtonClick();
                        }
                        catch (COMException)
                        {
                            
                        }
                    }
                }
            }
        }

        public void Zoom(int point1X, int point1Y, int point2X, int point2Y, int zoomLength, bool ensureVisible)
        {
            ExecuteTouchAction(() => UIElement.Zoom(point1X, point1Y, point2X, point2Y, zoomLength, ensureVisible, 0x7d00, 50), ensureVisible);
        }

        public static bool DebugModeOn
        {
            get =>
                isDebugModeOn;
            set
            {
                if (isDebugModeOn != value)
                {
                    isDebugModeOn = value;
                    if (isDebugModeOn)
                    {
                        Playback.SetDebugMode(0);
                    }
                    else
                    {
                        Playback.SetDebugMode(0);
                    }
                }
            }
        }

        public static ScreenElement Desktop
        {
            get
            {
                if (desktop == null)
                {
                    IntPtr desktopWindow = NativeMethods.GetDesktopWindow();
                    IScreenElement element = Playback.ScreenElementFromWindow(desktopWindow.ToInt32());
                    desktop = new ScreenElement();
                    desktop.UIElement = element;
                }
                return desktop;
            }
        }

        public static bool IgnoreVerification { get; set; }


        public static ICollection<int> ImeLanguageList =>
            imeLanguageList;

        private bool IsIETitleBarwithInvalidRect
        {
            get
            {
                if (ControlType.TitleBar.NameEquals(TechnologyElement.ControlTypeName) && "IEFrame".Equals(TechnologyElement.ClassName, StringComparison.OrdinalIgnoreCase))
                {
                    int num;
                    int num2;
                    int num3;
                    int num4;
                    TechnologyElement.GetBoundingRectangle(out num, out num2, out num3, out num4);
                    if (num4 < 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        internal static bool IsSkipStepOn =>
            ZappyTaskUtilities.IsSkipStepOn;

        internal static ILastInvocationInfo LastSearchInfo
        {
            get =>
                lastSearchInfo;
            set
            {
                lastSearchInfo = value;
            }
        }

        public static bool MatchExactHierarchy
        {
            get =>
                matchExactHierarchy;
            set
            {
                if (matchExactHierarchy != value && Playback != null)
                {
                    matchExactHierarchy = value;
                    SetPlaybackProperty(ExecuteParameter.EXACT_QID_MATCH, matchExactHierarchy);
                }
            }
        }

        public ScreenElement Parent =>
            new ScreenElement { UIElement = UIElement.Parent };

        internal static IRPFPlayback Playback
        {
            get
            {
                ThreadUtility.ThrowExceptionIfCrossThreadAccess(playback, null);
                return playback;
            }
            private set
            {
                playback = value;
                if (playback != null)
                {
                    ThreadUtility.CacheThreadInfo();
                }
            }
        }

        public static bool SearchInMinimizedWindows
        {
            get =>
                searchInMinimizedWindows;
            set
            {
                if (searchInMinimizedWindows != value && Playback != null)
                {
                    searchInMinimizedWindows = value;
                    SetPlaybackProperty(ExecuteParameter.SEARCH_IN_MINIMIZED_WINDOWS, value);
                }
            }
        }

        public static int SearchTimeout
        {
            get =>
                searchTimeout;
            set
            {
                if (searchTimeout != value && Playback != null)
                {
                    searchTimeout = value;
                    findAllTimeout = searchTimeout / 10;
                    SetPlaybackProperty(ExecuteParameter.SEARCH_TIMEOUT, value);
                }
            }
        }

        public static bool SendKeysAsScanCode
        {
#if COMENABLED
            get =>
                (TypeUnicodeConstants)GetPlaybackProperty(ExecuteParameter.TYPE_UNICODE) != TypeUnicodeConstants.TYPE_UNICODE_AUTOMATIC;
            set
            {
                SetPlaybackProperty(ExecuteParameter.TYPE_UNICODE, value ? TypeUnicodeConstants.TYPE_UNICODE_DISABLE : TypeUnicodeConstants.TYPE_UNICODE_AUTOMATIC);
            }
#else

            get;
            set;
#endif
        }

        public static int SendKeysDelay
        {
            get =>
                0;
            set
            {
                sendKeysDelay = 0;
                                                                                            }
        }

        public static SmartMatchOptions SmartMatchOptions
        {
            get =>
                smartMatchOptions;
            set
            {
                if (smartMatchOptions != value && Playback != null)
                {
                    smartMatchOptions = value;
                    SetPlaybackProperty(ExecuteParameter.SMART_MATCH_OPTIONS, value);
                }
            }
        }

        public ITaskActivityElement TechnologyElement =>
            technologyElement;

        internal static bool TopLevelWindowSinglePassSearch
        { get; set; }

        private IScreenElement UIElement
        {
            get
            {
                ThreadUtility.ThrowExceptionIfCrossThreadAccess(playback, uiElement);
                return uiElement;
            }
            set
            {
                uiElement = value;
                if (uiElement != null)
                {
                    technologyElement = uiElement.TechnologyElement;
                }
            }
        }

        internal static WaitForReadyLevel WaitForReadyLevel
        {
#if COMENABLED
            get =>
                (WaitForReadyLevel)GetPlaybackProperty(ExecuteParameter.WAIT_FOR_READY_LEVEL);
            set
            {
                SetPlaybackProperty(ExecuteParameter.WAIT_FOR_READY_LEVEL, value);
            }
#else

            get;
            set;
#endif
        }

        public static int WaitForReadyTimeout
        {
            get =>
                waitForReadyTimeout;
            set
            {
                if (waitForReadyTimeout != value && Playback != null)
                {
                    waitForReadyTimeout = value;
                    SetPlaybackProperty(ExecuteParameter.WFR_TIMEOUT, value);
                }
            }
        }

        private IntPtr WindowHandle =>
            TechnologyElement.WindowHandle;

                                        
                    }
}