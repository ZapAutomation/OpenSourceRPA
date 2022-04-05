using System;
using System.Collections.Generic;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.Decode.Helper
{
    internal interface IZappyTaskService
    {
        bool AddEventHandler(TaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        void AddExtensionToCoreTechnologyMapping(string extensionTechnologyName, string coreTechnologyName);
        bool AddGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        void Cleanup();
        TaskActivityElement ConvertTechnologyElement(ITaskActivityElement unmanagedMsaaElement);
        TaskActivityElement ConvertToMappedTechnologyElement(ITaskActivityElement element);
        IEnumerator<TaskActivityElement> GetChildren(ITaskActivityElement element, object parsedQueryIdCookie);
        string GetCoreTechnologyName(string extensionTechnologyName);
        TaskActivityElement GetElementFromNativeElement(string technologyName, object nativeElement);
        TaskActivityElement GetElementFromPoint(int pointX, int pointY);
        TaskActivityElement GetElementFromWindowHandle(IntPtr handle);
        IList<T> GetExtensions<T>() where T : class;
        TaskActivityElement GetFirstChild(TaskActivityElement element);

        TaskActivityElement GetFocusedElement();
        TaskActivityElement GetNextSibling(TaskActivityElement element);
        TaskActivityElement GetParent(TaskActivityElement element);
        TaskActivityElement GetPreviousSibling(TaskActivityElement element);
        TaskActivityElement GetTopLevelElementUsingWindowTree(ITaskActivityElement element);
        void Initialize();
        bool RemoveEventHandler(TaskActivityElement element, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        bool RemoveGlobalEventHandler(ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink);
        void StartSession(bool recordingSession);
        void StopSession();
        UITechnologyManager TechnologyManagerByName(string technologyName);
        UITechnologyManager TechnologyManagerFromHandle(IntPtr handle);
        void UpdateQueryIdForTopLevelElement(TaskActivityElement element);

        bool EatNavigationTimeoutException { get; set; }

        bool IsSessionStarted { get; }

        IUITechnologyPluginManager PluginManager { get; }

        TaskActivityElement RootElement { get; }
    }
}