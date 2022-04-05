namespace Zappy.Helpers
{
    public class CustomListBoxHelper
    {
        public string DisplayText { get; set; }
        public string Path { get; set; }


        public override string ToString()
        {
            return DisplayText;
        }
    }
}
