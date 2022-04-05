﻿using System.Drawing;

namespace ZappyLogger.Entities
{
    public class Bookmark
    {
        #region Fields

        #endregion

        #region cTor

        public Bookmark(int lineNum)
        {
            LineNum = lineNum;
            Text = "";
            Overlay = new BookmarkOverlay();
        }

        public Bookmark(int lineNum, string comment)
        {
            LineNum = lineNum;
            Text = comment;
            Overlay = new BookmarkOverlay();
        }

        #endregion

        #region Properties

        public int LineNum { get; set; }

        public string Text { get; set; }

        public BookmarkOverlay Overlay { get; set; }

        /// <summary>
        /// Position offset of the overlay as set by the user by dragging the overlay with the mouse.
        /// </summary>
        public Size OverlayOffset { get; set; }

        #endregion
    }
}