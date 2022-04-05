using Accessibility;
using html;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Decode.Mssa
{
    public sealed class MsaaZappyPlugin : UITechnologyManager, IDisposable
    {
        private bool disposed;
        private static readonly IList<ControlType> doNotGenerateVisibleOnlyControlTypes = InitializeDoNotGenerateVisibleOnlyControlTypes();
        internal const int ERROR_E_FAIL = -2147467259;
        private MsaaEventManager eventManager;
        private static readonly IList<ControlType> exactMatchControlTypes = InitializeExactMatchControlTypes();
        private static MsaaZappyPlugin instance;
        private static readonly object lockObject = new object();
        private bool sessionStarted;
        private const string SilverLight2ClassName = "ATL:";
        private const string SilverLight3ClassName = "MicrosoftSilverlight";
        private const string SilverlightObjectType = "application/x-silverlight";
        private const string SilverlightTechnologyName = "Microsoft Silverlight";
        private StaThreadWorker staThreadWorker;
        private WinEvent winEvent;

        private MsaaZappyPlugin()
        {
            
            InitializeTechnologyManagerProperties();
            staThreadWorker = StaThreadWorker.Instance;
            IntPtr desktopWindow = NativeMethods.GetDesktopWindow();
            if (desktopWindow != IntPtr.Zero)
            {
                try
                {
                    NativeMethods.AccessibleObjectFromWindow(desktopWindow);
                }
                catch (SystemException)
                {
                }
            }
        }

        public override bool AddEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) =>
            eventManager.AddEventHandler(element, eventType, eventSink);

        public override bool AddGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            bool flag = false;
            if (ZappyTaskEventType.OnFocus == eventType)
            {
                WinEvent.ActionNotify = eventSink;
                flag = true;
            }
            return flag;
        }

        public override void CancelStep()
        {
        }

        public override ITaskActivityElement ConvertToThisTechnology(ITaskActivityElement elementToConvert, out int supportLevel)
        {
            ITaskActivityElement element4;
            try
            {
                object nativeElement = elementToConvert.NativeElement;
                if (nativeElement != null && nativeElement.GetType() == typeof(IntPtr))
                {
                    supportLevel = 1;
                    return GetElementFromWindowHandle((IntPtr)nativeElement);
                }
                ITaskActivityElement elementFromNativeElement = null;
                supportLevel = 0;
                string className = NativeMethods.GetClassName(elementToConvert.WindowHandle);
                if (!string.IsNullOrEmpty(className) && (string.Equals(className, "MicrosoftSilverlight", StringComparison.OrdinalIgnoreCase) || className.Contains("ATL:")))
                {
                    object[] args = { "Microsoft Silverlight" };
                                    }
                IHTMLObjectElement element2 = nativeElement as IHTMLObjectElement;
                if (element2 != null && !string.IsNullOrEmpty(element2.type) && element2.type.Contains("application/x-silverlight"))
                {
                    object[] objArray2 = { "Microsoft Silverlight" };
                                    }
                IAccessible accessibleFromServiceProvider = nativeElement as IAccessible;
                if (accessibleFromServiceProvider == null)
                {
                    IServiceProvider serviceProvider = nativeElement as IServiceProvider;
                    accessibleFromServiceProvider = ZappyTaskUtilities.GetAccessibleFromServiceProvider(serviceProvider);
                }
                if (accessibleFromServiceProvider != null)
                {
                    object[] objArray3 = { accessibleFromServiceProvider, 0 };
                    elementFromNativeElement = GetElementFromNativeElement(objArray3);
                }
                if (elementFromNativeElement != null)
                {
                    if (ZappyTaskUtilities.IsWinformsClassName(elementFromNativeElement.ClassName))
                    {
                        supportLevel = 100;
                    }
                    else
                    {
                        supportLevel = 1;
                    }
                }
                element4 = elementFromNativeElement;
            }
            catch (SystemException exception)
            {
                if (exception is NotImplementedException)
                {
                    supportLevel = 0;
                    element4 = elementToConvert;
                }
                else
                {
                    MsaaUtility.MapAndThrowException(exception, true);
                    throw;
                }
            }
            return element4;
        }

        public void Dispose()
        {
            object lockObject = MsaaZappyPlugin.lockObject;
            lock (lockObject)
            {
                if (!disposed)
                {
                    
                    StopSession();
                    instance = null;
                    disposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        internal IEnumerator GetChildren(ITaskActivityElement element) =>
            GetChildren(element, null);

        public override IEnumerator GetChildren(ITaskActivityElement element, object parsedQueryIdCookie) =>
            new ChildrenEnumerator(element as MsaaElement);

        public override int GetControlSupportLevel(IntPtr windowHandle) =>
            MsaaUtility.GetControlSupportLevel(windowHandle);

        public override int GetControlSupportLevel(AutomationElement element) =>
            MsaaUtility.GetControlSupportLevel(element);

        public override ITaskActivityElement GetElementFromAutomationElement(AutomationElement element, AutomationElement ceilingElement)
        {
            
            MsaaElement technologyElement = null;
            try
            {
                AccWrapper accessibleObjectFromAE = MsaaUtility.GetAccessibleObjectFromAE(element);
                if (accessibleObjectFromAE != null)
                {
                    if (element.IsMSAAWindow)
                    {
                        technologyElement = new MsaaElement(accessibleObjectFromAE.Parent);
                    }
                    else
                    {
                        technologyElement = new MsaaElement(accessibleObjectFromAE);
                    }
                }
            }
            catch (SystemException exception)
            {
                MsaaUtility.MapAndThrowException(exception, true);
                throw;
            }
            MsaaUtility.SetCeilingElement(technologyElement, ceilingElement);
            return technologyElement;
        }

        public override ITaskActivityElement GetElementFromNativeElement(object nativeElement)
        {
            
            ZappyTaskUtilities.CheckForNull(nativeElement, "nativeElement");
            MsaaElement element = null;
            IntPtr zero = IntPtr.Zero;
            object[] objArray = nativeElement as object[];
            if (objArray == null || objArray.Length < 2 || objArray.Length > 3)
            {
                object[] args = { nativeElement.ToString() };
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Resource.InvalidNativeElement" + args));
            }
            IAccessible accessibleObject = objArray[0] as IAccessible;
            int childId = (int)objArray[1];
            if (objArray.Length == 3)
            {
                zero = (IntPtr)objArray[2];
            }
            if (accessibleObject != null)
            {
                element = new MsaaElement(zero, accessibleObject, childId);
            }
            return element;
        }

        public override ITaskActivityElement GetElementFromPoint(int pointX, int pointY)
        {
                        
            NativeMethods.POINT ptScreen = new NativeMethods.POINT(pointX, pointY);
            MsaaElement element = null;
            try
            {
                AccWrapper accessibleWrapper = MsaaUtility.FastAccessibleObjectFromPoint(ptScreen);
                if (accessibleWrapper == null)
                {
                    
                    accessibleWrapper = AccWrapper.GetAccWrapperFromPoint(ptScreen);
                }
                else
                {
                    
                }
                if (accessibleWrapper != null)
                {
                    element = new MsaaElement(accessibleWrapper);
                }
            }
            catch (SystemException exception)
            {
                MsaaUtility.MapAndThrowException(exception, true);
                throw;
            }
            return element;
        }

        public override ITaskActivityElement GetElementFromPoint(int pointX, int pointY, AutomationElement ceilingElement)
        {
            MsaaElement technologyElement = base.GetElementFromPoint(pointX, pointY, ceilingElement) as MsaaElement;
            MsaaUtility.SetCeilingElement(technologyElement, ceilingElement);
            return technologyElement;
        }

        public override ITaskActivityElement GetElementFromWindowHandle(IntPtr handle)
        {
            object[] args = { handle };
            
            ZappyTaskUtilities.CheckForNull(handle, "handle");
            AccWrapper accessibleWrapper = null;
            try
            {
                accessibleWrapper = AccWrapper.GetAccWrapperFromWindow(handle);
            }
            catch (ZappyTaskControlNotAvailableException exception)
            {
                object[] objArray2 = { exception.Message };
                CrapyLogger.log.WarnFormat("MSAA : GetElementFromWindowHandle - exception {0}", objArray2);
            }
            if (accessibleWrapper == null)
            {
                return null;
            }
            return new MsaaElement(accessibleWrapper);
        }


        public override ITaskActivityElement GetFocusedElement(IntPtr handle)
        {
            
            MsaaElement lastFocusedElement = winEvent.GetLastFocusedElement();
            if (lastFocusedElement != null)
            {
                object[] objArray1 = { lastFocusedElement };
                
                return lastFocusedElement;
            }
            if (handle == IntPtr.Zero)
            {
                handle = NativeMethods.GetFocus();
            }
            AccWrapper accWrapperFromWindow = null;
            try
            {
                accWrapperFromWindow = AccWrapper.GetAccWrapperFromWindow(handle);
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
                        if (accWrapperFromWindow != null)
            {
                IAccessible accessibleObject = accWrapperFromWindow.AccessibleObject;
                int childId = 0;
                object accFocus = null;
                while (accessibleObject != null)
                {
                    childId = 0;
                    try
                    {
                        accFocus = accessibleObject.accFocus;
                    }
                    catch (SystemException)
                    {
                        break;
                    }
                    if (accFocus is int)
                    {
                        childId = (int)accFocus;
                        if (childId == 0)
                        {
                            break;
                        }
                        accFocus = MsaaUtility.GetChildIAccessible(accessibleObject, childId);
                    }
                    if (accFocus == null)
                    {
                        break;
                    }
                    accessibleObject = accFocus as IAccessible;
                }
                if (accessibleObject != null)
                {
                    lastFocusedElement = new MsaaElement(accessibleObject, childId);
                }
            }
            object[] args = { lastFocusedElement };
            
            return lastFocusedElement;
        }

        public override ILastInvocationInfo GetLastInvocationInfo() =>
            null;

        public override ITaskActivityElement GetNextSibling(ITaskActivityElement element) =>
            Navigate(element, AccessibleNavigation.Next);

        public override ITaskActivityElement GetParent(ITaskActivityElement element)
        {
            ITaskActivityElement parent = null;
            MsaaElement element3 = element as MsaaElement;
            if (element3 != null && !element3.IsBoundayForHostedControl)
            {
                parent = element3.Parent;
            }
            return parent;
        }

        public override ITaskActivityElement GetPreviousSibling(ITaskActivityElement element) =>
            Navigate(element, AccessibleNavigation.Previous);

        public override IUISynchronizationWaiter GetSynchronizationWaiter(ITaskActivityElement element, ZappyTaskEventType eventType)
        {
            throw new NotImplementedException();
        }

        private static IList<ControlType> InitializeDoNotGenerateVisibleOnlyControlTypes() =>
            new List<ControlType> {
                ControlType.TreeItem,
                ControlType.ListItem,
                ControlType.MenuItem,
                ControlType.CheckBoxTreeItem
            };

        private static IList<ControlType> InitializeExactMatchControlTypes() =>
            new List<ControlType> { ControlType.Cell };

        private void InitializeTechnologyManagerProperties()
        {
            SetTechnologyManagerProperty(UITechnologyManagerProperty.WindowLessTreeSwitchingSupported, false);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.SearchSupported, false);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.FilterPropertiesForSearchSupported, false);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.DoNotGenerateVisibleOnlySearchConfiguration, doNotGenerateVisibleOnlyControlTypes);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.ExactQueryIdMatch, exactMatchControlTypes);
            SetTechnologyManagerProperty(UITechnologyManagerProperty.MergeSingleSessionObjects, false);
        }

        internal object InvokeDelegateOnStaThread(Delegate methodDelegate, params object[] args)
        {
            object lockObject = MsaaZappyPlugin.lockObject;
            lock (lockObject)
            {
                return sessionStarted ? staThreadWorker.InvokeDelegate(methodDelegate, args) : null;
            }
        }

        public override bool MatchElement(ITaskActivityElement element, object parsedQueryIdCookie, out bool useEngine)
        {
            throw new NotSupportedException();
        }

        internal static MsaaElement Navigate(ITaskActivityElement element, AccessibleNavigation navDir)
        {
            object[] args = { element, navDir };
            
            MsaaElement parameter = element as MsaaElement;
            ZappyTaskUtilities.CheckForNull(parameter, "element");
            MsaaElement element3 = null;
            AccWrapper accessibleWrapper = parameter.AccessibleWrapper.Navigate(navDir);
            if (accessibleWrapper != null)
            {
                element3 = new MsaaElement(accessibleWrapper);
            }
            return element3;
        }

        public override string ParseQueryId(string queryElement, out object parsedQueryIdCookie)
        {
            throw new NotSupportedException();
        }

        public override void ProcessMouseEnter(IntPtr handle)
        {
        }

        public override bool RemoveEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) =>
            eventManager.RemoveEventHandler(element, eventType);

        public override bool RemoveGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink)
        {
            bool flag = false;
            if (sessionStarted && ZappyTaskEventType.OnFocus == eventType && WinEvent.ActionNotify == eventSink)
            {
                WinEvent.ActionNotify = null;
                flag = true;
            }
            return flag;
        }

        public override object[] Search(object parsedQueryIdCookie, ITaskActivityElement parentElement, int maxDepth)
        {
            throw new NotSupportedException();
        }

        public override void StartSession(bool recordingSession)
        {
            object lockObject = MsaaZappyPlugin.lockObject;
            lock (lockObject)
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("MsaaTestPlugin");
                }
                if (!sessionStarted)
                {
                    
                    winEvent = new WinEvent(recordingSession);
                    eventManager = new MsaaEventManager(this);
                    sessionStarted = true;
                    IsRecording = recordingSession;
                }
                else
                {
                    
                }
            }
        }

        public override void StopSession()
        {
            object lockObject = MsaaZappyPlugin.lockObject;
            lock (lockObject)
            {
                if (sessionStarted)
                {
                    
                    if (winEvent != null)
                    {
                        winEvent.Dispose();
                        winEvent = null;
                    }
                    if (eventManager != null)
                    {
                        eventManager.Dispose();
                        eventManager = null;
                    }
                    sessionStarted = false;
                    
                }
                else
                {
                    
                }
            }
        }

        public static MsaaZappyPlugin Instance
        {
            get
            {
                if (instance == null)
                {
                    object lockObject = MsaaZappyPlugin.lockObject;
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new MsaaZappyPlugin();
                        }
                    }
                }
                return instance;
            }
        }

        internal bool IsRecording { get; set; }

        public override string TechnologyName =>
            "MSAA";

        internal WinEvent WinEvent =>
            winEvent;
    }
}