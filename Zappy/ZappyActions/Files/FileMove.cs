using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                [Description("Move File From Source To Destination")]
    public class FileMove : TemplateAction
    {
                                [Category("Input")]
        [Description("Source FilePath")]
        public DynamicProperty<string> FromFilePath { get; set; }

                                [Category("Input")]
        [Description("Destination FilePath")]
        public DynamicProperty<string> DestinationFolder { get; set; }

        [Category("Optional")]
        [Description("Filename of the file moved")]
        public DynamicProperty<string> FileName { get; set; }

        [Category("Output")]
        public string DestinationFilePath { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            string DestinationfileName = Path.GetExtension(DestinationFolder);
                        if (string.IsNullOrWhiteSpace(FileName))
            {
                FileName = Path.GetFileName(FromFilePath);
            }
            DestinationFilePath = string.Concat(DestinationFolder, "\\", FileName);
            File.Move(FromFilePath, DestinationFilePath);
                                                                    }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FromFilePath:" + this.FromFilePath + " ToFilePath:" + this.DestinationFolder;
        }
    }
}
