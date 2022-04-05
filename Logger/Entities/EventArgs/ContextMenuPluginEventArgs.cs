using System.Collections.Generic;
using ZappyLogger.ColumnizaeLib;

namespace ZappyLogger.Entities.EventArgs
{
    public class ContextMenuPluginEventArgs : System.EventArgs
    {
        #region Fields

        #endregion

        #region cTor

        public ContextMenuPluginEventArgs(IContextMenuEntry entry, IList<int> logLines, ILogLineColumnizer columnizer,
            IZappyLoggerCallback callback)
        {
            this.Entry = entry;
            this.LogLines = logLines;
            this.Columnizer = columnizer;
            this.Callback = callback;
        }

        #endregion

        #region Properties

        public IContextMenuEntry Entry { get; }

        public IList<int> LogLines { get; }

        public ILogLineColumnizer Columnizer { get; }

        public IZappyLoggerCallback Callback { get; }

        #endregion
    }
}