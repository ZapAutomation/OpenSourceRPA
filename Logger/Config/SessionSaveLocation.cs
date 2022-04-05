using System;

namespace ZappyLogger.Config
{
    [Serializable]
    public enum SessionSaveLocation
    {
        DocumentsDir,
        SameDir,
        OwnDir,
        LoadedSessionFile
    }
}