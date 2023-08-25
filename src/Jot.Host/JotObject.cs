using Jot.Core.Commit;
using Jot.Extensions;
using System.Drawing;
using System.IO.Abstractions;
using System.Text;

namespace Jot
{
    public class JotObject<T> : JotObject
    {
        public JotObject(IFileInfo fileInfo, byte[] content) : base(fileInfo, content)
        {
        }
    }

    public class JotObject
    {
        protected JotObject(IFileInfo fileInfo, byte[] bytes)
        {
            FileInfo = fileInfo;
            Hash = new ObjectId(fileInfo.Name);
            Bytes = bytes;
        }

        public static JotObject<T> CreateInstanceFromObject<T>(ObjectId objectId, IFileSystem fileSystem)
        {
            return CreateInstanceFromObject<T>(fileSystem.FileInfo.New(objectId));
        }

        public static JotObject<T> CreateInstanceFromObject<T>(IFileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"ObjectFile:'{fileInfo.FullName}' does not exist");
            }

            var content = fileInfo.GetBytes();

            var typeKey = content.GetObjectType();

            var typeMappings = TypeMappings.Get();

            var exists = typeMappings.TryGetValue(typeKey, out var mapping);

            if (!exists)
            {
                throw new NotImplementedException($"Type:'{typeKey}' not mapped");
            }

            if (!mapping!.Equals(typeof(T)))
            {
                throw new ArgumentException("Type missmatch");
            }

            return new JotObject<T>(fileInfo, content);
        }

        public ObjectId Hash { get; }
        public byte[] Bytes { get; }
        public IFileInfo FileInfo { get; }
    }
}