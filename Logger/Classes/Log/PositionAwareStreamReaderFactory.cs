using System.IO;
using ZappyLogger.Entities;
using ZappyLogger.Interface;

namespace ZappyLogger.Classes.Log
{
    internal static class PositionAwareStreamReaderFactory
    {
        #region Internals

        internal static ILogStreamReader CreateStreamReader(Stream stream, EncodingOptions encodingOptions, bool useSystemReader)
        {
            if (useSystemReader)
            {
                return new PositionAwareStreamReader(stream, encodingOptions);
            }
            else
            {
                return new PositionAwareStreamReaderLegacy(stream, encodingOptions);
            }
        }

        #endregion
    }
}