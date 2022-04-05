using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.Invoker;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;
using Zappy.ZappyTaskEditor.EditorPage.Forms;
using Zappy.ZappyTaskEditor.ExecutionHelpers;
using Zappy.ZappyTaskEditor.Nodes;

namespace Zappy.ZappyTaskEditor.EditorPage.ElementPicker
{
    internal sealed class DynamicPropertyPickerForm : Form
    {
        private Button okButton;
        private Button cancelButton;
        private DataGridView gridProperties;
        private LabelColumn labelColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private CSharpCodeEditor txtExpression;
        private TabPage tabPage3;
        private Label label1;
        private RichTextBox txtStaticValue;
        private ComboBox cmdNodes;
        private Button cmdTestCompile;
        private CheckBox chkEvalOnFirstUse;
        private Label lblValueName;
        private Label labelType;
        private DataGridViewCheckBoxColumn conditionEnabledColumn;
        private LabelColumn conditionNameColumn;
        private DataGridViewTextBoxColumn Category;
        private DataGridViewTextBoxColumn Type;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer3;
        private TabPage tabPage2;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DynamicPropertyPickerForm));
            this.cancelButton = new System.Windows.Forms.Button();
            this.gridProperties = new System.Windows.Forms.DataGridView();
            this.conditionEnabledColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.conditionNameColumn = new Zappy.ZappyTaskEditor.ExecutionHelpers.LabelColumn();
            this.Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.okButton = new System.Windows.Forms.Button();
            this.labelColumn1 = new Zappy.ZappyTaskEditor.ExecutionHelpers.LabelColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cmdNodes = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cmdTestCompile = new System.Windows.Forms.Button();
            this.chkEvalOnFirstUse = new System.Windows.Forms.CheckBox();
            this.txtExpression = new Zappy.ZappyTaskEditor.EditorPage.Forms.CSharpCodeEditor();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.txtStaticValue = new System.Windows.Forms.RichTextBox();
            this.lblValueName = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
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
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabStop = false;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // gridProperties
            // 
            this.gridProperties.AllowUserToAddRows = false;
            this.gridProperties.AllowUserToDeleteRows = false;
            this.gridProperties.AllowUserToOrderColumns = true;
            this.gridProperties.AllowUserToResizeRows = false;
            resources.ApplyResources(this.gridProperties, "gridProperties");
            this.gridProperties.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridProperties.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gridProperties.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(230)))));
            this.gridProperties.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.conditionEnabledColumn,
            this.conditionNameColumn,
            this.Category,
            this.Type});
            this.gridProperties.EnableHeadersVisualStyles = false;
            this.gridProperties.Name = "gridProperties";
            this.gridProperties.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridProperties.TabStop = false;
            this.gridProperties.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridProperties_CellContentClick);
            // 
            // conditionEnabledColumn
            // 
            this.conditionEnabledColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.conditionEnabledColumn.DataPropertyName = "Select";
            this.conditionEnabledColumn.FillWeight = 10F;
            resources.ApplyResources(this.conditionEnabledColumn, "conditionEnabledColumn");
            this.conditionEnabledColumn.Name = "conditionEnabledColumn";
            this.conditionEnabledColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.conditionEnabledColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // conditionNameColumn
            // 
            this.conditionNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.conditionNameColumn.DataPropertyName = "Name";
            this.conditionNameColumn.FillWeight = 30F;
            resources.ApplyResources(this.conditionNameColumn, "conditionNameColumn");
            this.conditionNameColumn.Name = "conditionNameColumn";
            this.conditionNameColumn.ReadOnly = true;
            this.conditionNameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.conditionNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Category
            // 
            this.Category.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.Category, "Category");
            this.Category.Name = "Category";
            // 
            // Type
            // 
            this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.Type, "Type");
            this.Type.Name = "Type";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.TabStop = false;
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // labelColumn1
            // 
            this.labelColumn1.DataPropertyName = "PropertyName";
            this.labelColumn1.FillWeight = 30F;
            resources.ApplyResources(this.labelColumn1, "labelColumn1");
            this.labelColumn1.Name = "labelColumn1";
            this.labelColumn1.ReadOnly = true;
            this.labelColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.labelColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "DataType";
            this.dataGridViewTextBoxColumn1.FillWeight = 60F;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cmdNodes);
            this.tabPage1.Controls.Add(this.gridProperties);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cmdNodes
            // 
            resources.ApplyResources(this.cmdNodes, "cmdNodes");
            this.cmdNodes.FormattingEnabled = true;
            this.cmdNodes.Name = "cmdNodes";
            this.cmdNodes.SelectedIndexChanged += new System.EventHandler(this.cmdNodes_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cmdTestCompile);
            this.tabPage2.Controls.Add(this.chkEvalOnFirstUse);
            this.tabPage2.Controls.Add(this.txtExpression);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cmdTestCompile
            // 
            resources.ApplyResources(this.cmdTestCompile, "cmdTestCompile");
            this.cmdTestCompile.Name = "cmdTestCompile";
            this.cmdTestCompile.UseVisualStyleBackColor = true;
            this.cmdTestCompile.Click += new System.EventHandler(this.cmdTestCompile_Click);
            // 
            // chkEvalOnFirstUse
            // 
            resources.ApplyResources(this.chkEvalOnFirstUse, "chkEvalOnFirstUse");
            this.chkEvalOnFirstUse.Name = "chkEvalOnFirstUse";
            this.chkEvalOnFirstUse.UseVisualStyleBackColor = true;
            // 
            // txtExpression
            // 
            resources.ApplyResources(this.txtExpression, "txtExpression");
            this.txtExpression.Name = "txtExpression";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer3);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            resources.ApplyResources(this.splitContainer3, "splitContainer3");
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.txtStaticValue);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtStaticValue
            // 
            this.txtStaticValue.AcceptsTab = true;
            resources.ApplyResources(this.txtStaticValue, "txtStaticValue");
            this.txtStaticValue.Name = "txtStaticValue";
            this.txtStaticValue.TabStop = false;
            // 
            // lblValueName
            // 
            resources.ApplyResources(this.lblValueName, "lblValueName");
            this.lblValueName.Name = "lblValueName";
            // 
            // labelType
            // 
            resources.ApplyResources(this.labelType, "labelType");
            this.labelType.Name = "labelType";
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.okButton);
            this.splitContainer1.Panel1.Controls.Add(this.cancelButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lblValueName);
            this.splitContainer2.Panel2.Controls.Add(this.labelType);
            // 
            // DynamicPropertyPickerForm
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.splitContainer1);
            this.Name = "DynamicPropertyPickerForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DynamicPropertyPickerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
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

        private IDynamicProperty _PreviousDynamicPorperty;

        //public string DynamicKeyOrValue
        //{
        //    get { return BuildVariableValue(); }
        //}

        public void BuildVariableValue()
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                if (cmdNodes.SelectedIndex > -1)
                {
                    _PreviousDynamicPorperty.ResetFlags();
                    if ((cmdNodes.SelectedItem as ComboboxItem).Action.GetType() != typeof(VariableNodeAction))
                    {
                        bool _IsChecked = false;
                        Guid _SeletcedActionID = Guid.Empty;
                        string _PropertyName = string.Empty;
                        {
                            _SeletcedActionID = (cmdNodes.SelectedItem as ComboboxItem).Action.SelfGuid;
                            _PropertyName = string.Empty;
                            for (int i = 0; i < gridProperties.RowCount; i++)
                            {
                                _IsChecked = Convert.ToBoolean(gridProperties.Rows[i].Cells[0].Value);
                                if (_IsChecked)
                                {
                                    _PropertyName = gridProperties.Rows[i].Cells[1].Value.ToString();
                                    break;
                                }
                            }
                        }
                        if (_IsChecked)
                            _PreviousDynamicPorperty.DymanicKey = ZappyExecutionContext.GetKey(_SeletcedActionID, _PropertyName);
                    }
                    else
                        _PreviousDynamicPorperty.DymanicKey = SharedConstants.VariableNameBegin + ((cmdNodes.SelectedItem as ComboboxItem).Action as VariableNodeAction).VariableName + CrapyConstants.VariableNameEnd;
                }
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                _PreviousDynamicPorperty.ResetFlags();
                _PreviousDynamicPorperty.RuntimeScript = txtExpression.Text;
                _PreviousDynamicPorperty.EvaluateOnFirstUse = chkEvalOnFirstUse.Checked;
            }
            else
            {
                _PreviousDynamicPorperty.ResetFlags();
                string __Value = txtStaticValue.Text;
                if (string.IsNullOrEmpty(__Value))
                    __Value = string.Empty;

                //if (__Value.StartsWith(SharedConstants.EscapedVariableNameBegin))
                //    __Value = __Value.Substring(1);
                _PreviousDynamicPorperty.ObjectValue = __Value;
            }
        }

        private IZappyAction CurrentNode;
        private string PropertyName;
        private TaskEditorPage Page;
        PropertyDescriptor _PropertyDescriptor;
        Type _FunctionType = null;
        public DynamicPropertyPickerForm(object gridCellValue, IZappyAction CurrentNode, PropertyDescriptor PropertyDescriptor, TaskEditorPage Page)
        {
            this.InitializeComponent();
            this.CurrentNode = CurrentNode;
            this.Page = Page;
            _PropertyDescriptor = PropertyDescriptor;
            PropertyName = _PropertyDescriptor.Name;
            RunScriptFunctionType attr = _PropertyDescriptor.Attributes.OfType<RunScriptFunctionType>().FirstOrDefault();
            if (attr != null)
                _FunctionType = attr.FunctionType;
            _PreviousDynamicPorperty = gridCellValue as IDynamicProperty;
        }

        public class ComboboxItem
        {
            public string Text { get; set; }

            public IZappyAction Action { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void DynamicPropertyPickerForm_Load(object sender, EventArgs e)
        {
            Node node = null;
            if (string.IsNullOrWhiteSpace(txtExpression.Text))
                txtExpression.Text = "context =>  ";
            if (Page != null)
            {
                foreach (Node pageNode in Page.Nodes)
                {
                    if (pageNode.Activity == CurrentNode)
                    {
                        node = pageNode;
                        continue;
                    }
                    //if (pageNode.Activity is VariableNodeAction)
                    //    continue;
                    cmdNodes.Items.Add(new ComboboxItem() { Text = string.IsNullOrEmpty(pageNode.Activity.DisplayName) ? pageNode.Name : pageNode.Activity.DisplayName, Action = pageNode.Activity });
                }
            }

            labelType.Text =
                _PropertyDescriptor.PropertyType.GetFriendlyName();//.Replace("Zappy.InputData.", "");
            lblValueName.Text = _PropertyDescriptor.Name;
            if (_PreviousDynamicPorperty != null)
            {
                if (_PreviousDynamicPorperty.DymanicKeySpecified)
                {
                    string _DynamicKey = _PreviousDynamicPorperty.DymanicKey;

                    if (_DynamicKey.StartsWith(SharedConstants.VariableNameBegin) &&
                    _DynamicKey.EndsWith(CrapyConstants.VariableNameEnd))
                    {
                        Guid _ActionId = Guid.Empty;
                        string _Propertyname = "EvaluationExpression";

                        if (_DynamicKey.IndexOf(':') > 0) //guid:propertyname
                        {
                            string[] splitedValues = _DynamicKey.Split(':');
                            string guid = splitedValues[0].Replace(SharedConstants.VariableNameBegin, "");
                            if (Guid.TryParse(guid, out _ActionId))
                                _Propertyname = splitedValues[1].Replace(CrapyConstants.VariableNameEnd, "");
                        }
                        else //variable name
                        {
                            string _VariableName = _DynamicKey.Replace(SharedConstants.VariableNameBegin, string.Empty).
                                Replace(CrapyConstants.VariableNameEnd, string.Empty);

                            foreach (Node pageNode in Page.Nodes)
                            {
                                if (pageNode.Activity is VariableNodeAction && (pageNode.Activity as VariableNodeAction).VariableName == _VariableName)
                                {
                                    _ActionId = pageNode.Id;
                                    break;
                                }
                            }
                        }

                        SelectVariableValueFromActionID_Propertyname(_ActionId, _Propertyname);
                        tabControl1.SelectedTab = tabPage1;
                    }
                }
                else if (_PreviousDynamicPorperty.ValueSpecified)
                {
                    string __Value = _PreviousDynamicPorperty.ObjectValue.ToString();

                    if (string.IsNullOrEmpty(__Value))
                        __Value = string.Empty;

                    //if (__Value.StartsWith(SharedConstants.VariableNameBegin))
                    //    __Value = SharedConstants.EscapedVariableNameBegin + __Value.Substring(SharedConstants.VariableNameBegin.Length);

                    txtStaticValue.Text = __Value;
                    tabControl1.SelectedTab = tabPage3;
                }
                else if (_PreviousDynamicPorperty.RuntimeScriptSpecified)
                {
                    chkEvalOnFirstUse.Checked = _PreviousDynamicPorperty.EvaluateOnFirstUse;
                    txtExpression.Text = _PreviousDynamicPorperty.RuntimeScript;
                    tabControl1.SelectedTab = tabPage2;
                }
                //never null
                else
                {
                    //Making previous node by default in the list as most likely
                    for (int i = 0; i < cmdNodes.Items.Count; i++)
                    {
                        if ((cmdNodes.Items[i] as ComboboxItem).Action.NextGuid == this.CurrentNode.SelfGuid)
                        {
                            cmdNodes.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }

            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;

            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            }

        }

        void ChangeLanguage(LanguageZappy languageZappy)
        {
            try
            {
                string lang = "en";
                if (languageZappy == LanguageZappy.jp)
                {
                    lang = "ja-JP";
                }
                ComponentResourceManager resources = null;

                foreach (Control c in this.Controls)
                {
                    if (c is TabControl)
                    {
                        foreach (Control c1 in c.Controls)
                        {
                            if (c1 is TabPage)
                            {
                                foreach (Control c2 in c1.Controls)
                                {
                                    resources = new ComponentResourceManager(typeof(DynamicPropertyPickerForm));
                                    resources.ApplyResources(c2, c2.Name, new CultureInfo(lang));
                                }
                            }
                            resources = new ComponentResourceManager(typeof(DynamicPropertyPickerForm));
                            resources.ApplyResources(c1, c1.Name, new CultureInfo(lang));
                        }
                    }
                    else
                    {
                        resources = new ComponentResourceManager(typeof(DynamicPropertyPickerForm));
                        resources.ApplyResources(c, c.Name, new CultureInfo(lang));
                    }
                }
            }


            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtExpression.HandleHostKeyDownEvent(tabControl1.SelectedTab == tabPage2);
        }

        private void SelectVariableValueFromActionID_Propertyname(Guid _ActionId, string _Propertyname)
        {
            for (int i = 0; i < cmdNodes.Items.Count; i++)
            {
                if ((cmdNodes.Items[i] as ComboboxItem).Action.SelfGuid == _ActionId)
                {
                    cmdNodes.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < gridProperties.RowCount; i++)
            {
                if (_Propertyname == gridProperties.Rows[i].Cells[1].Value.ToString())
                {
                    gridProperties.Rows[i].Cells[0].Value = true;
                    break;
                }
            }
        }

        void ConfigureCodeEditor()
        {
            //txtExpression.CaretStyle = CaretStyle.Block;

        }

        private void cmdNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmdNodes.SelectedItem != null)
            {
                gridProperties.Rows.Clear();
                IZappyAction _zappyAction = (cmdNodes.SelectedItem as ComboboxItem).Action;
                PropertyInfo[] _Properties = ActionTypeInfo.AllPropertyInfo[_zappyAction.GetType()];
                //Check if types match
                //Generate custom proeprties and grid here
                gridProperties.AutoGenerateColumns = false;
                //gridProperties.Columns.Add("Category", "Category");
                //gridProperties.Columns.Add("Type", "Type");
                foreach (var propertyInfo in _Properties)
                {
                    string Category = string.Empty;
                    try
                    {
                        Category = propertyInfo.GetCustomAttribute<CategoryAttribute>().Category;
                    }
                    catch { }

                    if (string.IsNullOrEmpty(Category))
                    {
                        Category = "Misc";
                    }

                    bool browsableProperty = true;
                    BrowsableAttribute browsableAttribute = propertyInfo.GetCustomAttribute<BrowsableAttribute>();
                    if (browsableAttribute != null)
                        browsableProperty = browsableAttribute.Browsable;

                    if (Category != "Common" && browsableProperty)
                    {
                        gridProperties.Rows.Add
                            (false, propertyInfo.Name, Category, propertyInfo.PropertyType.GetFriendlyName());
                    }
                }
                //gridProperties.DataSource = _Properties;

                //SORT Descending - OUTPUT FIRST
                DataGridViewColumnCollection dataGridViewColumnCollection = this.gridProperties.Columns;
                //if (dataGridViewColumnCollection != null)
                this.gridProperties.Sort(dataGridViewColumnCollection["Category"], ListSortDirection.Descending);
            }
        }

        private void gridProperties_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //clean all rows
            foreach (DataGridViewRow row in gridProperties.Rows)
            {
                row.Cells["conditionEnabledColumn"].Value = false;
            }

            //check select row
            if (gridProperties.CurrentRow != null) gridProperties.CurrentRow.Cells["conditionEnabledColumn"].Value = true;
        }

        private void cmdTestCompile_Click(object sender, EventArgs e)
        {
            try
            {
                if (_FunctionType != null)
                {
                    Type[] _GenericArgs = _FunctionType.GetGenericArguments();
                    if (_GenericArgs.Length == 2 && _GenericArgs[0] == typeof(string) && _GenericArgs[1] == typeof(bool))
                    {
                        Func<string, bool> _VariableExpression = ZappyExecutionContext.ExpandDynamicExpression<string, bool>(txtExpression.Text);
                    }
                    else if (_GenericArgs.Length == 3 && _GenericArgs[0] == typeof(ZappyExecutionContext) && _GenericArgs[1] == typeof(IZappyAction))
                    {
                        Func<ZappyExecutionContext, IZappyAction, object> _VariableExpression = ZappyExecutionContext.ExpandDynamicExpression<ZappyExecutionContext, IZappyAction, object>(txtExpression.Text);
                    }
                }
                else
                {
                    Func<ZappyExecutionContext, object> _VariableExpression = ZappyExecutionContext.ExpandDynamicExpression<ZappyExecutionContext, object>(txtExpression.Text);
                }
                MessageBox.Show("Script Evaluated Successfully !");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}