using System.Windows.Forms;
using Microsoft.Msagl.GraphViewerGdi;

namespace Zappy.Analytics
{
    partial class frmProcessGraph
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
            Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation planeTransformation1 = new Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProcessGraph));
            Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation planeTransformation2 = new Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tbrFrom = new StatusStripTaskBar();
            this.tbrTo = new StatusStripTaskBar();
            this.viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cmdReset = new System.Windows.Forms.ToolStripButton();
            this.lblFrom = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmdFromMinus = new System.Windows.Forms.ToolStripButton();
            this.cmdFromPlus = new System.Windows.Forms.ToolStripButton();
            this.lblTo = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmdToMinus = new System.Windows.Forms.ToolStripButton();
            this.cmdToPlus = new System.Windows.Forms.ToolStripButton();
            this.txtFilterValue = new StatusStripTextbox();
            this.cmdFilter = new System.Windows.Forms.ToolStripButton();
            this.cmdExport = new System.Windows.Forms.ToolStripButton();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.advanceEditor = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.gViewer1 = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProcessName = new System.Windows.Forms.TextBox();
            this.cmdRunDiscovery = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSteps = new System.Windows.Forms.NumericUpDown();
            this.txtAccuracyThreshold = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tmrProcessDiscoveryCheck = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSteps)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbrFrom
            // 
            this.tbrFrom.AutoSize = false;
            this.tbrFrom.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tbrFrom.Name = "tbrFrom";
            this.tbrFrom.Size = new System.Drawing.Size(160, 69);
            this.tbrFrom.ValueChanged += new System.EventHandler(this.tbrFrom_ValueChanged);
            // 
            // tbrTo
            // 
            this.tbrTo.AutoSize = false;
            this.tbrTo.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tbrTo.Name = "tbrTo";
            this.tbrTo.Size = new System.Drawing.Size(160, 69);
            this.tbrTo.ValueChanged += new System.EventHandler(this.tbrTo_ValueChanged);
            // 
            // viewer
            // 
            this.viewer.ArrowheadLength = 10D;
            this.viewer.AsyncLayout = false;
            this.viewer.AutoScroll = true;
            this.viewer.BackwardEnabled = false;
            this.viewer.BuildHitTree = true;
            this.viewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
            this.viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewer.EdgeInsertButtonVisible = true;
            this.viewer.FileName = "";
            this.viewer.ForwardEnabled = false;
            this.viewer.Graph = null;
            this.viewer.InsertingEdge = false;
            this.viewer.LayoutAlgorithmSettingsButtonVisible = true;
            this.viewer.LayoutEditingEnabled = false;
            this.viewer.Location = new System.Drawing.Point(0, 0);
            this.viewer.LooseOffsetForRouting = 0.25D;
            this.viewer.Margin = new System.Windows.Forms.Padding(6);
            this.viewer.MouseHitDistance = 0.05D;
            this.viewer.Name = "viewer";
            this.viewer.NavigationVisible = true;
            this.viewer.NeedToCalculateLayout = true;
            this.viewer.OffsetForRelaxingInRouting = 0.6D;
            this.viewer.PaddingForEdgeRouting = 1.5D;
            this.viewer.PanButtonPressed = false;
            this.viewer.SaveAsImageEnabled = true;
            this.viewer.SaveAsMsaglEnabled = true;
            this.viewer.SaveButtonVisible = true;
            this.viewer.SaveGraphButtonVisible = true;
            this.viewer.SaveInVectorFormatEnabled = true;
            this.viewer.Size = new System.Drawing.Size(1140, 1003);
            this.viewer.TabIndex = 0;
            this.viewer.TightOffsetForRouting = 0.125D;
            this.viewer.ToolBarIsVisible = true;
            this.viewer.Transform = planeTransformation1;
            this.viewer.UndoRedoButtonsVisible = true;
            this.viewer.WindowZoomButtonPressed = false;
            this.viewer.ZoomF = 1D;
            this.viewer.ZoomWindowThreshold = 0.05D;
            this.viewer.Click += new System.EventHandler(this.viewer_Click);
            this.viewer.DoubleClick += new System.EventHandler(this.viewer_DoubleClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1924, 1062);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer1);
            this.tabPage4.Location = new System.Drawing.Point(8, 39);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.tabPage4.Size = new System.Drawing.Size(1908, 1015);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Activity Graph";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(4, 6);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.advanceEditor);
            this.splitContainer1.Panel2.Controls.Add(this.viewer);
            this.splitContainer1.Size = new System.Drawing.Size(1900, 1003);
            this.splitContainer1.SplitterDistance = 754;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dataGridView1);
            this.splitContainer2.Panel1.Controls.Add(this.statusStrip1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer2.Size = new System.Drawing.Size(754, 1003);
            this.splitContainer2.SplitterDistance = 675;
            this.splitContainer2.SplitterWidth = 8;
            this.splitContainer2.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 15;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(754, 604);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdReset,
            this.lblFrom,
            this.cmdFromMinus,
            this.tbrFrom,
            this.cmdFromPlus,
            this.lblTo,
            this.cmdToMinus,
            this.tbrTo,
            this.cmdToPlus,
            this.txtFilterValue,
            this.cmdFilter,
            this.cmdExport});
            this.statusStrip1.Location = new System.Drawing.Point(0, 604);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 20, 0);
            this.statusStrip1.Size = new System.Drawing.Size(754, 71);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // cmdReset
            // 
            this.cmdReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdReset.Image = ((System.Drawing.Image)(resources.GetObject("cmdReset.Image")));
            this.cmdReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdReset.Name = "cmdReset";
            this.cmdReset.Size = new System.Drawing.Size(24, 69);
            this.cmdReset.Text = "toolStripDropDownButton1";
            this.cmdReset.Click += new System.EventHandler(this.cmdReset_Click);
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = false;
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(75, 66);
            this.lblFrom.Text = "From:";
            // 
            // cmdFromMinus
            // 
            this.cmdFromMinus.AutoSize = false;
            this.cmdFromMinus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdFromMinus.Image = ((System.Drawing.Image)(resources.GetObject("cmdFromMinus.Image")));
            this.cmdFromMinus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdFromMinus.Name = "cmdFromMinus";
            this.cmdFromMinus.Size = new System.Drawing.Size(29, 69);
            this.cmdFromMinus.Text = "-";
            this.cmdFromMinus.Click += new System.EventHandler(this.cmdFromMinus_Click);
            // 
            // cmdFromPlus
            // 
            this.cmdFromPlus.AutoSize = false;
            this.cmdFromPlus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdFromPlus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdFromPlus.Name = "cmdFromPlus";
            this.cmdFromPlus.Size = new System.Drawing.Size(35, 69);
            this.cmdFromPlus.Text = "+";
            this.cmdFromPlus.Click += new System.EventHandler(this.cmdFromPlus_Click);
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = false;
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(45, 66);
            this.lblTo.Text = "To:";
            // 
            // cmdToMinus
            // 
            this.cmdToMinus.AutoSize = false;
            this.cmdToMinus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdToMinus.Image = ((System.Drawing.Image)(resources.GetObject("cmdToMinus.Image")));
            this.cmdToMinus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdToMinus.Name = "cmdToMinus";
            this.cmdToMinus.Size = new System.Drawing.Size(29, 69);
            this.cmdToMinus.Text = "-";
            this.cmdToMinus.Click += new System.EventHandler(this.cmdToMinus_Click);
            // 
            // cmdToPlus
            // 
            this.cmdToPlus.AutoSize = false;
            this.cmdToPlus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdToPlus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdToPlus.Name = "cmdToPlus";
            this.cmdToPlus.Size = new System.Drawing.Size(35, 69);
            this.cmdToPlus.Text = "+";
            this.cmdToPlus.Click += new System.EventHandler(this.cmdToPlus_Click);
            // 
            // txtFilterValue
            // 
            this.txtFilterValue.AutoSize = false;
            this.txtFilterValue.Name = "txtFilterValue";
            this.txtFilterValue.Size = new System.Drawing.Size(160, 69);
            this.txtFilterValue.ValueChanged += new System.EventHandler(this.txtFilterValue_ValueChanged);
            // 
            // cmdFilter
            // 
            this.cmdFilter.AutoSize = false;
            this.cmdFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdFilter.Image = ((System.Drawing.Image)(resources.GetObject("cmdFilter.Image")));
            this.cmdFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdFilter.Margin = new System.Windows.Forms.Padding(0);
            this.cmdFilter.Name = "cmdFilter";
            this.cmdFilter.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.cmdFilter.Size = new System.Drawing.Size(38, 38);
            this.cmdFilter.Text = "Filter";
            this.cmdFilter.Click += new System.EventHandler(this.cmdFilter_Click);
            // 
            // cmdExport
            // 
            this.cmdExport.AutoSize = false;
            this.cmdExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdExport.Image = ((System.Drawing.Image)(resources.GetObject("cmdExport.Image")));
            this.cmdExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdExport.Name = "cmdExport";
            this.cmdExport.Size = new System.Drawing.Size(36, 36);
            this.cmdExport.Text = "toolStripSplitButton1";
            this.cmdExport.Click += new System.EventHandler(this.cmdExport_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(6);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(754, 320);
            this.propertyGrid1.TabIndex = 1;
            // 
            // advanceEditor
            // 
            this.advanceEditor.Dock = System.Windows.Forms.DockStyle.Right;
            this.advanceEditor.Location = new System.Drawing.Point(1102, 0);
            this.advanceEditor.Margin = new System.Windows.Forms.Padding(6);
            this.advanceEditor.Name = "advanceEditor";
            this.advanceEditor.Size = new System.Drawing.Size(38, 1003);
            this.advanceEditor.TabIndex = 2;
            this.advanceEditor.Text = "Advanced";
            this.advanceEditor.UseVisualStyleBackColor = true;
            this.advanceEditor.Click += new System.EventHandler(this.advanceEditor_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.gViewer1);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.txtProcessName);
            this.tabPage3.Controls.Add(this.cmdRunDiscovery);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.txtSteps);
            this.tabPage3.Controls.Add(this.txtAccuracyThreshold);
            this.tabPage3.Location = new System.Drawing.Point(8, 39);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage3.Size = new System.Drawing.Size(1908, 1015);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Process Discovery";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // gViewer1
            // 
            this.gViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gViewer1.ArrowheadLength = 10D;
            this.gViewer1.AsyncLayout = false;
            this.gViewer1.AutoScroll = true;
            this.gViewer1.BackwardEnabled = false;
            this.gViewer1.BuildHitTree = true;
            this.gViewer1.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
            this.gViewer1.EdgeInsertButtonVisible = true;
            this.gViewer1.FileName = "";
            this.gViewer1.ForwardEnabled = false;
            this.gViewer1.Graph = null;
            this.gViewer1.InsertingEdge = false;
            this.gViewer1.LayoutAlgorithmSettingsButtonVisible = true;
            this.gViewer1.LayoutEditingEnabled = true;
            this.gViewer1.Location = new System.Drawing.Point(6, 56);
            this.gViewer1.LooseOffsetForRouting = 0.25D;
            this.gViewer1.Margin = new System.Windows.Forms.Padding(6);
            this.gViewer1.MouseHitDistance = 0.05D;
            this.gViewer1.Name = "gViewer1";
            this.gViewer1.NavigationVisible = true;
            this.gViewer1.NeedToCalculateLayout = true;
            this.gViewer1.OffsetForRelaxingInRouting = 0.6D;
            this.gViewer1.PaddingForEdgeRouting = 1.5D;
            this.gViewer1.PanButtonPressed = false;
            this.gViewer1.SaveAsImageEnabled = true;
            this.gViewer1.SaveAsMsaglEnabled = true;
            this.gViewer1.SaveButtonVisible = true;
            this.gViewer1.SaveGraphButtonVisible = true;
            this.gViewer1.SaveInVectorFormatEnabled = true;
            this.gViewer1.Size = new System.Drawing.Size(2258, 1227);
            this.gViewer1.TabIndex = 6;
            this.gViewer1.TightOffsetForRouting = 0.125D;
            this.gViewer1.ToolBarIsVisible = true;
            this.gViewer1.Transform = planeTransformation2;
            this.gViewer1.UndoRedoButtonsVisible = true;
            this.gViewer1.WindowZoomButtonPressed = false;
            this.gViewer1.ZoomF = 1D;
            this.gViewer1.ZoomWindowThreshold = 0.05D;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 17);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 25);
            this.label3.TabIndex = 14;
            this.label3.Text = "Process Name";
            // 
            // txtProcessName
            // 
            this.txtProcessName.Location = new System.Drawing.Point(166, 15);
            this.txtProcessName.Margin = new System.Windows.Forms.Padding(4);
            this.txtProcessName.Name = "txtProcessName";
            this.txtProcessName.Size = new System.Drawing.Size(166, 31);
            this.txtProcessName.TabIndex = 13;
            // 
            // cmdRunDiscovery
            // 
            this.cmdRunDiscovery.Location = new System.Drawing.Point(1000, 12);
            this.cmdRunDiscovery.Margin = new System.Windows.Forms.Padding(4);
            this.cmdRunDiscovery.Name = "cmdRunDiscovery";
            this.cmdRunDiscovery.Size = new System.Drawing.Size(176, 37);
            this.cmdRunDiscovery.TabIndex = 11;
            this.cmdRunDiscovery.Text = "Run Discovery";
            this.cmdRunDiscovery.UseVisualStyleBackColor = true;
            this.cmdRunDiscovery.Click += new System.EventHandler(this.cmdRunDiscovery_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(670, 17);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 25);
            this.label2.TabIndex = 10;
            this.label2.Text = "Prediction Steps";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(350, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 25);
            this.label1.TabIndex = 9;
            this.label1.Text = "Weighted Accuracy";
            // 
            // txtSteps
            // 
            this.txtSteps.Location = new System.Drawing.Point(848, 15);
            this.txtSteps.Margin = new System.Windows.Forms.Padding(4);
            this.txtSteps.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.txtSteps.Name = "txtSteps";
            this.txtSteps.Size = new System.Drawing.Size(120, 31);
            this.txtSteps.TabIndex = 8;
            this.txtSteps.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // txtAccuracyThreshold
            // 
            this.txtAccuracyThreshold.Location = new System.Drawing.Point(556, 15);
            this.txtAccuracyThreshold.Margin = new System.Windows.Forms.Padding(4);
            this.txtAccuracyThreshold.Name = "txtAccuracyThreshold";
            this.txtAccuracyThreshold.Size = new System.Drawing.Size(100, 31);
            this.txtAccuracyThreshold.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chart1);
            this.tabPage1.Location = new System.Drawing.Point(8, 39);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(6);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage1.Size = new System.Drawing.Size(1908, 1015);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Process Analytics";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(6, 6);
            this.chart1.Margin = new System.Windows.Forms.Padding(6);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Time / Process";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1896, 1003);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "Analytics";
            // 
            // tmrProcessDiscoveryCheck
            // 
            this.tmrProcessDiscoveryCheck.Interval = 1000;
            this.tmrProcessDiscoveryCheck.Tick += new System.EventHandler(this.tmrProcessDiscoveryCheck_Tick);
            // 
            // frmProcessGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 1062);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmProcessGraph";
            this.Text = "Analytics";
            this.Load += new System.EventHandler(this.frmProcessGraph_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSteps)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }
        GViewer viewer;



        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabPage tabPage3;
        private GViewer gViewer1;
        private System.Windows.Forms.Button cmdRunDiscovery;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txtSteps;
        private System.Windows.Forms.TextBox txtAccuracyThreshold;
        private System.Windows.Forms.TextBox txtProcessName;
        private System.Windows.Forms.Timer tmrProcessDiscoveryCheck;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblFrom;
        private System.Windows.Forms.ToolStripButton cmdFromPlus;
        private System.Windows.Forms.ToolStripStatusLabel lblTo;
        private System.Windows.Forms.ToolStripButton cmdToPlus;
        private System.Windows.Forms.ToolStripButton cmdFilter;
        private ToolStripButton cmdFromMinus;
        private ToolStripButton cmdToMinus;
        private StatusStripTaskBar tbrTo;
        private StatusStripTaskBar tbrFrom;
        private StatusStripTextbox txtFilterValue;
        private ToolStripButton cmdReset;
        private ToolStripButton cmdExport;
        private Button advanceEditor;
    }
}