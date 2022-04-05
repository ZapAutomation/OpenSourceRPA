using System;
using System.Windows.Forms;

namespace ZappyLogger.Dialogs
{
    public class LogGridCell : DataGridViewTextBoxCell
    {
        #region cTor

        public LogGridCell()
            : base()
        {
        }

        #endregion

        #region Properties

        public override Type EditType
        {
            get { return typeof(LogCellEditingControl); }
        }

        #endregion
    }
}