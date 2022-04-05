using System;
using System.Diagnostics;
using System.Globalization;

namespace Zappy.ActionMap.Browser
{
    internal class HtmlTraceListener : TextWriterTraceListener, IDisposable
    {


        //public HtmlTraceListener(string fileName) : base(fileName)
        //{
        //    using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ZappyTask.Extension.Utility.LogHeader.htm")))
        //    {
        //        base.WriteLine(reader.ReadToEnd());
        //    }
        //}

        private static string GetHtmlMessage(string message)
        {
            string messageTagName = GetMessageTagName(message);
            if (string.IsNullOrEmpty(messageTagName))
            {
                return message;
            }
            object[] args = { messageTagName, message };
            return string.Format(CultureInfo.InvariantCulture, "<{0}>{1}</{0}>", args);
        }

        private static string GetMessageTagName(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (message.StartsWith("V,", StringComparison.Ordinal))
                {
                    return "h4";
                }
                if (message.StartsWith("I,", StringComparison.Ordinal))
                {
                    return "h3";
                }
                if (message.StartsWith("W,", StringComparison.Ordinal))
                {
                    return "h2";
                }
                if (message.StartsWith("E,", StringComparison.Ordinal))
                {
                    return "h1";
                }
            }
            return null;
        }

        public override void Write(string message)
        {
            base.Write(GetHtmlMessage(message));
        }

        public override void WriteLine(string message)
        {
            base.WriteLine(GetHtmlMessage(message));
        }
    }
}