using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Crapy.ActionMap.TaskTechnology;

namespace Crapy.ActionMap.Browser
{
    public abstract class BrowserHelper
    {
        public abstract void ClearCache();
        public abstract void ClearCookies();
        public abstract BrowserButtonType GetBrowserButtonType(TaskActivityElement element);
        public abstract string GetPageTitle(string windowTitle);
        public abstract Uri GetUrlFromBrowserDocumentWindow(TaskActivityElement element);
        public abstract bool IsBrowserDocumentWindow(TaskActivityElement element);
        public abstract bool IsBrowserDocumentWindow(IntPtr windowHandle);
        public abstract bool IsBrowserProcess(Process process);
        public abstract bool IsBrowserWindow(TaskActivityElement element);
        public abstract bool IsBrowserWindow(string className);


        public abstract bool AllowPopups { get; set; }

        public abstract Uri Homepage { get; }
    }
}