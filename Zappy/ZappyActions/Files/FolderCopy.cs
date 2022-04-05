using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
    [Description("Copy Folder From One Path To Other ")]
    public class FolderCopy : TemplateAction
    {
        public FolderCopy()
        {
            CopySubDirs = true;
        }

        [Category("Input")]
        [Description("Source folder path")]
        public DynamicProperty<string> FromFolderPath { get; set; }

        [Category("Input")]
        [Description("Destination folder path")]
        public DynamicProperty<string> ToFolderPath { get; set; }

        [Category("Optional")]
        [Description("Destination folder path")]
        public DynamicProperty<bool> CopySubDirs { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            DirectoryCopy(FromFolderPath, ToFolderPath, CopySubDirs);
        }
       
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
                        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
                        if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

                        FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

                        if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

                public override string AuditInfo()
        {
            return base.AuditInfo() + " FromFolderPath:" + this.FromFolderPath + " ToFilePath:" + this.ToFolderPath;
        }
    }


}

