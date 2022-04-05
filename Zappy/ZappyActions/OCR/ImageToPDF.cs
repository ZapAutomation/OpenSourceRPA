using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.OCR
{
    [Description("OCR To Searchable PDF")]
    public class ImageToPDF : TemplateAction
    {
        public ImageToPDF()
        {
            Language = "eng";
            AdditionalArguments = "";
        }
        [Category("Input")]
        [Description("Image path")]
        public DynamicProperty<string> ImagePath { get; set; }

        [DefaultValue("eng")]
        [Category("Optional")]
        public DynamicProperty<string> Language { get; set; }

        [Category("Optional")]
        public DynamicProperty<string> AdditionalArguments { get; set; }

        [Description("Gets PDF file path")]
        [Category("Output")]
        public string Path { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Path = string.Empty;
            ProcessImageUsingTessaract processImageUsingTessaract = new ProcessImageUsingTessaract();
            Path =
                processImageUsingTessaract.GeneratePDFExternalTesseractEngine
                    (ImagePath, Language, Path, AdditionalArguments);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " ImagePath:" + this.ImagePath + " Language:" + this.Language + " OcrPath:" + this.Path;
        }
    }
}