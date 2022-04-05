#define Enable_ExcelOutlookPlugin

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.Plugins.ChromeBrowser;
using Zappy.Plugins.Excel;
using Zappy.Plugins.Outlook;
using Zappy.Trapy;
using ZappyMessages;
using ZappyMessages.Helpers;
using ZappyMessages.Logger;
using ZappyMessages.PubSub;
using DateTime = System.DateTime;

namespace Zappy
{
    class Program
    {
        public static Form UI_Instance { get; private set; }

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                //Required to check if DPI Aware
                NativeMethods.SetProcessDpiAwareness(NativeMethods.PROCESS_DPI_AWARENESS.Process_DPI_Unaware);
                NativeMethods.SetProcessDpiAwarenessContext((int)NativeMethods.DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE);
            }
            catch
            {

            }

            try
            {
                CrapyConstants.Init();
            }
            catch
            {

            }

            bool createdNew;

            Mutex m = new Mutex(true, "_ZappyAI_Zappy_", out createdNew);

            if (!createdNew)
            {
                CommonProgram.RunProcessAsTask(Path.Combine(ZappyMessagingConstants.StartupPath,
                        "ZappyRunner.exe"), "EditFile");
                Thread.Sleep(1000);
                return;
            }
            CommonProgram.AssemblyResolve();
            CommonProgram.PrecompileAssemblies();


            Application.EnableVisualStyles();
            Application.DoEvents();

            try
            {
                PubSubService.Instance.Start();
                UI_Instance = new ClientUI();

                CommonProgram.CleanupFoldersForAppLaunch();
                if (ApplicationSettingProperties.Instance.EnableTraceLog)
                    ConsoleLogger.Init("Trace.log");
#if Enable_ExcelOutlookPlugin
                ExcelCommunicator.Init();
                OutlookCommunicator.Init();
#endif
                ZappyPlaybackCommunicator.Init();
                if (ApplicationSettingProperties.Instance.EnableTraceLog)
                {
                    LoggerSubscription _subscription = new LoggerSubscription();
                    PubSubService.Instance.Subscribe("Trace", _subscription, null);
                }

                ChromeActionRecieverService.Init();
                Application.Run(UI_Instance);
                TrapyService.Stop();
                CommonProgram.CleanupFoldersForAppLaunch();
                m.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to start due to " + ex);
                CrapyLogger.log.Error(ex);
            }
            finally
            {
                PubSubService.Instance.Stop();
            }
        }
    }

}
