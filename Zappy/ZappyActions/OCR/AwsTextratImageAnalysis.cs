using System.ComponentModel;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.OCR
{
    public class AwsTextratImageAnalysis : AwsTextExtractServices
    {
        [Category("Input")]
        [Description("Inputed image file path - JPEG or PNG format")]
        public DynamicProperty<string> InputFilePath { get; set; }

        
        [Category("Output")]
        public string OutputString { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            OutputString = GetAwsDocumentText(InputFilePath);

        }
       
    }
}

