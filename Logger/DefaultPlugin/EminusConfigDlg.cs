using System;
using System.Windows.Forms;

namespace ZappyLogger.DefaultPlugin
{
    public partial class EminusConfigDlg : Form
    {
        #region Fields

        #endregion

        #region cTor

        public EminusConfigDlg(EminusConfig config)
        {
            InitializeComponent();
            this.TopLevel = false;
            this.Config = config;

            this.hostTextBox.Text = config.host;
            this.portTextBox.Text = "" + config.port;
            this.passwordTextBox.Text = config.password;
        }

        #endregion

        #region Properties

        public EminusConfig Config { get; set; }

        #endregion

        #region Public methods

        public void ApplyChanges()
        {
            this.Config.host = this.hostTextBox.Text;
            try
            {
                this.Config.port = short.Parse(this.portTextBox.Text);
            }
            catch (FormatException)
            {
                this.Config.port = 0;
            }

            this.Config.password = this.passwordTextBox.Text;
        }

        #endregion
    }
}