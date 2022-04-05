using System;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.Decode.Helper;

namespace Zappy.Plugins.ChromeBrowser.Chrome
{
    public partial class WebExtractDataForm : Form
    {
        public string htmlTableId { get; set; }

        public string url;

        public WebExtractDataForm(object HtmlTableId)
        {
            InitializeComponent();
            htmlTableId = null;
            htmlTableId = HtmlTableId as string;
            ChromeActionRecieverService.TblClickEvent += ChromeActionRecieverService_TblClickEvent;
        }

        private void ChromeActionRecieverService_TblClickEvent(object sender, string e)
        {
            this.htmlTableId = e;

            this.Activate();
            this.BringToFront();
            this.Show();
            this.Focus();
            if (this.htmlTableId.Contains("table"))
            {
                int pos = this.htmlTableId.IndexOf("table");
                int length = pos + 5;
                string sub = this.htmlTableId.Substring(0, length);
                this.htmlTableId = sub;
            }
            else
            {
                this.htmlTableId = null;
                MessageBox.Show("Click on correct table");
            }

        }

        private void ExtractTable_Click(object sender, EventArgs e)
        {
            //Send a request and wait for a response
            
            //Tuple<int, ExcelRequest, string> _Request = new Tuple<int, ExcelRequest, string>(_RequestCount++,
            //        ExcelRequest.GetCellProperty,
            //        ZappySerializer.SerializeObject(cellInfo) + CrapyConstants.StringArrayDelemiter + propertyName);
            //_PubSub.Publish(_RequestChannel, ZappySerializer.SerializeObject(_Request));
            //_mre.Reset();
            //if (_mre.Wait(_ExcelResponseTimeout))
            //    return ZappySerializer.DeserializeObject<Tuple<int, object>>(_Response).Item2;
            //throw new Exception("Unable To Get Cell Value");

            //string currOpenTabUrl = null;
            url = textBox1.Text.ToString();
            Process.Start("chrome.exe", url);
            //IntPtr CHandle = GetChromeHandle();
            //if (!CHandle.Equals(IntPtr.Zero))
            //{
            //    currOpenTabUrl = getChromeUrl(CHandle);
            //}
            //if (url.Equals(currOpenTabUrl))
            //{
            //    NativeMethods.ShowWindow(CHandle, NativeMethods.WindowShowStyle.Show);
            //    NativeMethods.BringWindowToTop(CHandle);
            //    //NativeMethods.SetForegroundWindow(CHandle);

            //}
            //else
            //    Process.Start("chrome.exe", url);

            //PubSubService.Instance.Publish(PubSubTopicRegister.Zappy2ChromeRequest, PubSubMessages.StartTableRecordingMessage);
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            //PubSubService.Instance.Publish(PubSubTopicRegister.Zappy2ChromeRequest, PubSubMessages.StopTableRecordingMessage);
            DialogResult = DialogResult.OK;
            ChromeActionRecieverService.TblClickEvent -= ChromeActionRecieverService_TblClickEvent;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            htmlTableId = null;
            DialogResult = DialogResult.Cancel;
            ChromeActionRecieverService.TblClickEvent -= ChromeActionRecieverService_TblClickEvent;
        }
        public static IntPtr GetChromeHandle()
        {
            IntPtr ChromeHandle = IntPtr.Zero;
            Process[] procsExe = Process.GetProcessesByName("chrome");

            foreach (Process process in procsExe)
            {
                // the chrome process must have a window
                if (process.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }
                ChromeHandle = process.MainWindowHandle;
                break;
            }
            return ChromeHandle;
        }
        public static string getChromeUrl(IntPtr winHandle)
        {
            string browserUrl = null;
            string sb = NativeMethods.GetWindowText(winHandle);

            AutomationElement elm = AutomationElement.FromHandle(winHandle);
            AutomationElement elmUrlBar = elm.FindFirst(TreeScope.Descendants,
              new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

            // if it can be found, get the value from the URL bar
            if (elmUrlBar != null)
            {
                return (string)elmUrlBar.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
            }
            return browserUrl;
        }

        private void WebExtractDataForm_Load(object sender, EventArgs e)
        {
            //htmlTableId = null;
            //ChromeAction cAction = new ChromeAction();
            //cAction.CommandName = "StartTableRecordingMessage";
            ////cAction.CommandTarget.Add("Test");
            //CommonProgram.StartPlaybackFromIZappyAction(cAction);

            //cAction.Invoke(null, null);
        }
    }
}
