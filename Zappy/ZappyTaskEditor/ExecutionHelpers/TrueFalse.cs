using System;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public sealed class TrueFalse : DataType<bool>
    {
        public TrueFalse() : this(new Boolean()) { }

        public TrueFalse(Boolean value) : base(value) { }

        public override void SetValue(object value)
        {
            try
            {
                this.Value = Convert.ToBoolean(value);
            }
            catch
            {
                throw;
            }
        }

        public override string ToExpression()
        {
            return string.Format("{0}", this.Value.ToString().ToLower());
        }

        public static implicit operator Boolean(TrueFalse item) => item.Value;

        public static implicit operator TrueFalse(Boolean item) => new TrueFalse(item);
    }
}