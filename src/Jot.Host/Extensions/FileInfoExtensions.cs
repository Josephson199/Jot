using System.Diagnostics;
using System.IO.Abstractions;

namespace Jot.Extensions
{
    public static class FileInfoExtensions
    {
        public static byte[] GetBytes(this IFileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(nameof(fileInfo.FullName));
            }

            using var fs = fileInfo.OpenRead();

            using var binaryReader = new BinaryReader(fs);

            return binaryReader.ReadBytes((int)fs.Length);
        }

        public static JotObject<T> ToJotObject<T>(this IFileInfo fileInfo)
        {
            return JotObject.CreateInstanceFromObject<T>(fileInfo);
        }

        public static IFileInfo New(this IFileInfoFactory factory, ObjectId objectId)
        {
            var path = objectId.CreateObjectFilePath();

            return factory.New(path.Value);
        }

        public static string GetRelativePath(this IFileInfo fileInfo)
        {
            return Path.GetRelativePath(Environment.CurrentDirectory, fileInfo.FullName);
        }
    }
}