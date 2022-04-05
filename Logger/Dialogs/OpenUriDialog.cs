﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ZappyLogger.Dialogs
{
    public partial class OpenUriDialog : Form
    {
        #region Fields

        #endregion

        #region cTor

        public OpenUriDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public string Uri
        {
            get { return this.uriComboBox.Text; }
        }

        public IList<string> UriHistory { get; set; }

        #endregion

        #region Events handler

        private void OpenUriDialog_Load(object sender, EventArgs e)
        {
            if (this.UriHistory != null)
            {
                this.uriComboBox.Items.Clear();
                foreach (string uri in this.UriHistory)
                {
                    this.uriComboBox.Items.Add(uri);
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.UriHistory = new List<string>();
            foreach (object item in this.uriComboBox.Items)
            {
                this.UriHistory.Add(item.ToString());
            }
            if (this.UriHistory.Contains(this.uriComboBox.Text))
            {
                this.UriHistory.Remove(this.uriComboBox.Text);
            }
            this.UriHistory.Insert(0, this.uriComboBox.Text);
            while (this.UriHistory.Count > 20)
            {
                this.UriHistory.RemoveAt(this.UriHistory.Count - 1);
            }
        }

        #endregion
    }
}