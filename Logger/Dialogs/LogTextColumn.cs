using System.Windows.Forms;

namespace ZappyLogger.Dialogs
{
    public class LogTextColumn : DataGridViewColumn
    {
        #region cTor

        public LogTextColumn()
            : base(new LogGridCell())
        {
        }

        #endregion
    }
}