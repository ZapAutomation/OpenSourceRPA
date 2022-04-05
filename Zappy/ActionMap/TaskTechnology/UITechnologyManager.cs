using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.CustomMarshalers;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.Helpers.Interface;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.ActionMap.TaskTechnology
{

#if COMENABLED
    [Guid("CAF9DAD2-AA2F-47ad-953A-C4596EB6A3E1"), ComVisible(true)]
#endif
    public abstract class UITechnologyManager : IUITechnologyManager
    {
        private PropertyBag<UITechnologyManagerProperty> properties = new PropertyBag<UITechnologyManagerProperty>("UITechnologyManagerProperty");

        public abstract bool AddEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        public abstract bool AddGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        internal static bool AreEqual(UITechnologyManager manager1, UITechnologyManager manager2) =>
            manager1 == manager2 || manager1 != null && manager2 != null && AreEqual(manager1.TechnologyName, manager2.TechnologyName);

        internal static bool AreEqual(string technologyName1, string technologyName2) =>
            string.Equals(technologyName1, technologyName2, StringComparison.OrdinalIgnoreCase);

        public abstract void CancelStep();
        public abstract ITaskActivityElement ConvertToThisTechnology(ITaskActivityElement elementToConvert, out int supportLevel);
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "", MarshalTypeRef = typeof(EnumeratorToEnumVariantMarshaler), MarshalCookie = "")]
        [ComVisible(true)]
        public abstract IEnumerator GetChildren(ITaskActivityElement element, object parsedQueryIdCookie);
        public abstract int GetControlSupportLevel(IntPtr windowHandle);
        public virtual int GetControlSupportLevel(AutomationElement element)
        {
            int windowedAncestor = GetWindowedAncestor(element);
            if (windowedAncestor > 0)
            {
                return GetControlSupportLevel(new IntPtr(windowedAncestor));
            }
            return 0;
        }

        public virtual ITaskActivityElement GetElementFromAutomationElement(AutomationElement element, AutomationElement ceilingElement)
        {
            int windowedAncestor = GetWindowedAncestor(element);
            if (windowedAncestor > 0)
            {
                return GetElementFromWindowHandle(new IntPtr(windowedAncestor));
            }
            return null;
        }

        public abstract ITaskActivityElement GetElementFromNativeElement(object nativeElement);
        public abstract ITaskActivityElement GetElementFromPoint(int pointX, int pointY);
        public virtual ITaskActivityElement GetElementFromPoint(int pointX, int pointY, AutomationElement ceilingElement) =>
            GetElementFromPoint(pointX, pointY);

        public abstract ITaskActivityElement GetElementFromWindowHandle(IntPtr handle);
        public abstract ITaskActivityElement GetFocusedElement(IntPtr handle);
        public virtual ITaskActivityElement GetFocusedElement(AutomationElement ceilingElement)
        {
            int windowedAncestor = GetWindowedAncestor(AutomationElement.FocusedElement);
            if (windowedAncestor > 0)
            {
                return GetFocusedElement(new IntPtr(windowedAncestor));
            }
            return null;
        }


        public abstract ILastInvocationInfo GetLastInvocationInfo();
        public abstract ITaskActivityElement GetNextSibling(ITaskActivityElement element);
        public abstract ITaskActivityElement GetParent(ITaskActivityElement element);
        public abstract ITaskActivityElement GetPreviousSibling(ITaskActivityElement element);
        internal virtual TaskActivityElement GetRootElement() =>
            null;

        public abstract IUISynchronizationWaiter GetSynchronizationWaiter(ITaskActivityElement element, ZappyTaskEventType eventType);
        public virtual object GetTechnologyManagerProperty(UITechnologyManagerProperty propertyName)
        {
            if (!properties.ContainsKey(propertyName))
            {
                                throw new NotSupportedException();
            }
            return properties.GetProperty<object>(propertyName);
        }

        internal T GetTechnologyManagerProperty<T>(UITechnologyManagerProperty propertyName)
        {
            if (!properties.ContainsKey(propertyName))
            {
                throw new NotSupportedException();
            }
            return properties.GetProperty<T>(propertyName);
        }

        internal static T GetTechnologyManagerProperty<T>(IUITechnologyManager techManager, UITechnologyManagerProperty property)
        {
            T local = default(T);
            try
            {
                object technologyManagerProperty = techManager.GetTechnologyManagerProperty(property);
                if (technologyManagerProperty != null && typeof(T).IsAssignableFrom(technologyManagerProperty.GetType()))
                {
                    local = (T)technologyManagerProperty;
                }
            }
            catch (COMException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (NotImplementedException)
            {
            }
            return local;
        }

        private int GetWindowedAncestor(AutomationElement element)
        {
            if (element != null)
            {
                int nativeWindowHandle = element.Current.NativeWindowHandle;
                while (element != null && nativeWindowHandle == 0)
                {
                    element = TreeWalker.ControlViewWalker.GetParent(element);
                    if (element != null)
                    {
                        nativeWindowHandle = element.Current.NativeWindowHandle;
                    }
                }
                if (element != null && nativeWindowHandle != 0)
                {
                    return nativeWindowHandle;
                }
            }
            return -1;
        }


        public abstract bool MatchElement(ITaskActivityElement element, object parsedQueryIdCookie, out bool useEngine);

        public abstract string ParseQueryId(string queryElement, out object parsedQueryIdCookie);
        public abstract void ProcessMouseEnter(IntPtr handle);
        public abstract bool RemoveEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        public abstract bool RemoveGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        public abstract object[] Search(object parsedQueryIdCookie, ITaskActivityElement parentElement, int maxDepth);
        public virtual void SetTechnologyManagerProperty(UITechnologyManagerProperty propertyName, object propertyValue)
        {
            switch (propertyName)
            {
                case UITechnologyManagerProperty.None:
                case UITechnologyManagerProperty.ContainerScrollingSupported | UITechnologyManagerProperty.SearchSupported:
                case (UITechnologyManagerProperty)4:
                case (UITechnologyManagerProperty)5:
                case (UITechnologyManagerProperty)6:
                case (UITechnologyManagerProperty)7:
                case (UITechnologyManagerProperty)8:
                case (UITechnologyManagerProperty)9:
                    break;

                case UITechnologyManagerProperty.SearchSupported:
                    properties.AddProperty<bool>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.ContainerScrollingSupported:
                    properties.AddProperty<bool>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.WaitForReadyTimeout:
                    properties.AddProperty<int>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.SearchTimeout:
                    properties.AddProperty<int>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.SmartMatchOptions:
                    properties.AddProperty<SmartMatchOptions>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.ExactQueryIdMatch:
                    properties.AddProperty<object>(propertyName, propertyValue, false);
                    return;

                case UITechnologyManagerProperty.WaitForReadyLevel:
                    properties.AddProperty<WaitForReadyLevel>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.NumberOfSearchRetriesForFailFast:
                    properties.AddProperty<int>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.WindowLessTreeSwitchingSupported:
                    properties.AddProperty<bool>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.DoNotGenerateVisibleOnlySearchConfiguration:
                    properties.AddProperty<IList<ControlType>>(propertyName, propertyValue, false);
                    return;

                case UITechnologyManagerProperty.FilterPropertiesForSearchSupported:
                    properties.AddProperty<bool>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.NavigationTimeout:
                    properties.AddProperty<int>(propertyName, propertyValue, true);
                    return;

                case UITechnologyManagerProperty.MergeSingleSessionObjects:
                    if (propertyValue == null || !(propertyValue is bool))
                    {
                        properties.AddProperty<object>(propertyName, propertyValue, false);
                        break;
                    }
                    properties.AddProperty<bool>(propertyName, propertyValue, true);
                    return;

                default:
                    return;
            }
        }

        public abstract void StartSession(bool recordingSession);
        public abstract void StopSession();

        public abstract string TechnologyName { get; }
    }
}
