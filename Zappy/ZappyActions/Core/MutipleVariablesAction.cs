using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Zappy.Plugins.ChromeBrowser;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core.Helper;

namespace Zappy.ZappyActions.Core
{
    [Description("Declaration of multiple variable")]
    public sealed class MutipleVariablesAction : TemplateAction, IVariableAction
    {
        public MutipleVariablesAction()
        {
            Variables = new List<string>();
        }

        [Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
                "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(CsvConverter))]
        [Category("Input")]
        [Description("Declare variables here - multiple variable seperated on new lines")]
        public List<string> Variables { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {

        }
    }

}
