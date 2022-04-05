using System.Threading;
using ZappyMessages;
using ZappyMessages.Logger;

namespace ZappyOutlookAddIn
{
    public partial class ThisAddIn
    {

        /// <summary>
        /// Singleton instance to this add-in.
        /// </summary>
        internal static ThisAddIn Instance
        {
            get;
            private set;
        }
        private ZappyOutlookCommunicator _Communicator;
        private Mutex m;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            m = new Mutex(true, "_ZappyAI_Outlook_Addin_", out bool createdNew);
            Instance = this;
            ConsoleLogger.Init("OutlookLog.txt");
            if (!createdNew)
            {
                ConsoleLogger.Info("Zappy Outlook addin already running");
                return;
            }
            _Communicator = new ZappyOutlookCommunicator();
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            m.Dispose();
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
