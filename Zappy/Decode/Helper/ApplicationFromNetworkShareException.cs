using System;
using System.Runtime.Serialization;

namespace Zappy.Decode.Helper
{
    internal class ApplicationFromNetworkShareException : ZappyTaskException
    {
        public ApplicationFromNetworkShareException()
        {
        }

        public ApplicationFromNetworkShareException(string message) : base(message)
        {
        }

        protected ApplicationFromNetworkShareException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ApplicationFromNetworkShareException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }



}
