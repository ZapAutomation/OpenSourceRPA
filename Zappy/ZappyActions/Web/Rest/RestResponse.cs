using RestSharp;
using RestSharp.Extensions;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Zappy.ZappyActions.Web.Rest
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RestResponse
    {
        public byte[] RawResponse { get; internal set; }
        public string Response { get; internal set; }
        public int StatusCode { get; internal set; }
        public string ErrorMessage { get; internal set; }
        public string Status { get; internal set; }
        public string ContentType { get; internal set; }
        public RestResponse(IRestResponse source)
        {
            this = new RestResponse();
            try
            {
                this.RawResponse = source.RawBytes;
                this.Status = source.ResponseStatus.ToString();
                if (source.ResponseStatus == ResponseStatus.Error)
                {
                    this.ErrorMessage = source.ErrorMessage;
                }
                this.ContentType = source.ContentType;
                this.StatusCode = (int)ReflectionExtensions.ChangeType(source.StatusCode, (TypeInfo)typeof(int));
                this.Response = MiscExtensions.AsString(source.RawBytes);
            }
            catch (Exception exception)
            {
                Trace.TraceError("Rest Response: " + exception.ToString());
            }
        }
    }
}

