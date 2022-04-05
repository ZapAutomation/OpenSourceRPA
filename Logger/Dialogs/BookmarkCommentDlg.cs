using System.Windows.Forms;

namespace ZappyLogger.Dialogs
{
    public partial class BookmarkCommentDlg : Form
    {
        #region cTor

        public BookmarkCommentDlg()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public string Comment
        {
            set { this.commentTextBox.Text = value; }
            get { return this.commentTextBox.Text; }
        }

        #endregion
    }
}