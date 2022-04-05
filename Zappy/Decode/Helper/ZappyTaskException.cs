using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.Decode.Helper
{
    public class ZappyTaskException : Exception
    {
        private bool isUserMessageSpecified;
        private const string MessageSpecifiedParameter = "IsUserSpecifiedMessage";

        public ZappyTaskException()
        {
        }

        public ZappyTaskException(string message) : this(message, null)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ZappyTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            isUserMessageSpecified = info.GetBoolean("IsUserSpecifiedMessage");
        }

        public ZappyTaskException(string message, Exception innerException) : base(message, innerException)
        {
            if (!string.IsNullOrEmpty(message))
            {
                isUserMessageSpecified = true;
            }
        }

        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ZappyTaskUtilities.CheckForNull(info, "info");
            if (ErrorInfo != null)
            {
                info.AddValue("Source", ErrorInfo.Source);
            }
            info.AddValue("IsUserSpecifiedMessage", isUserMessageSpecified);
            base.GetObjectData(info, context);
        }

        [CLSCompliant(false)]
        protected void SetHResult(uint hResult)
        {
            HResult = (int)hResult;
        }

        public string BasicMessage
        {
            get
            {
                if (isUserMessageSpecified)
                {
                    return base.Message;
                }
                return DefaultMessage;
            }
        }

        protected virtual string DefaultMessage =>
            "BasicExceptionMessage.ZappyTaskException";

        public ILastInvocationInfo ErrorInfo { get; set; }

        public object ExceptionSource { get; set; }

        public int HResult
        {
            get =>
                base.HResult;
            set
            {
                base.HResult = value;
            }
        }

        public override string Message
        {
            get
            {
                string basicMessage = BasicMessage;
                if (!string.IsNullOrEmpty(ZappyTaskControlDescription))
                {
                    object[] args = { basicMessage, ZappyTaskControlDescription };
                    basicMessage = string.Format(CultureInfo.CurrentCulture, "BasicExceptionMessage.AdditionalDetailsMessage", args);
                    if (ErrorInfo != null && ErrorInfo.InnerInfo != null)
                    {
                        object[] objArray2 = { basicMessage, ErrorInfo.InnerInfo.Message };
                        basicMessage = string.Format(CultureInfo.CurrentCulture, "BasicExceptionMessage.AppendInnerInfo", objArray2);
                    }
                }
                return basicMessage;
            }
        }

        internal string ZappyTaskControlDescription { get; set; }
    }



}
