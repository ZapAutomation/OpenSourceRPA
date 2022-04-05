using System;
using System.Windows.Forms;

namespace ZappyLogger.Dialogs
{
    public partial class SearchProgressDialog : Form
    {
        #region Fields

        #endregion

        #region cTor

        public SearchProgressDialog()
        {
            InitializeComponent();
            this.ShouldStop = false;
        }

        #endregion

        #region Properties

        public bool ShouldStop { get; private set; }

        #endregion

        #region Events handler

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.ShouldStop = true;
        }

        #endregion
    }
}