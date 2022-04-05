using System;
using System.ComponentModel;
using System.Windows;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Code
{
    [Description("Set the object to clipboard")]
    public class PutObjectToClipboard : TemplateAction
    {
        [Category("Input")]
        [Description("Set the object to clipboard")]
        public DynamicProperty<object> Object { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (!CommonProgram.SetTextInClipboard(Object.Value))
                throw new Exception("Unable to \"Set\" text to clipboard!");
        }
    }
}