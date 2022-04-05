using System.Collections.Generic;
using ZappyLogger.Classes.Highlight;

namespace ZappyLogger.Entities
{
    public class HighlightResults
    {
        #region Fields

        #endregion

        #region Properties

        public IList<HilightEntry> HighlightEntryList { get; set; } = new List<HilightEntry>();

        #endregion
    }
}