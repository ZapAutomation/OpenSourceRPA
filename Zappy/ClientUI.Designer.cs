namespace Zappy
{
    partial class ClientUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientUI));
            this.cmdStart = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.taskEditorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.StartStopRecordingTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ocrSnippingToolToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.logViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triggerManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nugetManagerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAnalyticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateZappyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartStopZappyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mIscellineousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zappyVersionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportLearnedActivityToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addFirefoxAddinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.applicationSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureHotkeysToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.japaneaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdStart
            // 
            resources.ApplyResources(this.cmdStart, "cmdStart");
            this.cmdStart.BackColor = System.Drawing.Color.Lime;
            this.cmdStart.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.UseVisualStyleBackColor = false;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.notifyIcon1, "notifyIcon1");
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.taskEditorToolStripMenuItem1,
            this.StartStopRecordingTaskToolStripMenuItem,
            this.pluginsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // taskEditorToolStripMenuItem1
            // 
            resources.ApplyResources(this.taskEditorToolStripMenuItem1, "taskEditorToolStripMenuItem1");
            this.taskEditorToolStripMenuItem1.Name = "taskEditorToolStripMenuItem1";
            this.taskEditorToolStripMenuItem1.Click += new System.EventHandler(this.taskEditorOpenFile);
            // 
            // StartStopRecordingTaskToolStripMenuItem
            // 
            resources.ApplyResources(this.StartStopRecordingTaskToolStripMenuItem, "StartStopRecordingTaskToolStripMenuItem");
            this.StartStopRecordingTaskToolStripMenuItem.Name = "StartStopRecordingTaskToolStripMenuItem";
            this.StartStopRecordingTaskToolStripMenuItem.Click += new System.EventHandler(this.taskEditorToolStripMenuItem_Click);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ocrSnippingToolToolStripMenuItem1,
            this.logViewerToolStripMenuItem,
            this.triggerManagerToolStripMenuItem,
            this.nugetManagerToolStripMenuItem1,
            this.viewAnalyticsToolStripMenuItem});
            resources.ApplyResources(this.pluginsToolStripMenuItem, "pluginsToolStripMenuItem");
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            // 
            // ocrSnippingToolToolStripMenuItem1
            // 
            resources.ApplyResources(this.ocrSnippingToolToolStripMenuItem1, "ocrSnippingToolToolStripMenuItem1");
            this.ocrSnippingToolToolStripMenuItem1.Name = "ocrSnippingToolToolStripMenuItem1";
            this.ocrSnippingToolToolStripMenuItem1.Click += new System.EventHandler(this.ocrSnippingToolToolStripMenuItem1_Click);
            // 
            // logViewerToolStripMenuItem
            // 
            resources.ApplyResources(this.logViewerToolStripMenuItem, "logViewerToolStripMenuItem");
            this.logViewerToolStripMenuItem.Name = "logViewerToolStripMenuItem";
            this.logViewerToolStripMenuItem.Click += new System.EventHandler(this.logViewerToolStripMenuItem_Click);
            // 
            // triggerManagerToolStripMenuItem
            // 
            resources.ApplyResources(this.triggerManagerToolStripMenuItem, "triggerManagerToolStripMenuItem");
            this.triggerManagerToolStripMenuItem.Name = "triggerManagerToolStripMenuItem";
            this.triggerManagerToolStripMenuItem.Click += new System.EventHandler(this.triggerManagerToolStripMenuItem_Click);
            // 
            // nugetManagerToolStripMenuItem1
            // 
            resources.ApplyResources(this.nugetManagerToolStripMenuItem1, "nugetManagerToolStripMenuItem1");
            this.nugetManagerToolStripMenuItem1.Name = "nugetManagerToolStripMenuItem1";
            this.nugetManagerToolStripMenuItem1.Click += new System.EventHandler(this.NugetManagerToolStripMenuItem1_Click);
            // 
            // viewAnalyticsToolStripMenuItem
            // 
            resources.ApplyResources(this.viewAnalyticsToolStripMenuItem, "viewAnalyticsToolStripMenuItem");
            this.viewAnalyticsToolStripMenuItem.Name = "viewAnalyticsToolStripMenuItem";
            this.viewAnalyticsToolStripMenuItem.Click += new System.EventHandler(this.viewAnalyticsToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateZappyToolStripMenuItem,
            this.StartStopZappyToolStripMenuItem,
            this.mIscellineousToolStripMenuItem,
            this.settingsToolStripMenuItem1});
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            // 
            // updateZappyToolStripMenuItem
            // 
            resources.ApplyResources(this.updateZappyToolStripMenuItem, "updateZappyToolStripMenuItem");
            this.updateZappyToolStripMenuItem.Name = "updateZappyToolStripMenuItem";
            this.updateZappyToolStripMenuItem.Click += new System.EventHandler(this.updateZappyToolStripMenuItem_Click);
            // 
            // StartStopZappyToolStripMenuItem
            // 
            resources.ApplyResources(this.StartStopZappyToolStripMenuItem, "StartStopZappyToolStripMenuItem");
            this.StartStopZappyToolStripMenuItem.Name = "StartStopZappyToolStripMenuItem";
            this.StartStopZappyToolStripMenuItem.Click += new System.EventHandler(this.StartToolStripMenuItem_Click);
            // 
            // mIscellineousToolStripMenuItem
            // 
            this.mIscellineousToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zappyVersionToolStripMenuItem1,
            this.exportLearnedActivityToolStripMenuItem1,
            this.addFirefoxAddinToolStripMenuItem});
            resources.ApplyResources(this.mIscellineousToolStripMenuItem, "mIscellineousToolStripMenuItem");
            this.mIscellineousToolStripMenuItem.Name = "mIscellineousToolStripMenuItem";
            // 
            // zappyVersionToolStripMenuItem1
            // 
            resources.ApplyResources(this.zappyVersionToolStripMenuItem1, "zappyVersionToolStripMenuItem1");
            this.zappyVersionToolStripMenuItem1.Name = "zappyVersionToolStripMenuItem1";
            this.zappyVersionToolStripMenuItem1.Click += new System.EventHandler(this.zappyVersionToolStripMenuItem_Click);
            // 
            // exportLearnedActivityToolStripMenuItem1
            // 
            resources.ApplyResources(this.exportLearnedActivityToolStripMenuItem1, "exportLearnedActivityToolStripMenuItem1");
            this.exportLearnedActivityToolStripMenuItem1.Name = "exportLearnedActivityToolStripMenuItem1";
            this.exportLearnedActivityToolStripMenuItem1.Click += new System.EventHandler(this.exportLearnedTaskToolStripMenuItem_Click);
            // 
            // addFirefoxAddinToolStripMenuItem
            // 
            this.addFirefoxAddinToolStripMenuItem.Image = global::Zappy.Properties.Resources.firefox_logo;
            this.addFirefoxAddinToolStripMenuItem.Name = "addFirefoxAddinToolStripMenuItem";
            resources.ApplyResources(this.addFirefoxAddinToolStripMenuItem, "addFirefoxAddinToolStripMenuItem");
            this.addFirefoxAddinToolStripMenuItem.Click += new System.EventHandler(this.addFirefoxAddinToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applicationSettingsToolStripMenuItem,
            this.configureHotkeysToolStripMenuItem1,
            this.languageToolStripMenuItem1});
            resources.ApplyResources(this.settingsToolStripMenuItem1, "settingsToolStripMenuItem1");
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            // 
            // applicationSettingsToolStripMenuItem
            // 
            resources.ApplyResources(this.applicationSettingsToolStripMenuItem, "applicationSettingsToolStripMenuItem");
            this.applicationSettingsToolStripMenuItem.Name = "applicationSettingsToolStripMenuItem";
            this.applicationSettingsToolStripMenuItem.Click += new System.EventHandler(this.applicationSettingsToolStripMenuItem_Click);
            // 
            // configureHotkeysToolStripMenuItem1
            // 
            resources.ApplyResources(this.configureHotkeysToolStripMenuItem1, "configureHotkeysToolStripMenuItem1");
            this.configureHotkeysToolStripMenuItem1.Name = "configureHotkeysToolStripMenuItem1";
            this.configureHotkeysToolStripMenuItem1.Click += new System.EventHandler(this.configureHotkeysToolStripMenuItem1_Click);
            // 
            // languageToolStripMenuItem1
            // 
            this.languageToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem1,
            this.japaneaseToolStripMenuItem});
            resources.ApplyResources(this.languageToolStripMenuItem1, "languageToolStripMenuItem1");
            this.languageToolStripMenuItem1.Name = "languageToolStripMenuItem1";
            // 
            // englishToolStripMenuItem1
            // 
            this.englishToolStripMenuItem1.Name = "englishToolStripMenuItem1";
            resources.ApplyResources(this.englishToolStripMenuItem1, "englishToolStripMenuItem1");
            this.englishToolStripMenuItem1.Click += new System.EventHandler(this.EnglishToolStripMenuItem1_Click);
            // 
            // japaneaseToolStripMenuItem
            // 
            this.japaneaseToolStripMenuItem.Name = "japaneaseToolStripMenuItem";
            resources.ApplyResources(this.japaneaseToolStripMenuItem, "japaneaseToolStripMenuItem");
            this.japaneaseToolStripMenuItem.Click += new System.EventHandler(this.JapaneaseToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ClientUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClientUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientUI_FormClosing);
            this.Load += new System.EventHandler(this.ClientUI_Load);
            this.Resize += new System.EventHandler(this.ClientUI_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

#endregion
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StartStopRecordingTaskToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ocrSnippingToolToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem updateZappyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem taskEditorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem StartStopZappyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triggerManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem applicationSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureHotkeysToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem viewAnalyticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mIscellineousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportLearnedActivityToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem zappyVersionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem japaneaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFirefoxAddinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nugetManagerToolStripMenuItem1;
    }
}