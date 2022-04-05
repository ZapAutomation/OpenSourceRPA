using System;
using System.Data;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public sealed class Collection : DataType<DataTable>
    {
        public Collection() : this(new DataTable()) { }

        public Collection(DataTable value) : base(value) { }

        public override void SetValue(object value)
        {
            try
            {
                this.Value = (DataTable)value;
            }
            catch
            {
                throw;
            }
        }

        public override string ToExpression()
        {
            throw new NotImplementedException();
        }

        public static implicit operator DataTable(Collection item) => item.Value;

        public static implicit operator Collection(DataTable item) => new Collection(item);
    }
}