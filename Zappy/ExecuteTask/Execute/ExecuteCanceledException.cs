using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Zappy.Decode.Helper;

namespace Zappy.ExecuteTask.Execute
{
    internal class ExecuteCanceledException : ZappyTaskException
    {
        public ExecuteCanceledException()
        {
            SetHResult(0x800704c7);
        }

        public ExecuteCanceledException(string message) : base(message)
        {
            SetHResult(0x800704c7);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ExecuteCanceledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            SetHResult(0x800704c7);
        }

        public ExecuteCanceledException(string message, Exception innerException) : base(message, innerException)
        {
            SetHResult(0x800704c7);
        }
    }
}