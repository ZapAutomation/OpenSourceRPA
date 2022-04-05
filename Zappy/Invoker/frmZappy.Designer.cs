using System.ComponentModel;
using System.Windows.Forms;
using Zappy.Invoker.ZappyControls;

namespace Zappy.Invoker
{
    partial class frmZappy
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmZappy));
            this.cmdZappy = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cmdZappy2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblExecute = new System.Windows.Forms.ToolStripButton();
            this.cmbRepeatCount = new System.Windows.Forms.ToolStripComboBox();
            this.lblExport = new System.Windows.Forms.ToolStripButton();
            this.lblImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTaskEditor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lblDelete = new System.Windows.Forms.ToolStripButton();
            this.treeView1 = new Zappy.Invoker.ZappyControls.ZappyTreeView();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdZappy
            // 
            this.cmdZappy.BackColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.cmdZappy, "cmdZappy");
            this.cmdZappy.Name = "cmdZappy";
            this.cmdZappy.Click += new System.EventHandler(this.cmdZappy_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "gnome_session_reboot_eDD_icon.ico");
            this.imageList1.Images.SetKeyName(1, "PSendKeys");
            this.imageList1.Images.SetKeyName(2, "Click");
            this.imageList1.Images.SetKeyName(3, "DoubleClick");
            this.imageList1.Images.SetKeyName(4, "RightClick");
            this.imageList1.Images.SetKeyName(5, "Wheel");
            this.imageList1.Images.SetKeyName(6, "SendKeys");
            this.imageList1.Images.SetKeyName(7, "SendKeys");
            this.imageList1.Images.SetKeyName(8, "Chrome");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            // 
            // cmdZappy2
            // 
            this.cmdZappy2.BackColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.cmdZappy2, "cmdZappy2");
            this.cmdZappy2.Name = "cmdZappy2";
            this.cmdZappy2.Click += new System.EventHandler(this.cmdZappy_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.treeView1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.toolStrip1.CanOverflow = false;
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblExecute,
            this.cmbRepeatCount,
            this.lblExport,
            this.lblImport,
            this.toolStripSeparator4,
            this.toolStripTaskEditor,
            this.toolStripSeparator2,
            this.lblDelete});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip1.Name = "toolStrip1";
            // 
            // lblExecute
            // 
            resources.ApplyResources(this.lblExecute, "lblExecute");
            this.lblExecute.Name = "lblExecute";
            this.lblExecute.Click += new System.EventHandler(this.lblExecute_Click);
            // 
            // cmbRepeatCount
            // 
            resources.ApplyResources(this.cmbRepeatCount, "cmbRepeatCount");
            this.cmbRepeatCount.BackColor = System.Drawing.Color.White;
            this.cmbRepeatCount.DropDownHeight = 50;
            this.cmbRepeatCount.Items.AddRange(new object[] {
            resources.GetString("cmbRepeatCount.Items"),
            resources.GetString("cmbRepeatCount.Items1"),
            resources.GetString("cmbRepeatCount.Items2"),
            resources.GetString("cmbRepeatCount.Items3")});
            this.cmbRepeatCount.Name = "cmbRepeatCount";
            // 
            // lblExport
            // 
            this.lblExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.lblExport, "lblExport");
            this.lblExport.Name = "lblExport";
            this.lblExport.Click += new System.EventHandler(this.lblExport_Click);
            // 
            // lblImport
            // 
            this.lblImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.lblImport, "lblImport");
            this.lblImport.Name = "lblImport";
            this.lblImport.Click += new System.EventHandler(this.lblImport_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripTaskEditor
            // 
            resources.ApplyResources(this.toolStripTaskEditor, "toolStripTaskEditor");
            this.toolStripTaskEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripTaskEditor.Name = "toolStripTaskEditor";
            this.toolStripTaskEditor.Click += new System.EventHandler(this.toolStripTaskEditor_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // lblDelete
            // 
            this.lblDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.lblDelete, "lblDelete");
            this.lblDelete.Name = "lblDelete";
            this.lblDelete.Click += new System.EventHandler(this.lblDelete_Click);
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.CheckBoxes = true;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HotTracking = true;
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.ItemHeight = 22;
            this.treeView1.LabelEdit = true;
            this.treeView1.LineColor = System.Drawing.Color.White;
            this.treeView1.Name = "treeView1";
            //this.treeView1.Pin = null;
            this.treeView1.ShowNodeToolTips = true;
            //this.treeView1.UnPin = null;
            this.treeView1.RequestDisplayText += new System.EventHandler<Zappy.Invoker.NodeRequestTextEventArgs>(this.treeView1_RequestDisplayText);
            this.treeView1.RequestEditText += new System.EventHandler<Zappy.Invoker.NodeRequestTextEventArgs>(this.treeView1_RequestEditText);
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1_AfterCheck);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // frmZappy
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cmdZappy2);
            this.Controls.Add(this.cmdZappy);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmZappy";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Deactivate += new System.EventHandler(this.frmZappy_Deactivate);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        private Label cmdZappy;
        private ZappyTreeView treeView1;
        private ImageList imageList1;
        private Label cmdZappy2;
        private Panel panel1;
        private ToolStrip toolStrip1;
        private ToolStripButton lblExecute;
        private ToolStripButton lblExport;
        private ToolStripButton lblImport;
        private ToolStripComboBox cmbRepeatCount;
        private ToolStripButton lblDelete;
        private ToolStripButton toolStripTaskEditor;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator4;
    }
}