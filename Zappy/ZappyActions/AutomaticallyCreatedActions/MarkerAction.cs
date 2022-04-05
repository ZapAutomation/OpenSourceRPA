using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Serializable]
    [Description("Gets Marker Information")]
    public class MarkerAction : ZappyTaskAction
    {
        private string markerInfo;

        public MarkerAction() : this(string.Empty)
        {
        }

        public MarkerAction(string markerInfo)
        {
            this.markerInfo = markerInfo;
        }

        internal override string GetParameterString() =>
            markerInfo;

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                    }

        [XmlAttribute]
        public string MarkerInformation
        {
            get =>
                markerInfo;
            set
            {
                markerInfo = value;
                NotifyPropertyChanged("MarkerInformation");
            }
        }
    }
}

