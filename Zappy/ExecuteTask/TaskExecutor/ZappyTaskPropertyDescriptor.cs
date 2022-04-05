using System;
using System.Globalization;

namespace Zappy.ExecuteTask.TaskExecutor
{
    [CLSCompliant(true)]
    public class ZappyTaskPropertyDescriptor
    {
        private ZappyTaskPropertyAttributes attributes;
        private string category;
        private const string ControlSpecific = "ControlSpecific";
        private Type dataType;
        private const string Generic = "Generic";

        public ZappyTaskPropertyDescriptor(Type dataType) : this(dataType, ZappyTaskPropertyAttributes.Readable)
        {
        }

        public ZappyTaskPropertyDescriptor(Type dataType, ZappyTaskPropertyAttributes attributes) : this(dataType, attributes, string.Empty)
        {
            if ((attributes & ZappyTaskPropertyAttributes.CommonToTechnology) != ZappyTaskPropertyAttributes.None)
            {
                category = "Generic";
            }
            else
            {
                category = "ControlSpecific";
            }
        }

        public ZappyTaskPropertyDescriptor(Type dataType, string category) : this(dataType, ZappyTaskPropertyAttributes.Readable, category)
        {
        }

        public ZappyTaskPropertyDescriptor(Type dataType, ZappyTaskPropertyAttributes attributes, string category)
        {
            this.dataType = dataType;
            this.attributes = attributes;
            this.category = category;
        }

        public override string ToString()
        {
            object[] args = { attributes, dataType, category };
            return string.Format(CultureInfo.CurrentCulture, "Attributes: {0}, DataType: {1} Category: {2}", args);
        }

        public ZappyTaskPropertyAttributes Attributes =>
            attributes;

        public string Category =>
            category;

        public Type DataType =>
            dataType;
    }
}