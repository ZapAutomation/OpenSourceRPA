using System;
using System.ComponentModel;
using SharedZappyInterface;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Helpers;
using Zappy.Invoker;
using ZappyMessages.Helpers;

namespace Zappy.Analytics
{
    internal class AnalyticsDataObject
    {

        [Browsable(false)]
        public IZappyAction TaskAction { get;  set; }

        [Browsable(false)]
        public InternalNode ProcessNode { get; set; }

        [Browsable(false)]
        public Microsoft.Msagl.Drawing.Node GraphNode { get; set; }

        public DateTime Time
        {
            get { return TaskAction.Timestamp; }
        }


        public bool Select { get; set; }

        public string Process { get; set; }

        public string Action
        {
            get { return DisplayHelper.NodeTextHelper(TaskAction, true); }
        }

        public string ScreenElement
        {
            get { return DisplayHelper.GetActionElementName(TaskAction); }
        }
        public string Value
        {
            get
            {
               return GetDisplayValue();
            }
            set
            {
                //DisplayHelper.ActionValueSetterHelper(TaskAction, value);
            }
        }

        protected virtual string GetDisplayValue()
        {
            return DisplayHelper.ActionValueHelper(TaskAction, false);

        }
    }
}
