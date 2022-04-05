using System;
using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Triggers
{
    [Description("File Change Trigger")]
    public class FileChangeTrigger : FolderChangeTrigger
    {
        public FileChangeTrigger()
        {
            _FileName = string.Empty;
        }

        DynamicProperty<string> _FileName;
        [Category("Input")]
        [Description("File path to which user want to apply the trigger")]
        public DynamicProperty<string> FilePath
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                try
                {
                    if (!string.IsNullOrEmpty(value) && ((File.GetAttributes(value) & FileAttributes.Directory) != FileAttributes.Directory))
                    {
                        Filter = System.IO.Path.GetFileName(value);
                        FolderPath = System.IO.Path.GetDirectoryName(value);
                    }
                }
                catch
                {
                    FolderPath = _FileName = string.Empty;
                }
            }
        }

        public override IDisposable  RegisterTrigger(IZappyExecutionContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    Filter = System.IO.Path.GetFileName(FilePath);
                    FolderPath = System.IO.Path.GetDirectoryName(FilePath);
                }
            }
            catch
            {
                FolderPath = _FileName = string.Empty;
            }
            return ConfigureFileSystemTrigger(this);
        }
                                
    }
}

