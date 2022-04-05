using System;
using System.ComponentModel;
using Zappy.Helpers;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.Loops
{
    [Description("Loop End")]
    public class LoopEndAction : IZappyLoopEndAction
    {
        public LoopEndAction()
        {
            SelfGuid = Guid.NewGuid(); Id = ActionIDRegister.GetUniqueId(); Timestamp = WallClock.UtcNow;
            _NextGuid = Guid.Empty;
        }


        [System.Xml.Serialization.XmlIgnore, Newtonsoft.Json.JsonIgnore] [Browsable(false)] public long Id { get; set; }

        [Category("Common")]
        public string DisplayName { get; set; }

                [Browsable(false)] public System.DateTime Timestamp { get; set; }
        [Browsable(false)] [Category("Internals")] public string ExeName { get; set; }
        [Browsable(false)] public int TimeOutInMilliseconds { get; set; }

        [Category("Common")] public bool ContinueOnError { get; set; }
        [Category("Common")] public int PauseTimeAfterAction { get; set; }
        [Category("Common")] public int NumberOfRetries { get; set; }

        [Category("Internals")] public Guid LoopStartGuid { get; set; }
        [Category("Internals")] public Guid SelfGuid { get; set; }
        [Category("Internals")] public Guid NextGuid { get; set; }
        [Category("Internals")] public Guid ErrorHandlerGuid { get; set; }
        [Browsable(false)] public int EditorLocationX { get; set; }
        [Browsable(false)] public int EditorLocationY { get; set; }

        protected Guid _NextGuid;
        protected IZappyLoopStartAction _LoopStartAction;
        bool _Initialized;

                                        protected virtual void Initialize(IZappyExecutionContext context)
        {
            if (!_Initialized)
            {
                _NextGuid = NextGuid;
                object _LoopStart = null;
                context.ContextData.TryGetValue(CrapyConstants.ZappyCurrentLoopStart, out _LoopStart);
                _LoopStartAction = _LoopStart as IZappyLoopStartAction;
                _Initialized = true;
            }
        }

                                                public virtual void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            ZappyExecutionContext _context = context as ZappyExecutionContext;

            Initialize(context);
            if (_LoopStartAction != null && _LoopStartAction.CheckLoopTermination())
            {
                _LoopStartAction.FinalizeLoop(context);
                NextGuid = _NextGuid;
            }
            else
                NextGuid = LoopStartGuid;
        }

        public virtual string AuditInfo()
        {
            return HelperFunctions.HumanizeNameForIZappyAction(this);
        }
    }


}
