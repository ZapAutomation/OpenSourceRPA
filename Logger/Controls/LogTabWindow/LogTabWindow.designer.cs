﻿using System.Windows.Forms;
using ZappyLogger.Dialogs;
using ZappyLogger.Docking;

namespace ZappyLogger.Controls.LogTabWindow
{
	partial class LogTabWindow
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
            ZappyLogger.Docking.DockPanelSkin dockPanelSkin1 = new ZappyLogger.Docking.DockPanelSkin();
            ZappyLogger.Docking.AutoHideStripSkin autoHideStripSkin1 = new ZappyLogger.Docking.AutoHideStripSkin();
            ZappyLogger.Docking.DockPanelGradient dockPanelGradient1 = new ZappyLogger.Docking.DockPanelGradient();
            ZappyLogger.Docking.TabGradient tabGradient1 = new ZappyLogger.Docking.TabGradient();
            ZappyLogger.Docking.DockPaneStripSkin dockPaneStripSkin1 = new ZappyLogger.Docking.DockPaneStripSkin();
            ZappyLogger.Docking.DockPaneStripGradient dockPaneStripGradient1 = new ZappyLogger.Docking.DockPaneStripGradient();
            ZappyLogger.Docking.TabGradient tabGradient2 = new ZappyLogger.Docking.TabGradient();
            ZappyLogger.Docking.DockPanelGradient dockPanelGradient2 = new ZappyLogger.Docking.DockPanelGradient();
            ZappyLogger.Docking.TabGradient tabGradient3 = new ZappyLogger.Docking.TabGradient();
            ZappyLogger.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new ZappyLogger.Docking.DockPaneStripToolWindowGradient();
            ZappyLogger.Docking.TabGradient tabGradient4 = new ZappyLogger.Docking.TabGradient();
            ZappyLogger.Docking.TabGradient tabGradient5 = new ZappyLogger.Docking.TabGradient();
            ZappyLogger.Docking.DockPanelGradient dockPanelGradient3 = new ZappyLogger.Docking.DockPanelGradient();
            ZappyLogger.Docking.TabGradient tabGradient6 = new ZappyLogger.Docking.TabGradient();
            ZappyLogger.Docking.TabGradient tabGradient7 = new ZappyLogger.Docking.TabGradient();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogTabWindow));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.linesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.sizeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.currentLineLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.loadProgessBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openURIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFromClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.multiFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiFileEnabledStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multifileMaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.loadProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBookmarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.lastUsedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.goToLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookmarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleBookmarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jumpToNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jumpToPrevToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBookmarkListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnFinderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.encodingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSCIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aNSIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iSO88591ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uTF8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uTF16ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.timeshiftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeshiftMenuTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.copyMarkedLinesIntoNewTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hilightingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.cellSelectModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideLineColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.lockInstanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.host = new System.Windows.Forms.CheckBox();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.dockPanel = new ZappyLogger.Docking.DockPanel();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonBookmark = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonBubbles = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonTail = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.highlightGroupsComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.externalToolsToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.followTailCheckBox = new System.Windows.Forms.CheckBox();
            this.tabContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeThisTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeOtherTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.tabColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.copyPathToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateTimeDragControl = new ZappyLogger.Dialogs.DateTimeDragControl();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.tabContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.linesLabel,
            this.sizeLabel,
            this.currentLineLabel,
            this.loadProgessBar,
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 506);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(962, 23);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // linesLabel
            // 
            this.linesLabel.AutoSize = false;
            this.linesLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.linesLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.linesLabel.Name = "linesLabel";
            this.linesLabel.Size = new System.Drawing.Size(90, 18);
            this.linesLabel.Text = "0";
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = false;
            this.sizeLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sizeLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(90, 18);
            this.sizeLabel.Text = "0";
            // 
            // currentLineLabel
            // 
            this.currentLineLabel.AutoSize = false;
            this.currentLineLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.currentLineLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.currentLineLabel.Name = "currentLineLabel";
            this.currentLineLabel.Size = new System.Drawing.Size(90, 18);
            this.currentLineLabel.Text = "L:";
            // 
            // loadProgessBar
            // 
            this.loadProgessBar.Name = "loadProgessBar";
            this.loadProgessBar.Size = new System.Drawing.Size(50, 17);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(50, 18);
            this.statusLabel.Text = "Ready";
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowMerge = false;
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.optionToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.menuStrip1.Location = new System.Drawing.Point(0, 28);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(962, 28);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openURIToolStripMenuItem,
            this.closeFileToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.newFromClipboardToolStripMenuItem,
            this.toolStripSeparator8,
            this.multiFileToolStripMenuItem,
            this.toolStripSeparator7,
            this.loadProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.exportBookmarksToolStripMenuItem,
            this.toolStripSeparator14,
            this.lastUsedToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(44, 24);
            this.toolStripMenuItem1.Text = "File";
            this.toolStripMenuItem1.DropDownOpening += new System.EventHandler(this.toolStripMenuItem1_DropDownOpening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::ZappyLogger.Properties.Resources.folder_blue;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openURIToolStripMenuItem
            // 
            this.openURIToolStripMenuItem.Name = "openURIToolStripMenuItem";
            this.openURIToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.openURIToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.openURIToolStripMenuItem.Text = "Open URL...";
            this.openURIToolStripMenuItem.ToolTipText = "Opens a file by entering a URL which is supported by a file system plugin";
            this.openURIToolStripMenuItem.Click += new System.EventHandler(this.openURIToolStripMenuItem_Click);
            // 
            // closeFileToolStripMenuItem
            // 
            this.closeFileToolStripMenuItem.Name = "closeFileToolStripMenuItem";
            this.closeFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.closeFileToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.closeFileToolStripMenuItem.Text = "Close File";
            this.closeFileToolStripMenuItem.Click += new System.EventHandler(this.closeFileToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // newFromClipboardToolStripMenuItem
            // 
            this.newFromClipboardToolStripMenuItem.Name = "newFromClipboardToolStripMenuItem";
            this.newFromClipboardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newFromClipboardToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.newFromClipboardToolStripMenuItem.Text = "New tab from clipboard";
            this.newFromClipboardToolStripMenuItem.ToolTipText = "Creates a new tab with content from clipboard";
            this.newFromClipboardToolStripMenuItem.Click += new System.EventHandler(this.newFromClipboardToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(294, 6);
            // 
            // multiFileToolStripMenuItem
            // 
            this.multiFileToolStripMenuItem.CheckOnClick = true;
            this.multiFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.multiFileEnabledStripMenuItem,
            this.multifileMaskToolStripMenuItem});
            this.multiFileToolStripMenuItem.Name = "multiFileToolStripMenuItem";
            this.multiFileToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.multiFileToolStripMenuItem.Text = "MultiFile";
            this.multiFileToolStripMenuItem.ToolTipText = "Treat multiple files as one large file (e.g. data.log, data.log.1, data.log.2,..." +
    ")";
            this.multiFileToolStripMenuItem.Click += new System.EventHandler(this.multiFileToolStripMenuItem_Click);
            // 
            // multiFileEnabledStripMenuItem
            // 
            this.multiFileEnabledStripMenuItem.CheckOnClick = true;
            this.multiFileEnabledStripMenuItem.Name = "multiFileEnabledStripMenuItem";
            this.multiFileEnabledStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.multiFileEnabledStripMenuItem.Text = "Enable MultiFile";
            this.multiFileEnabledStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // multifileMaskToolStripMenuItem
            // 
            this.multifileMaskToolStripMenuItem.Name = "multifileMaskToolStripMenuItem";
            this.multifileMaskToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.multifileMaskToolStripMenuItem.Text = "File name mask...";
            this.multifileMaskToolStripMenuItem.Click += new System.EventHandler(this.multifileMaskToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(294, 6);
            // 
            // loadProjectToolStripMenuItem
            // 
            this.loadProjectToolStripMenuItem.Name = "loadProjectToolStripMenuItem";
            this.loadProjectToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.loadProjectToolStripMenuItem.Text = "Load session...";
            this.loadProjectToolStripMenuItem.ToolTipText = "Load a saved session (list of log files)";
            this.loadProjectToolStripMenuItem.Click += new System.EventHandler(this.loadProjectToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.saveProjectToolStripMenuItem.Text = "Save session...";
            this.saveProjectToolStripMenuItem.ToolTipText = "Save a session (all open tabs)";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // exportBookmarksToolStripMenuItem
            // 
            this.exportBookmarksToolStripMenuItem.Name = "exportBookmarksToolStripMenuItem";
            this.exportBookmarksToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.exportBookmarksToolStripMenuItem.Text = "Export bookmarks...";
            this.exportBookmarksToolStripMenuItem.ToolTipText = "Write a list of bookmarks and their comments to a CSV file";
            this.exportBookmarksToolStripMenuItem.Click += new System.EventHandler(this.exportBookmarksToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(294, 6);
            // 
            // lastUsedToolStripMenuItem
            // 
            this.lastUsedToolStripMenuItem.Name = "lastUsedToolStripMenuItem";
            this.lastUsedToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.lastUsedToolStripMenuItem.Text = "Last used";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(297, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToLineToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.filterToolStripMenuItem,
            this.bookmarksToolStripMenuItem,
            this.columnFinderToolStripMenuItem,
            this.toolStripSeparator2,
            this.encodingToolStripMenuItem,
            this.toolStripSeparator4,
            this.timeshiftToolStripMenuItem,
            this.timeshiftMenuTextBox,
            this.toolStripSeparator3,
            this.copyMarkedLinesIntoNewTabToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(119, 24);
            this.toolStripMenuItem2.Text = "View/Navigate";
            // 
            // goToLineToolStripMenuItem
            // 
            this.goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            this.goToLineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.goToLineToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.goToLineToolStripMenuItem.Text = "Go to line...";
            this.goToLineToolStripMenuItem.Click += new System.EventHandler(this.goToLineToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.searchToolStripMenuItem.Text = "Search...";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.Image = global::ZappyLogger.Properties.Resources.search_blue;
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.filterToolStripMenuItem.Text = "Filter";
            this.filterToolStripMenuItem.Click += new System.EventHandler(this.filterToolStripMenuItem_Click);
            // 
            // bookmarksToolStripMenuItem
            // 
            this.bookmarksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleBookmarkToolStripMenuItem,
            this.jumpToNextToolStripMenuItem,
            this.jumpToPrevToolStripMenuItem,
            this.showBookmarkListToolStripMenuItem});
            this.bookmarksToolStripMenuItem.Name = "bookmarksToolStripMenuItem";
            this.bookmarksToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.bookmarksToolStripMenuItem.Text = "Bookmarks";
            // 
            // toggleBookmarkToolStripMenuItem
            // 
            this.toggleBookmarkToolStripMenuItem.Image = global::ZappyLogger.Properties.Resources.check_blue;
            this.toggleBookmarkToolStripMenuItem.Name = "toggleBookmarkToolStripMenuItem";
            this.toggleBookmarkToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.toggleBookmarkToolStripMenuItem.Size = new System.Drawing.Size(258, 26);
            this.toggleBookmarkToolStripMenuItem.Text = "Toggle Bookmark";
            this.toggleBookmarkToolStripMenuItem.Click += new System.EventHandler(this.toggleBookmarkToolStripMenuItem_Click);
            // 
            // jumpToNextToolStripMenuItem
            // 
            this.jumpToNextToolStripMenuItem.Image = global::ZappyLogger.Properties.Resources.down_blue;
            this.jumpToNextToolStripMenuItem.Name = "jumpToNextToolStripMenuItem";
            this.jumpToNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.jumpToNextToolStripMenuItem.Size = new System.Drawing.Size(258, 26);
            this.jumpToNextToolStripMenuItem.Text = "Jump to next";
            this.jumpToNextToolStripMenuItem.Click += new System.EventHandler(this.jumpToNextToolStripMenuItem_Click);
            // 
            // jumpToPrevToolStripMenuItem
            // 
            this.jumpToPrevToolStripMenuItem.Image = global::ZappyLogger.Properties.Resources.up_blue;
            this.jumpToPrevToolStripMenuItem.Name = "jumpToPrevToolStripMenuItem";
            this.jumpToPrevToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.jumpToPrevToolStripMenuItem.Size = new System.Drawing.Size(258, 26);
            this.jumpToPrevToolStripMenuItem.Text = "Jump to prev";
            this.jumpToPrevToolStripMenuItem.Click += new System.EventHandler(this.jumpToPrevToolStripMenuItem_Click);
            // 
            // showBookmarkListToolStripMenuItem
            // 
            this.showBookmarkListToolStripMenuItem.Name = "showBookmarkListToolStripMenuItem";
            this.showBookmarkListToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.showBookmarkListToolStripMenuItem.Size = new System.Drawing.Size(258, 26);
            this.showBookmarkListToolStripMenuItem.Text = "Bookmark list";
            this.showBookmarkListToolStripMenuItem.Click += new System.EventHandler(this.showBookmarkListToolStripMenuItem_Click);
            // 
            // columnFinderToolStripMenuItem
            // 
            this.columnFinderToolStripMenuItem.CheckOnClick = true;
            this.columnFinderToolStripMenuItem.Name = "columnFinderToolStripMenuItem";
            this.columnFinderToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.columnFinderToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.columnFinderToolStripMenuItem.Text = "Column finder";
            this.columnFinderToolStripMenuItem.Click += new System.EventHandler(this.columnFinderToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(210, 6);
            // 
            // encodingToolStripMenuItem
            // 
            this.encodingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aSCIIToolStripMenuItem,
            this.aNSIToolStripMenuItem,
            this.iSO88591ToolStripMenuItem,
            this.uTF8ToolStripMenuItem,
            this.uTF16ToolStripMenuItem});
            this.encodingToolStripMenuItem.Name = "encodingToolStripMenuItem";
            this.encodingToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.encodingToolStripMenuItem.Text = "Encoding";
            // 
            // aSCIIToolStripMenuItem
            // 
            this.aSCIIToolStripMenuItem.Name = "aSCIIToolStripMenuItem";
            this.aSCIIToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.aSCIIToolStripMenuItem.Tag = "";
            this.aSCIIToolStripMenuItem.Text = "ASCII";
            this.aSCIIToolStripMenuItem.Click += new System.EventHandler(this.aSCIIToolStripMenuItem_Click);
            // 
            // aNSIToolStripMenuItem
            // 
            this.aNSIToolStripMenuItem.Name = "aNSIToolStripMenuItem";
            this.aNSIToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.aNSIToolStripMenuItem.Tag = "";
            this.aNSIToolStripMenuItem.Text = "ANSI";
            this.aNSIToolStripMenuItem.Click += new System.EventHandler(this.aNSIToolStripMenuItem_Click);
            // 
            // iSO88591ToolStripMenuItem
            // 
            this.iSO88591ToolStripMenuItem.Name = "iSO88591ToolStripMenuItem";
            this.iSO88591ToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.iSO88591ToolStripMenuItem.Text = "ISO-8859-1";
            this.iSO88591ToolStripMenuItem.Click += new System.EventHandler(this.iSO88591ToolStripMenuItem_Click);
            // 
            // uTF8ToolStripMenuItem
            // 
            this.uTF8ToolStripMenuItem.Name = "uTF8ToolStripMenuItem";
            this.uTF8ToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.uTF8ToolStripMenuItem.Text = "UTF8";
            this.uTF8ToolStripMenuItem.Click += new System.EventHandler(this.uTF8ToolStripMenuItem_Click);
            // 
            // uTF16ToolStripMenuItem
            // 
            this.uTF16ToolStripMenuItem.Name = "uTF16ToolStripMenuItem";
            this.uTF16ToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.uTF16ToolStripMenuItem.Text = "Unicode";
            this.uTF16ToolStripMenuItem.Click += new System.EventHandler(this.uTF16ToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(210, 6);
            // 
            // timeshiftToolStripMenuItem
            // 
            this.timeshiftToolStripMenuItem.CheckOnClick = true;
            this.timeshiftToolStripMenuItem.Name = "timeshiftToolStripMenuItem";
            this.timeshiftToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.timeshiftToolStripMenuItem.Text = "Timeshift";
            this.timeshiftToolStripMenuItem.ToolTipText = "If supported by the columnizer, you can set an offset to the displayed log time";
            this.timeshiftToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.timeshiftToolStripMenuItem_CheckStateChanged);
            // 
            // timeshiftMenuTextBox
            // 
            this.timeshiftMenuTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeshiftMenuTextBox.Enabled = false;
            this.timeshiftMenuTextBox.Name = "timeshiftMenuTextBox";
            this.timeshiftMenuTextBox.Size = new System.Drawing.Size(100, 27);
            this.timeshiftMenuTextBox.Text = "+00:00:00.000";
            this.timeshiftMenuTextBox.ToolTipText = "Time offset (hh:mm:ss.fff)";
            this.timeshiftMenuTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.timeshiftMenuTextBox_KeyDown);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(210, 6);
            // 
            // copyMarkedLinesIntoNewTabToolStripMenuItem
            // 
            this.copyMarkedLinesIntoNewTabToolStripMenuItem.Name = "copyMarkedLinesIntoNewTabToolStripMenuItem";
            this.copyMarkedLinesIntoNewTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.copyMarkedLinesIntoNewTabToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.copyMarkedLinesIntoNewTabToolStripMenuItem.Text = "Copy to Tab";
            this.copyMarkedLinesIntoNewTabToolStripMenuItem.ToolTipText = "Copies all selected lines into a new tab page";
            this.copyMarkedLinesIntoNewTabToolStripMenuItem.Click += new System.EventHandler(this.copyMarkedLinesIntoNewTabToolStripMenuItem_Click);
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnizerToolStripMenuItem,
            this.hilightingToolStripMenuItem1,
            this.settingsToolStripMenuItem,
            this.toolStripSeparator6,
            this.cellSelectModeToolStripMenuItem,
            this.alwaysOnTopToolStripMenuItem,
            this.hideLineColumnToolStripMenuItem,
            this.toolStripSeparator19,
            this.lockInstanceToolStripMenuItem});
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            this.optionToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.optionToolStripMenuItem.Text = "Options";
            this.optionToolStripMenuItem.DropDownOpening += new System.EventHandler(this.optionToolStripMenuItem_DropDownOpening);
            // 
            // columnizerToolStripMenuItem
            // 
            this.columnizerToolStripMenuItem.Name = "columnizerToolStripMenuItem";
            this.columnizerToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.columnizerToolStripMenuItem.Text = "Columnizer...";
            this.columnizerToolStripMenuItem.ToolTipText = "Splits various kinds of logfiles into fixed columns";
            this.columnizerToolStripMenuItem.Click += new System.EventHandler(this.selectFilterToolStripMenuItem_Click);
            // 
            // hilightingToolStripMenuItem1
            // 
            this.hilightingToolStripMenuItem1.Name = "hilightingToolStripMenuItem1";
            this.hilightingToolStripMenuItem1.Size = new System.Drawing.Size(260, 26);
            this.hilightingToolStripMenuItem1.Text = "Highlighting and triggers...";
            this.hilightingToolStripMenuItem1.Click += new System.EventHandler(this.hilightingToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.settingsToolStripMenuItem.Text = "Settings...";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(257, 6);
            // 
            // cellSelectModeToolStripMenuItem
            // 
            this.cellSelectModeToolStripMenuItem.CheckOnClick = true;
            this.cellSelectModeToolStripMenuItem.Name = "cellSelectModeToolStripMenuItem";
            this.cellSelectModeToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.cellSelectModeToolStripMenuItem.Text = "Cell select mode";
            this.cellSelectModeToolStripMenuItem.ToolTipText = "Switches between foll row selection and single cell selection mode";
            this.cellSelectModeToolStripMenuItem.Click += new System.EventHandler(this.cellSelectModeToolStripMenuItem_Click);
            // 
            // alwaysOnTopToolStripMenuItem
            // 
            this.alwaysOnTopToolStripMenuItem.CheckOnClick = true;
            this.alwaysOnTopToolStripMenuItem.Name = "alwaysOnTopToolStripMenuItem";
            this.alwaysOnTopToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.alwaysOnTopToolStripMenuItem.Text = "Always on top";
            this.alwaysOnTopToolStripMenuItem.Click += new System.EventHandler(this.alwaysOnTopToolStripMenuItem_Click);
            // 
            // hideLineColumnToolStripMenuItem
            // 
            this.hideLineColumnToolStripMenuItem.CheckOnClick = true;
            this.hideLineColumnToolStripMenuItem.Name = "hideLineColumnToolStripMenuItem";
            this.hideLineColumnToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.hideLineColumnToolStripMenuItem.Text = "Hide line column";
            this.hideLineColumnToolStripMenuItem.Click += new System.EventHandler(this.hideLineColumnToolStripMenuItem_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(257, 6);
            // 
            // lockInstanceToolStripMenuItem
            // 
            this.lockInstanceToolStripMenuItem.Name = "lockInstanceToolStripMenuItem";
            this.lockInstanceToolStripMenuItem.Size = new System.Drawing.Size(260, 26);
            this.lockInstanceToolStripMenuItem.Text = "Lock instance";
            this.lockInstanceToolStripMenuItem.ToolTipText = "When enabled all new launched ZappyLogger instances will redirect to this window";
            this.lockInstanceToolStripMenuItem.Click += new System.EventHandler(this.lockInstanceToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureToolStripMenuItem,
            this.configureToolStripSeparator});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(56, 24);
            this.toolsToolStripMenuItem.Text = "Tools";
            this.toolsToolStripMenuItem.ToolTipText = "Launch external tools (configure in the settings)";
            this.toolsToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolsToolStripMenuItem_DropDownItemClicked);
            // 
            // configureToolStripMenuItem
            // 
            this.configureToolStripMenuItem.Name = "configureToolStripMenuItem";
            this.configureToolStripMenuItem.Size = new System.Drawing.Size(158, 26);
            this.configureToolStripMenuItem.Text = "Configure...";
            this.configureToolStripMenuItem.Click += new System.EventHandler(this.configureToolStripMenuItem_Click);
            // 
            // configureToolStripSeparator
            // 
            this.configureToolStripSeparator.Name = "configureToolStripSeparator";
            this.configureToolStripSeparator.Size = new System.Drawing.Size(155, 6);
            // 
            // host
            // 
            this.host.AccessibleName = "host";
            this.host.AutoSize = true;
            this.host.BackColor = System.Drawing.Color.Transparent;
            this.host.Location = new System.Drawing.Point(9, 1);
            this.host.Name = "host";
            this.host.Size = new System.Drawing.Size(80, 22);
            this.host.TabIndex = 7;
            this.host.Text = "Follow tail";
            this.host.UseVisualStyleBackColor = false;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.dockPanel);
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(962, 450);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.Enabled = false;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.Enabled = false;
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(962, 506);
            this.toolStripContainer1.TabIndex = 13;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.externalToolsToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip4);
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.BackColor = System.Drawing.SystemColors.Control;
            this.dockPanel.DefaultFloatWindowSize = new System.Drawing.Size(600, 400);
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.SystemColors.Control;
            this.dockPanel.DocumentStyle = ZappyLogger.Docking.DocumentStyle.DockingWindow;
            this.dockPanel.Location = new System.Drawing.Point(0, 0);
            this.dockPanel.Margin = new System.Windows.Forms.Padding(0);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.ShowDocumentIcon = true;
            this.dockPanel.Size = new System.Drawing.Size(962, 450);
            dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
            autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
            tabGradient1.EndColor = System.Drawing.SystemColors.Control;
            tabGradient1.StartColor = System.Drawing.SystemColors.Control;
            tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            autoHideStripSkin1.TabGradient = tabGradient1;
            autoHideStripSkin1.TextFont = new System.Drawing.Font("Segoe UI", 9F);
            dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
            tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
            dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
            dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
            dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
            tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
            dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
            dockPaneStripSkin1.TextFont = new System.Drawing.Font("Segoe UI", 9F);
            tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
            tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
            tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
            dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
            tabGradient5.EndColor = System.Drawing.SystemColors.Control;
            tabGradient5.StartColor = System.Drawing.SystemColors.Control;
            tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
            dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
            tabGradient6.EndColor = System.Drawing.SystemColors.InactiveCaption;
            tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.TextColor = System.Drawing.SystemColors.InactiveCaptionText;
            dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
            tabGradient7.EndColor = System.Drawing.Color.Transparent;
            tabGradient7.StartColor = System.Drawing.Color.Transparent;
            tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
            dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
            dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
            this.dockPanel.Skin = dockPanelSkin1;
            this.dockPanel.TabIndex = 14;
            this.dockPanel.ActiveContentChanged += new System.EventHandler(this.dockPanel_ActiveContentChanged);
            // 
            // toolStrip4
            // 
            this.toolStrip4.AllowMerge = false;
            this.toolStrip4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStrip4.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip4.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOpen,
            this.toolStripSeparator10,
            this.toolStripButtonSearch,
            this.toolStripButtonFilter,
            this.toolStripSeparator11,
            this.toolStripButtonBookmark,
            this.toolStripButtonUp,
            this.toolStripButtonDown,
            this.toolStripSeparator1,
            this.toolStripButtonBubbles,
            this.toolStripSeparator15,
            this.toolStripButtonTail,
            this.toolStripSeparator17,
            this.highlightGroupsComboBox});
            this.toolStrip4.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip4.Location = new System.Drawing.Point(3, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(379, 28);
            this.toolStrip4.TabIndex = 7;
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpen.Image = global::ZappyLogger.Properties.Resources.folder_blue;
            this.toolStripButtonOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(23, 18);
            this.toolStripButtonOpen.Text = "toolStripButton1";
            this.toolStripButtonOpen.ToolTipText = "Open file";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.toolStripButtonOpen_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButtonSearch
            // 
            this.toolStripButtonSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSearch.Image = global::ZappyLogger.Properties.Resources.search_icon_blue;
            this.toolStripButtonSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSearch.Name = "toolStripButtonSearch";
            this.toolStripButtonSearch.Size = new System.Drawing.Size(23, 18);
            this.toolStripButtonSearch.Text = "toolStripButtonSearch";
            this.toolStripButtonSearch.ToolTipText = "Search";
            this.toolStripButtonSearch.Click += new System.EventHandler(this.toolStripButtonSearch_Click);
            // 
            // toolStripButtonFilter
            // 
            this.toolStripButtonFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFilter.Image = global::ZappyLogger.Properties.Resources.search_folder_blue;
            this.toolStripButtonFilter.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFilter.Name = "toolStripButtonFilter";
            this.toolStripButtonFilter.Size = new System.Drawing.Size(23, 18);
            this.toolStripButtonFilter.Text = "toolStripButton1";
            this.toolStripButtonFilter.ToolTipText = "Filter window";
            this.toolStripButtonFilter.Click += new System.EventHandler(this.toolStripButtonFilter_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButtonBookmark
            // 
            this.toolStripButtonBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBookmark.Image = global::ZappyLogger.Properties.Resources.check_blue;
            this.toolStripButtonBookmark.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBookmark.Name = "toolStripButtonBookmark";
            this.toolStripButtonBookmark.Size = new System.Drawing.Size(23, 18);
            this.toolStripButtonBookmark.Text = "toolStripButton1";
            this.toolStripButtonBookmark.ToolTipText = "Toggle bookmark";
            this.toolStripButtonBookmark.Click += new System.EventHandler(this.toolStripButtonBookmark_Click);
            // 
            // toolStripButtonUp
            // 
            this.toolStripButtonUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUp.Image = global::ZappyLogger.Properties.Resources.up_blue;
            this.toolStripButtonUp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUp.Name = "toolStripButtonUp";
            this.toolStripButtonUp.Size = new System.Drawing.Size(23, 18);
            this.toolStripButtonUp.Text = "toolStripButton1";
            this.toolStripButtonUp.ToolTipText = "Go to previous bookmark";
            this.toolStripButtonUp.Click += new System.EventHandler(this.toolStripButtonUp_Click);
            // 
            // toolStripButtonDown
            // 
            this.toolStripButtonDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDown.Image = global::ZappyLogger.Properties.Resources.down_blue;
            this.toolStripButtonDown.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDown.Name = "toolStripButtonDown";
            this.toolStripButtonDown.Size = new System.Drawing.Size(23, 18);
            this.toolStripButtonDown.Text = "toolStripButton1";
            this.toolStripButtonDown.ToolTipText = "Go to next bookmark";
            this.toolStripButtonDown.Click += new System.EventHandler(this.toolStripButtonDown_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButtonBubbles
            // 
            this.toolStripButtonBubbles.CheckOnClick = true;
            this.toolStripButtonBubbles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBubbles.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBubbles.Image")));
            this.toolStripButtonBubbles.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolStripButtonBubbles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBubbles.Name = "toolStripButtonBubbles";
            this.toolStripButtonBubbles.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonBubbles.Text = "Show bookmark bubbles";
            this.toolStripButtonBubbles.Click += new System.EventHandler(this.toolStripButtonBubbles_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripButtonTail
            // 
            this.toolStripButtonTail.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonTail.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTail.Image")));
            this.toolStripButtonTail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTail.Name = "toolStripButtonTail";
            this.toolStripButtonTail.Size = new System.Drawing.Size(34, 24);
            this.toolStripButtonTail.Text = "tail";
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(6, 23);
            // 
            // highlightGroupsComboBox
            // 
            this.highlightGroupsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.highlightGroupsComboBox.DropDownWidth = 250;
            this.highlightGroupsComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.highlightGroupsComboBox.Name = "highlightGroupsComboBox";
            this.highlightGroupsComboBox.Size = new System.Drawing.Size(150, 28);
            this.highlightGroupsComboBox.ToolTipText = "Select the current highlight settings for the log file (right-click to open highl" +
    "ight settings)";
            this.highlightGroupsComboBox.DropDownClosed += new System.EventHandler(this.highlightGroupsComboBox_DropDownClosed);
            this.highlightGroupsComboBox.SelectedIndexChanged += new System.EventHandler(this.highlightGroupsComboBox_SelectedIndexChanged);
            this.highlightGroupsComboBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.highlightGroupsComboBox_MouseUp);
            // 
            // externalToolsToolStrip
            // 
            this.externalToolsToolStrip.AllowMerge = false;
            this.externalToolsToolStrip.BackColor = System.Drawing.SystemColors.ControlLight;
            this.externalToolsToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.externalToolsToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.externalToolsToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.externalToolsToolStrip.Location = new System.Drawing.Point(3, 0);
            this.externalToolsToolStrip.Name = "externalToolsToolStrip";
            this.externalToolsToolStrip.Size = new System.Drawing.Size(1, 0);
            this.externalToolsToolStrip.TabIndex = 8;
            this.externalToolsToolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.externalToolsToolStrip_ItemClicked);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // followTailCheckBox
            // 
            this.followTailCheckBox.AutoSize = true;
            this.followTailCheckBox.Location = new System.Drawing.Point(397, 512);
            this.followTailCheckBox.Name = "followTailCheckBox";
            this.followTailCheckBox.Size = new System.Drawing.Size(84, 19);
            this.followTailCheckBox.TabIndex = 14;
            this.followTailCheckBox.Text = "Follow tail";
            this.followTailCheckBox.UseVisualStyleBackColor = true;
            this.followTailCheckBox.Click += new System.EventHandler(this.followTailCheckBox_Click);
            // 
            // tabContextMenuStrip
            // 
            this.tabContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tabContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeThisTabToolStripMenuItem,
            this.closeOtherTabsToolStripMenuItem,
            this.closeAllTabsToolStripMenuItem,
            this.toolStripSeparator13,
            this.tabColorToolStripMenuItem,
            this.tabRenameToolStripMenuItem,
            this.toolStripSeparator16,
            this.copyPathToClipboardToolStripMenuItem,
            this.findInExplorerToolStripMenuItem});
            this.tabContextMenuStrip.Name = "tabContextMenuStrip";
            this.tabContextMenuStrip.Size = new System.Drawing.Size(233, 184);
            // 
            // closeThisTabToolStripMenuItem
            // 
            this.closeThisTabToolStripMenuItem.Name = "closeThisTabToolStripMenuItem";
            this.closeThisTabToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.closeThisTabToolStripMenuItem.Text = "Close this tab";
            this.closeThisTabToolStripMenuItem.Click += new System.EventHandler(this.closeThisTabToolStripMenuItem_Click);
            // 
            // closeOtherTabsToolStripMenuItem
            // 
            this.closeOtherTabsToolStripMenuItem.Name = "closeOtherTabsToolStripMenuItem";
            this.closeOtherTabsToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.closeOtherTabsToolStripMenuItem.Text = "Close other tabs";
            this.closeOtherTabsToolStripMenuItem.ToolTipText = "Close all tabs except of this one";
            this.closeOtherTabsToolStripMenuItem.Click += new System.EventHandler(this.closeOtherTabsToolStripMenuItem_Click);
            // 
            // closeAllTabsToolStripMenuItem
            // 
            this.closeAllTabsToolStripMenuItem.Name = "closeAllTabsToolStripMenuItem";
            this.closeAllTabsToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.closeAllTabsToolStripMenuItem.Text = "Close all tabs";
            this.closeAllTabsToolStripMenuItem.ToolTipText = "Close all tabs";
            this.closeAllTabsToolStripMenuItem.Click += new System.EventHandler(this.closeAllTabsToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(229, 6);
            // 
            // tabColorToolStripMenuItem
            // 
            this.tabColorToolStripMenuItem.Name = "tabColorToolStripMenuItem";
            this.tabColorToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.tabColorToolStripMenuItem.Text = "Tab color...";
            this.tabColorToolStripMenuItem.ToolTipText = "Sets the tab color";
            this.tabColorToolStripMenuItem.Click += new System.EventHandler(this.tabColorToolStripMenuItem_Click);
            // 
            // tabRenameToolStripMenuItem
            // 
            this.tabRenameToolStripMenuItem.Name = "tabRenameToolStripMenuItem";
            this.tabRenameToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.tabRenameToolStripMenuItem.Text = "Tab rename...";
            this.tabRenameToolStripMenuItem.ToolTipText = "Set the text which is shown on the tab";
            this.tabRenameToolStripMenuItem.Click += new System.EventHandler(this.tabRenameToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(229, 6);
            // 
            // copyPathToClipboardToolStripMenuItem
            // 
            this.copyPathToClipboardToolStripMenuItem.Name = "copyPathToClipboardToolStripMenuItem";
            this.copyPathToClipboardToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.copyPathToClipboardToolStripMenuItem.Text = "Copy path to clipboard";
            this.copyPathToClipboardToolStripMenuItem.ToolTipText = "The complete file name (incl. path) is copied to clipboard";
            this.copyPathToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyPathToClipboardToolStripMenuItem_Click);
            // 
            // findInExplorerToolStripMenuItem
            // 
            this.findInExplorerToolStripMenuItem.Name = "findInExplorerToolStripMenuItem";
            this.findInExplorerToolStripMenuItem.Size = new System.Drawing.Size(232, 24);
            this.findInExplorerToolStripMenuItem.Text = "Find in Explorer";
            this.findInExplorerToolStripMenuItem.ToolTipText = "Opens an Explorer window and selects the log file";
            this.findInExplorerToolStripMenuItem.Click += new System.EventHandler(this.findInExplorerToolStripMenuItem_Click);
            // 
            // dateTimeDragControl
            // 
            this.dateTimeDragControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimeDragControl.BackColor = System.Drawing.SystemColors.Control;
            this.dateTimeDragControl.DateTime = new System.DateTime(((long)(0)));
            this.dateTimeDragControl.DragOrientation = ZappyLogger.Dialogs.DateTimeDragControl.DragOrientations.Vertical;
            this.dateTimeDragControl.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimeDragControl.HoverColor = System.Drawing.Color.LightGray;
            this.dateTimeDragControl.Location = new System.Drawing.Point(611, 508);
            this.dateTimeDragControl.Margin = new System.Windows.Forms.Padding(0);
            this.dateTimeDragControl.MaxDateTime = new System.DateTime(9999, 12, 31, 23, 59, 59, 999);
            this.dateTimeDragControl.MinDateTime = new System.DateTime(((long)(0)));
            this.dateTimeDragControl.Name = "dateTimeDragControl";
            this.dateTimeDragControl.Size = new System.Drawing.Size(188, 20);
            this.dateTimeDragControl.TabIndex = 14;
            this.dateTimeDragControl.ValueChanged += new ZappyLogger.Dialogs.DateTimeDragControl.ValueChangedEventHandler(this.dateTimeDragControl_ValueChanged);
            this.dateTimeDragControl.ValueDragged += new ZappyLogger.Dialogs.DateTimeDragControl.ValueDraggedEventHandler(this.dateTimeDragControl_ValueDragged);
            // 
            // LogTabWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 529);
            this.Controls.Add(this.followTailCheckBox);
            this.Controls.Add(this.dateTimeDragControl);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "LogTabWindow";
            this.Text = "ZappyLogger";
            this.Activated += new System.EventHandler(this.LogTabWindow_Activated);
            this.Deactivate += new System.EventHandler(this.LogTabWindow_Deactivate);
            this.SizeChanged += new System.EventHandler(this.LogTabWindow_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.LogWindow_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.LogTabWindow_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.LogWindow_DragOver);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LogTabWindow_KeyDown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.tabContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel linesLabel;
        private System.Windows.Forms.ToolStripStatusLabel sizeLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripStatusLabel currentLineLabel;
        private System.Windows.Forms.ToolStripProgressBar loadProgessBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripMenuItem closeFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeshiftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyMarkedLinesIntoNewTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hilightingToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cellSelectModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox timeshiftMenuTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bookmarksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleBookmarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jumpToNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jumpToPrevToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encodingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSCIIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aNSIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uTF8ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uTF16ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnizerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private DateTimeDragControl dateTimeDragControl;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem showBookmarkListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripButton toolStripButtonSearch;
        private System.Windows.Forms.ToolStripButton toolStripButtonFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton toolStripButtonBookmark;
        private System.Windows.Forms.ToolStripButton toolStripButtonUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private CheckBox host;
        private CheckBox followTailCheckBox;
        private ToolStripButton toolStripButtonTail;
        private ToolStripMenuItem hideLineColumnToolStripMenuItem;
        private ToolStripMenuItem lastUsedToolStripMenuItem;
        private ContextMenuStrip tabContextMenuStrip;
        private ToolStripMenuItem closeThisTabToolStripMenuItem;
        private ToolStripMenuItem closeOtherTabsToolStripMenuItem;
        private ToolStripMenuItem closeAllTabsToolStripMenuItem;
        private ToolStripMenuItem tabColorToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem loadProjectToolStripMenuItem;
        private ToolStripMenuItem saveProjectToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripButton toolStripButtonBubbles;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripMenuItem copyPathToClipboardToolStripMenuItem;
        private ToolStripMenuItem findInExplorerToolStripMenuItem;
        private ToolStripMenuItem exportBookmarksToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripComboBox highlightGroupsComboBox;
        private ToolStrip externalToolsToolStrip;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem configureToolStripMenuItem;
        private ToolStripSeparator configureToolStripSeparator;
        private ToolStripMenuItem multifileMaskToolStripMenuItem;
        private ToolStripMenuItem multiFileEnabledStripMenuItem;
        private ToolStripMenuItem iSO88591ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripMenuItem lockInstanceToolStripMenuItem;
        private ToolStripMenuItem newFromClipboardToolStripMenuItem;
        private ToolStripMenuItem openURIToolStripMenuItem;
        private ToolStripMenuItem columnFinderToolStripMenuItem;
        private DockPanel dockPanel;
        private ToolStripMenuItem tabRenameToolStripMenuItem;
    }
}

