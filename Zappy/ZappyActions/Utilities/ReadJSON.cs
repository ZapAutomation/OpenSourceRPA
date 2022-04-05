using System;
using System.Activities;
using System.ComponentModel;
using Newtonsoft.Json;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Utilities
{

    public class ReadJSON : TemplateAction
    {
        [RequiredArgument]
        [Category("Input")]
        public DynamicProperty<string> Filename { get; set; }

        [RequiredArgument]
        [Category("Output")]
        public System.Data.DataTable DataTable { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            var filename = Filename;
            filename = Environment.ExpandEnvironmentVariables(filename);
            string json = System.IO.File.ReadAllText(filename);
            System.Data.DataTable dt = (System.Data.DataTable)JsonConvert.DeserializeObject(json, (typeof(System.Data.DataTable)));
            DataTable= dt;
        }
    }
}