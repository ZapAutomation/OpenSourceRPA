using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                [Description("Gets Folder Information")]
    public class FolderInfoGet : TemplateAction
    {
                                [Category("Input")]
        [Description("Sets FolderPath to get an more infomation about folder")]
        public DynamicProperty<string> FolderPath { get; set; }

                                [Category("Output")]
        [Description("FolderName")]
        public string FolderName { get; set; }

                                [Category("Output")]
        [Description("Folder Count")]
        public int FolderCount { get; set; }

                                [Category("Output")]
        [Description("Files counts of folder contain")]
        public int FileCount { get; set; }

                                [Category("Output")]
        [Description("Time of file creation")]
        public System.DateTime CreationTime { get; set; }

                                [Category("Output")]
        [Description("LastAccessTime of folder")]
        public System.DateTime LastAccessTime { get; set; }

                                [Category("Output")]
        [Description("LastWriteTime to the folder")]
        public System.DateTime LastWriteTime { get; set; }



                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

            DirectoryInfo folderInfo = new DirectoryInfo(FolderPath);

            this.FolderName = folderInfo.Name;
            this.FolderCount = folderInfo.GetDirectories().Length;
            this.FileCount = folderInfo.GetFiles().Length;
            this.CreationTime = folderInfo.CreationTime;
            this.LastAccessTime = folderInfo.LastAccessTime;
            this.LastWriteTime = folderInfo.LastWriteTime;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FolderName:" + this.FolderName + " FolderPath:" + this.FolderPath + " FolderCount:" + this.FolderCount + " LastAccessTime:" + this.LastAccessTime + " LastWriteTime:" + this.LastWriteTime;
        }
    }
}
