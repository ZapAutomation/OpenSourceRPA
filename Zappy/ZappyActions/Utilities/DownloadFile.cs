using System;
using System.Activities;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using System.Net;
using System.ComponentModel;

namespace Zappy.ZappyActions.Utilities
{
    public class DownloadFile : TemplateAction
    {
        [RequiredArgument]
        [Category("Input")]
        [Description("The URL of the resource to download")]
        public DynamicProperty<string> URL { get; set; }

        [RequiredArgument]
        [Category("Input")]
        [Description("The name/path of the file to be placed on the local computer")]
        public DynamicProperty<string> LocalPath { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            using (var client = new System.Net.WebClient())
            {
                                                client.Headers.Add("User-Agent: Other");
                                client.DownloadFileTaskAsync(new Uri(URL), LocalPath).Wait();
            }
        }


    }
}