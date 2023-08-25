using System.IO;

namespace Jot
{
    public class FilePath
    {
        public FilePath(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public string GetRelativePath()
        {
            return Path.GetRelativePath(Environment.CurrentDirectory, Value);
        }

        public override string ToString() { return Value; }
    }
}