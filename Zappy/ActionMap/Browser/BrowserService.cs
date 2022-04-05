using System;
using System.Diagnostics.CodeAnalysis;
using Crapy.ActionMap.TaskTechnology;

namespace Crapy.ActionMap.Browser
{
    public abstract class BrowserService : IDisposable
    {
        public abstract void Back();
        public abstract void Close();
        public abstract void Dispose();
        public virtual object ExecuteScript(string script, params object[] args) =>
            null;

        public abstract void Forward();
        public abstract void NavigateToHomepage();
        public abstract void NavigateToUrl(Uri uri);
        public abstract void PerformDialogAction(BrowserDialogAction browserDialogAction, object actionParameter);
        public abstract void Refresh();

        public abstract void StopPageLoad();

        public abstract TaskActivityElement DocumentWindow { get; }

        public abstract TaskActivityElement TopLevelWindow { get; }

        public abstract Uri Uri { get; }

        public abstract Version Version { get; }
    }
}