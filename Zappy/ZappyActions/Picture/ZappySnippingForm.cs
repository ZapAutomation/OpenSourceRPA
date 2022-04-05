using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.Invoker;
using Zappy.ZappyActions.OCR.Snipping.SnippingTool;
using Zappy.ZappyActions.Picture.Helpers;
using Zappy.ZappyTaskEditor;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyActions.Picture
{
    public partial class ZappySnippingForm : Form
    {
        private bool _isSnipping = false;

        public Bitmap ImageLocation { get; set; }
        public string ClickLocation { get; set; }
        private System.Drawing.Image ClippedImage;
        //private bool GetImagePoints = false;
        public ZappySnippingForm(object imageObject)
        {
            InitializeComponent();
            ImageObject obj = imageObject as ImageObject;
            this.MinimumSize = new Size(0, 0);
            this.Size = new Size(687, 463);
            ImageLocation = obj.PatternFile;// as Bitmap;
            //Point
            ClickLocation = obj.ClickLocation;
            textBox1.Text = ClickLocation;
            //comboBox1.SelectedIndex = 0;
            try
            {
                if (ImageLocation != null)
                {
                    //ImagePanelOcr.BackgroundImage = ImageLocation;
                    //ImagePanelOcr.BackgroundImage = ClippedImage;
                    pictureBox1.Image = ImageLocation;
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void SnippingTool_Start(object sender, EventArgs e)
        {
            if (_isSnipping)
            {
                this.Size = DefaultMinimumSize;
            }
        }
        private void Clip_Click(object sender, EventArgs e)
        {
            if (!_isSnipping)
            {
                SnippingTool.Start += SnippingTool_Start;
                SnippingTool.AreaSelected += SnippingToolOnAreaSelected;
                SnippingTool.Cancel += SnippingToolOnCancel;
                this.Size = DefaultMinimumSize;
                Thread.Sleep(500);
                _isSnipping = true;
                SnippingTool.Snip();
            }
        }

        private void SnippingToolOnCancel(object sender, EventArgs e)
        {
            try
            {
                _isSnipping = false;
                this.Size = new Size(687, 463); ;
                SnippingTool.Start -= SnippingTool_Start;
                SnippingTool.AreaSelected -= SnippingToolOnAreaSelected;
                SnippingTool.Cancel -= SnippingToolOnCancel;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void SnippingToolOnAreaSelected(object sender, EventArgs e)
        {
            _isSnipping = false;
            this.Size = new Size(687, 463);
            //OutputResultOcr.Text = "Processing....";
            ClippedImage = SnippingTool.Image;
            //ImagePanelOcr.BackgroundImage = ClippedImage;
            //Task.Factory.StartNew(() => ProcessOcrImage(SnippingTool.Image));
            pictureBox1.Image = ClippedImage;
        }


        private void ZappyOcrForm_Load(object sender, EventArgs e)
        {
            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            }
        }

        private void ZappyOcrForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SnippingTool.Start -= SnippingTool_Start;
            SnippingTool.AreaSelected -= SnippingToolOnAreaSelected;
            SnippingTool.Cancel -= SnippingToolOnCancel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (ClippedImage != null)
            {
                ImageLocation = (Bitmap)ClippedImage;
            }
            //if (GetImagePoints == true)
            ClickLocation = textBox1.Text;
            //else
            //    ClickLocation = (string)comboBox1.SelectedItem;

            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = e.X + "," + e.Y;
            //GetImagePoints = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = (string)comboBox1.SelectedItem;
        }

        public void ChangeLanguage(LanguageZappy languageZappy)
        {
            try
            {
                string lang = LocalizeTaskEditorHelper.LanguagePicker(languageZappy);

                ComponentResourceManager resources = null;

                foreach (Control c in this.Controls)
                {
                    resources = new ComponentResourceManager(typeof(ZappySnippingForm));
                    resources.ApplyResources(c, c.Name, new CultureInfo(lang));

                }

                foreach (Control c in this.Controls)
                {
                    if (c is Panel)
                    {
                        foreach (Control c1 in c.Controls)
                        {
                            resources = new ComponentResourceManager(typeof(ZappySnippingForm));
                            resources.ApplyResources(c1, c1.Name, new CultureInfo(lang));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void LoadfromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //openFileDialog1.Filter = "Bitmap Files|*.bmp";
            openFileDialog1.Title = "Select a Image File";
            openFileDialog1.Multiselect = false;
            openFileDialog1.ValidateNames = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open))
                    {
                        ClippedImage = Image.FromStream(fs);
                        pictureBox1.Image = ClippedImage;
                    }
                }
                catch
                {
                    MessageBox.Show("Select a valid image");
                }

            }           
        }
    }
}
