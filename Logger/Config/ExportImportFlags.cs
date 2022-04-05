using System;

namespace ZappyLogger.Config
{
    [Flags]
    public enum ExportImportFlags : long
    {
        None = 0,
        HighlightSettings = 1,
        ColumnizerMasks = 2,
        HighlightMasks = 4,
        ToolEntries = 8,
        Other = 16,
        All = HighlightSettings | ColumnizerMasks | HighlightMasks | ToolEntries | Other
    }
}