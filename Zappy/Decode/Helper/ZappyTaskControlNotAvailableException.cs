using System;
using System.Runtime.Serialization;

namespace Zappy.Decode.Helper
{
    public class ZappyTaskControlNotAvailableException : ZappyTaskException
    {
        public ZappyTaskControlNotAvailableException() : this(null, null, null, null)
        {
        }

        public ZappyTaskControlNotAvailableException(Exception innerException) : this(null, innerException, null, null)
        {
        }

        public ZappyTaskControlNotAvailableException(string message) : this(message, null, null, null)
        {
        }

                                
        protected ZappyTaskControlNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ZappyTaskControlNotAvailableException(string message, Exception innerException) : this(message, innerException, null, null)
        {
        }

        public ZappyTaskControlNotAvailableException(string uiTaskControl, object exceptionSource) : this(null, null, uiTaskControl, exceptionSource)
        {
        }

        public ZappyTaskControlNotAvailableException(Exception innerException, string uiTaskControl, object exceptionSource) : this(null, innerException, uiTaskControl, exceptionSource)
        {
        }

        public ZappyTaskControlNotAvailableException(string message, string uiTaskControl, object exceptionSource) : this(message, null, uiTaskControl, exceptionSource)
        {
        }

        public ZappyTaskControlNotAvailableException(string message, Exception innerException, string uiTaskControl, object exceptionSource) : base(message, innerException)
        {
            SetHResult(0xf004f000);
            ZappyTaskControlDescription = uiTaskControl;
            ExceptionSource = exceptionSource;
        }

        protected override string DefaultMessage =>
            "BasicExceptionMessage.ZappyTaskControlNotAvailableException";
    }



}
