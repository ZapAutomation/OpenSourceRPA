using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Files
{
    public class FileSave : TemplateAction
    {
        [Category("Input")]
        [Description("File Path")]
        public DynamicProperty<string> FilePath { get; set; }

                                [Category("Input")]
        [Description("Data to Save")]
        public DynamicProperty<byte[]> DataToSave { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            File.WriteAllBytes(FilePath, DataToSave);
        }
    }
}
