namespace Zappy.ZappyActions.Picture
{
    partial class ZappySnippingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZappySnippingForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ImagePanelOcr = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Clip = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.loadfromFile = new System.Windows.Forms.Button();
            this.ImagePanelOcr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
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
            resources.ApplyResources(this.ImagePanelOcr, "ImagePanelOcr");
            this.ImagePanelOcr.Controls.Add(this.label2);
            this.ImagePanelOcr.Controls.Add(this.comboBox1);
            this.ImagePanelOcr.Controls.Add(this.label1);
            this.ImagePanelOcr.Controls.Add(this.textBox1);
            this.ImagePanelOcr.Controls.Add(this.pictureBox1);
            this.ImagePanelOcr.Name = "ImagePanelOcr";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3"),
            resources.GetString("comboBox1.Items4")});
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // Clip
            // 
            resources.ApplyResources(this.Clip, "Clip");
            this.Clip.ImageList = this.imageList1;
            this.Clip.Name = "Clip";
            this.Clip.UseVisualStyleBackColor = true;
            this.Clip.Click += new System.EventHandler(this.Clip_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.TabStop = false;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabStop = false;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // loadfromFile
            // 
            resources.ApplyResources(this.loadfromFile, "loadfromFile");
            this.loadfromFile.ImageList = this.imageList1;
            this.loadfromFile.Name = "loadfromFile";
            this.loadfromFile.UseVisualStyleBackColor = true;
            this.loadfromFile.Click += new System.EventHandler(this.LoadfromFile_Click);
            // 
            // ZappySnippingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.loadfromFile);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.ImagePanelOcr);
            this.Controls.Add(this.Clip);
            this.Name = "ZappySnippingForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ZappyOcrForm_FormClosing);
            this.Load += new System.EventHandler(this.ZappyOcrForm_Load);
            this.ImagePanelOcr.ResumeLayout(false);
            this.ImagePanelOcr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel ImagePanelOcr;
        private System.Windows.Forms.Button Clip;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button loadfromFile;
    }
}