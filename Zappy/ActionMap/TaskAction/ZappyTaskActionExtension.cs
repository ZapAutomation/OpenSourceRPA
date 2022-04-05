using System;
using System.Xml.Serialization;
using Zappy.ActionMap.HelperClasses;

namespace Zappy.ActionMap.TaskAction
{
    public class ZappyTaskActionExtension
    {
        [NonSerialized]
        private AndCondition extendedProperties;
        internal static readonly string ImagePathProperty = "ImagePath";
        internal static readonly string InteractionPoint = "InteractionPoint";
        internal static readonly string SnapShotTimeStamp = "SnapShotTimeStamp";

        [XmlElement(ElementName = "AndCondition")]
        public AndCondition ExtendedProperties
        {
            get =>
                extendedProperties;
            set
            {
                extendedProperties = value;
                if (extendedProperties != null)
                {
                    extendedProperties.Name = "ExtendedProperties";
                }
            }
        }
    }
}