using System;

namespace Zappy.ActionMap.HelperClasses
{
    internal class RethrowException : Exception
    {
        public RethrowException(Exception ex, bool isNotSupported = true)
        {
            InternalException = ex;
            IsNotSupported = isNotSupported;
        }

        public Type DataType { get; set; }

        public Exception InternalException { get; set; }

        public bool IsNotSupported { get; set; }

        public object Value { get; set; }
    }
}