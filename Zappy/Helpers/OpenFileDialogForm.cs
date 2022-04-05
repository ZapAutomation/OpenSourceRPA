using System;
using System.ComponentModel;
using System.Globalization;
using System.Security;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.Invoker;
using Zappy.ZappyTaskEditor;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.Helpers
{
    public class OpenFileDialogForm : Form
    {
        private Button selectButton;
        private Button selectFile;
        private OpenFileDialog openFileDialog1;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenFileDialogForm));
            this.selectFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selectFile
            // 
            resources.ApplyResources(this.selectFile, "selectFile");
            this.selectFile.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.selectFile.Name = "selectFile";
            this.selectFile.UseVisualStyleBackColor = true;
            this.selectFile.Click += new System.EventHandler(this.selectFile_Click);
            // 
            // OpenFileDialogForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.selectFile);
            this.Name = "OpenFileDialogForm";
            this.ResumeLayout(false);

        }
        public OpenFileDialogForm()
        {
            InitializeComponent();

            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            }
        }

        public string FilePath { get; set; }

        public string[] MultiFilePaths { get; set; }
        public OpenFileDialogForm(string text, bool openMultipleFiles) : this()
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = text;
            openFileDialog1.Multiselect = openMultipleFiles;
        }

        private void selectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FilePath = openFileDialog1.FileName;
                    MultiFilePaths = openFileDialog1.FileNames;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                                    $"Details:\n\n{ex.StackTrace}");
                }
            }
            else
            {
                throw new Exception("No file selected");
            }

        }

        public void ChangeLanguage(LanguageZappy languageZappy)
        {
            try
            {
                string lang = LocalizeTaskEditorHelper.LanguagePicker(languageZappy);
                ComponentResourceManager resources = null;
                foreach (Control c in this.Controls)
                {
                    resources = new ComponentResourceManager(typeof(OpenFileDialogForm));
                    resources.ApplyResources(c, c.Name, new CultureInfo(lang));
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }
    }

}