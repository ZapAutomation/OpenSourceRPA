using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.ZappyActions.AutomaticallyCreatedActions;
using Zappy.ZappyActions.Core;
using Zappy.ZappyTaskEditor.ExecutionHelpers;
using Zappy.ZappyTaskEditor.Helper;
using DragAction = Zappy.ZappyActions.AutomaticallyCreatedActions.DragAction;

namespace Zappy.ZappyTaskEditor.EditorPage.Forms
{
    public sealed class ActivityForm : Form
    {
        private Panel activityPanel;
        private TreeView activityTreeView;
        private Label activityLabel;
        private TextBox activityTextBox;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivityForm));
            this.activityPanel = new System.Windows.Forms.Panel();
            this.activityTreeView = new System.Windows.Forms.TreeView();
            this.activityTextBox = new System.Windows.Forms.TextBox();
            this.activityLabel = new System.Windows.Forms.Label();
            this.activityPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // activityPanel
            // 
            resources.ApplyResources(this.activityPanel, "activityPanel");
            this.activityPanel.BackColor = System.Drawing.Color.White;
            this.activityPanel.Controls.Add(this.activityTreeView);
            this.activityPanel.Controls.Add(this.activityTextBox);
            this.activityPanel.Controls.Add(this.activityLabel);
            this.activityPanel.Name = "activityPanel";
            // 
            // activityTreeView
            // 
            resources.ApplyResources(this.activityTreeView, "activityTreeView");
            this.activityTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.activityTreeView.FullRowSelect = true;
            this.activityTreeView.HotTracking = true;
            this.activityTreeView.Name = "activityTreeView";
            this.activityTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("activityTreeView.Nodes")))});
            this.activityTreeView.ShowLines = false;
            this.activityTreeView.TabStop = false;
            // 
            // activityTextBox
            // 
            resources.ApplyResources(this.activityTextBox, "activityTextBox");
            this.activityTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.activityTextBox.Name = "activityTextBox";
            this.activityTextBox.TabStop = false;
            // 
            // activityLabel
            // 
            resources.ApplyResources(this.activityLabel, "activityLabel");
            this.activityLabel.Name = "activityLabel";
            // 
            // ActivityForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.activityPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ActivityForm";
            this.Load += new System.EventHandler(this.ActivityForm_Load);
            this.activityPanel.ResumeLayout(false);
            this.activityPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        private ActivityForm()
        {
            this.InitializeComponent();
            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            }

        }

        public static Panel Create()
        {
            var form = new ActivityForm();
            form.activityTreeView.SetWindowTheme("explorer");
            form.activityTreeView.Tag = ZappyTaskEditorHelper.GetAllActivities();
            form.activityTreeView.ItemDrag += TreeView1_ItemDrag;
            form.activityTreeView.MouseMove += ActivityTreeViewOnMouseHover;
            form.activityTextBox.TextChanged += (sender, e) => form.FilterTreeView();
            form.FilterTreeView();
            return form.activityPanel;
        }
        static System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();


        private static void ActivityTreeViewOnMouseHover(object sender, MouseEventArgs e)
        {
            var treeView = sender as TreeView;

            // Get the node at the current mouse pointer location.
            TreeNode theNode = treeView.GetNodeAt(e.X, e.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if ((theNode != null))
            {
                // Verify that the tag property is not "null".
                //if (theNode.Tag != null)
                //{
                // Change the ToolTip only if the pointer moved to a new node.
                if (theNode.ImageKey.ToString() != ToolTip1.GetToolTip(treeView))
                {
                    ToolTip1.SetToolTip(treeView, theNode.ImageKey.ToString());
                }

                //}
                //else
                //{
                //    ToolTip1.SetToolTip(treeView, "");
                //}
            }
            else     // Pointer is not over a node so clear the ToolTip.
            {
                ToolTip1.SetToolTip(treeView, "");
            }

            //ToolTip1.SetToolTip(treeView, "Test");
            //treeView.ShowNodeToolTips = true;
        }

        private static void TreeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var treeView = sender as TreeView;
            treeView.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void FilterTreeView()
        {
            var filterText = this.activityTextBox.Text.ToLower().Replace(" ", string.Empty);
            if (filterText.Length == 1)
            {
                //to avoid flickering
                return;
            }
            this.activityTreeView.BeginUpdate(); // avoid flickering.
            this.activityTreeView.Nodes.Clear();
            // internal activities
            //var generalActivities = this.activityTreeView.Nodes.Add("General");
            //generalActivities.Nodes.Add(typeof(VariableNodeAction).FullName, "New Variable").EnsureVisible();
            //generalActivities.Nodes.Add(typeof(LoopStartNodeAction).FullName, "Loop Collection").EnsureVisible();
            //generalActivities.Nodes.Add(typeof(EndNodeAction).FullName, "Start / End").EnsureVisible();
            // external activities
            var dataSource = this.activityTreeView.Tag as IEnumerable<Type>;
            foreach (Type type in dataSource)
            {
                //Ignoreed activities list
                if (type == typeof(StartNodeAction) || type == typeof(LaunchApplicationAction)
                    || type == typeof(KeyboardAction) || type == typeof(RemoveKeysAction)
                    || type == typeof(SetBaseAction) || type == typeof(SetStateAction)
                    || type == typeof(MediaAction) || type == typeof(SetValueAction)
                    || type == typeof(ErrorAction) || type == typeof(InvokeAction)
                    || type == typeof(MarkerAction) || type == typeof(TouchAction)
                    || type == typeof(SystemAction) || type == typeof(DragAction) || type == typeof(DragDropAction)

                    )//|| type == typeof(TestStepMarkerAction) 
                {

                }
                else
                {
                    ActivityFormDisplayHelper activityFormDisplay = CrapyConstants.TypeToActivityFormDisplayHelper[type];

                    if (activityFormDisplay.ComparisionString.Contains(filterText))
                    {
                        //Object with groupkey, groupnodetext and description for type to optimise loading
                        var groupKey = activityFormDisplay.GroupKey;
                        string groupNodeText = activityFormDisplay.GroupNodeText;
                        // temp.Add(groupNodeText);

                        //Commenting out based on feedback
                        //if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
                        //{
                        //    groupNodeText = LocalizeTaskEditorHelper.ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage, groupNodeText);
                        //}
                        //else if (ApplicationSettingProperties.Instance.ZappyUILanguage == LanguageZappy.pol)
                        //{
                        //    groupNodeText = LocalizeTaskEditorHelper.ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage, groupNodeText);
                        //}
                        if (!this.activityTreeView.Nodes.ContainsKey(groupKey))
                        {
                            //if (string.IsNullOrEmpty(filterText))
                            this.activityTreeView.Nodes.Add(groupKey, groupNodeText);
                        }
                        var nodeText = CommonProgram.HumanizeNameForGivenType(type);                       
                        //TODO Add image key here
                        //this.activityTreeView.ImageList = new ImageList();
                        //public void Add(string key, System.Drawing.Image image);

                        DescriptionAttribute MyAttribute =
                            activityFormDisplay.MyAttribute;
                        string description;
                        if (MyAttribute != null)
                            description = MyAttribute.Description;
                        else
                        {
                            description = string.Empty;
                        }
                        if (string.IsNullOrEmpty(filterText))
                            this.activityTreeView.Nodes[groupKey].Nodes.Add(type.FullName, nodeText, description);
                        else
                        {
                            this.activityTreeView.Nodes[groupKey].Nodes.Add(type.FullName, nodeText, description);
                            this.activityTreeView.Nodes[groupKey].Expand();
                        }
                    }
                }
            }
            this.activityTreeView.Sort();
            this.activityTreeView.EndUpdate();
        }

        private void ActivityForm_Load(object sender, EventArgs e)
        {

        }

        public void ChangeLanguage(LanguageZappy languageZappy)
        {
            try
            {
                string lang = LocalizeTaskEditorHelper.LanguagePicker(languageZappy);

                ComponentResourceManager resources = null;

                foreach (Control c in this.Controls)
                {
                    foreach (Control c1 in c.Controls)
                    {
                        if (c1 is Label)
                        {
                            resources = new ComponentResourceManager(typeof(ActivityForm));
                            resources.ApplyResources(c1, c1.Name, new CultureInfo(lang));
                        }
                    }
                }

                //foreach (TextBox item in activityTextBox.Item)
                //{

                //    resources = new ComponentResourceManager(typeof(ActivityForm));
                //    resources.ApplyResources(item, item.Name, new CultureInfo(lang));
                //}
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }



    }
}