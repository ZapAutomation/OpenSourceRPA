using System.ComponentModel;
using System.Media;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{

            
        [Description("Generate Sound")]
    public class Beep : TemplateAction
    {
                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            SystemSounds.Beep.Play();
        }
    }
}

