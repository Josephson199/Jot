namespace Jot.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JotObjectTypeAttribute : Attribute
    {
        public JotObjectTypeAttribute(string type)
        {
            Type = type;
        }

        public string Type { get; }
    }
}