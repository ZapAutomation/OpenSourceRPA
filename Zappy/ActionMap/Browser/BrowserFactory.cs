using System;
using System.Collections.Generic;
using Crapy.ExecuteTask.Helpers;

namespace Crapy.ActionMap.Browser
{
    public abstract class BrowserFactory
    {
        public abstract BrowserHelper GetBrowserHelper();
        public abstract int GetBrowserSupportLevel(string browserName);
        public abstract BrowserService Launch();
        public abstract BrowserService Launch(Uri uri);
        public abstract BrowserService Launch(string[] arguments);
        public abstract BrowserService Locate(IntPtr handle);
        public abstract BrowserService Locate(PropertyExpressionCollection searchProperties, PropertyExpressionCollection filterProperties);
        public abstract BrowserService[] LocateAll(PropertyExpressionCollection searchProperties, PropertyExpressionCollection filterProperties);

        public abstract string Name { get; }

        public abstract IList<string> SupportedVersions { get; }

        public abstract string TechnologyManagerTypeName { get; }
    }
}