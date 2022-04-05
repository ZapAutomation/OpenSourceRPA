using System.ComponentModel;
using Crapy.Chrome;
using Crapy.Decode.Hooks.Keyboard;
using Crapy.Invoker;

namespace Crapy.Analytics
{
    internal class TaskEditorDataObject: AnalyticsDataObject
    {
        [Browsable(false)]
        public bool DisplayFullText { get; set; }

        protected override string GetDisplayValue()
        {

            if (!DisplayFullText && (TaskAction is SendKeysAction || TaskAction is ChromeActionKeyboard))
                return DisplayHelper.NodeTextHelper(TaskAction, false);
            return base.GetDisplayValue();
        }


    }
}