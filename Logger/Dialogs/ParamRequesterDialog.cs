﻿using System;
using System.Windows.Forms;

namespace ZappyLogger.Dialogs
{
    public partial class ParamRequesterDialog : Form
    {
        #region Fields

        #endregion

        #region cTor

        public ParamRequesterDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public string ParamName { get; set; }

        public string ParamValue { get; set; }

        public string[] Values { get; set; }

        #endregion

        #region Events handler

        private void ParamRequesterDialog_Shown(object sender, EventArgs e)
        {
            this.paramLabel.Text = ParamName;
            if (this.Values != null)
            {
                foreach (string value in this.Values)
                {
                    this.valueComboBox.Items.Add(value);
                }
                this.valueComboBox.SelectedIndex = 0;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.ParamValue = this.valueComboBox.Text;
        }

        #endregion
    }
}