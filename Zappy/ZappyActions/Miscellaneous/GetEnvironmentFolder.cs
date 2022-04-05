using System;
using System.Activities;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
    [Description("Gets the environments of selected Folder")]
    public class GetEnvironmentFolder : TemplateAction
    {
        [Category("Input"), RequiredArgument]
        [Description("Folder Name")]
        public System.Environment.SpecialFolder SpecialFolder { get; set; }

        [Category("Output"), XmlIgnore]
        [Description("Gets the path to the system special folder")]
        public string FolderPath { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            this.FolderPath = Environment.GetFolderPath(this.SpecialFolder);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " FolderPath:" + this.FolderPath;
        }
    }
}

