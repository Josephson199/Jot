namespace Jot
{
    public class DirectoryPath
    {
        public DirectoryPath(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}