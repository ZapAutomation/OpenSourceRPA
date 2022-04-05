﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ZappyLogger.Entities
{
    public class BookmarkCollection : ReadOnlyCollection<Bookmark>
    {
        #region Fields

        private SortedList<int, Bookmark> bookmarkList;

        #endregion

        #region cTor

        internal BookmarkCollection(SortedList<int, Bookmark> bookmarkList)
            : base(bookmarkList.Values)
        {
            this.bookmarkList = bookmarkList;
        }

        #endregion
    }
}