using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
    public class FileExtract : TemplateAction
    {
        public enum FileZipUnzip
        {
            Zip,
            Unzip
        };
        public FileExtract()
        {
            FilePath = string.Empty;
            SavePath = string.Empty;
        }
        [Category("Input")]
        [Description("Select one option")]
        public FileZipUnzip ZipUnzip { get; set; }

        [Category("Input")]
        [Description("Enter Zip or unzip File Path")]
        public DynamicProperty<string> FilePath { get; set; }

        [Category("Optional")]
        [Description("Enter Directory path for save zip/unzip file")]
        public DynamicProperty<string> SavePath { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if(FileZipUnzip.Unzip == ZipUnzip)
            {
                if (string.IsNullOrEmpty(SavePath))
                {
                    SavePath = System.IO.Path.GetDirectoryName(FilePath);
                }
                System.IO.Compression.ZipFile.ExtractToDirectory(FilePath, SavePath);
            }
            else
            {
                string[] lines = FilePath.ToString().Split('\\');
                int count = lines.Length;
                string zipfilename = lines[count - 1];
                if (string.IsNullOrEmpty(SavePath))
                {   
                    SavePath = System.IO.Path.GetDirectoryName(FilePath) + "\\" + zipfilename+".zip";
                }
                else if(!SavePath.ToString().Contains(".zip"))
                {
                    SavePath = SavePath.ToString() + "\\" + zipfilename+".zip";
                }
                System.IO.Compression.ZipFile.CreateFromDirectory(FilePath, SavePath);
            }
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " FilePath:" + this.FilePath + " SavePath:" + this.SavePath;
        }
    }
}
