using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.Invoker;
using Zappy.ZappyActions.ElementPicker.Input;
using Zappy.ZappyTaskEditor.ExecutionHelpers;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyTaskEditor.EditorPage.ElementPicker
{
    public sealed class ElementPickerForm : Form
    {
        private Button pickButton;
        private Button okButton;
        private Button cancelButton;
        private DataGridView conditionGrid;
        private Button clearButton;
        private Button testButton;
        private SplitContainer splitContainer1;
        private DataGridViewCheckBoxColumn conditionEnabledColumn;
        private LabelColumn conditionNameColumn;
        private DataGridViewTextBoxColumn conditionValueColumn;
        private Label label1;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ElementPickerForm));
            this.pickButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.conditionGrid = new System.Windows.Forms.DataGridView();
            this.conditionEnabledColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.conditionNameColumn = new Zappy.ZappyTaskEditor.ExecutionHelpers.LabelColumn();
            this.conditionValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.okButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.testButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.conditionGrid)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pickButton
            // 
            resources.ApplyResources(this.pickButton, "pickButton");
            this.pickButton.Name = "pickButton";
            this.pickButton.TabStop = false;
            this.pickButton.UseVisualStyleBackColor = true;
            this.pickButton.Click += new System.EventHandler(this.PickButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabStop = false;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // conditionGrid
            // 
            resources.ApplyResources(this.conditionGrid, "conditionGrid");
            this.conditionGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.conditionGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.conditionGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(230)))));
            this.conditionGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.conditionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.conditionGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.conditionEnabledColumn,
            this.conditionNameColumn,
            this.conditionValueColumn});
            this.conditionGrid.EnableHeadersVisualStyles = false;
            this.conditionGrid.Name = "conditionGrid";
            this.conditionGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.conditionGrid.TabStop = false;
            // 
            // conditionEnabledColumn
            // 
            this.conditionEnabledColumn.DataPropertyName = "Use";
            this.conditionEnabledColumn.FillWeight = 10F;
            resources.ApplyResources(this.conditionEnabledColumn, "conditionEnabledColumn");
            this.conditionEnabledColumn.Name = "conditionEnabledColumn";
            this.conditionEnabledColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.conditionEnabledColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // conditionNameColumn
            // 
            this.conditionNameColumn.DataPropertyName = "Name";
            this.conditionNameColumn.FillWeight = 30F;
            resources.ApplyResources(this.conditionNameColumn, "conditionNameColumn");
            this.conditionNameColumn.Name = "conditionNameColumn";
            this.conditionNameColumn.ReadOnly = true;
            this.conditionNameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.conditionNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // conditionValueColumn
            // 
            this.conditionValueColumn.DataPropertyName = "Value";
            this.conditionValueColumn.FillWeight = 60F;
            resources.ApplyResources(this.conditionValueColumn, "conditionValueColumn");
            this.conditionValueColumn.Name = "conditionValueColumn";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.TabStop = false;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Controls.Add(this.testButton);
            this.panel1.Controls.Add(this.clearButton);
            this.panel1.Controls.Add(this.pickButton);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Name = "panel1";
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.Controls.Add(this.conditionGrid);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Yellow;
            this.label1.Name = "label1";
            // 
            // testButton
            // 
            resources.ApplyResources(this.testButton, "testButton");
            this.testButton.Name = "testButton";
            this.testButton.TabStop = false;
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // clearButton
            // 
            resources.ApplyResources(this.clearButton, "clearButton");
            this.clearButton.Name = "clearButton";
            this.clearButton.TabStop = false;
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // ElementPickerForm
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ElementPickerForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ElementPickerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.conditionGrid)).EndInit();
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public ElementPickerForm(object gridCellValue)
        {
            this.InitializeComponent();
            this.InitializeVariables();
            //
            if (gridCellValue is string xml && xml.Length > 0
                && new ElementQuery(xml) is ElementQuery query)
            {
                this.Query = query;
            }
            else
            {
                this.Query = new ElementQuery();
            }
            this.conditionGrid.AutoGenerateColumns = false;
            this.conditionGrid.DataSource = this.Query.GetValue();
        }

        private void PickButton_Click(object sender, EventArgs e)
        {
            this.splitContainer1.Panel2Collapsed = false;
            this.pickButton.Enabled = false;
            this.testButton.Enabled = false;
            this.clearButton.Enabled = false;
            this.cancelButton.Enabled = false;
            this.okButton.Enabled = false;
            this.Pick();
        }

        public ElementQuery Query { get; private set; }

        private ElementHighlighterForm highlighter;

        private EditorPageInputDriver inputDriver;

        private InputEventArgs inputEvent;

        private Panel panel1;

        private CancellationTokenSource cts;

        private void InitializeVariables()
        {
            this.inputEvent = null;
            this.inputDriver = new EditorPageInputDriver();
            this.highlighter = new ElementHighlighterForm();
            this.splitContainer1.Panel2Collapsed = true;
            this.cts = new CancellationTokenSource();
            this.Query = new ElementQuery();
            this.FormClosing += (sender, e) =>
            {
                this.cts.Cancel();
                this.inputDriver.Dispose();
                this.highlighter.Close();
            };
            inputDriver.OnMouseMove += Input_OnMouseMove;
            inputDriver.OnKeyUp += Input_OnKeyUp;
        }

        private void Pick()
        {
            this.cts.Cancel();
            this.cts = new CancellationTokenSource();
            this.InspectAsync(cts.Token);
        }

        private void InspectAsync(CancellationToken token)
        {
            Task.Run(() =>
            {
                var e = this.inputEvent;
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                        if (e == this.inputEvent)
                        {
                            continue;
                        }
                        e = this.inputEvent;
                        var x = WinContext.Shared.GetElementFromPoint(e.X, e.Y);
                        var q = x.GetQuery();
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                        this.Query = q;
                        this.conditionGrid.Invoke(new Action(() =>
                        {
                            this.conditionGrid.DataSource = q.GetValue();
                        }));
                        this.highlighter.Invoke(new Action(() =>
                        {
                            this.highlighter.Render(x.Bounds, Color.Red);
                        }));
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.FormBorderStyle = FormBorderStyle.None;
                            this.WindowState = FormWindowState.Normal;
                            this.FormBorderStyle = FormBorderStyle.FixedSingle;
                        }
                    }
                    catch (ElementNotAvailableException)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                this.Invoke(new Action(() =>
                {
                    this.splitContainer1.Panel2Collapsed = true;
                    this.pickButton.Enabled = true;
                    this.testButton.Enabled = true;
                    this.clearButton.Enabled = true;
                    this.cancelButton.Enabled = true;
                    this.okButton.Enabled = true;
                }));
            });
        }

        private void Input_OnKeyUp(InputEventArgs e)
        {
            if (e.Key == KeyboardKey.LeftCtrl || e.Key == KeyboardKey.RightCtrl)
            {
                this.cts.Cancel();
            }
            else
            {
                this.inputEvent = e;
            }
        }

        private void Input_OnMouseMove(InputEventArgs e)
        {
            this.inputEvent = e;
        }

        private void ElementPickerForm_Load(object sender, EventArgs e)
        {
            if (ApplicationSettingProperties.Instance.ZappyUILanguage != LanguageZappy.general)
            {
                ChangeLanguage(ApplicationSettingProperties.Instance.ZappyUILanguage);
            }

            if (this.Query.Count == 0)
            {
                this.pickButton.PerformClick();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            this.Query = new ElementQuery();
            this.conditionGrid.DataSource = this.Query.GetValue();
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            var elements = WinContext.Shared.GetElementsFromQuery(this.Query);
            if (elements.Count() == 1)
            {
                this.highlighter.Render(elements.First().Bounds, Color.Yellow);
            }
            MessageBox.Show(
                string.Format("The query matched {0} element{1}.",
                elements.Count(),
                elements.Count() == 1 ? string.Empty : "s"));
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
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
                    if (c is Panel)
                    {
                        foreach (Control c1 in c.Controls)
                        {
                            resources = new ComponentResourceManager(typeof(ElementPickerForm));
                            resources.ApplyResources(c1, c1.Name, new CultureInfo(lang));
                        }

                    }
                }

                //foreach (Control c1 in this.Controls)
                //{
                //    if (c1 is Panel)
                //    {
                //        if (c1 is Label)
                //        {
                //            foreach (Control c2 in c1.Controls)
                //            {
                //                resources = new ComponentResourceManager(typeof(ElementPickerForm));
                //                resources.ApplyResources(c2, c.Name, new CultureInfo(lang));
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

    }
}