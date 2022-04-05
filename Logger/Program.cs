using System;
using System.IO;
using System.Windows.Forms;
using ZappyLogger.Controls.LogTabWindow;

namespace ZappyLogger
{

    internal static class Program
    {
        public static Form UI_Instance { get; private set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] orgArgs)
        {
            string[] fileNames = new string[1];
            fileNames[0] = Path.Combine("C:\\Users\\Comp5\\AppData\\Roaming\\IntegerAI_DEBUG\\Audit", "01-2019.audit");

            LogTabWindow logWin = new LogTabWindow(fileNames, 1, false);
            UI_Instance = logWin;
            Application.Run(UI_Instance);
        }


    }
}