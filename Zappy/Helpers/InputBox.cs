using System.Drawing;
using System.Windows.Forms;

namespace Zappy.Helpers
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }
        public InputBox(string Title, string PromptText, string FieldName, Icon icon, string Value, bool password)
        {
            InitializeComponent();
            this.Text = Title;
            if (icon != null)
                pictureBox1.Image = Bitmap.FromHicon(icon.Handle);
            lblTitle.Text = PromptText;
            lblFieldName.Text = FieldName;
            txtInputValue.Text = Value;
            if (password)
                this.txtInputValue.PasswordChar = '*';

        }
        public string Input { get { return txtInputValue.Text; } }

        private void txtInputValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonOk.PerformClick();
            }
        }
    }
}
