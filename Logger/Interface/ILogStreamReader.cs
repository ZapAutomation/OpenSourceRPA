using System.Text;

namespace ZappyLogger.Interface
{
    public interface ILogStreamReader
    {
        #region Properties

        long Position { get; set; }

        bool IsBufferComplete { get; }

        Encoding Encoding { get; }

        #endregion

        #region Public methods

        int ReadChar();
        string ReadLine();

        #endregion
    }
}