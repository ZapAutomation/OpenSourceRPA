using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Zappy.Helpers
{
    public partial class frmComboBox : Form
    {
        public frmComboBox()
        {
            InitializeComponent();
        }
        public frmComboBox(string Title, string PromptText, string FieldName, Icon icon, List<string> Value)
        {
            InitializeComponent();
            this.Text = Title;
            if (icon != null)
                pictureBox1.Image = Bitmap.FromHicon(icon.Handle);
            lblTitle.Text = PromptText;
            lblFieldName.Text = FieldName;
            comboBoxInput.Items.AddRange(Value.ToArray());
        }
        //public string Input { get { return txtInputValue.Text; } }

    }
}
