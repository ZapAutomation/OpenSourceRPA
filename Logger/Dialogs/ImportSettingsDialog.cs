﻿using System;
using System.Windows.Forms;
using ZappyLogger.Config;

namespace ZappyLogger.Dialogs
{
    public partial class ImportSettingsDialog : Form
    {
        #region Fields

        #endregion

        #region cTor

        public ImportSettingsDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public string FileName { get; private set; }

        public ExportImportFlags ImportFlags { get; private set; }

        #endregion

        #region Events handler

        private void ImportSettingsDialog_Load(object sender, EventArgs e)
        {
            foreach (Control ctl in this.optionsGroupBox.Controls)
            {
                if (ctl.Tag != null)
                {
                    (ctl as CheckBox).Checked = true;
                }
            }
        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Load Settings from file";
            dlg.DefaultExt = "dat";
            dlg.AddExtension = false;
            dlg.Filter = "Settings (*.dat)|*.dat|All files (*.*)|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.fileNameTextBox.Text = dlg.FileName;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.ImportFlags = ExportImportFlags.None;
            this.FileName = this.fileNameTextBox.Text;
            foreach (Control ctl in this.optionsGroupBox.Controls)
            {
                if (ctl.Tag != null)
                {
                    if ((ctl as CheckBox).Checked)
                    {
                        this.ImportFlags = this.ImportFlags | (ExportImportFlags)long.Parse(ctl.Tag as string);
                    }
                }
            }
        }

        #endregion
    }
}