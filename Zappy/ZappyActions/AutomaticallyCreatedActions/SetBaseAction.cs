using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Aggregator;

namespace Zappy.ZappyActions.AutomaticallyCreatedActions
{
    [Serializable]
    [Description("Sets Base")]
    public abstract class SetBaseAction : AggregatedAction
    {
        [NonSerialized]
        private TaskActivityElement sourceElement;
        private List<TaskActivityElement> sourceElements;

        protected SetBaseAction()
        {
        }

        protected SetBaseAction(TaskActivityElement uiElement) : base(uiElement)
        {
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public TaskActivityElement SourceElement
        {
            get =>
                sourceElement;
            set
            {
                sourceElement = value;
                NotifyPropertyChanged("SourceElement");
            }
        }

        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        public List<TaskActivityElement> SourceElements
        {
            get
            {
                if (sourceElements == null)
                {
                    sourceElements = new List<TaskActivityElement>();
                }
                return sourceElements;
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " SourceElements:" + this.SourceElements;
        }
    }
}