using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows.Automation;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    [Description("Gets Html Content From SetURL or active tab of Chrome")]
    public class GetHtmlContentFromUrl : TemplateAction
    {
        [Category("Input")]
        [Description("If input blank - then gets the data from current set url of chrome")]
        public DynamicProperty<string> URL { get; set; }

        [Category("Output")]
        [Description("HtmlResponeCode from the set URL")]
        public DynamicProperty<string> htmlResponseCode { get; set; }

                                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string _URL = string.Empty;
            if (string.IsNullOrEmpty(URL.ToString()))
            {
                _URL = GetActiveTabUrl();
            }
            else
            {
                _URL = URL.ToString();
            }
            htmlResponseCode = GetHtmlContain(_URL);

        }
        public static string GetHtmlContain(string url)
        {
            string htmlCode = string.Empty;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent: Other");

                var htmlData = client.DownloadData(url);
                htmlCode = Encoding.UTF8.GetString(htmlData);
                            }
            return htmlCode;
        }
        public static string GetActiveTabUrl()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");

            if (procsChrome.Length <= 0)
                return null;

            foreach (Process proc in procsChrome)
            {
                                if (proc.MainWindowHandle == IntPtr.Zero)
                    continue;

                                AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                var SearchBar = root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
                if (SearchBar != null)
                    return (string)SearchBar.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
            }
            return null;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " URL:" + this.URL + " htmlResponseCode:" + this.htmlResponseCode;
        }
    }
}
