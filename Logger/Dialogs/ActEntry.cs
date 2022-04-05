using ZappyLogger.ColumnizaeLib;

namespace ZappyLogger.Dialogs
{
    public struct ActEntry
    {
        public string Name { get; set; }

        public IKeywordAction Plugin { get; set; }
    }
}