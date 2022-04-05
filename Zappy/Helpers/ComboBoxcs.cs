using System.Drawing;
using System.Windows.Forms;

namespace Zappy.Helpers
{
    public partial class ComboBoxcs : Form
    {

        public ComboBoxcs()
        {
            InitializeComponent();

        }
        public ComboBoxcs(string Title, string PromptText, string FieldName, Icon icon, string[] list)
        {
            InitializeComponent();
            this.Text = Title;
            if (icon != null)
                imageBox1.Image = Bitmap.FromHicon(icon.Handle);
            labelTitle.Text = PromptText;
            labelFieldName.Text = FieldName;
            foreach (string value in list)
            {
                comboBox1.Items.Add(value);
            }
            this.Controls.Add(comboBox1);
        }
        public string SetvalueCombobox()
        {
            return comboBox1.Text;
        }

        public string Input { get; internal set; }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonOK.PerformClick();
            }
        }
    }
}
