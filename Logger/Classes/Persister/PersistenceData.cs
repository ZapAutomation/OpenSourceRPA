using System.Collections.Generic;
using System.Text;
using ZappyLogger.Classes.Filter;
using ZappyLogger.Entities;

namespace ZappyLogger.Classes.Persister
{
    public class PersistenceData
    {
        #region Fields

        public SortedList<int, Entities.Bookmark> bookmarkList = new SortedList<int, Entities.Bookmark>();
        public int bookmarkListPosition = 300;
        public bool bookmarkListVisible = false;
        public string columnizerName;
        public int currentLine = -1;
        public Encoding encoding;
        public string fileName = null;
        public bool filterAdvanced = false;
        public List<FilterParams> filterParamsList = new List<FilterParams>();
        public int filterPosition = 222;
        public bool filterSaveListVisible = false;
        public List<FilterTabData> filterTabDataList = new List<FilterTabData>();
        public bool filterVisible = false;
        public int firstDisplayedLine = -1;
        public bool followTail = true;
        public string highlightGroupName;
        public int lineCount;

        public bool multiFile = false;
        public int multiFileMaxDays;
        public List<string> multiFileNames = new List<string>();
        public string multiFilePattern;
        public SortedList<int, RowHeightEntry> rowHeightList = new SortedList<int, RowHeightEntry>();
        public string sessionFileName = null;
        public bool showBookmarkCommentColumn;
        public string tabName = null;

        #endregion
    }
}