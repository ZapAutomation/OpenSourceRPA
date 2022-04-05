using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.ZappyActions.Core.Helper;
using Zappy.ZappyTaskEditor.Nodes;

namespace Zappy.ZappyTaskEditor.EditorPage.Forms
{
    internal partial class ZappyTaskEditor : UserControl
    {
        public string ZappytaskFullFileName { get { return page.ZappytaskFullFileName; } }

        private string _FilePath;
        private RunZappyTaskHelper _runZappyTaskHelper;
        public ZappyTaskEditor(string FilePath, RunZappyTaskHelper runZappyTaskHelper)
        {
            InitializeComponent();
            _FilePath = FilePath;
            _runZappyTaskHelper = runZappyTaskHelper;
        }

        private void InitializeComponent()
        {
            this.pagePanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.textBoxType = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pagePanel
            // 
            this.pagePanel.AutoScroll = true;
            this.pagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pagePanel.Location = new System.Drawing.Point(0, 0);
            this.pagePanel.Name = "pagePanel";
            this.pagePanel.Size = new System.Drawing.Size(750, 561);
            this.pagePanel.TabIndex = 0;
            //this.pagePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pagePanel_MouseMove);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pagePanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(984, 561);
            this.splitContainer1.SplitterDistance = 750;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.propertyGrid1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBoxType);
            this.splitContainer2.Size = new System.Drawing.Size(230, 561);
            this.splitContainer2.SplitterDistance = 509;
            this.splitContainer2.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.AllowDrop = true;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(230, 509);
            this.propertyGrid1.TabIndex = 4;
            this.propertyGrid1.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propertyGrid1_SelectedGridItemChanged);
            // 
            // textBoxType
            // 
            this.textBoxType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxType.Location = new System.Drawing.Point(0, 0);
            this.textBoxType.Multiline = true;
            this.textBoxType.Name = "textBoxType";
            this.textBoxType.ReadOnly = true;
            this.textBoxType.Size = new System.Drawing.Size(230, 48);
            this.textBoxType.TabIndex = 0;
            // 
            // ZappyTaskEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.Name = "ZappyTaskEditor";
            this.Size = new System.Drawing.Size(984, 561);
            this.Load += new System.EventHandler(this.PageForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private Panel pagePanel;


        //public Point LastMousePosition { get; set; }
        private SplitContainer splitContainer1;

        public event EventHandler StateChanged;

        internal TaskEditorPage page;

        public PageState State { get { return page.State; } }

        public ActivityForm ActivityForm { get; set; }
        List<IZappyAction> undoActionList;
        private void PageForm_Load(object sender, System.EventArgs e)
        {
            this.page = TaskEditorPage.Create(_FilePath, _runZappyTaskHelper);
            undoActionList = new List<IZappyAction>();
            foreach(Node n in page.Nodes)
            {
                undoActionList.Add(n.Activity);
            }
            propertyGrid1.Tag = page;
            propertyGrid1.PropertySort = PropertySort.Categorized;
            Page_ActiveNodeChanged(null);
            this.page.OnStateChanged += Page_OnStateChanged;
            this.page.Show(this.pagePanel);
            this.page.ActiveNodeChanged += Page_ActiveNodeChanged;
            this.page.PageDirty += Page_PageDirty;
            this.page.UpdateUndoAction += Page_UpdateUndoAction;

            Page_OnStateChanged(null, EventArgs.Empty);
        }

        private void Page_UpdateUndoAction()
        {
            List<IZappyAction> listToAdd = new List<IZappyAction>();
            listToAdd.AddRange(undoActionList);
            page.undoLinkedList.AddFirst(listToAdd);
            undoActionList.Clear();
            //Update backup list of actions

            for (int i = 0; i < page.Nodes.Count; i++)
            {
                IZappyAction newNodeAction = CommonProgram.DeepClone<IZappyAction>(page.Nodes[i].Activity);
                undoActionList.Add(newNodeAction);
            }

            if (page.undoLinkedList.Count > 100)
            {
                page.undoLinkedList.RemoveLast();
            }
        }

        public bool IsDirty { get { return page.IsDirty; } set { page.IsDirty = value; } }
        //List<IZappyAction> undoActionList;
        private void Page_PageDirty()
        {
            EditorDirty?.Invoke(this.Parent as TabPage);
        }

        public event Action<TabPage> EditorDirty;

        //bool _FirstPropertyPageLoad = true;

        private void Page_ActiveNodeChanged(Node obj)
        {
            try
            {
                if (DesignMode)
                    return;
                if (obj != null)
                {
                    if (obj.Activity != null)
                    {
                        //if (ReferenceEquals(this.propertyGrid1.SelectedObject, obj.Activity))
                        //    propertyGrid1.Refresh();
                        //else always update
                        this.propertyGrid1.SelectedObject = obj.Activity;

                        if (this.propertyGrid1.SelectedGridItem != null)
                        {
                            //propertyGrid1.SelectedObject = this.page._runZappyTaskHelper.runZappyTask.NestedExecuteActivities.Activities;
                            GridItem root = this.propertyGrid1.SelectedGridItem;

                            while (root.Parent != null)
                                root = root.Parent;

                            foreach (GridItem item in root.GridItems)
                            {
                                //TODO - Update it as required
                                //item.Label == "Internals" || -Make them hidden
                                if (item.Label == "Common" || item.Label == "Output" || item.Label == "Internals" || item.Label == "Optional")
                                {
                                    if (PageFormTabbed.expandInternalProperties)
                                        item.Expanded = true;
                                    else
                                        item.Expanded = false;
                                }
                            }
                        }
                    }

                }
                else
                {
                    if (page.SelectedNodes.Count == 0)
                    {
                        //if (!ReferenceEquals(this.propertyGrid1.SelectedObject, this.page._PageZappyTask))
                        //{
                        propertyGrid1.SelectedObject = this.page._PageZappyTask;
                        //}
                    }
                    else
                    {
                        object[] _SelectedActions = new object[page.SelectedNodes.Count];
                        int i = 0;
                        foreach (Node item in page.SelectedNodes)
                        {
                            if (item != null && item.Activity != null)
                                _SelectedActions[i++] = item.Activity;
                        }
                        propertyGrid1.SelectedObjects = _SelectedActions;
                    }
                }
                //if (_FirstPropertyPageLoad)
                //{
                //    if (this.propertyGrid1.SelectedGridItem == null)
                //    {
                //        propertyGrid1.SelectedObject = this.page._runZappyTaskHelper.runZappyTask.NestedExecuteActivities.Activities;
                //    }
                //    GridItem root = this.propertyGrid1.SelectedGridItem;

                //    while (root.Parent != null)
                //        root = root.Parent;

                //    foreach (GridItem item in root.GridItems)
                //    {
                //        //TODO - Update it as required
                //        //item.Label == "Internals" || -Make them hidden
                //        if (item.Label == "Common" || item.Label == "Output" || item.Label == "Internals" || item.Label == "Optional")
                //        {
                //            item.Expanded = false;
                //        }
                //    }

                //    //_FirstPropertyPageLoad = false;
                //}

            }
            catch
            {

            }
            propertyGrid1.Refresh();
        }

        private void Page_OnStateChanged(object sender, EventArgs e)
        {
            propertyGrid1.Enabled = page.State == PageState.Stopped;
            StateChanged?.Invoke(this.Parent as TabPage, EventArgs.Empty);
        }

        private void RunButton_Click(object sender, System.EventArgs e)
        {
            Run();
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            RunDebug();
        }

        //private void StopButton_Click(object sender, EventArgs e)
        //{
        //    if (this.page != null)
        //    {
        //        this.page.Stop();
        //    }
        //}

        private void StepDebugButton_Click(object sender, EventArgs e)
        {
            NextStep();
        }

        public void Run()
        {
            SaveZappyTask(true);
            if (this.page != null)
            {
                this.page.Run();
            }
        }

        public void RunDebug()
        {
            SaveZappyTask(true);
            if (this.page != null)
            {
                this.page.RunDebug();
            }
        }
        public void RunWithoutDebug()
        {
            SaveZappyTask(true);
            if (this.page != null)
            {
                this.page.RunWithoutDebug();
            }
        }

        public void SaveZappyTask(bool useSameFileIfAvailable)
        {
            if (this.page != null)
                page.SaveZappyTask(useSameFileIfAvailable);
        }

        public void NextStep()
        {
            if (this.page != null)
            {
                this.page._AutoStep = false;
                this.page.Step();
            }
        }

        internal void ContinueDebug()
        {
            this.page?.ContinueDebug();
        }

        public void StopExecution()
        {
            if (this.page != null)
            {
                this.page.Stop(true);
            }
        }

        internal void AddBreakpoint()
        {
            this.page?.SetBreakpoint();
        }

        public bool ActionExecutionUpdate(Guid Current, string Result, Guid Next)
        {
            return page.ExecutionProgress(Current, Result, Next);
        }
        public void ActionExecutionTrace(IZappyAction Current, string Result, Guid Next)
        {
            page.ActionExecutionTrace(Current, Result, Next);
        }



        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            //Type type = typeof(IDynamicProperty);

            if (propertyGrid1.SelectedGridItem?.PropertyDescriptor != null)
                //if (type.IsAssignableFrom(propertyGrid1.SelectedGridItem?.PropertyDescriptor.PropertyType))
                //{
                //TODO: use when automating ui
                //    IDynamicProperty dp = propertyGrid1.SelectedGridItem?.Value as IDynamicProperty;
                //    if (dp != null) textBoxType.Text = dp.ElementType.GetFriendlyName();
                //}
                //else
                //{
                textBoxType.Text =
                    "Type: " + propertyGrid1.SelectedGridItem.PropertyDescriptor.PropertyType.GetFriendlyName() + Environment.NewLine +
                    "Action: " + CommonProgram.HumanizeNameForGivenType(propertyGrid1.SelectedObject.GetType());
            //}
        }


        //private void pagePanel_MouseMove(object sender, MouseEventArgs e)
        //{
        //    LastMousePosition = e.Location;
        //}
    }
}
