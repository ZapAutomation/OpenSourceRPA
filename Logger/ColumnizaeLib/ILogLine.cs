namespace ZappyLogger.ColumnizaeLib
{
    public interface ILogLine : ITextValue
    {
        #region Properties

        string FullLine { get; }

        int LineNumber { get; }

        #endregion
    }
}