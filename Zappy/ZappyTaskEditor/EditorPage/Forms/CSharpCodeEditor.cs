using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Zappy.ZappyTaskEditor.EditorPage.Forms
{


    public class CSharpCodeEditor : UserControl
    {
        Dictionary<Keys, Action> _FilterKeys;
        public CSharpCodeEditor()
        {
            InitializeComponent();
            _FilterKeys = new Dictionary<Keys, Action>();
        }
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSharpCodeEditor));
            this.PanelSearch = new System.Windows.Forms.Panel();
            this.BtnNextSearch = new System.Windows.Forms.Button();
            this.BtnPrevSearch = new System.Windows.Forms.Button();
            this.BtnCloseSearch = new System.Windows.Forms.Button();
            this.TxtSearch = new System.Windows.Forms.TextBox();
            this._CodeArea = new ScintillaNET.Scintilla();
            this.PanelSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelSearch
            // 
            this.PanelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.PanelSearch.BackColor = Color.White;
            this.PanelSearch.BorderStyle = BorderStyle.FixedSingle;
            this.PanelSearch.Controls.Add(this.BtnNextSearch);
            this.PanelSearch.Controls.Add(this.BtnPrevSearch);
            this.PanelSearch.Controls.Add(this.BtnCloseSearch);
            this.PanelSearch.Controls.Add(this.TxtSearch);
            this.PanelSearch.Location = new System.Drawing.Point(406, 3);
            this.PanelSearch.Name = "PanelSearch";
            this.PanelSearch.Size = new System.Drawing.Size(292, 40);
            this.PanelSearch.TabIndex = 10;
            this.PanelSearch.Visible = false;
            // 
            // BtnNextSearch
            // 
            this.BtnNextSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.BtnNextSearch.FlatStyle = FlatStyle.Flat;
            this.BtnNextSearch.ForeColor = Color.White;
            this.BtnNextSearch.Image = ((System.Drawing.Image)(resources.GetObject("BtnNextSearch.Image")));
            this.BtnNextSearch.Location = new System.Drawing.Point(233, 4);
            this.BtnNextSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnNextSearch.Name = "BtnNextSearch";
            this.BtnNextSearch.Size = new System.Drawing.Size(25, 30);
            this.BtnNextSearch.TabIndex = 9;
            this.BtnNextSearch.Tag = "Find next (Enter)";
            this.BtnNextSearch.UseVisualStyleBackColor = true;
            this.BtnNextSearch.Click += new System.EventHandler(this.BtnNextSearch_Click);
            // 
            // BtnPrevSearch
            // 
            this.BtnPrevSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.BtnPrevSearch.FlatStyle = FlatStyle.Flat;
            this.BtnPrevSearch.ForeColor = Color.White;
            this.BtnPrevSearch.Image = ((System.Drawing.Image)(resources.GetObject("BtnPrevSearch.Image")));
            this.BtnPrevSearch.Location = new System.Drawing.Point(205, 4);
            this.BtnPrevSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnPrevSearch.Name = "BtnPrevSearch";
            this.BtnPrevSearch.Size = new System.Drawing.Size(25, 30);
            this.BtnPrevSearch.TabIndex = 8;
            this.BtnPrevSearch.Tag = "Find previous (Shift+Enter)";
            this.BtnPrevSearch.UseVisualStyleBackColor = true;
            this.BtnPrevSearch.Click += new System.EventHandler(this.BtnPrevSearch_Click);
            // 
            // BtnCloseSearch
            // 
            this.BtnCloseSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            this.BtnCloseSearch.FlatStyle = FlatStyle.Flat;
            this.BtnCloseSearch.ForeColor = Color.White;
            this.BtnCloseSearch.Image = ((System.Drawing.Image)(resources.GetObject("BtnCloseSearch.Image")));
            this.BtnCloseSearch.Location = new System.Drawing.Point(261, 4);
            this.BtnCloseSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnCloseSearch.Name = "BtnCloseSearch";
            this.BtnCloseSearch.Size = new System.Drawing.Size(25, 30);
            this.BtnCloseSearch.TabIndex = 7;
            this.BtnCloseSearch.Tag = "Close (Esc)";
            this.BtnCloseSearch.UseVisualStyleBackColor = true;
            this.BtnCloseSearch.Click += new System.EventHandler(this.BtnClearSearch_Click);
            // 
            // TxtSearch
            // 
            this.TxtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            this.TxtSearch.BorderStyle = BorderStyle.None;
            this.TxtSearch.Font = new System.Drawing.Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.TxtSearch.Location = new System.Drawing.Point(10, 6);
            this.TxtSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtSearch.Name = "TxtSearch";
            this.TxtSearch.Size = new System.Drawing.Size(189, 25);
            this.TxtSearch.TabIndex = 6;
            this.TxtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            this.TxtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtSearch_KeyDown);
            // 
            // TextArea
            // 
            this._CodeArea.CaretForeColor = Color.White;
            this._CodeArea.Dock = DockStyle.Fill;
            this._CodeArea.IndentationGuides = IndentView.LookBoth;
            this._CodeArea.Location = new System.Drawing.Point(0, 0);
            this._CodeArea.Name = "TextArea";
            this._CodeArea.Size = new System.Drawing.Size(701, 448);
            this._CodeArea.TabIndex = 11;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.PanelSearch);
            this.Controls.Add(this._CodeArea);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(701, 448);
            this.Load += new System.EventHandler(this.UserControl1_Load);
            this.PanelSearch.ResumeLayout(false);
            this.PanelSearch.PerformLayout();
            this.ResumeLayout(false);

        }


        private System.Windows.Forms.Panel PanelSearch;
        private System.Windows.Forms.Button BtnNextSearch;
        private System.Windows.Forms.Button BtnPrevSearch;
        private System.Windows.Forms.Button BtnCloseSearch;
        private System.Windows.Forms.TextBox TxtSearch;
        Scintilla _CodeArea;

        #endregion

        private void UserControl1_Load(object sender, EventArgs e)
        {
            // STYLING
            InitColors();

            InitSyntaxColoring();

            // NUMBER MARGIN
            InitNumberMargin();

            // BOOKMARK MARGIN
            InitBookmarkMargin();

            // CODE FOLDING MARGIN
            InitCodeFolding();

            // DRAG DROP
            InitDragDropFile();

            // DEFAULT FILE
            //LoadDataFromFile("../../MainForm.cs");

            // INIT HOTKEYS
            InitHotkeys();
        }

        private void InitColors()
        {
            _CodeArea.SetSelectionBackColor(true, IntToColor(0x114D9C));
        }

        private void InitHotkeys()
        {
            _CodeArea.ClearCmdKey(Keys.Control | Keys.F);
            _CodeArea.ClearCmdKey(Keys.Control | Keys.R);
            _CodeArea.ClearCmdKey(Keys.Control | Keys.H);
            _CodeArea.ClearCmdKey(Keys.Control | Keys.L);
            _CodeArea.ClearCmdKey(Keys.Control | Keys.U);

            AddHotKey(OpenSearch, Keys.F, true);
            AddHotKey(OpenFindDialog, Keys.F, true, false, true);
            AddHotKey(OpenReplaceDialog, Keys.R, true);
            AddHotKey(OpenReplaceDialog, Keys.H, true);
            AddHotKey(Uppercase, Keys.U, true);
            AddHotKey(Lowercase, Keys.L, true);
            AddHotKey(ZoomIn, Keys.Oemplus, true);
            AddHotKey(ZoomOut, Keys.OemMinus, true);
            AddHotKey(ZoomDefault, Keys.D0, true);
            AddHotKey(CloseSearch, Keys.Escape);
        }

        public void AddHotKey(Action function, Keys key, bool ctrl = false, bool shift = false, bool alt = false)
        {
            if (ctrl)
                key |= Keys.Control;
            if (alt)
                key |= Keys.Alt;
            if (shift)
                key |= Keys.Shift;

            _FilterKeys[key] = function;
        }

        public void HandleHostKeyDownEvent(bool Attach)
        {
            if (Attach)
                ParentForm.KeyDown += CheckForKeys;
            else
                ParentForm.KeyDown -= CheckForKeys;
        }

        public override string Text { get { return _CodeArea.Text; } set { _CodeArea.Text = value; } }
        bool _HandleKeys = true;
        public void CheckForKeys(object sender, KeyEventArgs e)
        {
            if (!_HandleKeys)
                return;
            Action _Handler = null;
            if (_FilterKeys.TryGetValue(e.KeyData, out _Handler))
                _Handler();
        }


        private void InitSyntaxColoring()
        {

            // Configure the default style
            _CodeArea.StyleResetDefault();
            _CodeArea.Styles[Style.Default].Font = "Consolas";
            _CodeArea.Styles[Style.Default].Size = 10;
            _CodeArea.Styles[Style.Default].BackColor = IntToColor(0x212121);
            _CodeArea.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
            _CodeArea.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            _CodeArea.Styles[Style.Cpp.Identifier].ForeColor = IntToColor(0xD0DAE2);
            _CodeArea.Styles[Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
            _CodeArea.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
            _CodeArea.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
            _CodeArea.Styles[Style.Cpp.Number].ForeColor = IntToColor(0xFFFF00);
            _CodeArea.Styles[Style.Cpp.String].ForeColor = IntToColor(0xFFFF00);
            _CodeArea.Styles[Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
            _CodeArea.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            _CodeArea.Styles[Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
            _CodeArea.Styles[Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
            _CodeArea.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            _CodeArea.Styles[Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
            _CodeArea.Styles[Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
            _CodeArea.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
            _CodeArea.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
            _CodeArea.Styles[Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);

            _CodeArea.Lexer = Lexer.Cpp;

            _CodeArea.SetKeywords(0, "class extends implements import interface new case do while else if for in switch throw get set function var try catch finally while with default break continue delete return each const namespace package include use is as instanceof typeof author copy default deprecated eventType example exampleText exception haxe inheritDoc internal link mtasc mxmlc param private return see serial serialData serialField since throws usage version langversion playerversion productversion dynamic private public partial static intrinsic internal native override protected AS3 final super this arguments null Infinity NaN undefined true false abstract as base bool break by byte case catch char checked class const continue decimal default delegate do double descending explicit event extern else enum false finally fixed float for foreach from goto group if implicit in int interface internal into is lock long new null namespace object operator out override orderby params private protected public readonly ref return switch struct sbyte sealed short sizeof stackalloc static string select this throw true try typeof uint ulong unchecked unsafe ushort using var virtual volatile void while where yield");
            _CodeArea.SetKeywords(1, "void Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path File System Windows Forms ScintillaNET");

        }

        #region Numbers, Bookmarks, Code Folding

        /// <summary>
        /// the background color of the text area
        /// </summary>
        private const int BACK_COLOR = 0x2A211C;

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private const int FORE_COLOR = 0xB7B7B7;

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;
        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = true;

        private void InitNumberMargin()
        {

            _CodeArea.Styles[Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            _CodeArea.Styles[Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
            _CodeArea.Styles[Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            _CodeArea.Styles[Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

            var nums = _CodeArea.Margins[NUMBER_MARGIN];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            _CodeArea.MarginClick += TextArea_MarginClick;
        }

        private void InitBookmarkMargin()
        {

            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = _CodeArea.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = _CodeArea.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(IntToColor(0xFF003B));
            marker.SetForeColor(IntToColor(0x000000));
            marker.SetAlpha(100);

        }

        private void InitCodeFolding()
        {

            _CodeArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));
            _CodeArea.SetFoldMarginHighlightColor(true, IntToColor(BACK_COLOR));

            // Enable code folding
            _CodeArea.SetProperty("fold", "1");
            _CodeArea.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            _CodeArea.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            _CodeArea.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            _CodeArea.Margins[FOLDING_MARGIN].Sensitive = true;
            _CodeArea.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                _CodeArea.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // styles for [+] and [-]
                _CodeArea.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            _CodeArea.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            _CodeArea.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            _CodeArea.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            _CodeArea.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            _CodeArea.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            _CodeArea.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            _CodeArea.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            _CodeArea.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

        }

        private void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == BOOKMARK_MARGIN)
            {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                var line = _CodeArea.Lines[_CodeArea.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                }
                else
                {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        #endregion

        #region Drag & Drop File

        public void InitDragDropFile()
        {

            _CodeArea.AllowDrop = true;
            _CodeArea.DragEnter += delegate (object sender, DragEventArgs e)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            };
            _CodeArea.DragDrop += delegate (object sender, DragEventArgs e)
            {

                // get file drop
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {

                    Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
                    if (a != null)
                    {

                        string path = a.GetValue(0).ToString();

                        LoadDataFromFile(path);

                    }
                }
            };

        }

        private void LoadDataFromFile(string path)
        {
            if (File.Exists(path))
            {
                //FileName.Text = Path.GetFileName(path);
                _CodeArea.Text = File.ReadAllText(path);
            }
        }

        #endregion

        #region Main Menu Commands

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSearch();
        }

        private void findDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFindDialog();
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenReplaceDialog();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _CodeArea.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _CodeArea.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _CodeArea.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _CodeArea.SelectAll();
        }

        private void selectLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Line line = _CodeArea.Lines[_CodeArea.CurrentLine];
            _CodeArea.SetSelection(line.Position + line.Length, line.Position);
        }

        private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _CodeArea.SetEmptySelection(0);
        }

        private void indentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Indent();
        }

        private void outdentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Outdent();
        }

        private void uppercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uppercase();
        }

        private void lowercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lowercase();
        }


        void ToggleWordWrap()
        {
            _CodeArea.WrapMode = _CodeArea.WrapMode == WrapMode.Word ? WrapMode.None : WrapMode.Word;
        }
        void ToggleIndentationGuides()
        {
            _CodeArea.IndentationGuides = _CodeArea.IndentationGuides == IndentView.LookBoth ? IndentView.None : IndentView.LookBoth;
        }

        void ToggleViewWhitespace()
        {
            _CodeArea.ViewWhitespace = _CodeArea.ViewWhitespace == WhitespaceMode.Invisible ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
        }

        void CollapseAll()
        { _CodeArea.FoldAll(FoldAction.Contract); }

        void ExpandAll()
        {
            _CodeArea.FoldAll(FoldAction.Expand);

        }

        #endregion

        #region Uppercase / Lowercase

        private void Lowercase()
        {

            // save the selection
            int start = _CodeArea.SelectionStart;
            int end = _CodeArea.SelectionEnd;

            // modify the selected text
            _CodeArea.ReplaceSelection(_CodeArea.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            _CodeArea.SetSelection(start, end);
        }

        private void Uppercase()
        {

            // save the selection
            int start = _CodeArea.SelectionStart;
            int end = _CodeArea.SelectionEnd;

            // modify the selected text
            _CodeArea.ReplaceSelection(_CodeArea.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            _CodeArea.SetSelection(start, end);
        }

        #endregion

        #region Indent / Outdent

        private void Indent()
        {
            // we use this hack to send "Shift+Tab" to scintilla, since there is no known API to indent,
            // although the indentation function exists. Pressing TAB with the editor focused confirms this.
            GenerateKeystrokes("{TAB}");
        }

        private void Outdent()
        {
            // we use this hack to send "Shift+Tab" to scintilla, since there is no known API to outdent,
            // although the indentation function exists. Pressing Shift+Tab with the editor focused confirms this.
            GenerateKeystrokes("+{TAB}");
        }

        private void GenerateKeystrokes(string keys)
        {
            _HandleKeys = false;

            _CodeArea.Focus();
            SendKeys.Send(keys);

            _HandleKeys = true;
        }

        #endregion

        #region Zoom

        private void ZoomIn()
        {
            _CodeArea.ZoomIn();
        }

        private void ZoomOut()
        {
            _CodeArea.ZoomOut();
        }

        private void ZoomDefault()
        {
            _CodeArea.Zoom = 0;
        }


        #endregion

        #region Quick Search Bar

        bool SearchIsOpen = false;

        private void OpenSearch()
        {

            SearchManager.SearchBox = TxtSearch;
            SearchManager.TextArea = _CodeArea;

            if (!SearchIsOpen)
            {
                SearchIsOpen = true;

                InvokeIfNeeded(delegate ()
                {
                    PanelSearch.Visible = true;
                    TxtSearch.Text = SearchManager.LastSearch;
                    TxtSearch.Focus();
                    TxtSearch.SelectAll();
                });
            }
            else
            {
                InvokeIfNeeded(delegate ()
                {
                    TxtSearch.Focus();
                    TxtSearch.SelectAll();
                });
            }
        }
        private void CloseSearch()
        {
            if (SearchIsOpen)
            {
                SearchIsOpen = false;
                InvokeIfNeeded(delegate ()
                {
                    PanelSearch.Visible = false;
                    //CurBrowser.GetBrowser().StopFinding(true);
                });
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            CloseSearch();
        }

        private void BtnPrevSearch_Click(object sender, EventArgs e)
        {
            SearchManager.Find(false, false);
        }
        private void BtnNextSearch_Click(object sender, EventArgs e)
        {
            SearchManager.Find(true, false);
        }
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchManager.Find(true, true);
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsHotkey(e, Keys.Enter))
            {
                SearchManager.Find(true, false);
            }
            if (IsHotkey(e, Keys.Enter, true) || IsHotkey(e, Keys.Enter, false, true))
            {
                SearchManager.Find(false, false);
            }
        }

        public bool IsHotkey(KeyEventArgs eventData, Keys key, bool ctrl = false, bool shift = false, bool alt = false)
        {
            return eventData.KeyCode == key && eventData.Control == ctrl && eventData.Shift == shift && eventData.Alt == alt;
        }

        #endregion

        #region Find & Replace Dialog

        private void OpenFindDialog()
        {

        }
        private void OpenReplaceDialog()
        {


        }

        #endregion

        #region Utils

        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        public void InvokeIfNeeded(Action action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }




        #endregion


    }
}
