namespace Zappy.ActionMap.HelperClasses
{
    public class ActionParameter
    {
        public ActionParameter()
        {
        }

        public ActionParameter(string name)
        {
            Name = name;
        }

        public ActionParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}