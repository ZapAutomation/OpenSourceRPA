using System;
using System.ComponentModel;
using System.Drawing.Design;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.ElementPicker.Helper;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyActions.ElementPicker
{
    [Description("To Get Decision Of Exists Element")]
    public sealed class ElementExists : DecisionNodeAction
    {
        public ElementExists()
        {
            SelfGuid = Guid.NewGuid(); Id = ActionIDRegister.GetUniqueId(); Timestamp = WallClock.UtcNow;

        }

        [Category("Input")]

        [Description("Element")]
        [Editor(typeof(ElementPickerEditor), typeof(UITypeEditor))] [TypeConverter(typeof(ExpandableObjectConverter))] public string Element { get; set; }

        public override bool Execute(IZappyExecutionContext context)
        {
            ZappyExecutionContext _context = context as ZappyExecutionContext;
            EvaluationResult = _context.CountElements(this.Element) == 1;
            return EvaluationResult;
        }
    }
}
