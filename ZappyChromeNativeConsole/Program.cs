using System;
using System.Threading;
using ZappyChromeNativeConsole.ChromeHelper;
using ZappyMessages;
using ZappyMessages.Logger;

namespace ZappyChromeNativeConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            ConsoleLogger.Init("chromelog.txt");
            Mutex m = new Mutex(true, "_ZappyAI_Chrome_Extension_", out bool createdNew);
            if (!createdNew)
            {
                ConsoleLogger.Info("Zappy Chrome engine already running");
                return;
            }
            // Open the ServiceHost to create listeners and start listening for messages.
            try
            {
                ChromeNativeService.InitChromeNativeService();
                ChromeActionManager.Init();
            }
            catch (Exception ex)
            {
                ConsoleLogger.Error(ex);
            }
            m.Dispose();
            //ChromeNativeConsoleLogger.Close();
        }



    }
}
