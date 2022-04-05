using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Documents
{
    public abstract class TemplateDocumentAction : TemplateAction
    {

        [Category("Input")]
        [Description("Input Data")]
        public DynamicProperty<string> InputData { get; set; }

        [Category("Output")]
        public string OutputData { get; set; }
    }
}