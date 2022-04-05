using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.CustomMarshalers;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;

namespace Zappy.ExecuteTask.Helpers.Interface
{
#if COMENABLED
    [Guid("1984584A-4C77-488a-BD8B-3D7FE868F5B8"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibType(TypeLibTypeFlags.FNonExtensible)]
#endif
    public interface IUITechnologyManager
    {
        [DispId(1)]
        string TechnologyName { get; }
        [DispId(2)]
        void StartSession(bool recordingSession);
        [DispId(3)]
        void StopSession();
        [DispId(4)]
        ITaskActivityElement GetElementFromNativeElement(object nativeElement);
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "", MarshalTypeRef = typeof(EnumeratorToEnumVariantMarshaler), MarshalCookie = "")]
        [DispId(5), ComVisible(true)]
        IEnumerator GetChildren(ITaskActivityElement element, object parsedQueryIdCookie);
        [DispId(6)]
        ITaskActivityElement GetElementFromWindowHandle(IntPtr handle);
        [DispId(7)]
        ITaskActivityElement GetElementFromPoint(int pointX, int pointY);
        [DispId(8)]
        ITaskActivityElement GetParent(ITaskActivityElement element);
        [DispId(9)]
        ITaskActivityElement GetNextSibling(ITaskActivityElement element);
        [DispId(10)]
        ITaskActivityElement GetPreviousSibling(ITaskActivityElement element);
        [DispId(11)]
        ITaskActivityElement GetFocusedElement(IntPtr handle);
        [DispId(12)]
        bool AddEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        [DispId(13)]
        bool RemoveEventHandler(ITaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        [DispId(14)]
        IUISynchronizationWaiter GetSynchronizationWaiter(ITaskActivityElement element, ZappyTaskEventType eventType);
        [DispId(15)]
        object[] Search(object parsedQueryIdCookie, ITaskActivityElement parentElement, int maxDepth);
        [DispId(0x10)]
        string ParseQueryId(string queryElement, out object parsedQueryIdCookie);
        [DispId(0x11)]
        bool MatchElement(ITaskActivityElement element, object parsedQueryIdCookie, out bool useEngine);
        [DispId(0x12)]
        ILastInvocationInfo GetLastInvocationInfo();
        [DispId(0x13)]
        void CancelStep();
        [DispId(20)]
        object GetTechnologyManagerProperty(UITechnologyManagerProperty propertyName);
        [DispId(0x15)]
        void SetTechnologyManagerProperty(UITechnologyManagerProperty propertyName, object propertyValue);
        [DispId(0x16)]
        ITaskActivityElement ConvertToThisTechnology(ITaskActivityElement elementToConvert, out int supportLevel);
        [DispId(0x17)]
        int GetControlSupportLevel(IntPtr windowHandle);
    }
}