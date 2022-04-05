using System;
using System.Runtime.Serialization;
using Zappy.Decode.Helper;

namespace Zappy.ActionMap.TaskAction
{
    internal class ZappyTaskControlInvalidActionException : ZappyTaskException
    {
        public ZappyTaskControlInvalidActionException()
        {
        }

        public ZappyTaskControlInvalidActionException(string message) : base(message)
        {
        }

        protected ZappyTaskControlInvalidActionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ZappyTaskControlInvalidActionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }



}
