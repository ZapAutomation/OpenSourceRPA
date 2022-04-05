using System.Drawing;
using System.Windows.Forms;

namespace Zappy.Helpers
{
    public partial class InputBoxMultipleValues : Form
    {
        public InputBoxMultipleValues()
        {
            InitializeComponent();
        }
        public InputBoxMultipleValues(string Title, string PromptText,
            Icon icon, string FieldName1, string FieldName2, string FieldName3, string FieldName4,
            string FieldName5, string FieldName6, string FieldName7,
            string Value1, string Value2, string Value3, string Value4,
            string Value5, string Value6, string Value7)
        {
            InitializeComponent();
            this.Text = Title;
            if (icon != null)
                pictureBox1.Image = Bitmap.FromHicon(icon.Handle);
            lblTitle.Text = PromptText;
            lblFieldName1.Text = FieldName1;
            lblFieldName2.Text = FieldName2;
            lblFieldName3.Text = FieldName3;
            lblFieldName4.Text = FieldName4;
            lblFieldName5.Text = FieldName5;
            lblFieldName6.Text = FieldName6;
            lblFieldName7.Text = FieldName7;
            txtInputValue1.Text = Value1;
            txtInputValue2.Text = Value2;
            txtInputValue3.Text = Value3;
            txtInputValue4.Text = Value4;
            txtInputValue5.Text = Value5;
            txtInputValue6.Text = Value6;
            txtInputValue7.Text = Value7;
        }
        public string Input1 { get { return txtInputValue1.Text; } }
        public string Input2 { get { return txtInputValue2.Text; } }
        public string Input3 { get { return txtInputValue3.Text; } }
        public string Input4 { get { return txtInputValue4.Text; } }
        public string Input5 { get { return txtInputValue5.Text; } }
        public string Input6 { get { return txtInputValue6.Text; } }
        public string Input7 { get { return txtInputValue7.Text; } }

    }
}
