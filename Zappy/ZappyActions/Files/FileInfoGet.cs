using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                    [Description("Gets File Information")]
    public class FileInfoGet : TemplateAction
    {
                                [Category("Input")]
        [Description("FilePath for getting the information")]
        public DynamicProperty<string> FilePath { get; set; }

                                [Category("Output")]
        [Description("Selected FileName")]
        public string FileName { get; set; }

                                [Category("Output")]
        [Description("Gets extension of selected file")]
        public string FileExtension { get; set; }

                                [Category("Output")]
        [Description("File Size")]
        public decimal FileSize { get; set; }

                                [Category("Output")]
        [Description("File CreationTime")]
        public System.DateTime CreationTime { get; set; }

                                [Category("Output")]
        [Description("LastAccessTime of file")]
        public System.DateTime LastAccessTime { get; set; }

                                [Category("Output")]
        [Description("Last Write Time to the file")]
        public System.DateTime LastWriteTime { get; set; }


                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            FileInfo FileInfo = new FileInfo(FilePath);
            this.FileName = FileInfo.Name;
            this.FileExtension = FileInfo.Extension;
            this.FileSize = FileInfo.Length;
            this.CreationTime = FileInfo.CreationTime;
            this.LastAccessTime = FileInfo.LastAccessTime;
            this.LastWriteTime = FileInfo.LastWriteTime;
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath + " FileName:" + this.FileName + " FileExtension:" + this.FileExtension + " FileSize:" + this.FileSize;
        }
    }
}
