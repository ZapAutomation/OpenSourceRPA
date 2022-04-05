using System;
using System.Drawing;
using System.Windows.Forms;

namespace Zappy.ZappyActions.OCR.Snipping.Forms
{
    public partial class OcrResultForm : Form
    {
        public OcrResultForm()
        {
            InitializeComponent();
        }

        public static void ShowOcr(String result)
        {
            var form = new OcrResultForm();
            {
                form.txtLog.Text = result.Replace("\n", Environment.NewLine);
                form.txtLog.ForeColor = SystemColors.WindowText;
            }
            form.txtLog.Select(0, 0);
            form.Show();
        }
    }
}
