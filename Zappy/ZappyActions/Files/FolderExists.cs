using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;

namespace Zappy.ZappyActions.Files
{
                [Description("To Check Folder Exist Or Not ?")]
    public class FolderExists : DecisionNodeAction
    {
                                [Description("Folder Path")]
        [Category("Input")]
        public DynamicProperty<string> FolderPath { get; set; }

                                                        public override bool Execute(IZappyExecutionContext context)
        {
            return Directory.Exists(FolderPath);
        }
    }
}
