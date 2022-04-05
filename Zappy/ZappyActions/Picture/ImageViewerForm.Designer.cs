namespace Zappy.ZappyActions.Picture
{
    partial class ImageViewerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageViewerForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ImagePanelOcr = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
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
            this.ImagePanelOcr.Controls.Add(this.pictureBox1);
            this.ImagePanelOcr.Name = "ImagePanelOcr";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // ImageViewerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ImagePanelOcr);
            this.Name = "ImageViewerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ZappyOcrForm_FormClosing);
            this.Load += new System.EventHandler(this.ZappyOcrForm_Load);
            this.ImagePanelOcr.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel ImagePanelOcr;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}