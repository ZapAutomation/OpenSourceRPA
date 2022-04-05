using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{

    [Description("Move Folder From One Path To Other ")]
    public class FolderMove : TemplateAction
    {
                                [Category("Input")]
        [Description("Source folder path")]
        public DynamicProperty<string> FromFolderPath { get; set; }

                                [Category("Input")]
        [Description("Destination folder path")]
        public DynamicProperty<string> ToFolderPath { get; set; }

        [Category("Optional")]
        [Description("If you want to move folder without the from folder name")]
        public DynamicProperty<bool> DoNotCopyFromFolderName { get; set; }

        [Category("Output")]
        [Description("Destination folder path with souce folder name")]
        public string DestinationFolderPath { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (DoNotCopyFromFolderName)
                DestinationFolderPath = ToFolderPath;
            else
                DestinationFolderPath = CreateDestinationFolderName(FromFolderPath, ToFolderPath);
            Directory.Move(FromFolderPath, DestinationFolderPath);
        }
        private string CreateDestinationFolderName(string FromFolderPath, string ToFolderPath)
        {
            var directoryInfo = new DirectoryInfo(FromFolderPath);
            return Path.Combine(ToFolderPath, directoryInfo.Name);
        }

                public override string AuditInfo()
        {
            return base.AuditInfo() + " FromFolderPath:" + this.FromFolderPath + " ToFilePath:" + this.ToFolderPath;
        }
    }


}

