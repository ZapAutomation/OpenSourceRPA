using System.Collections.Generic;
using System.Windows.Forms;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public abstract class DataType
    {
        public abstract string Id { get; }

        public abstract string Name { get; }

        public abstract object GetValue();

        public abstract void SetValue(object value);

        public abstract string ToExpression();

        public abstract DataGridViewCell CellTemplate { get; }

        public static List<DataType> GetCommonTypes()
        {
            return new List<DataType>()
            {
                new Text(),
                new Number(),
                new TrueFalse(),
                                            };
        }

        public static DataType GetDefault() => new Text();

        public static DataType CreateInstance(string fullName)
        {
            if (fullName == typeof(string).FullName)
                return new Text();

            else if (fullName == typeof(bool).FullName)
                return new TrueFalse();
            else return new Number();

                                                                                                                    }
    }

    public abstract class DataType<T> : DataType
    {
        public override string Id => this.GetType().FullName;

        public override string Name => this.GetType().Name;

        public override DataGridViewCell CellTemplate => new GhostTextBoxCell();

        protected T Value { get; set; }

        protected DataType(T value)
        {
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is DataType<T> other && this.Value.Equals(other.Value);
        }

        public override object GetValue()
        {
            return this.Value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
