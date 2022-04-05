using System.ComponentModel;
using System.Net;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Web
{
    [Description("Upload file on server using Rest API")]
    public class RestAPIUploadFile : TemplateAction
    {
        public RestAPIUploadFile()
        {
            APIUrl = "";
            FilePath = "";

        }
        [Category("Input")]
        [Description("API url")]
        public DynamicProperty<string> APIUrl { get; set; }

        [Category("Input")]
        [Description("Enter File Path which you want to upload on server")]
        public DynamicProperty<string> FilePath { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            WebClient myWebClient = new WebClient();
            byte[] responseArray = myWebClient.UploadFile(APIUrl, FilePath);
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " APIUrl:" + this.APIUrl + " FilePath:" + this.FilePath;
        }
    }
}