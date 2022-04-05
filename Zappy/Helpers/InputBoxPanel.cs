using System.Drawing;
using System.Windows.Forms;

namespace Zappy.Helpers
{
    public partial class InputBoxPanel : Form
    {
        public InputBoxPanel()
        {
            InitializeComponent();
        }
        public InputBoxPanel(string Title, string PromptText, string FieldName, Icon icon, string[] Value, bool password)
        {
            InitializeComponent();
            this.Text = Title;
            if (icon != null)
                pictureBox1.Image = Bitmap.FromHicon(icon.Handle);
            lblTitle.Text = PromptText;
            lblFieldName.Text = FieldName;
            txtInputValue.Lines = Value;
            if (password)
                this.txtInputValue.PasswordChar = '*';

        }
        public string[] Input { get { return txtInputValue.Lines; } }


    }
}
