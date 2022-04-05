using System;
using System.Windows.Forms;
using ZappyLogger.Controls.LogTabWindow;

namespace ZappyLogger.Classes
{
    internal class ZappyLoggerApplicationContext : ApplicationContext
    {
        #region Fields

        private readonly ZappyLoggerProxy proxy;

        #endregion

        #region cTor

        public ZappyLoggerApplicationContext(ZappyLoggerProxy proxy, LogTabWindow firstLogWin)
        {
            this.proxy = proxy;
            this.proxy.LastWindowClosed += new ZappyLoggerProxy.LastWindowClosedEventHandler(proxy_LastWindowClosed);
            firstLogWin.Show();
        }

        #endregion

        #region Events handler

        private void proxy_LastWindowClosed(object sender, EventArgs e)
        {
            ExitThread();
            Application.Exit();
        }

        #endregion
    }
}