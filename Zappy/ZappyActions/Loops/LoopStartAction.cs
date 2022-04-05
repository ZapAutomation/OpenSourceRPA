using System;
using System.ComponentModel;
using Zappy.Helpers;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Loops
{
            
    [Description("Loop Start Action")]
    public abstract class LoopStartAction : IZappyLoopStartAction
    {
        public LoopStartAction()
        {
            SelfGuid = Guid.NewGuid(); Id = ActionIDRegister.GetUniqueId(); Timestamp = WallClock.UtcNow;
            TerminateLoop = false;
            _Next = Guid.Empty;
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

        [Category("Internals")] public Guid LoopEndGuid { get; set; }

        [Category("Internals")] public Guid SelfGuid { get; set; }

        [Category("Internals")] public Guid NextGuid { get; set; }
        [Category("Internals")] public Guid ErrorHandlerGuid { get; set; }

        [Browsable(false)] public int EditorLocationX { get; set; }
        [Browsable(false)] public int EditorLocationY { get; set; }

        public abstract string AuditInfo();
                        
        [Category("Internals")]
        public DynamicProperty<bool> TerminateLoop { get; set; }

        protected Guid _Next;
        protected IZappyLoopStartAction _PreviousLoopStart;
        protected bool _Initialized;

        protected virtual void Initialize(IZappyExecutionContext context)
        {
            if (!_Initialized)
            {
                TerminateLoop = false;
                object _PrevLoopStart = null;
                context.ContextData.TryGetValue(CrapyConstants.ZappyCurrentLoopStart, out _PrevLoopStart);
                _PreviousLoopStart = _PrevLoopStart as IZappyLoopStartAction;
                context.ContextData[CrapyConstants.ZappyCurrentLoopStart] = this;
                _Next = NextGuid;
                _Initialized = true;
            }
        }

        public virtual void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Initialize(context);
            if (CheckLoopTermination())
                return;
        }

                                        public virtual bool CheckLoopTermination()
        {
            if (TerminateLoop)
            {
                NextGuid = LoopEndGuid;
                return true;
            }
            return false;
        }

        public virtual void FinalizeLoop(IZappyExecutionContext context)
        {
            context.ContextData[CrapyConstants.ZappyCurrentLoopStart] = _PreviousLoopStart;
            NextGuid = _Next;
            _Initialized = false;
        }

        public IZappyLoopEndAction GetEndLoopActionInternal<T>() where T : IZappyLoopEndAction, new()
        {
            T _EndAction = new T();
            _EndAction.LoopStartGuid = this.SelfGuid;
            LoopEndGuid = _EndAction.SelfGuid;
            return _EndAction;
        }

        public abstract IZappyLoopEndAction GetEndLoopAction();

        [Category("Internals")]
        public abstract ZappyLoopType LoopType
        {
            get;
        }
    }


}
