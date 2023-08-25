using System.CommandLine.IO;

namespace Jot.Tests.Extensions
{
    public static class IStandardStreamWriterExtensions
    {
        public static string[] ToArray(this IStandardStreamWriter writer, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            return writer.ToString()!.Split(Environment.NewLine, options);
        }
    }
}
