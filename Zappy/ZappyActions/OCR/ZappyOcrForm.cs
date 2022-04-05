using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zappy.Helpers;
using Zappy.ZappyActions.OCR.Snipping.SnippingTool;

namespace Zappy.ZappyActions.OCR
{
    public partial class ZappyOcrForm : Form
    {
        private bool _isSnipping = false;
        public ZappyOcrForm()
        {
            InitializeComponent();

        }

        private void SnippingTool_Start(object sender, EventArgs e)
        {
            if (_isSnipping)
            {
                this.Hide();
            }
        }

        private void Clip_Click(object sender, EventArgs e)
        {
            if (!_isSnipping)
            {
                OutputResultOcr.Text = string.Empty;
                SnippingTool.Start += SnippingTool_Start;
                SnippingTool.AreaSelected += SnippingToolOnAreaSelected;
                SnippingTool.Cancel += SnippingToolOnCancel;
                this.Hide();
                Thread.Sleep(500);
                _isSnipping = true;
                SnippingTool.Snip();
            }
        }
        private void ProcessOcrImage(Image image)
        {
            ProcessImageUsingTessaract processImageUsingTessaract = new ProcessImageUsingTessaract();
            var result = processImageUsingTessaract.ProcessBitmapImage((Bitmap)image, cmbLanguages.SelectedItem.ToString());
            if (!this.IsDisposed)
            {
                BeginInvoke(new Action(() =>
                 ProcessOcrResults(result)));
            }
        }

        private void ProcessOcrResults(string result)
        {
            OutputResultOcr.Text = result;
            OutputResultOcr.ForeColor = SystemColors.WindowText;
            OutputResultOcr.Select(0, 0);
        }

        private void SnippingToolOnCancel(object sender, EventArgs e)
        {
            try
            {
                _isSnipping = false;
                this.Show();
                SnippingTool.Start -= SnippingTool_Start;
                SnippingTool.AreaSelected -= SnippingToolOnAreaSelected;
                SnippingTool.Cancel -= SnippingToolOnCancel;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void SnippingToolOnAreaSelected(object sender, EventArgs e)
        {
            _isSnipping = false;
            this.Show();
            OutputResultOcr.Text = "Processing....";
            ImagePanelOcr.BackgroundImage = SnippingTool.Image.Clone() as Image;
            Task.Factory.StartNew(() => ProcessOcrImage(SnippingTool.Image));
        }


        private void ZappyOcrForm_Load(object sender, EventArgs e)
        {
            string _DataPath = Path.Combine(CrapyConstants.TesseractFolder, "tessdata");
            string[] _Files = Directory.GetFiles(_DataPath, "*.traineddata");

            for (int i = 0; i < _Files.Length; i++)
            {
                cmbLanguages.Items.Add(Path.GetFileNameWithoutExtension(_Files[i]));
            }
            cmbLanguages.SelectedItem = "eng";
        }

        private void cmbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ZappyOcrForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SnippingTool.Start -= SnippingTool_Start;
            SnippingTool.AreaSelected -= SnippingToolOnAreaSelected;
            SnippingTool.Cancel -= SnippingToolOnCancel;
        }
    }
}
