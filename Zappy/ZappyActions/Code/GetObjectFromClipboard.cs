using System.ComponentModel;
using System.Windows;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
    [Description("Gets Object from clipboard")]
    public class GetObjectFromClipboard : TemplateAction
    {
        [Category("Output")]
        [Description("Return object from clipboard")]
        public object Result { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Result = Clipboard.GetDataObject();
        }


    }
}