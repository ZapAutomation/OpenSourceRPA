﻿using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZappyLogger.Classes;
using ZappyLogger.ColumnizaeLib;
using ZappyLogger.Config;
using ZappyLogger.Docking;
using ZappyLogger.Entities;
using ZappyLogger.Interface;

namespace ZappyLogger.Dialogs
{
    public partial class BookmarkWindow : DockContent, ISharedToolWindow, IBookmarkView
    {
        #region Fields

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly object paintLock = new object();

        private IBookmarkData bookmarkData;
        private ILogPaintContext logPaintContext;
        private ILogView logView;

        #endregion

        #region cTor

        public BookmarkWindow()
        {
            InitializeComponent();
            this.bookmarkDataGridView.CellValueNeeded += boomarkDataGridView_CellValueNeeded;
            this.bookmarkDataGridView.CellPainting += boomarkDataGridView_CellPainting;
        }

        #endregion

        #region Properties

        public bool LineColumnVisible
        {
            set { this.bookmarkDataGridView.Columns[2].Visible = value; }
        }

        public bool ShowBookmarkCommentColumn
        {
            get { return this.commentColumnCheckBox.Checked; }
            set
            {
                this.commentColumnCheckBox.Checked = value;
                ShowCommentColumn(value);
            }
        }

        #endregion

        #region Public methods

        public void SetColumnizer(ILogLineColumnizer columnizer)
        {
            PaintHelper.SetColumnizer(columnizer, this.bookmarkDataGridView);
            if (this.bookmarkDataGridView.ColumnCount > 0)
            {
                this.bookmarkDataGridView.Columns[0].Width = 20;
            }

            DataGridViewTextBoxColumn commentColumn = new DataGridViewTextBoxColumn();
            commentColumn.HeaderText = "Bookmark Comment";
            commentColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            commentColumn.Resizable = DataGridViewTriState.NotSet;
            commentColumn.DividerWidth = 1;
            commentColumn.ReadOnly = true;
            commentColumn.Width = 250;
            commentColumn.MinimumWidth = 130;
            this.bookmarkDataGridView.Columns.Insert(1, commentColumn);
            ShowCommentColumn(this.commentColumnCheckBox.Checked);
            ResizeColumns();
        }

        /// <summary>
        /// Called from LogWindow after reloading and when double clicking a header divider.
        /// </summary>
        public void ResizeColumns()
        {
            //this.bookmarkDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            for (int i = 2; i < this.bookmarkDataGridView.ColumnCount; ++i)
            {
                this.bookmarkDataGridView.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.DisplayedCells);
            }
        }

        public void UpdateView()
        {
            this.bookmarkDataGridView.RowCount = this.bookmarkData != null ? this.bookmarkData.Bookmarks.Count : 0;
            this.ResizeColumns();
            this.bookmarkDataGridView.Refresh();
        }

        /// <summary>
        /// Called from LogWindow if the bookmark text was changed via popup window
        /// </summary>
        /// <param name="bookmark"></param>
        public void BookmarkTextChanged(Bookmark bookmark)
        {
            int rowIndex = this.bookmarkDataGridView.CurrentCellAddress.Y;
            if (rowIndex == -1)
            {
                return;
            }
            if (this.bookmarkData.Bookmarks[rowIndex] == bookmark)
            {
                this.bookmarkTextBox.Text = bookmark.Text;
            }
            this.bookmarkDataGridView.Refresh();
        }

        public void SelectBookmark(int lineNum)
        {
            if (this.bookmarkData.IsBookmarkAtLine(lineNum))
            {
                if (this.bookmarkDataGridView.Rows.Count < this.bookmarkData.Bookmarks.Count)
                {
                    // just for the case... There was an exception but I cannot find the cause
                    this.UpdateView();
                }
                int row = this.bookmarkData.GetBookmarkIndexForLine(lineNum);
                this.bookmarkDataGridView.CurrentCell = this.bookmarkDataGridView.Rows[row].Cells[0];
            }
        }

        public void SetBookmarkData(IBookmarkData bookmarkData)
        {
            this.bookmarkData = bookmarkData;
            this.bookmarkDataGridView.RowCount = this.bookmarkData != null ? this.bookmarkData.Bookmarks.Count : 0;
            HideIfNeeded();
        }

        public void PreferencesChanged(Preferences newPreferences, bool isLoadTime, SettingsFlags flags)
        {
            if ((flags & SettingsFlags.GuiOrColors) == SettingsFlags.GuiOrColors)
            {
                SetFont(newPreferences.fontName, newPreferences.fontSize);
                if (this.bookmarkDataGridView.Columns.Count > 1 && newPreferences.setLastColumnWidth)
                {
                    this.bookmarkDataGridView.Columns[this.bookmarkDataGridView.Columns.Count - 1].MinimumWidth =
                        newPreferences.lastColumnWidth;
                }
                PaintHelper.ApplyDataGridViewPrefs(this.bookmarkDataGridView, newPreferences);
            }
        }

        public void SetCurrentFile(FileViewContext ctx)
        {
            if (ctx != null)
            {
                _logger.Debug("Current file changed to {0}", ctx.LogView.FileName);
                lock (this.paintLock)
                {
                    this.logView = ctx.LogView;
                    this.logPaintContext = ctx.LogPaintContext;
                }
                this.SetColumnizer(ctx.LogView.CurrentColumnizer);
            }
            else
            {
                this.logView = null;
                this.logPaintContext = null;
            }
            UpdateView();
        }

        public void FileChanged()
        {
            // nothing to do
        }

        #endregion

        #region Overrides

        protected override string GetPersistString()
        {
            return WindowTypes.BookmarkWindow.ToString();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.splitContainer1.Visible)
            {
                Rectangle r = this.ClientRectangle;
                e.Graphics.FillRectangle(SystemBrushes.ControlLight, r);
                RectangleF rect = r;
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString("No bookmarks in current file", SystemFonts.DialogFont, SystemBrushes.WindowText,
                    r, sf);
            }
            else
            {
                base.OnPaint(e);
            }
        }

        #endregion

        #region Private Methods

        private void SetFont(string fontName, float fontSize)
        {
            Font font = new Font(new FontFamily(fontName), fontSize);
            this.bookmarkDataGridView.DefaultCellStyle.Font = font;
            this.bookmarkDataGridView.RowTemplate.Height = font.Height + 4;
            this.bookmarkDataGridView.Refresh();
        }


        private void CommentPainting(DataGridView gridView, int rowIndex, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
            {
                /**/
                Color backColor = e.CellStyle.SelectionBackColor;
                Brush brush;
                if (gridView.Focused)
                {
                    //_logger.logDebug("CellPaint Focus");
                    brush = new SolidBrush(e.CellStyle.SelectionBackColor);
                }
                else
                {
                    //_logger.logDebug("CellPaint No Focus");
                    Color color = Color.FromArgb(255, 170, 170, 170);
                    brush = new SolidBrush(color);
                }
                e.Graphics.FillRectangle(brush, e.CellBounds);
                brush.Dispose();
                /**/
            }
            else
            {
                e.CellStyle.BackColor = Color.White;
                e.PaintBackground(e.CellBounds, false);
            }
            e.PaintContent(e.CellBounds);
        }

        private void DeleteSelectedBookmarks()
        {
            List<int> lineNumList = new List<int>();
            foreach (DataGridViewRow row in this.bookmarkDataGridView.SelectedRows)
            {
                if (row.Index != -1)
                {
                    lineNumList.Add(this.bookmarkData.Bookmarks[row.Index].LineNum);
                }
            }
            if (this.logView != null)
            {
                this.logView.DeleteBookmarks(lineNumList);
            }
        }

        private static void InvalidateCurrentRow(DataGridView gridView)
        {
            if (gridView.CurrentCellAddress.Y > -1)
            {
                gridView.InvalidateRow(gridView.CurrentCellAddress.Y);
            }
        }

        private void CurrentRowChanged(int rowIndex)
        {
            if (rowIndex == -1)
            {
                // multiple selection or no selection at all
                this.bookmarkTextBox.Enabled = false;
                // disable the control first so that changes made to it won't propagate to the bookmark item
                this.bookmarkTextBox.Text = "";
            }
            else
            {
                Bookmark bookmark = this.bookmarkData.Bookmarks[rowIndex];
                this.bookmarkTextBox.Text = bookmark.Text;
                this.bookmarkTextBox.Enabled = true;
            }
        }


        private void ShowCommentColumn(bool show)
        {
            this.bookmarkDataGridView.Columns[1].Visible = show;
        }

        private void HideIfNeeded()
        {
            this.splitContainer1.Visible = this.bookmarkDataGridView.RowCount > 0;
        }

        #endregion

        #region Events handler

        private void boomarkDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (this.bookmarkData == null)
            {
                return;
            }

            lock (this.paintLock)
            {
                try
                {
                    if (e.RowIndex < 0 || e.ColumnIndex < 0 || this.bookmarkData.Bookmarks.Count <= e.RowIndex)
                    {
                        e.Handled = false;
                        return;
                    }
                    int lineNum = this.bookmarkData.Bookmarks[e.RowIndex].LineNum;

                    //if (e.ColumnIndex == 1)
                    //{
                    //  CommentPainting(this.bookmarkDataGridView, lineNum, e);
                    //}
                    //else
                    {
                        PaintHelper.CellPainting(this.logPaintContext, this.bookmarkDataGridView, lineNum, e);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }

        private void boomarkDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (this.bookmarkData == null)
            {
                return;
            }

            if (e.RowIndex < 0 || e.ColumnIndex < 0 || this.bookmarkData.Bookmarks.Count <= e.RowIndex)
            {
                e.Value = "";
                return;
            }
            Bookmark bookmarkForLine = this.bookmarkData.Bookmarks[e.RowIndex];
            int lineNum = bookmarkForLine.LineNum;
            if (e.ColumnIndex == 1)
            {
                e.Value = bookmarkForLine.Text != null
                    ? bookmarkForLine.Text.Replace('\n', ' ').Replace('\r', ' ')
                    : null;
            }
            else
            {
                int columnIndex = e.ColumnIndex > 1 ? e.ColumnIndex - 1 : e.ColumnIndex;
                e.Value = this.logPaintContext.GetCellValue(lineNum, columnIndex);
            }
        }


        private void boomarkDataGridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (this.bookmarkDataGridView.CurrentRow != null)
            //{
            //  int lineNum = this.BookmarkList.Values[this.bookmarkDataGridView.CurrentRow.Index].LineNum;
            //  this.logWindow.SelectLogLine(lineNum);
            //}
        }

        private void boomarkDataGridView_ColumnDividerDoubleClick(object sender,
            DataGridViewColumnDividerDoubleClickEventArgs e)
        {
            e.Handled = true;
            ResizeColumns();
        }

        private void bookmarkGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.bookmarkDataGridView.CurrentCellAddress.Y >= 0 &&
                    this.bookmarkDataGridView.CurrentCellAddress.Y < this.bookmarkData.Bookmarks.Count)
                {
                    int lineNum = this.bookmarkData.Bookmarks[this.bookmarkDataGridView.CurrentCellAddress.Y].LineNum;
                    this.logView.SelectLogLine(lineNum);
                }
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.None)
            {
                DeleteSelectedBookmarks();
            }
            if (e.KeyCode == Keys.Tab)
            {
                if (this.bookmarkDataGridView.Focused)
                {
                    this.bookmarkTextBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private void bookmarkGridView_Enter(object sender, EventArgs e)
        {
            InvalidateCurrentRow(this.bookmarkDataGridView);
        }

        private void bookmarkGridView_Leave(object sender, EventArgs e)
        {
            InvalidateCurrentRow(this.bookmarkDataGridView);
        }

        private void deleteBookmarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedBookmarks();
        }

        private void bookmarkTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!bookmarkTextBox.Enabled)
            {
                return; // ignore all changes done while the control is disabled
            }

            int rowIndex = this.bookmarkDataGridView.CurrentCellAddress.Y;
            if (rowIndex == -1)
            {
                return;
            }

            if (this.bookmarkData.Bookmarks.Count <= rowIndex)
            {
                return;
            }

            Bookmark bookmark = this.bookmarkData.Bookmarks[rowIndex];
            bookmark.Text = this.bookmarkTextBox.Text;
            if (this.logView != null)
            {
                this.logView.RefreshLogView();
            }
        }

        private void bookmarkDataGridView_SelectionChanged(object sender, System.EventArgs e)
        {
            if (this.bookmarkDataGridView.SelectedRows.Count != 1
                || this.bookmarkDataGridView.SelectedRows[0].Index >= this.bookmarkData.Bookmarks.Count)
            {
                CurrentRowChanged(-1);
            }
            else
            {
                CurrentRowChanged(this.bookmarkDataGridView.SelectedRows[0].Index);
            }
        }

        private void bookmarkDataGridView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }

        private void bookmarkDataGridView_CellToolTipTextNeeded(object sender,
            DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex <= -1 || e.RowIndex >= this.bookmarkData.Bookmarks.Count)
            {
                return;
            }
            Bookmark bookmark = this.bookmarkData.Bookmarks[e.RowIndex];
            if (!string.IsNullOrEmpty(bookmark.Text))
            {
                e.ToolTipText = bookmark.Text;
                return;
            }
        }

        private void bookmarkDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Toggle bookmark when double-clicking on the first column
            if (e.ColumnIndex == 0 && e.RowIndex >= 0 && this.bookmarkDataGridView.CurrentRow != null)
            {
                int index = this.bookmarkDataGridView.CurrentRow.Index;
                int lineNum = this.bookmarkData.Bookmarks[this.bookmarkDataGridView.CurrentRow.Index].LineNum;
                this.bookmarkData.ToggleBookmark(lineNum);
                // we don't ask for confirmation if the bookmark has an associated comment...

                int boomarkCount = this.bookmarkData.Bookmarks.Count;
                this.bookmarkDataGridView.RowCount = boomarkCount;

                if (index < boomarkCount)
                {
                    this.bookmarkDataGridView.CurrentCell = this.bookmarkDataGridView.Rows[index].Cells[0];
                }
                else
                {
                    if (boomarkCount > 0)
                    {
                        this.bookmarkDataGridView.CurrentCell =
                            this.bookmarkDataGridView.Rows[boomarkCount - 1].Cells[0];
                    }
                }
                if (boomarkCount > index)
                {
                    CurrentRowChanged(index);
                }
                else
                {
                    if (boomarkCount > 0)
                    {
                        CurrentRowChanged(this.bookmarkDataGridView.RowCount - 1);
                    }
                    else
                    {
                        CurrentRowChanged(-1);
                    }
                }
                return;
            }

            if (this.bookmarkDataGridView.CurrentRow != null && e.RowIndex >= 0)
            {
                int lineNum = this.bookmarkData.Bookmarks[this.bookmarkDataGridView.CurrentRow.Index].LineNum;
                this.logView.SelectAndEnsureVisible(lineNum, true);
            }
        }

        private void removeCommentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show("Really remove bookmark comments for selected lines?", "ZappyLogger",
                    MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                List<int> lineNumList = new List<int>();
                foreach (DataGridViewRow row in this.bookmarkDataGridView.SelectedRows)
                {
                    if (row.Index != -1)
                    {
                        this.bookmarkData.Bookmarks[row.Index].Text = "";
                    }
                }

                this.bookmarkTextBox.Text = "";
                this.bookmarkDataGridView.Refresh();
                this.logView.RefreshLogView();
            }
        }

        private void commentColumnCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ShowCommentColumn(this.commentColumnCheckBox.Checked);
        }

        private void BookmarkWindow_ClientSizeChanged(object sender, EventArgs e)
        {
            if (Width > 0 && Height > 0)
            {
                if (Width > Height)
                {
                    this.splitContainer1.Orientation = Orientation.Vertical;
                    int distance = Width - 200;
                    this.splitContainer1.SplitterDistance = distance > this.splitContainer1.Panel1MinSize
                        ? distance
                        : this.splitContainer1.Panel1MinSize;
                }
                else
                {
                    this.splitContainer1.Orientation = Orientation.Horizontal;
                    int distance = Height - 200;
                    this.splitContainer1.SplitterDistance = distance > this.splitContainer1.Panel1MinSize
                        ? distance
                        : this.splitContainer1.Panel1MinSize;
                }
            }
            if (!this.splitContainer1.Visible)
            {
                // redraw the "no bookmarks" display
                Invalidate();
            }
        }

        private void bookmarkDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            HideIfNeeded();
        }

        private void bookmarkDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            HideIfNeeded();
        }

        private void BookmarkWindow_SizeChanged(object sender, EventArgs e)
        {
            //if (!this.splitContainer1.Visible)
            //{
            //  // redraw the "no bookmarks" display
            //  Invalidate();
            //} 
        }

        #endregion
    }
}