namespace Crapy.Analytics
{
    partial class TaskEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskEditor));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new Crapy.Analytics.ActivatableToolStrip();
            this.cmdCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.lblExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lblExecute = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdClearAll = new System.Windows.Forms.ToolStripButton();
            this.cmdShowTest = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.cmbRepeatCount = new System.Windows.Forms.ToolStripComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.White;
            this.dataGridView1.Location = new System.Drawing.Point(1, -2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(695, 380);
            this.dataGridView1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Gold;
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdCancel,
            this.toolStripSeparator3,
            this.cmdShowTest,
            this.toolStripSeparator5,
            this.lblExport,
            this.toolStripSeparator4,
            this.cmbRepeatCount,
            this.toolStripButton2,
            this.toolStripSeparator2,
            this.lblExecute,
            this.toolStripSeparator1,
            this.cmdClearAll});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 380);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStrip1.Size = new System.Drawing.Size(694, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // cmdCancel
            // 
            this.cmdCancel.AutoSize = false;
            this.cmdCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(110, 22);
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 23);
            // 
            // lblExport
            // 
            this.lblExport.AutoSize = false;
            this.lblExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblExport.Image = ((System.Drawing.Image)(resources.GetObject("lblExport.Image")));
            this.lblExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lblExport.Name = "lblExport";
            this.lblExport.Size = new System.Drawing.Size(110, 22);
            this.lblExport.Text = "Export";
            this.lblExport.Click += new System.EventHandler(this.lblExport_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // lblExecute
            // 
            this.lblExecute.AutoSize = false;
            this.lblExecute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblExecute.Image = ((System.Drawing.Image)(resources.GetObject("lblExecute.Image")));
            this.lblExecute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lblExecute.Name = "lblExecute";
            this.lblExecute.Size = new System.Drawing.Size(110, 22);
            this.lblExecute.Text = "Execute";
            this.lblExecute.Click += new System.EventHandler(this.lblExecute_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // cmdClearAll
            // 
            this.cmdClearAll.AutoSize = false;
            this.cmdClearAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdClearAll.Image = ((System.Drawing.Image)(resources.GetObject("cmdClearAll.Image")));
            this.cmdClearAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdClearAll.Name = "cmdClearAll";
            this.cmdClearAll.Size = new System.Drawing.Size(110, 22);
            this.cmdClearAll.Text = "Clear Selection";
            this.cmdClearAll.Click += new System.EventHandler(this.cmdClearAll_Click);
            // 
            // cmdShowTest
            // 
            this.cmdShowTest.AutoSize = false;
            this.cmdShowTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdShowTest.Image = ((System.Drawing.Image)(resources.GetObject("cmdShowTest.Image")));
            this.cmdShowTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdShowTest.Name = "cmdShowTest";
            this.cmdShowTest.Size = new System.Drawing.Size(110, 22);
            this.cmdShowTest.Text = "Show Text";
            this.cmdShowTest.Click += new System.EventHandler(this.cmdShowTest_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.AutoSize = false;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(43, 22);
            this.toolStripButton2.Text = "Repeat";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 23);
            // 
            // cmbRepeatCount
            // 
            this.cmbRepeatCount.AutoSize = false;
            this.cmbRepeatCount.BackColor = System.Drawing.Color.White;
            this.cmbRepeatCount.DropDownHeight = 50;
            this.cmbRepeatCount.IntegralHeight = false;
            this.cmbRepeatCount.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbRepeatCount.Name = "cmbRepeatCount";
            this.cmbRepeatCount.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmbRepeatCount.Size = new System.Drawing.Size(50, 23);
            // 
            // TaskEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 405);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaskEditor";
            this.Text = "TaskEditor";
            this.Load += new System.EventHandler(this.TaskEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripButton lblExecute;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton lblExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton cmdCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton cmdClearAll;
        private System.Windows.Forms.ToolStripButton cmdShowTest;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private ActivatableToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripButton2;
        private System.Windows.Forms.ToolStripComboBox cmbRepeatCount;
    }
}