namespace ZappyLogger.ColumnizaeLib
{
    /// <summary>
    /// This helper struct holds a log line and its line number (zero based).
    /// This struct is used by <see cref="IZappyLoggerCallback"/>.
    /// </summary>
    /// <seealso cref="IZappyLoggerCallback.AddPipedTab"/>
    public struct LineEntry
    {
        /// <summary>
        /// The content of the line.
        /// </summary>
        public ILogLine logLine;

        /// <summary>
        /// The line number. See <see cref="IZappyLoggerCallback.AddPipedTab"/> for an explanation of the line number.
        /// </summary>
        public int lineNum;
    }
}