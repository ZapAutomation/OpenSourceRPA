using System.Collections;
using System.ComponentModel;
using Zappy.Helpers;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Loops
{
                [Description("ForEach Loop Start Action ")]
    public sealed class ForEachLoopStartAction : LoopStartAction
    {
        public ForEachLoopStartAction() : base()
        {

        }

        [Description("Input")]
        public DynamicProperty<object> Enumerable { get; set; }

        [Description("Ouptput")]
        public object LoopItem { get; set; }

        private IEnumerator _Enumerator;
        public override string AuditInfo()
        {
            return HelperFunctions.HumanizeNameForIZappyAction(this);
        }

        protected override void Initialize(IZappyExecutionContext context)
        {
            if (_Initialized)
                return;
            base.Initialize(context);
            _Index = -1;
            _Enumerator = (Enumerable.Value as IEnumerable).GetEnumerator();
            _Enumerator.Reset();
            context.ContextData.TryGetValue(CrapyConstants.ZappyLoopItem, out _PreviousLoopItem);
            context.ContextData.TryGetValue(CrapyConstants.ZappyLoopIndex, out _PreviousLoopIndex);
        }

        int _Index;
        object _PreviousLoopIndex, _PreviousLoopItem;


        private bool MoveNext()
        {
            _Index++;
            TerminateLoop.Value = !_Enumerator.MoveNext();
            return !TerminateLoop.Value;
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            base.Invoke(context, actionInvoker);
            if (MoveNext())
            {
                LoopItem = _Enumerator.Current;
                context.ContextData[CrapyConstants.ZappyLoopItem] = _Enumerator.Current;
                context.ContextData[CrapyConstants.ZappyLoopIndex] = _Index;
            }
            else
                CheckLoopTermination();
        }

        public override void FinalizeLoop(IZappyExecutionContext context)
        {
            base.FinalizeLoop(context);
            context.ContextData[CrapyConstants.ZappyLoopIndex] = _PreviousLoopIndex;
            context.ContextData[CrapyConstants.ZappyLoopItem] = _PreviousLoopItem;
        }


        public override IZappyLoopEndAction GetEndLoopAction()
        {
            return GetEndLoopActionInternal<ForEachLoopEndAction>();
        }

        public override ZappyLoopType LoopType
        {
            get { return ZappyLoopType.ForEach; }
        }

    }

}