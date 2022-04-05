using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Miscellaneous.Helper;

namespace Zappy.ZappyActions.Validate
{
    [Description("Check Path Exists Or Not ?")]
    public class PathExists : DecisionNodeAction
    {
        [Category("Input"), RequiredArgument]
        [Description("Path to check")]
        public DynamicProperty<string> Path { get; set; }

        [Category("Optional")]
        [Description("PathType is File or Folder")]
        public PathType PathType { get; set; }

                                                public override bool Execute(IZappyExecutionContext context)
        {
            
                string path = this.Path;
                if (this.PathType == PathType.File)
                {
                    return File.Exists(path);
                }
                else
                {
                    return Directory.Exists(path);
                }       
        }


        public override string AuditInfo()
        {
            return base.AuditInfo() + " Path:" + this.Path;
        }
    }
}

