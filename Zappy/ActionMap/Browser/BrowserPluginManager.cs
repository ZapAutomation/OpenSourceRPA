using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Crapy.ActionMap.TaskTechnology;
using Crapy.ActionMap.ZappyTaskUtil;
using Crapy.Decode.Helper;
using Crapy.Decode.LogManager;
using Crapy.ExecuteTask.Helpers;
using Microsoft.Win32;

namespace Crapy.ActionMap.Browser
{
    internal class BrowserPluginManager
    {
        private static IList<BrowserFactory> browserFactoryList;
        private static List<BrowserHelper> browserHelperList;
        private static BrowserPluginManager instance = new BrowserPluginManager();
        private static readonly string InternetExplorerBrowserString = "Internet Explorer";
        private static readonly string InternetExplorerFormatString = "Internet Explorer {0}.{1}";
        private static object lockObject = new object();
        private static List<string> supportedBrowsers;

        private BrowserPluginManager()
        {
        }

        public BrowserButtonType GetBrowserButtonType(TaskActivityElement element)
        {
            BrowserButtonType none = BrowserButtonType.None;
            if (this.BrowserInfoList != null)
            {
                foreach (BrowserHelper helper in this.BrowserInfoList)
                {
                    try
                    {
                        none = helper.GetBrowserButtonType(element);
                        if (none != BrowserButtonType.None)
                        {
                            return none;
                        }
                    }
                    catch (NotSupportedException)
                    {
                    }
                    catch (NotImplementedException)
                    {
                    }
                    catch (ZappyTaskException exception)
                    {
                        object[] args = new object[] { exception.Message };
                        //CrapyLogger.log.WarnFormat("BPM: GetBrowserButtonType exception - {0}", args);

                    }
                }
            }
            return none;
        }

        public BrowserFactory GetBrowserFactoryByName(string browserName)
        {
            BrowserFactory factory = null;
            int num = 0;
            foreach (BrowserFactory factory2 in this.BrowserFactoryList)
            {
                int browserSupportLevel = factory2.GetBrowserSupportLevel(browserName);
                if (browserSupportLevel > num)
                {
                    factory = factory2;
                    num = browserSupportLevel;
                }
            }
            return factory;
        }

        private BrowserHelper GetBrowserFromDocument(TaskActivityElement element)
        {
            foreach (BrowserHelper helper2 in this.BrowserInfoList)
            {
                try
                {
                    if (helper2.IsBrowserDocumentWindow(element))
                    {
                        return helper2;
                    }
                }
                catch (NotSupportedException)
                {
                }
                catch (NotImplementedException)
                {
                }
                catch (ZappyTaskException exception)
                {
                    object[] args = new object[] { exception.Message };
                    //CrapyLogger.log.WarnFormat("BPM: GetBrowserFromDocument exception - {0}", args);

                }
            }
            return null;
        }

        private BrowserHelper GetBrowserFromDocumentWindow(IntPtr windowHandle)
        {
            foreach (BrowserHelper helper2 in this.BrowserInfoList)
            {
                try
                {
                    if (helper2.IsBrowserDocumentWindow(windowHandle))
                    {
                        return helper2;
                    }
                }
                catch (NotSupportedException)
                {
                }
                catch (NotImplementedException)
                {
                }
                catch (ZappyTaskException exception)
                {
                    object[] args = new object[] { exception.Message };
                    //CrapyLogger.log.WarnFormat("BPM: GetBrowserFromDocumentWindow exception - {0}", args);

                }
            }
            return null;
        }

        private BrowserHelper GetBrowserFromProcess(Process process)
        {
            foreach (BrowserHelper helper2 in this.BrowserInfoList)
            {
                try
                {
                    if (helper2.IsBrowserProcess(process))
                    {
                        return helper2;
                    }
                }
                catch (NotSupportedException)
                {
                }
                catch (NotImplementedException)
                {
                }
                catch (ZappyTaskException exception)
                {
                    object[] args = new object[] { exception.Message };
                    //CrapyLogger.log.WarnFormat("BPM: GetBrowserFromProcess exception - {0}", args);

                }
            }
            return null;
        }

        private BrowserHelper GetBrowserHelperFromWindow(TaskActivityElement element)
        {
            foreach (BrowserHelper helper2 in this.BrowserInfoList)
            {
                try
                {
                    if (helper2.IsBrowserWindow(element))
                    {
                        return helper2;
                    }
                }
                catch (NotSupportedException)
                {
                }
                catch (NotImplementedException)
                {
                }
                catch (ZappyTaskException exception)
                {
                    object[] args = new object[] { exception.Message };
                    //CrapyLogger.log.WarnFormat("BPM: GetBrowserHelperFromWindow exception - {0}", args);

                }
            }
            return null;
        }

        public Uri GetHomePage(Process process)
        {
            Uri homepage = null;
            BrowserHelper browserFromProcess = this.GetBrowserFromProcess(process);
            if (browserFromProcess != null)
            {
                homepage = browserFromProcess.Homepage;
            }
            return homepage;
        }

        public string GetPageTitle(TaskActivityElement element, string pageTitle)
        {
            BrowserHelper browserHelperFromWindow = this.GetBrowserHelperFromWindow(element);
            if (browserHelperFromWindow != null)
            {
                pageTitle = browserHelperFromWindow.GetPageTitle(pageTitle);
            }
            return pageTitle;
        }

        public Uri GetUrlFromBrowserDocumentWindow(TaskActivityElement element)
        {
            Uri urlFromBrowserDocumentWindow = null;
            BrowserHelper browserFromDocument = this.GetBrowserFromDocument(element);
            if (browserFromDocument != null)
            {
                urlFromBrowserDocumentWindow = browserFromDocument.GetUrlFromBrowserDocumentWindow(element);
            }
            return urlFromBrowserDocumentWindow;
        }

        public bool IsBrowserDocumentWindow(TaskActivityElement element) =>
            (this.GetBrowserFromDocument(element) != null);

        public bool IsBrowserDocumentWindow(IntPtr windowHandle) =>
            (this.GetBrowserFromDocumentWindow(windowHandle) != null);

        public bool IsBrowserProcess(Process process) =>
            (this.GetBrowserFromProcess(process) != null);

        public bool IsBrowserWindow(TaskActivityElement element) =>
            (this.GetBrowserHelperFromWindow(element) != null);

        public bool IsBrowserWindow(string className)
        {
            foreach (BrowserHelper helper in this.BrowserInfoList)
            {
                try
                {
                    if (helper.IsBrowserWindow(className))
                    {
                        return true;
                    }
                }
                catch (NotSupportedException)
                {
                }
                catch (NotImplementedException)
                {
                }
                catch (ZappyTaskException exception)
                {
                    object[] args = new object[] { exception.Message };
                    //CrapyLogger.log.WarnFormat("BPM: IsBrowserWindow exception - {0}", args);

                }
            }
            return false;
        }

        private IList<BrowserFactory> BrowserFactoryList
        {
            get
            {
                if (browserFactoryList == null)
                {
                    browserFactoryList = ZappyTaskService.Instance.GetExtensions<BrowserFactory>();
                }
                return browserFactoryList;
            }
        }

        private List<BrowserHelper> BrowserInfoList
        {
            get
            {
                if (browserHelperList == null)
                {
                    object lockObject = BrowserPluginManager.lockObject;
                    lock (lockObject)
                    {
                        if ((browserHelperList == null) && (this.BrowserFactoryList != null))
                        {
                            browserHelperList = new List<BrowserHelper>();
                            foreach (BrowserFactory factory in this.BrowserFactoryList)
                            {
                                try
                                {
                                    BrowserHelper browserHelper = factory.GetBrowserHelper();
                                    if (browserHelper != null)
                                    {
                                        browserHelperList.Add(browserHelper);
                                    }
                                }
                                catch (NotSupportedException)
                                {
                                }
                                catch (NotImplementedException)
                                {
                                }
                            }
                        }
                    }
                }
                return browserHelperList;
            }
        }

        public string InstalledInternetExplorer
        {
            get
            {
                string str = null;
                string str2 = ZappyTaskUtilities.GetRegistryValue<string>(Registry.LocalMachine, @"Software\Microsoft\Internet Explorer", "Version", false);
                if (str2 != null)
                {
                    Version version = new Version(str2);
                    object[] args = new object[] { version.Major, version.Minor };
                    str = string.Format(CultureInfo.InvariantCulture, InternetExplorerFormatString, args);
                }
                return str;
            }
        }

        public static BrowserPluginManager Instance =>
            instance;

        public string InternetExplorerBrowserName =>
            InternetExplorerBrowserString;

        public IList<string> SupportedBrowsers
        {
            get
            {
                if (supportedBrowsers == null)
                {
                    supportedBrowsers = new List<string>();
                    foreach (BrowserFactory factory in this.BrowserFactoryList)
                    {
                        supportedBrowsers.AddRange(factory.SupportedVersions);
                    }
                }
                return supportedBrowsers;
            }
        }
    }

}
