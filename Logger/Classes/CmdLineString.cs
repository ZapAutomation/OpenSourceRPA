namespace ZappyLogger.Classes
{
    /// <summary>
    /// Represents an string command line parameter.
    /// </summary>
    public class CmdLineString : CmdLineParameter
    {
        #region cTor

        public CmdLineString(string name, bool required, string helpMessage)
            : base(name, required, helpMessage)
        {
        }

        #endregion

        #region Public methods

        public static implicit operator string(CmdLineString s)
        {
            return s.Value;
        }

        #endregion
    }
}