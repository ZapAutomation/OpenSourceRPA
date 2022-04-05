using System;
using System.Globalization;
using System.Windows.Automation;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using ControlType = Zappy.ActionMap.HelperClasses.ControlType;

namespace Zappy.Plugins.Uia.Uia.EventWrappers
{
    internal class SliderEventWrapper : PropertyChangeEventWrapper
    {
        private UiaElement sourceElement;

        public SliderEventWrapper(UiaElement element, UiaElement sourceElement, ZappyTaskEventType eventType, IZappyTaskEventNotify eventSink) : base(element, eventType, eventSink, RangeValuePattern.ValueProperty, TreeScope.Element)
        {
            this.sourceElement = sourceElement;
            if (ControlType.Indicator.NameEquals(sourceElement.ControlTypeName))
            {
                LatestSourceElement = sourceElement;
                CurrentValueOrState = element.Value;
                NotifyEventDuringRemoval = true;
            }
        }

        protected override void OnPropertyChange(object sender, AutomationPropertyChangedEventArgs e)
        {
            double num;
            if (double.TryParse(e.NewValue.ToString(), out num))
            {
                CurrentValueOrState = Math.Round(num, 2).ToString(CultureInfo.InvariantCulture);
                LatestSourceElement = sourceElement;
                Notify();
            }
        }

        public override bool ShouldFireFakeEvent() =>
            sourceElement != null && ControlType.Indicator.NameEquals(sourceElement.ControlTypeName);
    }
}

