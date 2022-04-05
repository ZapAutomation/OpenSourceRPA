using System;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public sealed class Text : DataType<string>
    {
        public Text() : this(string.Empty) { }

        public Text(string value) : base(value ?? string.Empty) { }

        public override void SetValue(object value)
        {
            try
            {
                this.Value = Convert.ToString(value);
            }
            catch
            {
                throw;
            }
        }

        public override string ToExpression()
        {
            return string.Format("\"{0}\"", this.Value.Replace("\\", "\\\\").Replace("\"", "\\\""));
        }

        public static implicit operator String(Text item) => item.Value;

        public static implicit operator Text(String item) => new Text(item);

        public override string ToString() => this.Value;
    }
}