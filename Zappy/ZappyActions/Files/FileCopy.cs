using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
                        [Description("Copy File From Source To Destination")]
    public class FileCopy : TemplateAction
    {
        public FileCopy()
        {
            Overwrite = true;
        }

                                [Category("Input")]
        [Description("Source FilePath ")]
        public DynamicProperty<string> FromFilePath { get; set; }

                                [Category("Input")]
        [Description("Destination FilePath")]
        public DynamicProperty<string> ToFilePath { get; set; }

                                [Category("Input")]
        [Description("true if the destination file can be overwritten; otherwise, false")]
        public bool Overwrite { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            File.Copy(FromFilePath, ToFilePath, Overwrite);
        }


        public override string AuditInfo()
        {
            return base.AuditInfo() + " FromFilePath:" + this.FromFilePath + " ToFilePath:" + this.ToFilePath;
        }
    }
}
