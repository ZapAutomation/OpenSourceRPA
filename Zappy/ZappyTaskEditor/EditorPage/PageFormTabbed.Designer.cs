using System;
using System.Collections.Generic;
using System.ComponentModel;
////////using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core.Helper;
using Zappy.ZappyTaskEditor.EditorPage.Forms;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.EditorPage
{
    partial class PageFormTabbed
    {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageFormTabbed));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.SplitContainer();
            this.activityPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutCtrlXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyCtrlCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteCtrlVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentlyOpenedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExecutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExecuteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addBrkPtToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.stepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.executeSelectedActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startChildSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHiddenPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDynamicPropertyCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.samplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveAsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdNewTab = new System.Windows.Forms.ToolStripButton();
            this.cmdCloseTab = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdRun = new System.Windows.Forms.ToolStripButton();
            this.cmdDebug = new System.Windows.Forms.ToolStripButton();
            this.cmdStep = new System.Windows.Forms.ToolStripButton();
            this.cmdStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLearn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLogButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextTabStructure = new System.Windows.Forms.ToolStripTextBox();
            this.versionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableLayoutPanel1)).BeginInit();
            this.tableLayoutPanel1.Panel1.SuspendLayout();
            this.tableLayoutPanel1.Panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPageContextMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip2);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel1.Panel1
            // 
            this.tableLayoutPanel1.Panel1.Controls.Add(this.activityPanel);
            // 
            // tableLayoutPanel1.Panel2
            // 
            this.tableLayoutPanel1.Panel2.Controls.Add(this.tabControl1);
            // 
            // activityPanel
            // 
            this.activityPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.activityPanel, "activityPanel");
            this.activityPanel.Name = "activityPanel";
            // 
            // tabControl1
            // 
            this.tabControl1.ContextMenuStrip = this.tabPageContextMenu;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageContextMenu
            // 
            this.tabPageContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tabPageContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutCtrlXToolStripMenuItem,
            this.copyCtrlCToolStripMenuItem,
            this.pasteCtrlVToolStripMenuItem});
            this.tabPageContextMenu.Name = "tabPageContextMenu";
            resources.ApplyResources(this.tabPageContextMenu, "tabPageContextMenu");
            // 
            // cutCtrlXToolStripMenuItem
            // 
            this.cutCtrlXToolStripMenuItem.Name = "cutCtrlXToolStripMenuItem";
            resources.ApplyResources(this.cutCtrlXToolStripMenuItem, "cutCtrlXToolStripMenuItem");
            this.cutCtrlXToolStripMenuItem.Click += new System.EventHandler(this.cutCtrlXToolStripMenuItem_Click);
            // 
            // copyCtrlCToolStripMenuItem
            // 
            this.copyCtrlCToolStripMenuItem.Name = "copyCtrlCToolStripMenuItem";
            resources.ApplyResources(this.copyCtrlCToolStripMenuItem, "copyCtrlCToolStripMenuItem");
            this.copyCtrlCToolStripMenuItem.Click += new System.EventHandler(this.copyCtrlCToolStripMenuItem_Click);
            // 
            // pasteCtrlVToolStripMenuItem
            // 
            this.pasteCtrlVToolStripMenuItem.Name = "pasteCtrlVToolStripMenuItem";
            resources.ApplyResources(this.pasteCtrlVToolStripMenuItem, "pasteCtrlVToolStripMenuItem");
            this.pasteCtrlVToolStripMenuItem.Click += new System.EventHandler(this.pasteCtrlVToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.ExecutionToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.recentlyOpenedToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            resources.ApplyResources(this.newToolStripMenuItem, "newToolStripMenuItem");
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // recentlyOpenedToolStripMenuItem
            // 
            this.recentlyOpenedToolStripMenuItem.Name = "recentlyOpenedToolStripMenuItem";
            resources.ApplyResources(this.recentlyOpenedToolStripMenuItem, "recentlyOpenedToolStripMenuItem");
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            resources.ApplyResources(this.closeToolStripMenuItem, "closeToolStripMenuItem");
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator5,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.selectAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            resources.ApplyResources(this.undoToolStripMenuItem, "undoToolStripMenuItem");
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            resources.ApplyResources(this.redoToolStripMenuItem, "redoToolStripMenuItem");
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // ExecutionToolStripMenuItem
            // 
            this.ExecutionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.mnuExecuteToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.addBrkPtToolStrip,
            this.stepToolStripMenuItem,
            this.executeSelectedActionsToolStripMenuItem,
            this.startChildSessionToolStripMenuItem});
            this.ExecutionToolStripMenuItem.Name = "ExecutionToolStripMenuItem";
            resources.ApplyResources(this.ExecutionToolStripMenuItem, "ExecutionToolStripMenuItem");
            this.ExecutionToolStripMenuItem.DropDownOpening += new System.EventHandler(this.debugToolStripMenuItem_DropDownOpening);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            resources.ApplyResources(this.runToolStripMenuItem, "runToolStripMenuItem");
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runWithDebuggerClick);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            resources.ApplyResources(this.debugToolStripMenuItem, "debugToolStripMenuItem");
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem1_Click);
            // 
            // mnuExecuteToolStripMenuItem
            // 
            this.mnuExecuteToolStripMenuItem.Name = "mnuExecuteToolStripMenuItem";
            resources.ApplyResources(this.mnuExecuteToolStripMenuItem, "mnuExecuteToolStripMenuItem");
            this.mnuExecuteToolStripMenuItem.Click += new System.EventHandler(this.mnuExecuteToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem1_Click);
            // 
            // addBrkPtToolStrip
            // 
            this.addBrkPtToolStrip.Name = "addBrkPtToolStrip";
            resources.ApplyResources(this.addBrkPtToolStrip, "addBrkPtToolStrip");
            this.addBrkPtToolStrip.Click += new System.EventHandler(this.addBrkPtToolStripMenuItem_Click);
            // 
            // stepToolStripMenuItem
            // 
            this.stepToolStripMenuItem.Name = "stepToolStripMenuItem";
            resources.ApplyResources(this.stepToolStripMenuItem, "stepToolStripMenuItem");
            this.stepToolStripMenuItem.Click += new System.EventHandler(this.stepToolStripMenuItem_Click);
            // 
            // executeSelectedActionsToolStripMenuItem
            // 
            this.executeSelectedActionsToolStripMenuItem.Name = "executeSelectedActionsToolStripMenuItem";
            resources.ApplyResources(this.executeSelectedActionsToolStripMenuItem, "executeSelectedActionsToolStripMenuItem");
            this.executeSelectedActionsToolStripMenuItem.Click += new System.EventHandler(this.ExecuteSelectedActionsToolStripMenuItem_Click);
            // 
            // startChildSessionToolStripMenuItem
            // 
            this.startChildSessionToolStripMenuItem.Name = "startChildSessionToolStripMenuItem";
            resources.ApplyResources(this.startChildSessionToolStripMenuItem, "startChildSessionToolStripMenuItem");
            this.startChildSessionToolStripMenuItem.Click += new System.EventHandler(this.startChildSessionToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.logFolderToolStripMenuItem,
            this.viewHiddenPropertiesToolStripMenuItem,
            this.showDynamicPropertyCodeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            resources.ApplyResources(this.zoomInToolStripMenuItem, "zoomInToolStripMenuItem");
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            resources.ApplyResources(this.zoomOutToolStripMenuItem, "zoomOutToolStripMenuItem");
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // viewHiddenPropertiesToolStripMenuItem
            // 
            this.viewHiddenPropertiesToolStripMenuItem.Name = "viewHiddenPropertiesToolStripMenuItem";
            resources.ApplyResources(this.viewHiddenPropertiesToolStripMenuItem, "viewHiddenPropertiesToolStripMenuItem");
            this.viewHiddenPropertiesToolStripMenuItem.Click += new System.EventHandler(this.viewHiddenPropertiesToolStripMenuItem_Click);
            // 
            // showDynamicPropertyCodeToolStripMenuItem
            // 
            this.showDynamicPropertyCodeToolStripMenuItem.Name = "showDynamicPropertyCodeToolStripMenuItem";
            resources.ApplyResources(this.showDynamicPropertyCodeToolStripMenuItem, "showDynamicPropertyCodeToolStripMenuItem");
            this.showDynamicPropertyCodeToolStripMenuItem.Click += new System.EventHandler(this.showDynamicPropertyCodeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.samplesToolStripMenuItem,
            this.forumToolStripMenuItem,
            this.documentationToolStripMenuItem,
            this.versionToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // samplesToolStripMenuItem
            // 
            this.samplesToolStripMenuItem.Name = "samplesToolStripMenuItem";
            resources.ApplyResources(this.samplesToolStripMenuItem, "samplesToolStripMenuItem");
            this.samplesToolStripMenuItem.Click += new System.EventHandler(this.samplesToolStripMenuItem_Click);
            // 
            // forumToolStripMenuItem
            // 
            this.forumToolStripMenuItem.Name = "forumToolStripMenuItem";
            resources.ApplyResources(this.forumToolStripMenuItem, "forumToolStripMenuItem");
            this.forumToolStripMenuItem.Click += new System.EventHandler(this.forumToolStripMenuItem_Click);
            // 
            // documentationToolStripMenuItem
            // 
            this.documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
            resources.ApplyResources(this.documentationToolStripMenuItem, "documentationToolStripMenuItem");
            this.documentationToolStripMenuItem.Click += new System.EventHandler(this.documentationToolStripMenuItem_Click);
            // 
            // toolStrip2
            // 
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripButton,
            this.saveToolStripButton,
            this.saveAsToolStripButton,
            this.toolStripSeparator2,
            this.cmdNewTab,
            this.cmdCloseTab,
            this.toolStripSeparator3,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripSeparator7,
            this.cmdRun,
            this.cmdDebug,
            this.cmdStep,
            this.cmdStop,
            this.toolStripSeparator4,
            this.toolStripLearn,
            this.toolStripSeparator6,
            this.toolStripLogButton,
            this.toolStripSeparator1,
            this.toolStripTextTabStructure});
            this.toolStrip2.Name = "toolStrip2";
            // 
            // openToolStripButton
            // 
            resources.ApplyResources(this.openToolStripButton, "openToolStripButton");
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripButton
            // 
            resources.ApplyResources(this.saveToolStripButton, "saveToolStripButton");
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripButton
            // 
            resources.ApplyResources(this.saveAsToolStripButton, "saveAsToolStripButton");
            this.saveAsToolStripButton.Name = "saveAsToolStripButton";
            this.saveAsToolStripButton.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // cmdNewTab
            // 
            resources.ApplyResources(this.cmdNewTab, "cmdNewTab");
            this.cmdNewTab.Name = "cmdNewTab";
            this.cmdNewTab.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // cmdCloseTab
            // 
            resources.ApplyResources(this.cmdCloseTab, "cmdCloseTab");
            this.cmdCloseTab.Name = "cmdCloseTab";
            this.cmdCloseTab.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click_1);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButton2, "toolStripButton2");
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButton3, "toolStripButton3");
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // cmdRun
            // 
            resources.ApplyResources(this.cmdRun, "cmdRun");
            this.cmdRun.Name = "cmdRun";
            this.cmdRun.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // cmdDebug
            // 
            resources.ApplyResources(this.cmdDebug, "cmdDebug");
            this.cmdDebug.Name = "cmdDebug";
            this.cmdDebug.Click += new System.EventHandler(this.debugToolStripMenuItem1_Click);
            // 
            // cmdStep
            // 
            resources.ApplyResources(this.cmdStep, "cmdStep");
            this.cmdStep.Name = "cmdStep";
            this.cmdStep.Click += new System.EventHandler(this.stepToolStripMenuItem_Click);
            // 
            // cmdStop
            // 
            resources.ApplyResources(this.cmdStop, "cmdStop");
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Click += new System.EventHandler(this.stopToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripLearn
            // 
            resources.ApplyResources(this.toolStripLearn, "toolStripLearn");
            this.toolStripLearn.Name = "toolStripLearn";
            this.toolStripLearn.Click += new System.EventHandler(this.toolStripLearn_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // toolStripLogButton
            // 
            resources.ApplyResources(this.toolStripLogButton, "toolStripLogButton");
            this.toolStripLogButton.Name = "toolStripLogButton";
            this.toolStripLogButton.Click += new System.EventHandler(this.toolStripLogButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripTextTabStructure
            // 
            this.toolStripTextTabStructure.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.toolStripTextTabStructure, "toolStripTextTabStructure");
            this.toolStripTextTabStructure.Name = "toolStripTextTabStructure";
            this.toolStripTextTabStructure.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripTextTabStructure_KeyUp);
            // 
            // versionToolStripMenuItem
            // 
            this.versionToolStripMenuItem.Name = "versionToolStripMenuItem";
            resources.ApplyResources(this.versionToolStripMenuItem, "versionToolStripMenuItem");
            // 
            // logFolderToolStripMenuItem
            // 
            this.logFolderToolStripMenuItem.Name = "logFolderToolStripMenuItem";
            resources.ApplyResources(this.logFolderToolStripMenuItem, "logFolderToolStripMenuItem");
            this.logFolderToolStripMenuItem.Click += new System.EventHandler(this.logFolderToolStripMenuItem_Click);
            // 
            // PageFormTabbed
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.toolStripContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PageFormTabbed";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PageFormTabbed_FormClosing);
            this.Load += new System.EventHandler(this.PageForm_Load);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tableLayoutPanel1.Panel1.ResumeLayout(false);
            this.tableLayoutPanel1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tableLayoutPanel1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabPageContextMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        private ToolStripSeparator toolStripSeparator1;
        private ToolStripTextBox toolStripTextTabStructure;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem ExecutionToolStripMenuItem;
        private ToolStripMenuItem runToolStripMenuItem;
        private ToolStripMenuItem debugToolStripMenuItem;
        private ToolStripMenuItem stepToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
        private ToolStripMenuItem addBrkPtToolStrip;
        private Panel activityPanel;
        private SplitContainer tableLayoutPanel1;
        internal TabControl tabControl1;
        private ToolStripButton cmdRun;
        private ToolStripButton cmdDebug;
        private ToolStripButton cmdStep;
        private ToolStripButton cmdStop;
        private ToolStripButton cmdNewTab;
        private ToolStripButton cmdCloseTab;
        private ToolStripContainer toolStripContainer1;
        private ToolStrip toolStrip2;
        private ToolStripButton openToolStripButton;
        private ToolStripButton saveToolStripButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem mnuExecuteToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem samplesToolStripMenuItem;
        private ToolStripMenuItem forumToolStripMenuItem;
        private ContextMenuStrip tabPageContextMenu;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem copyCtrlCToolStripMenuItem;
        private ToolStripMenuItem cutCtrlXToolStripMenuItem;
        private ToolStripMenuItem pasteCtrlVToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton toolStripLearn;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem zoomInToolStripMenuItem;
        private ToolStripMenuItem zoomOutToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem executeSelectedActionsToolStripMenuItem;

        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private ToolStripButton toolStripButton3;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem recentlyOpenedToolStripMenuItem;
        private ToolStripMenuItem viewHiddenPropertiesToolStripMenuItem;
        private ToolStripMenuItem startChildSessionToolStripMenuItem;
        private ToolStripMenuItem showDynamicPropertyCodeToolStripMenuItem;
        private ToolStripMenuItem logFolderToolStripMenuItem;
        private ToolStripMenuItem versionToolStripMenuItem;
    }
}