using System;
using System.ComponentModel;
using Zappy.Helpers;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Loops
{
                [Description("ForLoop Start")]
    public class ForLoopStartAction : LoopStartAction
    {
        public ForLoopStartAction() : base()
        {
            StepValue = 1;
        }


        [Category("Optional")]
        [Description("Loop Data")]
        public DynamicProperty<object> LoopData { get; set; }

                                [Category("Input")]
        [Description("Enter Start Value of ForLoop")]
        public DynamicProperty<int> InitialValue { get; set; }

                                [Category("Input")]
        [Description("Enter End Value")]
        public DynamicProperty<int> FinalValue { get; set; }

                                [Category("Input")]
        public DynamicProperty<int> StepValue { get; set; }

                                [Category("Output")]
        [Description("Current loop execution value")]
        public int LoopIntValue { get; set; }


        bool TopToDown;
        int _Step = 0, _InitialValue;
        object _PreviousLoopIndex, _PreviousLoopData;

        public override string AuditInfo()
        {
            return HelperFunctions.HumanizeNameForIZappyAction(this) + " Current Iteration Value: " + LoopIntValue;

        }

        protected override void Initialize(IZappyExecutionContext context)
        {
            if (_Initialized)
                return;

            base.Initialize(context);
            _Step = StepValue.Value;
            LoopIntValue = _InitialValue = InitialValue.Value;
            TopToDown = _InitialValue <= FinalValue.Value;

            context.ContextData.TryGetValue(CrapyConstants.ZappyLoopIndex, out _PreviousLoopIndex);
            context.ContextData.TryGetValue(CrapyConstants.ZappyLoopData, out _PreviousLoopData);

            context.ContextData[CrapyConstants.ZappyLoopIndex] = _InitialValue;
            if (LoopData != null && LoopData.ValueSpecified)
                context.ContextData[CrapyConstants.ZappyLoopData] = LoopData.Value;
            else
                context.ContextData[CrapyConstants.ZappyLoopData] = null;

        }

        public override bool CheckLoopTermination()
        {
                        if (!TerminateLoop)
            {
                if (TopToDown)
                    TerminateLoop = LoopIntValue >= (FinalValue.Value + 1);
                else
                    TerminateLoop = LoopIntValue <= (FinalValue.Value - 1);
            }
            return base.CheckLoopTermination();
        }
                                        public override void FinalizeLoop(IZappyExecutionContext context)
        {
            base.FinalizeLoop(context);
            context.ContextData[CrapyConstants.ZappyLoopIndex] = _PreviousLoopIndex;
            context.ContextData[CrapyConstants.ZappyLoopData] = _PreviousLoopData;
                    }


        public void PerformStep(IZappyExecutionContext context)
        {
            LoopIntValue = LoopIntValue + _Step;
            context.ContextData[CrapyConstants.ZappyLoopIndex] = LoopIntValue;
        }
                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (InitialValue >= 0 && FinalValue >= 0 && FinalValue < InitialValue)
            {
                throw new Exception("Final value less than initial value");
            }
            base.Invoke(context, actionInvoker);            
        }

        public override IZappyLoopEndAction GetEndLoopAction()
        {
            return GetEndLoopActionInternal<ForLoopEndAction>();
        }

        public override ZappyLoopType LoopType
        {
            get { return ZappyLoopType.For; }
        }

    }

}