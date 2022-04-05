using System;
using System.Threading;
using ZappyMessages;
using ZappyMessages.Logger;

namespace ZappyExcelAddIn
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

        private ZappyExcelCommunicator _Communicator;
        private Mutex m;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            m = new Mutex(true, "_ZappyAI_Excel_Addin_", out bool createdNew);
            Instance = this;
            try
            {
                ConsoleLogger.Init("ExcelLog.txt");
                if (!createdNew)
                {
                    ConsoleLogger.Info("Zappy excel addin already running");
                    return;
                }
                _Communicator = new ZappyExcelCommunicator();
            }
            catch (Exception)
            {
            }

            //channel = new IpcChannel("ExcelUITest");
            //ChannelServices.RegisterChannel(channel, false);
            //RemotingConfiguration.RegisterWellKnownServiceType(typeof(UITestCommunicator),
            //    "ExcelUITest", WellKnownObjectMode.Singleton);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            m.Dispose();
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
