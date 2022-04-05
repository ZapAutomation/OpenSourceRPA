using ImageMagick;
using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Picture.Helpers;

namespace Zappy.ZappyActions.Picture
{
    [Description("CleanUp The Scan Image")]
    public class CleanupImage : TemplateAction
    {


        [Category("Input")]
        [Description("Path of the image")]
        public DynamicProperty<string> ImagePath { get; set; }


        [Category("Optional")]
        [DefaultValue("Output path for the cleaned image")]
        public string Path { get; set; }

                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            MagickImage image = new MagickImage();
            image.Read(ImagePath);
            TextCleanerScript textCleanerScript = new TextCleanerScript();
            IMagickImage CleanedImage = textCleanerScript.Execute(image);
            if (String.IsNullOrEmpty(Path))
                Path = ImagePath;
            CleanedImage.Write(Path);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Input Image Path : " + this.ImagePath + " Output Path :" + this.Path;
        }
    }
}
