using System;
using System.Linq;
using System.Security;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public sealed class Password : DataType<SecureString>
    {
        public Password() : this(new SecureString()) { }

        public Password(SecureString value) : base(value) { }

        public override void SetValue(object value)
        {
            try
            {
                this.Value.Clear();
                Convert.ToString(value).ToList().ForEach(this.Value.AppendChar);
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

        public static implicit operator SecureString(Password item) => item.Value;

        public static implicit operator Password(SecureString item) => new Password(item);
    }
}