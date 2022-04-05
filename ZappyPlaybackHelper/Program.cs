using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using ZappyPlaybackHelper.Interpreter;

namespace ZappyPlaybackHelper
{
    class Program
    {
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool SetProcessDpiAwarenessContext(int nFlags);

        static void Main(string[] args)
        {
            try
            {
                bool createdNew;
                Mutex m = new Mutex(true, "_ZappyAI_ExecutionEngine_Zappy_", out createdNew);
                if (!createdNew)
                {
                    MessageBox.Show("Zappy execution engine already running");
                    return;
                }
                //SetProcessDpiAwarenessContext(-1);
                CommonProgram.AssemblyResolve();
                CommonProgram.PrecompileAssemblies();
                //HotKeyManager.RegisterHotKey(Keys.K, KeyModifiers.Alt| KeyModifiers.Control);
                //HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
                CrapyLogger.Init();
                AuditLogger.Init();
                PlaybackHelperService _Service = new PlaybackHelperService();

                try
                {
                    while (true)
                    {
                        Thread.Sleep(10000);
                        Console.WriteLine("ping");
                    }
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.Error(ex);
                }
                m.Dispose();
            }

            catch (Exception ex1)
            {
                CrapyLogger.log.Error(ex1);
            }
        }

        //static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        //{
        //    PlaybackHelperService.CancelTask();
        //}
    }
}
