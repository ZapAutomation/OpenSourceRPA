namespace Zappy.Helpers
{
    partial class frmNugetManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNugetManager));
            this.rightpanel = new System.Windows.Forms.Panel();
            this.loadNugetPackage = new System.Windows.Forms.Button();
            this.loadDll = new System.Windows.Forms.Button();
            this.btnuninstall = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.rightpanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rightpanel
            // 
            this.rightpanel.Controls.Add(this.loadNugetPackage);
            this.rightpanel.Controls.Add(this.loadDll);
            this.rightpanel.Controls.Add(this.btnuninstall);
            resources.ApplyResources(this.rightpanel, "rightpanel");
            this.rightpanel.Name = "rightpanel";
            // 
            // loadNugetPackage
            // 
            resources.ApplyResources(this.loadNugetPackage, "loadNugetPackage");
            this.loadNugetPackage.Name = "loadNugetPackage";
            this.loadNugetPackage.UseVisualStyleBackColor = true;
            this.loadNugetPackage.Click += new System.EventHandler(this.loadNugetPackage_Click);
            // 
            // loadDll
            // 
            resources.ApplyResources(this.loadDll, "loadDll");
            this.loadDll.Name = "loadDll";
            this.loadDll.UseVisualStyleBackColor = true;
            this.loadDll.Click += new System.EventHandler(this.loadDll_Click);
            // 
            // btnuninstall
            // 
            resources.ApplyResources(this.btnuninstall, "btnuninstall");
            this.btnuninstall.Name = "btnuninstall";
            this.btnuninstall.UseVisualStyleBackColor = true;
            this.btnuninstall.Click += new System.EventHandler(this.btnuninstall_Click);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rightpanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.checkedListBox1);
            // 
            // checkedListBox1
            // 
            resources.ApplyResources(this.checkedListBox1, "checkedListBox1");
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Name = "checkedListBox1";
            // 
            // frmNugetManager
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "frmNugetManager";
            this.Load += new System.EventHandler(this.frmNugetManager_Load);
            this.rightpanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel rightpanel;
        private System.Windows.Forms.Button btnuninstall;
        private System.Windows.Forms.Button loadDll;
        private System.Windows.Forms.Button loadNugetPackage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
    }
}