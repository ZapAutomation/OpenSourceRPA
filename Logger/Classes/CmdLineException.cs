using System;

namespace ZappyLogger.Classes
{
    /// <summary>
    /// Represents an error occuring during command line parsing.
    /// </summary>
    public class CmdLineException : Exception
    {
        #region cTor

        public CmdLineException(string parameter, string message)
            :
            base(string.Format("Syntax error of parameter -{0}: {1}", parameter, message))
        {
        }

        public CmdLineException(string message)
            :
            base(message)
        {
        }

        #endregion
    }
}