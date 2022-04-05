
using System;

namespace Zappy.ZappyTaskEditor.EditorPage.ElementPicker
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Property : Attribute
    {
        public bool Enabled { get; }

        public bool Required { get; }

        public Property(bool Enabled = false, bool Required = false)
        {
            this.Enabled = Enabled;
            this.Required = Required;
        }
    }
}
