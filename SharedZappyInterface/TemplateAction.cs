using System;
using System.ComponentModel;
using Zappy.SharedInterface.Helper;

namespace Zappy.SharedInterface
{

    public abstract class TemplateAction : IZappyAction
    {
        protected TemplateAction()
        {
            SelfGuid = Guid.NewGuid(); Id = ActionIDRegister.GetUniqueId(); Timestamp = WallClock.UtcNow;
        }



        [System.Xml.Serialization.XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Browsable(false)]
        public long Id { get; set; }
        [Category("Common")]
        [Description("The display name of the activity")]
        public string DisplayName { get; set; }

        [Browsable(false)]
        public System.DateTime Timestamp { get; set; }
        [Browsable(false)]
        public string ExeName { get; set; }

                [Browsable(false)]
        public int TimeOutInMilliseconds { get; set; }
        [Description("Specifies if the automation should continue even when the activity throws an error")]
        [Category("Common")] public bool ContinueOnError { get; set; }

        [Description("Specifies the amount of time (in milliseconds) to pause after the current activity has finished execution")]
        [Category("Common")] public int PauseTimeAfterAction { get; set; }

        [Category("Common")] public int NumberOfRetries { get; set; }

        [Category("Common")]
        public Guid SelfGuid { get; set; }
        [Category("Common")]
        public Guid NextGuid { get; set; }

        [Category("Common")]
        public Guid ErrorHandlerGuid { get; set; }
        [Browsable(false)]
        public int EditorLocationX { get; set; }
        [Browsable(false)]
        public int EditorLocationY { get; set; }


        public abstract void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker);

                public virtual string AuditInfo()
        {
            if (string.IsNullOrEmpty(this.DisplayName))
                return "Action: " + HelperFunctions.HumanizeNameForIZappyAction(this);
            else
                return this.DisplayName + " Action: " + HelperFunctions.HumanizeNameForIZappyAction(this);
        }
    }
}