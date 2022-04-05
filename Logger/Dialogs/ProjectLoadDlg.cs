using System;
using System.Windows.Forms;

namespace ZappyLogger.Dialogs
{
    public partial class ProjectLoadDlg : Form
    {
        #region Fields

        #endregion

        #region cTor

        public ProjectLoadDlg()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public ProjectLoadDlgResult ProjectLoadResult { get; set; } = ProjectLoadDlgResult.Cancel;

        #endregion

        #region Events handler

        private void closeTabsButton_Click(object sender, EventArgs e)
        {
            this.ProjectLoadResult = ProjectLoadDlgResult.CloseTabs;
            Close();
        }

        private void newWindowButton_Click(object sender, EventArgs e)
        {
            this.ProjectLoadResult = ProjectLoadDlgResult.NewWindow;
            Close();
        }

        private void ignoreButton_Click(object sender, EventArgs e)
        {
            this.ProjectLoadResult = ProjectLoadDlgResult.IgnoreLayout;
            Close();
        }

        #endregion
    }
}