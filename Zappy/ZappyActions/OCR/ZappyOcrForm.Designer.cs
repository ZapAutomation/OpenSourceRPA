namespace Zappy.ZappyActions.OCR
{
    partial class ZappyOcrForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZappyOcrForm));
            this.Clip = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ImagePanelOcr = new System.Windows.Forms.Panel();
            this.OutputResultOcr = new System.Windows.Forms.TextBox();
            this.cmbLanguages = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Clip
            // 
            this.Clip.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Clip.ImageKey = "(none)";
            this.Clip.ImageList = this.imageList1;
            this.Clip.Location = new System.Drawing.Point(12, 5);
            this.Clip.Name = "Clip";
            this.Clip.Size = new System.Drawing.Size(88, 27);
            this.Clip.TabIndex = 0;
            this.Clip.Text = "Clip";
            this.Clip.UseVisualStyleBackColor = true;
            this.Clip.Click += new System.EventHandler(this.Clip_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "SnippingTool.png");
            this.imageList1.Images.SetKeyName(1, "false.png");
            this.imageList1.Images.SetKeyName(2, "export-3.png");
            this.imageList1.Images.SetKeyName(3, "snipping-tool-icon-256.png");
            this.imageList1.Images.SetKeyName(4, "baru-32-512.png");
            // 
            // ImagePanelOcr
            // 
            this.ImagePanelOcr.AutoSize = true;
            this.ImagePanelOcr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ImagePanelOcr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImagePanelOcr.Location = new System.Drawing.Point(0, 0);
            this.ImagePanelOcr.Name = "ImagePanelOcr";
            this.ImagePanelOcr.Size = new System.Drawing.Size(391, 410);
            this.ImagePanelOcr.TabIndex = 1;
            // 
            // OutputResultOcr
            // 
            this.OutputResultOcr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputResultOcr.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.OutputResultOcr.Location = new System.Drawing.Point(0, 0);
            this.OutputResultOcr.Multiline = true;
            this.OutputResultOcr.Name = "OutputResultOcr";
            this.OutputResultOcr.ReadOnly = true;
            this.OutputResultOcr.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputResultOcr.Size = new System.Drawing.Size(404, 410);
            this.OutputResultOcr.TabIndex = 2;
            this.OutputResultOcr.WordWrap = false;
            // 
            // cmbLanguages
            // 
            this.cmbLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguages.FormattingEnabled = true;
            this.cmbLanguages.ItemHeight = 13;
            this.cmbLanguages.Location = new System.Drawing.Point(145, 5);
            this.cmbLanguages.Name = "cmbLanguages";
            this.cmbLanguages.Size = new System.Drawing.Size(121, 21);
            this.cmbLanguages.TabIndex = 4;
            this.cmbLanguages.SelectedIndexChanged += new System.EventHandler(this.cmbLanguages_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(1, 38);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ImagePanelOcr);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.OutputResultOcr);
            this.splitContainer1.Size = new System.Drawing.Size(798, 410);
            this.splitContainer1.SplitterDistance = 391;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 5;
            // 
            // ZappyOcrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Clip);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.cmbLanguages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ZappyOcrForm";
            this.Text = "Clip To Text";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ZappyOcrForm_FormClosing);
            this.Load += new System.EventHandler(this.ZappyOcrForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Clip;
        private System.Windows.Forms.Panel ImagePanelOcr;
        private System.Windows.Forms.TextBox OutputResultOcr;
        private System.Windows.Forms.ComboBox cmbLanguages;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ImageList imageList1;
    }
}