using System.IO.Abstractions;

namespace Jot.Extensions
{
    public static class FileSystemExtensions
    {
        public static IDirectoryInfo GetCurrentDirectoryInfo(this IFileSystem fileSystem)
        {
            return fileSystem.DirectoryInfo.New(Environment.CurrentDirectory);
        }
    }
}
