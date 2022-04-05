namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public class Argument
    {
        public string Name { get; set; }

        internal string Type { get; set; }

        public string Value { get; set; }

        public Argument()
        {
            this.Name = string.Empty;
            this.Type = DataType.GetDefault().GetType().FullName;
            this.Value = string.Empty;
        }
    }
}