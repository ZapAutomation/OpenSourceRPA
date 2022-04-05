using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;
using AndCondition = Zappy.ActionMap.HelperClasses.AndCondition;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;
using DataGridUtility = Zappy.Plugins.Uia.Uia.Utilities.DataGridUtility;
using PropertyCondition = Zappy.ActionMap.HelperClasses.PropertyCondition;

namespace Zappy.Plugins.Uia.Uia
{
#if COMENABLED
    [ComVisible(true), Guid("17D90066-B2C1-45AB-8A56-F80234D00513") ]
#endif
    public sealed class UiaTechnologyManager : UITechnologyManager, IDisposable
    {
        private bool disposed;
        private Dictionary<ElementEventSink, EventWrapper> elementEventSinkMap = new Dictionary<ElementEventSink, EventWrapper>();
        private static readonly IList<ControlType> exactMatchControlTypes = InitializeExactMatchControlTypes();
        private static UiaTechnologyManager instance = new UiaTechnologyManager();
        private UiaElement lastAccessedComboBox;
        private UiaElement lastAccessedDatePicker;
        private UiaElement lastAccessedMenu;
        private EventWrapper lastEventForNotify;
        private AutomationElement lastFocussedComboBox;
        private bool lastFocusWasOnComboBox;
        private ILastInvocationInfo lastInfo;
        private IQueryCondition lastSearchCondition;
        private string lastSearchQueryId;
        private UiaElement lastStitchedListElement;
        private UiaElement lastUnstitchedListElement;
        private IntPtr menuEventProcessHandle;
        private ElementEventSink menuEventSink;
        private Dictionary<AutomationElement, AutomationElement> menuItemStitchMap = new Dictionary<AutomationElement, AutomationElement>();
        private Stack<UiaElement> propertyChangeMenuItemStack = new Stack<UiaElement>();
        private Dictionary<IQueryCondition, ITaskActivityElement> rowElementCache = new Dictionary<IQueryCondition, ITaskActivityElement>();
        private static readonly Regex searchConfigurationRegex = new Regex(@";(\[.*])?");
        private bool sessionStarted;
        private readonly object syncLock = new object();
        private UiaWorker uiaWorker = new UiaWorker();
        internal const int WindowsFormsHostSupportLevel = 0x65;

        internal UiaTechnologyManager()
        {
            InitializeTechnologyManagerProperties();
        }

        public override bool AddEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            UiaElement element2 = UiaUtility.TransformUiaElement(element);
            if (element2 != null)
            {
                object syncLock = this.syncLock;
                lock (syncLock)
                {
                    FireFakeEvent();
                    if (element2.ControlTypeName == ControlType.Button && string.Equals(UiaUtility.GetAutomationPropertyValue<string>(element2.InnerElement, AutomationElement.AutomationIdProperty), "PART_Button", StringComparison.Ordinal) && element2.Parent != null && element2.Parent.ControlTypeName == ControlType.DatePicker)
                    {
                        LastAccessedDatePicker = element2.Parent;
                    }
                    if (element2.ControlTypeName == ControlType.ComboBox)
                    {
                        LastAccessedComboBox = element2;
                    }
                    else if (element2.ControlTypeName == ControlType.Cell)
                    {
                        AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(element2.InnerElement);
                        if (firstChild != null && firstChild.Current.ControlType == System.Windows.Automation.ControlType.ComboBox)
                        {
                            LastAccessedComboBox = UiaElementFactory.GetUiaElement(firstChild, false);
                        }
                    }
                    UpdateMenuEventCache(element2, eventSink);
                    ElementEventSink key = new ElementEventSink(element2, eventType, eventSink);
                    if (!elementEventSinkMap.ContainsKey(key))
                    {
                        try
                        {
                            EventWrapper wrapper;
                            if (LastAccessedMenu != null)
                            {
                                if (PatternHelper.GetTogglePattern(element2.InnerElement) != null)
                                {
                                    wrapper = EventWrapper.GetEventWrapper(element2, eventType, eventSink);
                                }
                                else
                                {
                                    wrapper = EventWrapper.GetEventWrapper(LastAccessedMenu, eventType, eventSink);
                                }
                            }
                            else
                            {
                                wrapper = EventWrapper.GetEventWrapper(element2, eventType, eventSink);
                            }
                            if (wrapper != null)
                            {
                                if (wrapper.NotifyEventDuringRemoval)
                                {
                                    lastEventForNotify = wrapper;
                                }
                                elementEventSinkMap.Add(key, wrapper);
                                return true;
                            }
                        }
                        catch (Exception exception)
                        {
                            UiaUtility.MapAndThrowException(exception, element, false);
                            throw;
                        }
                    }
                }
            }
            return false;
        }

        public override bool AddGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) =>
            false;

        private bool AnalyzeElementFromPoint(UiaElement element)
        {
            UiaElement parent = element.Parent;
            if (parent != null && parent.ControlTypeName == ControlType.Calendar)
            {
                return false;
            }
            return true;
        }

        public override void CancelStep()
        {
        }

        private void ClearSessionRelatedCache()
        {
            menuEventProcessHandle = IntPtr.Zero;
            lastAccessedComboBox = null;
            lastAccessedMenu = null;
            lastFocussedComboBox = null;
            lastStitchedListElement = null;
            lastUnstitchedListElement = null;
            elementEventSinkMap.Clear();
            PropertyChangeMenuItemStack.Clear();
            MenuItemStitchMap.Clear();
            rowElementCache.Clear();
        }

        public override ITaskActivityElement ConvertToThisTechnology(ITaskActivityElement elementToConvert, out int supportLevel)
        {
            supportLevel = 0;
            if (UiaUtility.IsUiaElement(elementToConvert))
            {
                supportLevel = 100;
                return elementToConvert;
            }
            return UiaUtility.CheckForLegacyHostElements(this, elementToConvert, out supportLevel);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                StopSession();
                uiaWorker.StopWorkerThread();
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        internal bool DoMenuItemStitch(UiaElement element, ref UiaElement ancestor)
        {
            bool flag = false;
            if (LastAccessedMenu == null || ancestor.ControlTypeName != ControlType.Window || PropertyChangeMenuItemStack.Count <= 0)
            {
                return flag;
            }
            UiaElement parent = null;
            if (!MenuItemStitchMap.ContainsKey(element.InnerElement))
            {
                List<UiaElement> list = PropertyChangeMenuItemStack.ToList();
                int num = list.IndexOf(element) + 1;
                if (num >= 0 && num < list.Count)
                {
                    parent = list[num];
                }
            }
            else
            {
                parent = UiaElementFactory.GetUiaElement(MenuItemStitchMap[element.InnerElement], false);
            }
            if (!ShouldStitchMenuItem(element, parent))
            {
                return flag;
            }
            ancestor = parent;
            if (MenuItemStitchMap.ContainsKey(element.InnerElement))
            {
                MenuItemStitchMap.Remove(element.InnerElement);
            }
            MenuItemStitchMap.Add(element.InnerElement, ancestor.InnerElement);
            return true;
        }

        private void FireFakeEvent()
        {
            if (lastEventForNotify != null && lastEventForNotify.ShouldFireFakeEvent())
            {
                object[] args = { lastEventForNotify.Element };
                
                lastEventForNotify.Notify();
            }
            lastEventForNotify = null;
        }

        public override IEnumerator GetChildren(ITaskActivityElement element, object parsedQueryIdCookie)
        {
            UiaElement uiaElement = UiaUtility.TransformUiaElement(element);
            if (VirtualUiaElementUtility.NeedVirtualizedChildren(uiaElement))
            {
                object[] args = { uiaElement.FriendlyName, uiaElement.ControlTypeName };
                string infoMessage = string.Format(CultureInfo.CurrentCulture, Resources.IntermediateVirtualizedControlSearchFailure, args);
                lastInfo = new UiaSearchInfo(infoMessage, 1);
                return new VirtualChildrenEnumerator(uiaElement, parsedQueryIdCookie);
            }
            return new ChildrenEnumerator(uiaElement);
        }

        private UiaElement GetComboBoxElement(UiaElement comboBoxElement, UiaElement unstitchedListItem)
        {
            if (comboBoxElement != null && unstitchedListItem != null && NativeMethods.GetForegroundWindow() == comboBoxElement.WindowHandle && TaskActivityElement.IsState(comboBoxElement, AccessibleStates.Expanded))
            {
                try
                {
                    Rectangle boundingRectangle = unstitchedListItem.InnerElement.Current.BoundingRectangle;
                    UiaElement element2 = UiaUtility.TransformUiaElement(GetFocusedElement(IntPtr.Zero));
                    if (element2 != null && element2.ControlTypeName == ControlType.ListItem && element2.Parent != null && Automation.Compare(comboBoxElement.InnerElement, element2.Parent.InnerElement))
                    {
                        Rectangle objB = element2.InnerElement.Current.BoundingRectangle;
                        if (Equals(boundingRectangle, objB) && string.Equals(unstitchedListItem.InnerElement.Current.Name, element2.Name, StringComparison.Ordinal))
                        {
                            object[] args = { element2 };
                            
                            return element2;
                        }
                    }
                }
                catch (Win32Exception exception)
                {
                    if (exception.ErrorCode != -2147467259)
                    {
                        throw;
                    }
                }
            }
            return null;
        }

        public override int GetControlSupportLevel(IntPtr windowHandle)
        {
            UiaElement element;
            if (!(windowHandle != IntPtr.Zero))
            {
                return 0;
            }
                                    if (UiaUtility.IsDesktopWindowAndNotLegacy(windowHandle))
            {
                return 1;
            }
            if (UiaUtility.IsWindowOfSupportedTechnology(windowHandle, true))
            {
                return 100;
            }
            return UiaUtility.IsElementHost(this, windowHandle, out element);
        }

        public override int GetControlSupportLevel(AutomationElement element)
        {
            if (UiaUtility.IsDesktopWindowAndNotLegacy(element))
            {
                return 1;
            }
                                                                                    if (UiaUtility.IsWindowOfSupportedTechnology(element, true))
            {
                return 100;
            }
            if (element.Current.NativeWindowHandle != 0)
            {
                UiaElement element2;
                return GetSupportIfElementHost(UiaUtility.GetWindowHandleFromAncestor(element), out element2);
            }
            return 0;
        }

        public override ITaskActivityElement GetElementFromAutomationElement(AutomationElement automationElement, AutomationElement ceilingElement)
        {
            ITaskActivityElement element2;
            try
            {
                UiaElement uiaElement = UiaElementFactory.GetUiaElement(automationElement, true);
                if (!AnalyzeElementFromPoint(uiaElement))
                {
                    return uiaElement;
                }
                uiaElement = GetStitchedElementFromPoint(uiaElement, true);
                uiaElement.CeilingElement = ceilingElement;
                element2 = uiaElement;
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return element2;
        }

        public override ITaskActivityElement GetElementFromNativeElement(object nativeElement)
        {
            ITaskActivityElement uiaElement;
            try
            {
                
                AutomationElement automationElement = nativeElement as AutomationElement;
                uiaElement = UiaElementFactory.GetUiaElement(automationElement, false);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return uiaElement;
        }

        public override ITaskActivityElement GetElementFromPoint(int pointX, int pointY)
        {
            ITaskActivityElement stitchedElementFromPoint;
            try
            {
                object[] args = { pointX, pointY };
                
                AutomationElement parentOfIgnoredElement = AutomationElement.FromPoint(new Point(pointX, pointY));
                if (string.Equals(parentOfIgnoredElement.Current.ClassName, "ApplicationFrameWindow"))
                {
                    parentOfIgnoredElement = TreeWalkerHelper.GetFirstChild(parentOfIgnoredElement);
                }
                parentOfIgnoredElement = UiaUtility.GetParentOfIgnoredElement(parentOfIgnoredElement);
                if (parentOfIgnoredElement != null)
                {
                    ITaskActivityElement element3 = UiaUtility.GetHtmlElementFromPoint(parentOfIgnoredElement, pointX, pointY);
                    if (element3 != null)
                    {
                        return element3;
                    }
                }
                UiaElement uiaElement = UiaElementFactory.GetUiaElement(parentOfIgnoredElement, true);
                IntPtr windowHandle = uiaElement.WindowHandle;
                if (windowHandle != IntPtr.Zero)
                {
                    UiaUtility.IsWindowOfSupportedTechnology(windowHandle, true);
                }
                if (!AnalyzeElementFromPoint(uiaElement))
                {
                    return uiaElement;
                }
                uiaElement = MorphToScrollBarIfNeeded(uiaElement, pointX, pointY);
                stitchedElementFromPoint = GetStitchedElementFromPoint(uiaElement, true);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return stitchedElementFromPoint;
        }

        public override ITaskActivityElement GetElementFromPoint(int pointX, int pointY, AutomationElement ceilingElement)
        {
            UiaElement element = base.GetElementFromPoint(pointX, pointY, ceilingElement) as UiaElement;
            element.CeilingElement = ceilingElement;
            return element;
        }

        public override ITaskActivityElement GetElementFromWindowHandle(IntPtr handle)
        {
            ITaskActivityElement uiaElement;
            try
            {
                object[] args = { handle };
                
                UiaUtility.SwitchToWindow(handle);
                uiaElement = UiaElementFactory.GetUiaElement(AutomationElement.FromHandle(handle), true);
            }
            catch (Exception exception)
            {
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return uiaElement;
        }

        public override ITaskActivityElement GetFocusedElement(IntPtr handle)
        {
            ITaskActivityElement element4;
            try
            {
                
                AutomationElement focusedElement = AutomationElement.FocusedElement;
                if (LastFocusWasOnComboBox)
                {
                    LastFocusWasOnComboBox = false;
                    if (UiaUtility.GetAutomationPropertyValue<System.Windows.Automation.ControlType>(focusedElement, AutomationElement.ControlTypeProperty) == System.Windows.Automation.ControlType.Edit)
                    {
                        AutomationElement parent = TreeWalkerHelper.GetParent(focusedElement);
                        if (Equals(parent, LastFocussedComboBox))
                        {
                            focusedElement = parent;
                        }
                    }
                }
                UiaElement uiaElement = UiaElementFactory.GetUiaElement(focusedElement, true);
                if (!IsRecordingSession && uiaElement != null && uiaElement.ControlTypeName == ControlType.Edit && DataGridUtility.IsElementNotTemplateContentOfCell(uiaElement.InnerElement))
                {
                    uiaElement = uiaElement.Parent;
                }
                object[] args = { uiaElement };
                
                element4 = uiaElement;
            }
            catch (Exception exception)
            {
                object[] objArray2 = { exception };
                CrapyLogger.log.ErrorFormat("Uia.GetFocusedElement: Exception {0} occured", objArray2);
                UiaUtility.MapAndThrowException(exception, null, false);
                throw;
            }
            return element4;
        }

        public override ILastInvocationInfo GetLastInvocationInfo() =>
            lastInfo;

        public override ITaskActivityElement GetNextSibling(ITaskActivityElement element)
        {
            ITaskActivityElement target = null;
            UiaElement element3 = UiaUtility.TransformUiaElement(element);
            if (element3 != null)
            {
                target = UiaElementFactory.GetUiaElement(TreeWalkerHelper.GetNextSibling(element3.InnerElement), false);
                SetContainer(target, element);
            }
            UiaUtility.CopyCeilingElement(element, target);
            return target;
        }

        public override ITaskActivityElement GetParent(ITaskActivityElement element)
        {
            ITaskActivityElement target = null;
            UiaElement technologyElement = UiaUtility.TransformUiaElement(element);
            if (technologyElement != null && !technologyElement.IsBoundaryForHostedControl)
            {
                target = technologyElement.Parent;
                SetContainer(target, technologyElement);
            }
            UiaUtility.CopyCeilingElement(element, target);
            return target;
        }

        public override ITaskActivityElement GetPreviousSibling(ITaskActivityElement element)
        {
            ITaskActivityElement target = null;
            UiaElement element3 = UiaUtility.TransformUiaElement(element);
            if (element3 != null)
            {
                target = UiaElementFactory.GetUiaElement(TreeWalkerHelper.GetPreviousSibling(element3.InnerElement), false);
                SetContainer(target, element);
            }
            UiaUtility.CopyCeilingElement(element, target);
            return target;
        }

        internal override TaskActivityElement GetRootElement() =>
            new UiaElement(AutomationElement.RootElement);

        private UiaElement GetStitchedElementFromPoint(UiaElement elementFromPoint, bool tryWithFocusElement)
        {
            if (Equals(elementFromPoint, lastUnstitchedListElement))
            {
                return lastStitchedListElement;
            }
            if (LastAccessedComboBox != null)
            {
                try
                {
                    if (elementFromPoint == null || !ControlType.ListItem.NameEquals(elementFromPoint.ControlTypeName) || !TaskActivityElement.IsState(LastAccessedComboBox, AccessibleStates.Expanded) || !(LastAccessedComboBox.WindowHandle != elementFromPoint.WindowHandle))
                    {
                        return elementFromPoint;
                    }
                                        {
                        Process processById = Process.GetProcessById((int)LastAccessedComboBox.InnerElement.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty));
                        if (Process.GetProcessById((int)elementFromPoint.InnerElement.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty)).Id == processById.Id)
                        {
                            UiaElement comboBoxElement = null;
                            if (tryWithFocusElement)
                            {
                                comboBoxElement = GetComboBoxElement(LastAccessedComboBox, elementFromPoint);
                            }
                            if (comboBoxElement == null)
                            {
                                Condition condition = new System.Windows.Automation.PropertyCondition(AutomationElement.BoundingRectangleProperty, elementFromPoint.InnerElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty));
                                comboBoxElement = UiaElementFactory.GetUiaElement(LastAccessedComboBox.InnerElement.FindFirst(TreeScope.Descendants, condition), false);
                            }
                            if (comboBoxElement != null)
                            {
                                
                                lastUnstitchedListElement = elementFromPoint;
                                lastStitchedListElement = comboBoxElement;
                                return comboBoxElement;
                            }
                        }
                        return elementFromPoint;
                    }
                }
                catch (ElementNotAvailableException)
                {
                    LastAccessedComboBox = null;
                }
                catch (ZappyTaskException)
                {
                    LastAccessedComboBox = null;
                }
            }
            return elementFromPoint;
        }

        private int GetSupportIfElementHost(IntPtr windowHandle, out UiaElement element)
        {
            element = null;
            if (UiaUtility.IsWpfWindow(NativeMethods.GetAncestor(windowHandle, NativeMethods.GetAncestorFlag.GA_PARENT)))
            {
                ITaskActivityElement elementFromWindowHandle = GetElementFromWindowHandle(windowHandle);
                element = UiaUtility.TransformUiaElement(elementFromWindowHandle);
                if (element != null && UiaUtility.IsWpfFrameWorkId(element.InnerElement))
                {
                    return 0x65;
                }
            }
            return 0;
        }

        public override IUISynchronizationWaiter GetSynchronizationWaiter(ITaskActivityElement element, ZappyTaskEventType eventType) =>
            null;

        private static IList<ControlType> InitializeExactMatchControlTypes() =>
            new List<ControlType> { ControlType.Cell };

        private void InitializeTechnologyManagerProperties()
        {
            SetTechnologyManagerProperty(UITechnologyManagerProperty.WindowLessTreeSwitchingSupported, false);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.SearchSupported, false);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.FilterPropertiesForSearchSupported, false);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.DoNotGenerateVisibleOnlySearchConfiguration, null);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.ExactQueryIdMatch, exactMatchControlTypes);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.MergeSingleSessionObjects, false);
        }

        private static bool IsHandledByPlaybackEngineOrInvalid(IQueryCondition queryCondition)
        {
            PropertyCondition condition = queryCondition as PropertyCondition;
            if (condition != null)
            {
                foreach (string str in UiaConstants.PropertiesHandledByPlaybackEngine)
                {
                    if (string.Equals(condition.PropertyName, str, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                PropertyNames.ThrowIfInvalid(condition.PropertyName);
            }
            return false;
        }

                                                                                                                                
        private bool IsWindowMatched(IntPtr wnd, PropertyCondition nameCondition, PropertyCondition classNameCondition)
        {
            bool flag = true;
            if (nameCondition != null)
            {
                string windowText = NativeMethods.GetWindowText(wnd);
                flag = false;
                if (nameCondition.PropertyOperator == PropertyConditionOperator.Contains && windowText != null)
                {
                    flag = windowText.ToLower().Contains(nameCondition.ValueWrapper.ToLower());
                }
                else
                {
                    flag = string.Equals(windowText, nameCondition.ValueWrapper, StringComparison.OrdinalIgnoreCase);
                }
            }
            if (!flag || classNameCondition == null)
            {
                return flag;
            }
            flag = false;
            string className = NativeMethods.GetClassName(wnd);
            if (className != null && classNameCondition.PropertyOperator == PropertyConditionOperator.Contains)
            {
                return className.ToLower().Contains(classNameCondition.ValueWrapper.ToLower());
            }
            return string.Equals(className, classNameCondition.ValueWrapper, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsWindowMatchedToApplicationFrameWindowChild(IntPtr wnd, PropertyCondition nameCondition, PropertyCondition classNameCondition, out IntPtr matchedChildWindow)
        {
            bool flag = false;
            matchedChildWindow = IntPtr.Zero;
            if (string.Equals(NativeMethods.GetClassName(wnd), "ApplicationFrameWindow"))
            {
                IntPtr ptr = NativeMethods.FindWindowEx(wnd, IntPtr.Zero, classNameCondition.ValueWrapper, nameCondition.ValueWrapper);
                if (ptr != IntPtr.Zero)
                {
                    
                    flag = IsWindowMatched(ptr, nameCondition, classNameCondition);
                    if (flag)
                    {
                        matchedChildWindow = ptr;
                    }
                    return flag;
                }
                
            }
            return flag;
        }

        public override bool MatchElement(ITaskActivityElement element, object parsedQueryIdCookie, out bool useEngine)
        {
            useEngine = false;
            lastSearchCondition = parsedQueryIdCookie as IQueryCondition;
            if (lastSearchCondition != null)
            {
                foreach (PropertyCondition condition in lastSearchCondition.Conditions)
                {
                    if (!condition.Match(element) && !UiaUtility.IsConditionMatchesWpfElement(element, condition))
                    {
                        try
                        {
                            if (element != null && element.ControlTypeName == ControlType.Row && rowElementCache.ContainsKey(lastSearchCondition) && Equals(element, rowElementCache[lastSearchCondition]))
                            {
                                return true;
                            }
                        }
                        catch (ElementNotAvailableException)
                        {
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        return false;
                    }
                }
            }
            if (element != null && element.ControlTypeName == ControlType.Row && !rowElementCache.ContainsKey(lastSearchCondition))
            {
                rowElementCache.Add(lastSearchCondition, element);
            }
            return true;
        }

        private UiaElement MorphToScrollBarIfNeeded(UiaElement element, int pointX, int pointY)
        {
            if (Instance.IsRecordingSession)
            {
                for (UiaElement element2 = element; element2 != null && !UiaUtility.IsBoundaryElement(element2); element2 = UiaElementFactory.GetUiaElement(TreeWalkerHelper.GetParent(element2.InnerElement), false))
                {
                    Rectangle rectangle;
                    if (!UiaUtility.CheckForMenuItem(element2, element) && !ControlType.ComboBox.NameEquals(element2.ControlTypeName) && element.Parent != null && !ControlType.ScrollBar.NameEquals(element.Parent.ControlTypeName) && UiaUtility.IsScrollableContainer(element2, out rectangle) && !UiaUtility.IsPointInsideElementRectangle(rectangle, pointX, pointY) && UiaUtility.IsPointInsideElementRectangle(element2.GetBoundingRectangle(), pointX, pointY))
                    {
                        element = element.MorphIntoScrollBar();
                        return element;
                    }
                }
            }
            return element;
        }

        public override string ParseQueryId(string queryElement, out object parsedQueryIdCookie)
        {
            string str = SetSearchCondition(queryElement);
            parsedQueryIdCookie = lastSearchCondition;
            return str;
        }

        public override void ProcessMouseEnter(IntPtr handle)
        {
            if (handle == IntPtr.Zero || handle != menuEventProcessHandle && UiaUtility.IsWindowOfSupportedTechnology(handle, false))
            {
                object[] args = { handle, NativeMethods.GetWindowText(handle) };
                
                RemoveMenuEventHandler();
                UiaElement element = UiaUtility.TransformUiaElement(GetElementFromWindowHandle(handle));
                if (element != null)
                {
                                        {
                        IZappyTaskEventNotify eventSink = null;
                        try
                        {
                            EventWrapper wrapper = EventWrapper.GetEventWrapper(element, ControlType.Menu, ZappyTaskEventType.ValueChanged, eventSink);
                            if (wrapper != null)
                            {
                                ElementEventSink key = new ElementEventSink(element, ZappyTaskEventType.ValueChanged, eventSink);
                                elementEventSinkMap.Add(key, wrapper);
                                menuEventSink = key;
                                menuEventProcessHandle = handle;
                            }
                        }
                        catch (Exception exception)
                        {
                            UiaUtility.MapAndThrowException(exception, element, false);
                            throw;
                        }
                    }
                }
            }
        }

        public override bool RemoveEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            UiaElement element2 = UiaUtility.TransformUiaElement(element);
            if (element2 != null)
            {
                object syncLock = this.syncLock;
                lock (syncLock)
                {
                    FireFakeEvent();
                    ElementEventSink key = new ElementEventSink(element2, eventType, eventSink);
                    if (elementEventSinkMap.ContainsKey(key))
                    {
                        EventWrapper wrapper = elementEventSinkMap[key];
                        elementEventSinkMap.Remove(key);
                        EventWrapper[] arg = { wrapper };
                        uiaWorker.BeginInvoke(new RemoveUiaEventsAsync(RemoveUiaEventHandlers), arg);
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool RemoveGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) =>
            false;

        private void RemoveMenuEventHandler()
        {
            if (menuEventSink != null && elementEventSinkMap.ContainsKey(menuEventSink))
            {
                EventWrapper wrapper = elementEventSinkMap[menuEventSink];
                elementEventSinkMap.Remove(menuEventSink);
                EventWrapper[] arg = { wrapper };
                uiaWorker.BeginInvoke(new RemoveUiaEventsAsync(RemoveUiaEventHandlers), arg);
            }
            menuEventSink = null;
            menuEventProcessHandle = IntPtr.Zero;
        }

        private void RemoveUiaEventHandlers(EventWrapper[] uiaEvents)
        {
            try
            {
                foreach (EventWrapper wrapper in uiaEvents)
                {
                    try
                    {
                        wrapper.Dispose();
                    }
                    catch { }
                }
            }
            catch { }
        }

        public override object[] Search(object parsedQueryIdCookie, ITaskActivityElement parentElement, int maxDepth)
        {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                if (parentElement != null || parsedQueryIdCookie == null)
                {
                    throw new NotSupportedException();
                }
                int num = 1;
                SingleQueryId id = new SingleQueryId(searchConfigurationRegex.Replace(parsedQueryIdCookie.ToString(), string.Empty));
                if (id != null && id.SearchProperties != null)
                {
                    List<PropertyCondition> searchProperties = id.SearchProperties;
                    int technologyManagerProperty = GetTechnologyManagerProperty<int>(UITechnologyManagerProperty.SearchTimeout);
                    int queryPropertyIndex = id.GetQueryPropertyIndex("Name");
                    int num4 = id.GetQueryPropertyIndex("ClassName");
                    PropertyCondition nameCondition = queryPropertyIndex != -1 ? searchProperties[queryPropertyIndex] : null;
                    PropertyCondition classNameCondition = num4 != -1 ? searchProperties[num4] : null;
                    if (ZappyTaskUtilities.IsPhone)
                    {
                        AndConditionBuilder builder = new AndConditionBuilder();
                        if (queryPropertyIndex != -1)
                        {
                            builder.Append(nameCondition);
                        }
                        if (num4 != -1)
                        {
                            builder.Append(classNameCondition);
                        }
                        int num5 = id.GetQueryPropertyIndex("FrameworkId");
                        int num6 = id.GetQueryPropertyIndex("ControlType");
                        PropertyCondition condition = num5 != -1 ? searchProperties[num5] : null;
                        PropertyCondition condition2 = num6 != -1 ? searchProperties[num6] : null;
                        if (num5 != -1)
                        {
                            builder.Append(condition);
                        }
                        if (num6 != -1)
                        {
                            builder.Append(condition2);
                        }
                        AndCondition queryCondition = builder.Build();
                        stopwatch.Start();
                        for (int i = 0; i < num && stopwatch.ElapsedMilliseconds < technologyManagerProperty; i++)
                        {
                            bool flag;
                            UiaElement topLevelElementForPhoneUsingCondition = UiaUtility.GetTopLevelElementForPhoneUsingCondition(UiaUtility.GetUIAPropertyConditionFromIQueryCondition(queryCondition));
                            if (topLevelElementForPhoneUsingCondition != null && MatchElement(topLevelElementForPhoneUsingCondition, queryCondition, out flag))
                            {
                                return new object[] { topLevelElementForPhoneUsingCondition };
                            }
                        }
                    }
                    else if (queryPropertyIndex != -1 || num4 != -1)
                    {
                        stopwatch.Start();
                        for (int j = 0; j < num && stopwatch.ElapsedMilliseconds < technologyManagerProperty; j++)
                        {
                            try
                            {
                                IntPtr param2 = new IntPtr();
                                IntPtr targetWindow = IntPtr.Zero;
                                IntPtr matchingChildWindow = IntPtr.Zero;
                                NativeMethods.EnumWindows(delegate (IntPtr wnd, ref IntPtr param)
                                {
                                    if (IsWindowMatched(wnd, nameCondition, classNameCondition))
                                    {
                                        targetWindow = wnd;
                                    }
                                    else if (IsWindowMatchedToApplicationFrameWindowChild(wnd, nameCondition, classNameCondition, out matchingChildWindow))
                                    {
                                        targetWindow = matchingChildWindow;
                                    }
                                    return targetWindow == IntPtr.Zero;
                                }, param2);
                                if (targetWindow != IntPtr.Zero)
                                {
                                    ITaskActivityElement elementFromWindowHandle = GetElementFromWindowHandle(targetWindow);
                                    return new object[] { elementFromWindowHandle };
                                }
                            }
                            catch (NotImplementedException)
                            {
                            }
                        }
                    }
                }
                throw new Exception();
            }
            catch (Exception exception)
            {
                CrapyLogger.log.ErrorFormat(exception.Message);
                throw;
            }
            finally
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }
            }
        }

        private static void SetContainer(ITaskActivityElement target, ITaskActivityElement technologyElement)
        {
            if (target != null && technologyElement != null && technologyElement.SwitchingElement != null)
            {
                target.SwitchingElement = technologyElement.SwitchingElement;
            }
        }

        private string SetSearchCondition(string queryElement)
        {
            bool flag = false;
            lastSearchQueryId = queryElement;
            lastSearchCondition = null;
            lastInfo = null;
            string str = queryElement.Substring(0, queryElement.IndexOf(']') + 1);
            try
            {
                AndCondition condition = AndCondition.Parse(queryElement);
                List<IQueryCondition> list = new List<IQueryCondition>();
                bool flag2 = false;
                foreach (IQueryCondition condition2 in condition.Conditions)
                {
                    if (IsHandledByPlaybackEngineOrInvalid(condition2))
                    {
                        if (flag2)
                        {
                            str = str + " && ";
                        }
                        flag2 = true;
                        str = str + condition2;
                    }
                    else
                    {
                        list.Add(condition2);
                        flag = true;
                    }
                }
                if (flag)
                {
                    lastSearchCondition = new AndCondition(list.ToArray());
                }
            }
            catch (ArgumentException exception)
            {
                object[] args = { queryElement };
                CrapyLogger.log.ErrorFormat("UIA: UiaTestPlugin: Invalid Query Element {0}", args);
                object[] objArray2 = { queryElement };
                string infoMessage = string.Format(CultureInfo.CurrentCulture, Resources.InvalidUiaQueryElement, objArray2);
                lastInfo = new UiaSearchInfo(infoMessage, 0x80070057);
                throw new ArgumentException(infoMessage, exception);
            }
            return str;
        }

        private bool ShouldStitchMenuItem(UiaElement element, UiaElement parent)
        {
            bool flag = false;
            if (parent != null)
            {
                AutomationElement firstChild = TreeWalkerHelper.GetFirstChild(parent.InnerElement);
                VirtualizationContext unknown = VirtualizationContext.Unknown;
                int num = 0;
                while (num < 30 && firstChild != null)
                {
                    if (firstChild.Equals(element.InnerElement))
                    {
                        flag = true;
                        break;
                    }
                    firstChild = TreeWalkerHelper.GetNextSibling(firstChild, ref unknown);
                    num++;
                }
                if (num == 30 && string.Equals(parent.ClassName, element.ClassName, StringComparison.OrdinalIgnoreCase))
                {
                    flag = true;
                }
            }
            return flag;
        }

        public override void StartSession(bool recordingSession)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("UiaTechnologyManager");
            }
            if (!sessionStarted)
            {
                
                uiaWorker.StartWorkerThread();
                sessionStarted = true;
                IsRecordingSession = recordingSession;
                Instance = this;
            }
            else
            {
                
            }
        }

        public override void StopSession()
        {
            if (sessionStarted)
            {
                
                Instance = null;
                Dictionary<ElementEventSink, EventWrapper>.ValueCollection values = elementEventSinkMap.Values;
                if (values.Count > 0)
                {
                    EventWrapper[] arg = new EventWrapper[values.Count];
                    int num = 0;
                    foreach (EventWrapper wrapper in values)
                    {
                        arg[num++] = wrapper;
                    }
                    uiaWorker.BeginInvoke(new RemoveUiaEventsAsync(RemoveUiaEventHandlers), arg);
                }
                ClearSessionRelatedCache();
                uiaWorker.StopWorkerThread();
                sessionStarted = false;
                
            }
            else
            {
                
            }
        }

        private void UpdateMenuEventCache(UiaElement element, IZappyTaskEventNotify eventSink)
        {
            if (element.ControlTypeName == ControlType.MenuItem)
            {
                if (element.Parent != null && element.Parent.ControlTypeName == ControlType.Menu)
                {
                    RemoveMenuEventHandler();
                    LastAccessedMenu = element.Parent;
                }
            }
            else
            {
                LastAccessedMenu = null;
                PropertyChangeMenuItemStack.Clear();
            }
        }

        internal static UiaTechnologyManager Instance
        {
            get
            {
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        internal bool IsRecordingSession { get; set; }

        internal UiaElement LastAccessedComboBox
        {
            get
            {
                return lastAccessedComboBox;
            }
            set
            {
                lastAccessedComboBox = value;
            }
        }

        internal UiaElement LastAccessedDatePicker
        {
            get
            {
                return lastAccessedDatePicker;
            }
            set
            {
                lastAccessedDatePicker = value;
            }
        }

        internal UiaElement LastAccessedMenu
        {
            get
            {
                return lastAccessedMenu;
            }
            set
            {
                lastAccessedMenu = value;
            }
        }

        internal AutomationElement LastFocussedComboBox
        {
            get
            {
                return lastFocussedComboBox;
            }
            set
            {
                lastFocussedComboBox = value;
            }
        }

        internal bool LastFocusWasOnComboBox
        {
            get
            {
                return lastFocusWasOnComboBox;
            }
            set
            {
                lastFocusWasOnComboBox = value;
            }
        }

        internal Dictionary<AutomationElement, AutomationElement> MenuItemStitchMap =>
            menuItemStitchMap;

        internal Stack<UiaElement> PropertyChangeMenuItemStack =>
            propertyChangeMenuItemStack;

        public override string TechnologyName =>
            "UIA";

        private class ElementEventSink
        {
            private UiaElement element;
            private IZappyTaskEventNotify eventNotify;
            private ZappyTaskEventType eventType;

            internal ElementEventSink(UiaElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventNotify)
            {
                this.element = element;
                this.eventType = eventType;
                this.eventNotify = eventNotify;
            }

            public override bool Equals(object obj)
            {
                if (this == obj)
                {
                    return true;
                }
                ElementEventSink sink = obj as ElementEventSink;
                if (sink == null)
                {
                    return false;
                }
                return sink.eventType == eventType && Equals(sink.element, element) && Equals(sink.eventNotify, eventNotify);
            }

            public override int GetHashCode() =>
                element.GetHashCode() ^ eventType.GetHashCode() ^ (eventNotify != null ? eventNotify.GetHashCode() : 0);
        }

        private delegate void RemoveUiaEventsAsync(EventWrapper[] eventWrappers);
    }
}

