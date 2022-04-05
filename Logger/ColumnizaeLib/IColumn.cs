namespace ZappyLogger.ColumnizaeLib
{
    public interface IColumn : ITextValue
    {
        #region Properties

        IColumnizedLogLine Parent { get; }

        string FullValue { get; }

        string DisplayValue { get; }

        #endregion
    }
}