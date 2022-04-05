using Syroot.Windows.IO;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
    public class LatestModifiedFilePath : TemplateAction
    {
        public LatestModifiedFilePath()
        {
            Extension = ".";
            AddKnownFolderType = KnownFolderType.Downloads;
            Directory = "";
        }

        [Category("Input")]
        [Description("Set it as blank if using AddKnownFolderType")]
        public DynamicProperty<string> Directory { get; set; }

        [Category("Optional")]
        [Description("Select directory path, Default Downloads directory path is set")]
        public KnownFolderType AddKnownFolderType { get; set; }

        [Category("Optional")]
        [Description("Set the file extension, default is '.' and if you want to search all text files then set *.txt")]
        public DynamicProperty<string> Extension { get; set; }

        [Category("Output")]
        [Description("Return lasted modified file Path")]
        public string LatestFilePath { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            
            var _Directory = Directory;
            if (string.IsNullOrEmpty(Directory.ToString()))
            {
                _Directory = new KnownFolder(AddKnownFolderType).Path;
            }
            var dir = new DirectoryInfo(_Directory);
            dir.Refresh();
            Thread.Sleep(1000);
            LatestFilePath = GetNewestFile(_Directory.ToString(), Extension);
        }

        public static string GetNewestFile(string directory, string extension)
        {
            var dirInfo = new DirectoryInfo(directory);
            var file = (from f in dirInfo.GetFiles(extension) orderby f.LastWriteTime descending select f).First();
            string _FullPath = Path.Combine(directory.ToString(), file.ToString());
            return _FullPath;
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Directory:" + this.Directory + " Extension:" + this.Extension + " LatestFilePath:" + this.LatestFilePath;
        }
    }
}
