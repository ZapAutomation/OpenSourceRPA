using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Zappy.Decode.Mssa
{
    internal sealed class CommaListBuilder
    {
        private const string Comma = ",";
        private StringBuilder commaList = new StringBuilder();
        private const string EscapedComma = @"\,";

        public void AddRange(IEnumerable<string> stringCollection)
        {
            foreach (string str in stringCollection)
            {
                AddValue(str);
            }
        }

        public void AddRange(IEnumerable collection)
        {
            foreach (object obj2 in collection)
            {
                AddValue(obj2);
            }
        }

        public void AddValue(object value)
        {
            if (value == null)
            {
                AddValue(string.Empty);
            }
            else
            {
                AddValue(value.ToString());
            }
        }

        public void AddValue(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            if (commaList.Length == 0)
            {
                commaList.Append(EscapeForComma(value));
            }
            else
            {
                commaList.Append(",");
                commaList.Append(EscapeForComma(value));
            }
        }

        public static string EscapeForComma(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return s.Replace(",", @"\,");
            }
            return s;
        }

        public static List<string> GetCommaSeparatedValues(string value)
        {
            List<string> list = new List<string>();
            if (value != null)
            {
                int num = 0;
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < value.Length; i++)
                {
                    char ch = value[i];
                    if (ch != ',')
                    {
                        if (ch != '\\')
                        {
                            goto Label_006B;
                        }
                        num = 1;
                        builder.Append(value[i]);
                    }
                    else
                    {
                        if (num == 1)
                        {
                            builder[builder.Length - 1] = ',';
                        }
                        else
                        {
                            list.Add(builder.ToString());
                            builder = new StringBuilder();
                        }
                        num = 0;
                    }
                    continue;
                Label_006B:
                    num = 0;
                    builder.Append(value[i]);
                }
                list.Add(builder.ToString());
            }
            return list;
        }

        public override string ToString() =>
            commaList.ToString();
    }
}