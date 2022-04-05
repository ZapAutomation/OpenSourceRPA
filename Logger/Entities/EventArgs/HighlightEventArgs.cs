namespace ZappyLogger.Entities.EventArgs
{
    public class HighlightEventArgs : System.EventArgs
    {
        #region Fields

        #endregion

        #region cTor

        public HighlightEventArgs(int startLine, int count)
        {
            this.StartLine = startLine;
            this.Count = count;
        }

        #endregion

        #region Properties

        public int StartLine { get; }

        public int Count { get; }

        #endregion
    }
}