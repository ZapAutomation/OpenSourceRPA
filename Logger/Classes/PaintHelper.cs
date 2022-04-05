﻿using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZappyLogger.Classes.Highlight;
using ZappyLogger.ColumnizaeLib;
using ZappyLogger.Config;
using ZappyLogger.Dialogs;
using ZappyLogger.Entities;
using ZappyLogger.Interface;

namespace ZappyLogger.Classes
{
    internal class PaintHelper
    {
        #region Fields

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private Color bookmarkColor = Color.FromArgb(165, 200, 225);

        #endregion

        #region Properties

        private static Preferences Preferences
        {
            get { return ConfigManager.Settings.preferences; }
        }

        #endregion

        #region Public methods

        public static void CellPainting(ILogPaintContext logPaintCtx, DataGridView gridView, int rowIndex,
            DataGridViewCellPaintingEventArgs e)
        {
            if (rowIndex < 0 || e.ColumnIndex < 0)
            {
                e.Handled = false;
                return;
            }
            ILogLine line = logPaintCtx.GetLogLine(rowIndex);
            if (line != null)
            {
                HilightEntry entry = logPaintCtx.FindHilightEntry(line, true);
                e.Graphics.SetClip(e.CellBounds);
                if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                {
                    Color backColor = e.CellStyle.SelectionBackColor;
                    Brush brush;
                    if (gridView.Focused)
                    {
                        brush = new SolidBrush(e.CellStyle.SelectionBackColor);
                    }
                    else
                    {
                        Color color = Color.FromArgb(255, 170, 170, 170);
                        brush = new SolidBrush(color);
                    }
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                    brush.Dispose();
                }
                else
                {
                    Color bgColor = Color.White;
                    if (!DebugOptions.disableWordHighlight)
                    {
                        if (entry != null)
                        {
                            bgColor = entry.BackgroundColor;
                        }
                    }
                    else
                    {
                        if (entry != null)
                        {
                            bgColor = entry.BackgroundColor;
                        }
                    }
                    e.CellStyle.BackColor = bgColor;
                    e.PaintBackground(e.ClipBounds, false);
                }

                if (DebugOptions.disableWordHighlight)
                {
                    e.PaintContent(e.CellBounds);
                }
                else
                {
                    PaintCell(logPaintCtx, e, gridView, false, entry);
                }

                if (e.ColumnIndex == 0)
                {
                    Entities.Bookmark bookmark = logPaintCtx.GetBookmarkForLine(rowIndex);
                    if (bookmark != null)
                    {
                        Rectangle r; // = new Rectangle(e.CellBounds.Left + 2, e.CellBounds.Top + 2, 6, 6);
                        r = e.CellBounds;
                        r.Inflate(-2, -2);
                        Brush brush = new SolidBrush(logPaintCtx.BookmarkColor);
                        e.Graphics.FillRectangle(brush, r);
                        brush.Dispose();
                        if (bookmark.Text.Length > 0)
                        {
                            StringFormat format = new StringFormat();
                            format.LineAlignment = StringAlignment.Center;
                            format.Alignment = StringAlignment.Center;
                            Brush brush2 = new SolidBrush(Color.FromArgb(255, 190, 100, 0));
                            Font font = logPaintCtx.MonospacedFont;
                            e.Graphics.DrawString("i", font, brush2, new RectangleF(r.Left, r.Top, r.Width, r.Height),
                                format);
                            brush2.Dispose();
                        }
                    }
                }

                e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
                e.Handled = true;
            }
        }


        public static void SetColumnizer(ILogLineColumnizer columnizer, DataGridView gridView)
        {
            int rowCount = gridView.RowCount;
            int currLine = gridView.CurrentCellAddress.Y;
            int currFirstLine = gridView.FirstDisplayedScrollingRowIndex;

            try
            {
                gridView.Columns.Clear();
            }
            catch (ArgumentOutOfRangeException ae)
            {
                // Occures sometimes on empty gridViews (no lines) if bookmark window was closed and re-opened in floating mode. 
                // Don't know why.
                _logger.Error(ae);
            }

            DataGridViewTextBoxColumn markerColumn = new DataGridViewTextBoxColumn();
            markerColumn.HeaderText = "";
            markerColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            markerColumn.Resizable = DataGridViewTriState.False;
            markerColumn.DividerWidth = 1;
            markerColumn.ReadOnly = true;
            // markerColumn.HeaderCell.ContextMenuStrip = this.columnContextMenuStrip;
            gridView.Columns.Add(markerColumn);

            DataGridViewTextBoxColumn lineNumberColumn = new DataGridViewTextBoxColumn();
            lineNumberColumn.HeaderText = "Line";
            lineNumberColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            lineNumberColumn.Resizable = DataGridViewTriState.NotSet;
            lineNumberColumn.DividerWidth = 1;
            lineNumberColumn.ReadOnly = true;
            // lineNumberColumn.HeaderCell.ContextMenuStrip = this.columnContextMenuStrip;
            gridView.Columns.Add(lineNumberColumn);

            foreach (string colName in columnizer.GetColumnNames())
            {
                DataGridViewColumn titleColumn = new LogTextColumn();
                titleColumn.HeaderText = colName;
                titleColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                titleColumn.Resizable = DataGridViewTriState.NotSet;
                titleColumn.DividerWidth = 1;
                //titleColumn.HeaderCell.ContextMenuStrip = this.columnContextMenuStrip;
                gridView.Columns.Add(titleColumn);
            }

            gridView.RowCount = rowCount;
            if (currLine != -1)
            {
                gridView.CurrentCell = gridView.Rows[currLine].Cells[0];
            }
            if (currFirstLine != -1)
            {
                gridView.FirstDisplayedScrollingRowIndex = currFirstLine;
            }
            //gridView.Refresh();
            //AutoResizeColumns(gridView);
        }

        public static void AutoResizeColumns(DataGridView gridView)
        {
            try
            {
                gridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                if (gridView.Columns.Count > 1 && Preferences.setLastColumnWidth &&
                    gridView.Columns[gridView.Columns.Count - 1].Width < Preferences.lastColumnWidth
                )
                {
                    // It seems that using 'MinimumWidth' instead of 'Width' prevents the DataGridView's NullReferenceExceptions
                    //gridView.Columns[gridView.Columns.Count - 1].Width = this.Preferences.lastColumnWidth;
                    gridView.Columns[gridView.Columns.Count - 1].MinimumWidth = Preferences.lastColumnWidth;
                }
            }
            catch (NullReferenceException e)
            {
                // See https://connect.microsoft.com/VisualStudio/feedback/details/366943/autoresizecolumns-in-datagridview-throws-nullreferenceexception
                // There are some rare situations with null ref exceptions when resizing columns and on filter finished
                // So catch them here. Better than crashing.
                _logger.Error(e, "Error while resizing columns: ");
            }
        }

        public static void ApplyDataGridViewPrefs(DataGridView dataGridView, Preferences prefs)
        {
            if (dataGridView.Columns.Count > 1)
            {
                if (prefs.setLastColumnWidth)
                {
                    dataGridView.Columns[dataGridView.Columns.Count - 1].MinimumWidth = prefs.lastColumnWidth;
                }
                else
                {
                    // Workaround for a .NET bug which brings the DataGridView into an unstable state (causing lots of NullReferenceExceptions). 
                    dataGridView.FirstDisplayedScrollingColumnIndex = 0;

                    dataGridView.Columns[dataGridView.Columns.Count - 1].MinimumWidth = 5; // default
                }
            }
            if (dataGridView.RowCount > 0)
            {
                dataGridView.UpdateRowHeightInfo(0, true);
            }
            dataGridView.Invalidate();
            dataGridView.Refresh();
            AutoResizeColumns(dataGridView);
        }

        public static Rectangle BorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            Rectangle rect = new Rectangle();

            rect.X = advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.None ? 0 : 1;
            if (advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                advancedBorderStyle.Left == DataGridViewAdvancedCellBorderStyle.InsetDouble)
            {
                rect.X++;
            }

            rect.Y = advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.None ? 0 : 1;
            if (advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                advancedBorderStyle.Top == DataGridViewAdvancedCellBorderStyle.InsetDouble)
            {
                rect.Y++;
            }

            rect.Width = advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.None ? 0 : 1;
            if (advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                advancedBorderStyle.Right == DataGridViewAdvancedCellBorderStyle.InsetDouble)
            {
                rect.Width++;
            }

            rect.Height = advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.None ? 0 : 1;
            if (advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble ||
                advancedBorderStyle.Bottom == DataGridViewAdvancedCellBorderStyle.InsetDouble)
            {
                rect.Height++;
            }

            //rect.Width += this.owningColumn.DividerWidth;
            //rect.Height += this.owningRow.DividerHeight;

            return rect;
        }

        #endregion

        #region Private Methods

        private static void PaintCell(ILogPaintContext logPaintCtx, DataGridViewCellPaintingEventArgs e,
            DataGridView gridView, bool noBackgroundFill, HilightEntry groundEntry)
        {
            PaintHighlightedCell(logPaintCtx, e, gridView, noBackgroundFill, groundEntry);
        }


        private static void PaintHighlightedCell(ILogPaintContext logPaintCtx, DataGridViewCellPaintingEventArgs e,
            DataGridView gridView, bool noBackgroundFill, HilightEntry groundEntry)
        {
            object value = e.Value != null ? e.Value : "";
            IList<HilightMatchEntry> matchList = logPaintCtx.FindHilightMatches(value as ILogLine);
            // too many entries per line seem to cause problems with the GDI 
            while (matchList.Count > 50)
            {
                matchList.RemoveAt(50);
            }

            HilightMatchEntry hme = new HilightMatchEntry();
            hme.StartPos = 0;
            hme.Length = (value as string).Length;
            hme.HilightEntry = new HilightEntry(value as string,
                groundEntry != null ? groundEntry.ForegroundColor : Color.FromKnownColor(KnownColor.Black),
                groundEntry != null ? groundEntry.BackgroundColor : Color.Empty,
                false);
            matchList = MergeHighlightMatchEntries(matchList, hme);

            int leftPad = e.CellStyle.Padding.Left;
            RectangleF rect = new RectangleF(e.CellBounds.Left + leftPad, e.CellBounds.Top, e.CellBounds.Width,
                e.CellBounds.Height);
            Rectangle borderWidths = BorderWidths(e.AdvancedBorderStyle);
            Rectangle valBounds = e.CellBounds;
            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;
            if (e.CellStyle.Padding != Padding.Empty)
            {
                valBounds.Offset(e.CellStyle.Padding.Left, e.CellStyle.Padding.Top);
                valBounds.Width -= e.CellStyle.Padding.Horizontal;
                valBounds.Height -= e.CellStyle.Padding.Vertical;
            }


            TextFormatFlags flags =
                    TextFormatFlags.Left
                    | TextFormatFlags.SingleLine
                    | TextFormatFlags.NoPrefix
                    | TextFormatFlags.PreserveGraphicsClipping
                    | TextFormatFlags.NoPadding
                    | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.TextBoxControl
                ;

            //          | TextFormatFlags.VerticalCenter
            //          | TextFormatFlags.TextBoxControl
            //          TextFormatFlags.SingleLine


            //TextRenderer.DrawText(e.Graphics, e.Value as String, e.CellStyle.Font, valBounds, Color.FromKnownColor(KnownColor.Black), flags);

            Point wordPos = valBounds.Location;
            Size proposedSize = new Size(valBounds.Width, valBounds.Height);

            Rectangle r = gridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            e.Graphics.SetClip(e.CellBounds);

            foreach (HilightMatchEntry matchEntry in matchList)
            {
                Font font = matchEntry != null && matchEntry.HilightEntry.IsBold
                    ? logPaintCtx.BoldFont
                    : logPaintCtx.NormalFont;
                Brush bgBrush = matchEntry.HilightEntry.BackgroundColor != Color.Empty
                    ? new SolidBrush(matchEntry.HilightEntry.BackgroundColor)
                    : null;
                string matchWord = (value as string).Substring(matchEntry.StartPos, matchEntry.Length);
                Size wordSize = TextRenderer.MeasureText(e.Graphics, matchWord, font, proposedSize, flags);
                wordSize.Height = e.CellBounds.Height;
                Rectangle wordRect = new Rectangle(wordPos, wordSize);

                Color foreColor = matchEntry.HilightEntry.ForegroundColor;
                if ((e.State & DataGridViewElementStates.Selected) != DataGridViewElementStates.Selected)
                {
                    if (!noBackgroundFill && bgBrush != null && !matchEntry.HilightEntry.NoBackground)
                    {
                        e.Graphics.FillRectangle(bgBrush, wordRect);
                    }
                }
                else
                {
                    if (foreColor.Equals(Color.Black))
                    {
                        foreColor = Color.White;
                    }
                }
                TextRenderer.DrawText(e.Graphics, matchWord, font, wordRect,
                    foreColor, flags);

                wordPos.Offset(wordSize.Width, 0);
                if (bgBrush != null)
                {
                    bgBrush.Dispose();
                }
            }
        }


        /// <summary>
        /// Builds a list of HilightMatchEntry objects. A HilightMatchEntry spans over a region that is painted with the same foreground and 
        /// background colors.
        /// All regions which don't match a word-mode entry will be painted with the colors of a default entry (groundEntry). This is either the 
        /// first matching non-word-mode highlight entry or a black-on-white default (if no matching entry was found).
        /// </summary>
        /// <param name="matchList">List of all highlight matches for the current cell</param>
        /// <param name="groundEntry">The entry that is used as the default.</param>
        /// <returns>List of HilightMatchEntry objects. The list spans over the whole cell and contains color infos for every substring.</returns>
        private static IList<HilightMatchEntry> MergeHighlightMatchEntries(IList<HilightMatchEntry> matchList,
            HilightMatchEntry groundEntry)
        {
            // Fill an area with lenth of whole text with a default hilight entry
            HilightEntry[] entryArray = new HilightEntry[groundEntry.Length];
            for (int i = 0; i < entryArray.Length; ++i)
            {
                entryArray[i] = groundEntry.HilightEntry;
            }

            // "overpaint" with all matching word match enries
            // Non-word-mode matches will not overpaint because they use the groundEntry
            foreach (HilightMatchEntry me in matchList)
            {
                int endPos = me.StartPos + me.Length;
                for (int i = me.StartPos; i < endPos; ++i)
                {
                    if (me.HilightEntry.IsWordMatch)
                    {
                        entryArray[i] = me.HilightEntry;
                    }
                    else
                    {
                        //entryArray[i].ForegroundColor = me.HilightEntry.ForegroundColor;
                    }
                }
            }

            // collect areas with same hilight entry and build new highlight match entries for it
            IList<HilightMatchEntry> mergedList = new List<HilightMatchEntry>();
            if (entryArray.Length > 0)
            {
                HilightEntry currentEntry = entryArray[0];
                int lastStartPos = 0;
                int pos = 0;
                for (; pos < entryArray.Length; ++pos)
                {
                    if (entryArray[pos] != currentEntry)
                    {
                        HilightMatchEntry me = new HilightMatchEntry();
                        me.StartPos = lastStartPos;
                        me.Length = pos - lastStartPos;
                        me.HilightEntry = currentEntry;
                        mergedList.Add(me);
                        currentEntry = entryArray[pos];
                        lastStartPos = pos;
                    }
                }
                HilightMatchEntry me2 = new HilightMatchEntry();
                me2.StartPos = lastStartPos;
                me2.Length = pos - lastStartPos;
                me2.HilightEntry = currentEntry;
                mergedList.Add(me2);
            }
            return mergedList;
        }

        #endregion
    }
}