using System;
using System.Collections.Generic;
using System.ComponentModel;
////////using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
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
    internal partial class PageFormTabbed : Form
    {
        static int _TabCount = 1;

        private void PageFormTabbed_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.UI_Instance is ClientUI)
            {
                (Program.UI_Instance as ClientUI).ActionExecutionUpdate -= ActionExecutionUpdate;
                (Program.UI_Instance as ClientUI).ActionExecutionTrace -= ActionExecutionTrace;
            }
            if (tabControl1.TabPages.Count <= 0)
                return;
            foreach (TabPage item in tabControl1.TabPages)
            {
                tabControl1.SelectedTab = item;
                if (!CloseTab(true))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }
        public PageFormTabbed()
        {
            this.InitializeComponent();
            _InitialTaskFileName = "";
        }
        public PageFormTabbed(string InitialTaskFileName = "")
        {
            this.InitializeComponent();
            _InitialTaskFileName = InitialTaskFileName;
            ActivityForm.Create().Parent = this.activityPanel;
        }

        string _InitialTaskFileName;


        //public string FileName = string.Empty;

        //Dictionary to keep the track of files
        internal Dictionary<string, TabPage> fileNameTabPageDict = new Dictionary<string, TabPage>();

        internal static bool expandInternalProperties = false;

        internal static bool showDynamicPropertyCode = false;


        private void PageForm_Load(object sender, System.EventArgs e)
        {
            CreateNewTab(_InitialTaskFileName);
            if (Program.UI_Instance is ClientUI)
            {
                (Program.UI_Instance as ClientUI).ActionExecutionUpdate += ActionExecutionUpdate;
                (Program.UI_Instance as ClientUI).ActionExecutionTrace += ActionExecutionTrace;

            }

            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            }
            loadRecentFileTab();
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            updateviewHiddenPropertiesToolStripMenuItemText();
            updateshowDynamicPropertyCodeToolStripMenuItemText();
            versionToolStripMenuItem.Text = "Version " + Application.ProductVersion;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                if (tabControl1.SelectedTab.Tag != null && tabControl1.SelectedTab.Tag is RunZappyTaskHelper zappyTaskHelper)
                {
                    //TODO maintain this in dictionary - the path so that we can go to that file
                    // You will have zappytask with editor page and that has its filepath or guid
                    //StringBuilder sb = new StringBuilder();
                    List<string> sb = new List<string>();
                    sb.Add(GetRunZappyTaskName(zappyTaskHelper));
                    TaskEditorPage parentTaskEditorPage = zappyTaskHelper.ParentTaskEditorPage;
                    while (parentTaskEditorPage._runZappyTaskHelper != null)
                    {
                        sb.Insert(0, GetRunZappyTaskName(parentTaskEditorPage._runZappyTaskHelper));
                        parentTaskEditorPage = parentTaskEditorPage._runZappyTaskHelper.ParentTaskEditorPage;
                    }
                    sb.Insert(0, Path.GetFileName(parentTaskEditorPage.ZappytaskFullFileName));
                    string currentPath = string.Join("\\", sb.ToArray());

                    toolStripTextTabStructure.Text = currentPath;
                }
                else
                    toolStripTextTabStructure.Text = Path.GetFileName(tabControl1.SelectedTab.Name);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewTab();
        }

        public void CreateNewTab(string FileName = "", RunZappyTaskHelper runZappyTaskHelper = null)
        {
            try
            {
                string fileToCheck = null;
                if (!string.IsNullOrEmpty(FileName))
                {
                    fileToCheck = FileName;
                    SaveRecentFile(fileToCheck);
                }
                else if (runZappyTaskHelper != null)
                {
                    fileToCheck = runZappyTaskHelper.runZappyTask.SelfGuid.ToString();
                }
                if (!string.IsNullOrEmpty(fileToCheck) && fileNameTabPageDict.ContainsKey(fileToCheck))
                {
                    //Reload - close this tab and open new one
                    tabControl1.SelectedTab = fileNameTabPageDict[fileToCheck];
                    if (tabControl1.SelectedTab.Name.Equals(fileToCheck))
                    {
                        if (runZappyTaskHelper == null)
                        {
                            string askConfirmation = "Do you want to reload file from disk?";
                            DialogResult result =
                                MessageBox.Show("Reload file?", askConfirmation, MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                                CloseTab();
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                Forms.ZappyTaskEditor _ZappyTaskEditor = new Forms.ZappyTaskEditor(FileName, runZappyTaskHelper);

                TabPage _TabPage = new System.Windows.Forms.TabPage();
                _TabPage.Controls.Add(_ZappyTaskEditor);
                _TabPage.Location = new System.Drawing.Point(4, 24);
                _TabPage.Padding = new System.Windows.Forms.Padding(3);
                _TabPage.Size = new System.Drawing.Size(714, 497);
                _TabPage.TabIndex = 0;
                _TabPage.Tag = runZappyTaskHelper;
                if (runZappyTaskHelper != null && string.IsNullOrEmpty(FileName))
                {
                    _TabPage.Text = GetRunZappyTaskName(runZappyTaskHelper);
                    _TabPage.Name = fileToCheck;
                    fileNameTabPageDict[fileToCheck] = _TabPage;
                }
                else
                    _TabPage.Text = string.IsNullOrEmpty(FileName) ? "WorkSpace" + (_TabCount++).ToString() : Path.GetFileNameWithoutExtension(FileName);
                _TabPage.UseVisualStyleBackColor = true;
                if (!string.IsNullOrEmpty(FileName))
                {
                    _TabPage.Name = FileName;
                    fileNameTabPageDict[FileName] = _TabPage;
                }
                //else
                //{
                //    _TabPage.Name = "tabPage1";
                //}
                // 
                // uiTaskEditor1
                // 
                _ZappyTaskEditor.ActivityForm = null;
                _ZappyTaskEditor.Dock = DockStyle.Fill;
                _ZappyTaskEditor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                _ZappyTaskEditor.Location = new System.Drawing.Point(3, 3);
                _ZappyTaskEditor.Name = "uiTaskEditor1";
                _ZappyTaskEditor.Size = new System.Drawing.Size(708, 491);
                _ZappyTaskEditor.TabIndex = 0;
                _ZappyTaskEditor.StateChanged += UiTaskEditor1_StateChanged;
                _ZappyTaskEditor.EditorDirty += UiTaskEditor1_EditorDirty;
                lock (tabControl1)
                {
                    tabControl1.SuspendLayout();
                    tabControl1.TabPages.Add(_TabPage);
                    tabControl1.SelectedTab = _TabPage;
                    tabControl1.ResumeLayout();
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private string GetRunZappyTaskName(RunZappyTaskHelper runZappyTaskHelper)
        {
            return string.IsNullOrEmpty(runZappyTaskHelper.runZappyTask.DisplayName)
                        ? HelperFunctions.HumanizeNameForIZappyAction(runZappyTaskHelper.runZappyTask)
                        : runZappyTaskHelper.runZappyTask.DisplayName + " : Run Task";
        }

        private void UiTaskEditor1_EditorDirty(TabPage obj)
        {
            lock (tabControl1)
            {
                if (tabControl1.TabPages.Contains(obj))
                {
                    try
                    {
                        if (obj.Controls[0] is Forms.ZappyTaskEditor)
                        {
                            bool _IsDirty = (obj.Controls[0] as Forms.ZappyTaskEditor).IsDirty;
                            if (!_IsDirty)
                            {
                                obj.Text = obj.Text.TrimEnd('*');
                            }
                            else
                            {
                                if (!obj.Text.EndsWith("*"))
                                    obj.Text += "*";
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void UiTaskEditor1_StateChanged(object sender, EventArgs e)
        {
            SetupDebuggerContext();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                Forms.ZappyTaskEditor uit = tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor;

                if (this.cmdRun.Text == "Start")
                {
                    uit.RunWithoutDebug();
                }
                else if (this.cmdRun.Text == "Continue")
                {
                    uit.ContinueDebug();
                }
            }
        }

        private void runWithDebuggerClick(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                Forms.ZappyTaskEditor uit = tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor;

                if (this.cmdRun.Text == "Start")
                {
                    uit.Run();
                }
                else if (this.cmdRun.Text == "Continue")
                {
                    uit.ContinueDebug();
                }
            }
        }

        private void addBrkPtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.ZappyTaskEditor uit = tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor;
            uit.AddBreakpoint();
        }

        private void debugToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                Forms.ZappyTaskEditor uit = tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor;
                uit.RunDebug();
            }
        }
        private void mnuExecuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                Forms.ZappyTaskEditor uit = tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor;
                uit.RunWithoutDebug();
            }
        }


        private void stepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                Forms.ZappyTaskEditor uit = tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor;
                if (uit != null)
                    uit.NextStep();
            }
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                Forms.ZappyTaskEditor uit = tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor;
                uit.StopExecution();
                //For each
                stopTabPages();
            }
        }

        private void stopTabPages()
        {
            foreach (TabPage item in tabControl1.TabPages)
            {
                try
                {
                    if (item.Controls != null && item.Controls.Count > 0)
                    {
                        if ((item.Controls[0] as Forms.ZappyTaskEditor).page.State != PageState.Stopped)
                            (item.Controls[0] as Forms.ZappyTaskEditor).page.State = PageState.Stopped;
                    }
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.Error(ex);
                }
            }
        }



        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseTab();

        }

        public void ActionExecutionUpdate(Guid Current, string Result, Guid Next)
        {
            lock (tabControl1)
            {
                if (tabControl1.TabPages.Count <= 0)
                    return;
                foreach (TabPage item in tabControl1.TabPages)
                {
                    try
                    {
                        if (item.Controls != null && item.Controls.Count > 0)
                        {
                            if ((item.Controls[0] as Forms.ZappyTaskEditor).ActionExecutionUpdate(Current, Result, Next))
                            {
                                if (tabControl1.SelectedTab != item)
                                    tabControl1.SelectedTab = item;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.Error(ex);
                    }
                }
            }
        }

        private void ActionExecutionTrace(IZappyAction Current, string Result, Guid Next)
        {
            lock (tabControl1)
            {
                if (tabControl1.TabPages.Count <= 0)
                    return;
                foreach (TabPage item in tabControl1.TabPages)
                {
                    try
                    {
                        if (item.Controls != null && item.Controls.Count > 0)
                            (item.Controls[0] as Forms.ZappyTaskEditor).ActionExecutionTrace(Current, Result, Next);
                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.Error(ex);
                    }
                }
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveZappyTask(true);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Zappy Files|*.zappy";
            openFileDialog1.Title = "Select a Task File";
            openFileDialog1.Multiselect = true;
            openFileDialog1.ValidateNames = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    //try
                    //{
                    CreateNewTab(file);
                    //}
                    //catch (Exception ex)
                    //{
                    //    CrapyLogger.log.Error(ex);
                    //}
                }
            }

        }


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveZappyTask(false);

        }

        void SaveZappyTask(bool UseSameFileName)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                Forms.ZappyTaskEditor uit = tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor;
                if (uit != null)
                {
                    uit.SaveZappyTask(UseSameFileName);
                    //if (!string.IsNullOrEmpty(uit.ZappytaskFullFileName))
                    //{
                    //    tabControl1.SelectedTab.Text = Path.GetFileNameWithoutExtension(uit.ZappytaskFullFileName);
                    //    fileNameTabPageDict[uit.ZappytaskFullFileName] = tabControl1.SelectedTab;

                    //}
                }
            }
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            SetupDebuggerContext();

        }

        private void debugToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            SetupDebuggerContext();

        }

        private void SetupDebuggerContext()
        {
            newToolStripMenuItem.Enabled = true;
            openToolStripMenuItem.Enabled = true;

            if (tabControl1.TabPages.Count <= 0)
            {

                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                closeToolStripMenuItem.Enabled = false;
                runToolStripMenuItem.Enabled = false;
                debugToolStripMenuItem.Enabled = false;
                saveToolStripButton.Enabled = cmdDebug.Enabled = false;
                this.cmdRun.Text = "Continue";
                cmdStep.Enabled = cmdStop.Enabled = true;

                stepToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = false;
            }
            else
            {
                if (tabControl1.SelectedTab != null)
                {
                    PageState _State = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor).State;
                    if (_State == PageState.Running)
                    {
                        saveToolStripMenuItem.Enabled = false;
                        saveAsToolStripMenuItem.Enabled = false;
                        closeToolStripMenuItem.Enabled = false;

                        runToolStripMenuItem.Enabled = false;
                        debugToolStripMenuItem.Enabled = false;
                        saveToolStripButton.Enabled = cmdDebug.Enabled = false;
                        this.cmdRun.Text = "Continue";
                        cmdStep.Enabled = cmdStop.Enabled = true;

                        stepToolStripMenuItem.Enabled = true;
                        stopToolStripMenuItem.Enabled = true;
                    }
                    else if (_State == PageState.Stopped)
                    {
                        setStopStageTabForm();
                        stopTabPages();
                    }
                }
            }
        }

        private void setStopStageTabForm()
        {
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;

            runToolStripMenuItem.Enabled = true;
            debugToolStripMenuItem.Enabled = true;
            stepToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
            saveToolStripButton.Enabled = cmdDebug.Enabled = cmdRun.Enabled = true;
            this.cmdRun.Text = "Start";
            cmdStep.Enabled = cmdStop.Enabled = false;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupDebuggerContext();
        }

        private void cmdRunWithoutDebuger_Click(object sender, EventArgs e)
        {

        }

        private void samplesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Zappy Files|*.zappy";
            openFileDialog1.Title = "Select a Zappy Sample";
            openFileDialog1.Multiselect = true;
            openFileDialog1.ValidateNames = true;
            openFileDialog1.InitialDirectory = CrapyConstants.SamplesFolderStartupPath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    try
                    {
                        CreateNewTab(file);
                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.Error(ex);
                    }
                }
            }
        }

        private void forumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=pxZIi99RV0s&list=PLKsp8tC5ae7Q5UnE4vVLmKon4npO-URDX");
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            CreateNewTab();
        }

        private bool CloseTab(bool closeAll = false)
        {
            bool close = true;
            if (tabControl1.TabPages.Count <= 0)
                return close;
            lock (tabControl1)
            {
                TabPage _RemovalTab = tabControl1.SelectedTab;
                fileNameTabPageDict.Remove(_RemovalTab.Name);
                if (_RemovalTab == null)
                {
                    MessageBox.Show("No Selected Tab To Close");
                    return close;
                }

                if ((_RemovalTab.Controls[0] as Forms.ZappyTaskEditor).IsDirty)
                {
                    if ((_RemovalTab.Controls[0] as Forms.ZappyTaskEditor)?.page.Nodes.Count > 2)
                    {
                        string askConfirmation = "Do you want to save changes to " + _RemovalTab.Text + "?";
                        //can add this code to localize
                        DialogResult result = MessageBox.Show(askConfirmation, "Do you want to save changes?",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                        if (result == DialogResult.Yes)
                            SaveZappyTask(true);
                        else if (result == DialogResult.Cancel)
                            return false;
                    }
                }
                (_RemovalTab.Controls[0] as Forms.ZappyTaskEditor).StopExecution();
                if (!closeAll)
                {
                    tabControl1.SuspendLayout();
                    (_RemovalTab.Controls[0] as Forms.ZappyTaskEditor).StateChanged -= UiTaskEditor1_StateChanged;
                    (_RemovalTab.Controls[0] as Forms.ZappyTaskEditor).EditorDirty -= UiTaskEditor1_EditorDirty;
                    (_RemovalTab.Controls[0] as Forms.ZappyTaskEditor).Dispose();

                    tabControl1.SelectedIndex--;
                    tabControl1.TabPages.Remove(_RemovalTab);
                    tabControl1.ResumeLayout();

                    if (tabControl1.TabPages.Count > 0 && tabControl1.SelectedIndex < tabControl1.TabPages.Count-1)
                        tabControl1.SelectedTab = tabControl1.TabPages[tabControl1.SelectedIndex+1];
                }
            }
            return close;
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            CloseTab();
        }

        public void RenameZappyTaskHelperTabs()
        {
            lock (tabControl1)
            {
                tabControl1.SuspendLayout();
                foreach (var tabPage in tabControl1.TabPages)
                {
                    if (tabPage is TabPage _TabPage)
                    {
                        if (_TabPage.Tag is RunZappyTaskHelper runZappyTaskHelper)
                            _TabPage.Text = string.IsNullOrEmpty(runZappyTaskHelper.runZappyTask.DisplayName) ?
                                HelperFunctions.HumanizeNameForIZappyAction(runZappyTaskHelper.runZappyTask) : runZappyTaskHelper.runZappyTask.DisplayName + " : Run Task";
                    }
                }
                tabControl1.ResumeLayout();
            }

        }

        private void cutCtrlXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.Cut();
        }

        private void copyCtrlCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.Copy();
        }

        private void pasteCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.Paste();
        }

        private void toolStripLearn_Click(object sender, EventArgs e)
        {
            //Stop Learning
            if (ClientUI._TaskRecording)
            {
                //stop learning
                string file = LearnedActions.CreateLearnedActions();
                if (!string.IsNullOrEmpty(file)) CreateNewTab(file);
                //Open the task in new tab
                toolStripLearn.Text = "Record";
                toolStripLearn.ToolTipText = "Start Recording";

            }
            //Start Learning
            else if (!ClientUI._TaskRecording) // checking if not recording
            {
                //start learning
                LearnedActions.CreateLearnedActions();
                //toolStripLearn.Text = "Stop Learning";
                toolStripLearn.Text = toolStripLearn.ToolTipText = "Stop Recording";
            }
        }

        private ToolStripButton saveAsToolStripButton;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            saveAsToolStripMenuItem_Click(sender, e);
        }

        private ToolStripButton toolStripLogButton;

        private void toolStripLogButton_Click(object sender, EventArgs e)
        {
            ClientUI.logWindowShow();
        }

        private ToolStripMenuItem documentationToolStripMenuItem;

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(CrapyConstants.DocumentationFolderPath);
        }

        public void ChangeLanguage(LanguageZappy languageZappy)
        {
            try
            {
                string lang = LocalizeTaskEditorHelper.LanguagePicker(languageZappy);

                ComponentResourceManager resources = null;
                foreach (Control c in this.Controls)
                {
                    resources = new ComponentResourceManager(typeof(PageFormTabbed));
                    resources.ApplyResources(c, c.Name, new CultureInfo(lang));
                }

                foreach (ToolStripItem item in menuStrip1.Items)
                {
                    if (item is ToolStripDropDownItem)
                    {
                        resources = new ComponentResourceManager(typeof(PageFormTabbed));
                        foreach (ToolStripItem dropDownItem in ((ToolStripDropDownItem)item).DropDownItems)
                        {
                            resources.ApplyResources(dropDownItem, dropDownItem.Name, new CultureInfo(lang));
                        }
                    }
                    resources.ApplyResources(item, item.Name, new CultureInfo(lang));
                }

                foreach (ToolStripItem item2 in toolStrip2.Items)
                {
                    if (item2 is ToolStripDropDownItem)
                    {
                        resources = new ComponentResourceManager(typeof(PageFormTabbed));
                        foreach (ToolStripItem dropDownItem in ((ToolStripDropDownItem)item2).DropDownItems)
                        {
                            resources.ApplyResources(dropDownItem, dropDownItem.Name, new CultureInfo(lang));
                        }
                    }
                    resources.ApplyResources(item2, item2.Name, new CultureInfo(lang));
                }

                foreach (ToolStripItem item4 in tabPageContextMenu.Items)
                {
                    if (item4 is ToolStripDropDownItem)
                    {
                        resources = new ComponentResourceManager(typeof(PageFormTabbed));
                        foreach (ToolStripItem dropDownItem in ((ToolStripDropDownItem)item4).DropDownItems)
                        {
                            resources.ApplyResources(dropDownItem, dropDownItem.Name, new CultureInfo(lang));
                        }
                    }
                    resources.ApplyResources(item4, item4.Name, new CultureInfo(lang));
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.zoomIn(1);
            //PageRenderOptions.GridSize = PageRenderOptions.GridSize + 1;
            //taskEditorPage?.Canvas.Invalidate();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.zoomOut(-1);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.DeleteNode();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.PerformUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.PerformRedo();
        }


        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.SelectAllNodes();

        }

        private void ExecuteSelectedActionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.ExecuteSelectedActions();
        }


        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.Cut();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.Copy();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            TaskEditorPage taskEditorPage = (tabControl1.SelectedTab.Controls[0] as Forms.ZappyTaskEditor)?.page;
            taskEditorPage?.Paste();
        }

        const int MRUnumber = 10;
        List<string> MRUlist = new List<string>();
        //For recentFile FUnctionality
        private void SaveRecentFile(string path)
        {
            recentlyOpenedToolStripMenuItem.DropDownItems.Clear(); //clear all recent list from menu
            //LoadRecentList(); //load list from file
            if ((MRUlist.Contains(path))) //prevent duplication on recent list
                MRUlist.Remove(path); //insert given path into list
            MRUlist.Add(path);
            while (MRUlist.Count > MRUnumber) //keep list number not exceeded given value
            {
                MRUlist.RemoveAt(MRUlist.Count - 1);
            }
            foreach (string item in MRUlist)
            {
                ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);  //create new menu for each item in list
                recentlyOpenedToolStripMenuItem.DropDownItems.Insert(0, fileRecent); //add the menu to "recent" menu
            }
            //writing menu list to file
            StreamWriter stringToWrite = new StreamWriter(CrapyConstants.RecentFilePath); //create file called "Recent.txt" located on app folder
            foreach (string item in MRUlist)
            {
                stringToWrite.WriteLine(item); //write list to stream
            }
            stringToWrite.Flush(); //write stream to file
            stringToWrite.Close(); //close the stream and reclaim memory
        }
        /// <summary>
        /// load recent file list from file
        /// </summary>
        private void LoadRecentList()
        {//try to load file. If file isn't found, do nothing
            MRUlist.Clear();
            try
            {
                StreamReader listToRead = new StreamReader(CrapyConstants.RecentFilePath); //read file stream
                string line;
                while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                    MRUlist.Add(line); //insert to list
                listToRead.Close(); //close the stream
            }
            catch (Exception)
            {

                //throw;
            }

        }
        /// <summary>
        /// click menu handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecentFile_click(object sender, EventArgs e)
        {
            CreateNewTab(sender.ToString());
        }

        private void loadRecentFileTab()
        {
            LoadRecentList();
            foreach (string item in MRUlist)
            {
                ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);  //create new menu for each item in list
                recentlyOpenedToolStripMenuItem.DropDownItems.Insert(0, fileRecent); //add the menu to "recent" menu
            }
        }
     
        private void toolStripTextTabStructure_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode != Keys.Enter) return;

            //// your event handler here
            //string textBoxPath = toolStripTextTabStructure.Text;
            //string filename = null;
            //bool pathFound = false;
            //int i;
            //for (i = 0; i < tabControl1.TabCount; i++)
            //{
            //    filename = textBoxPath.Remove(textBoxPath.IndexOf("."));
            //    if (tabControl1.TabPages[i].Text.Equals(filename))
            //    {
            //        pathFound = true;
            //        break;
            //    }

            //}
            //if (pathFound)
            //    tabControl1.SelectedTab = tabControl1.TabPages[i];
            //else
            //    MessageBox.Show("Could not find '" + textBoxPath + "'.Check spelling and try again.");
        }

        private void viewHiddenPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //set bool value and change text
            expandInternalProperties = !expandInternalProperties;

            updateviewHiddenPropertiesToolStripMenuItemText();
        }

        private void updateviewHiddenPropertiesToolStripMenuItemText()
        {
            viewHiddenPropertiesToolStripMenuItem.Text = expandInternalProperties ? "Collapse Hidden Properties": "Expand Hidden Properties";
        }

        private void startChildSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void showDynamicPropertyCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showDynamicPropertyCode = !showDynamicPropertyCode;

            updateshowDynamicPropertyCodeToolStripMenuItemText();
        }

        private void updateshowDynamicPropertyCodeToolStripMenuItemText()
        {
            showDynamicPropertyCodeToolStripMenuItem.Text = showDynamicPropertyCode ? "Hide Dynamic Property Code" : "Show Dynamic Property Code";
        }

        private void logFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(CrapyConstants.LogFolder);
        }
    }
}
