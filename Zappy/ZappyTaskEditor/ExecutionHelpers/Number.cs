using System;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public sealed class Number : DataType<decimal>
    {
        public Number() : this(new Decimal()) { }

        public Number(Decimal value) : base(value) { }

        public override void SetValue(object value)
        {
            try
            {
                this.Value = Convert.ToDecimal(value);
            }
            catch
            {
                throw;
            }
        }

        public override string ToExpression()
        {
            return string.Format("{0}", this.Value);
        }

        public static implicit operator Decimal(Number item) => item.Value;

        public static implicit operator Number(Decimal item) => new Number(item);

        public static implicit operator Int32(Number item) => Convert.ToInt32(item.Value);
    }
}