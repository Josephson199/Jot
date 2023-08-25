using Jot.Attributes;
using Jot.Core.Tree;
using Jot.Extensions;
using Jot.Functions;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;

namespace Jot.Core.Blob
{

    [JotObjectType(Type)]
    public class BlobObject
    {
        public const string Type = "blob";

        private BlobObject(SourceFile sourceFile)
        {
            if (!sourceFile.Exists)
            {
                throw new FileNotFoundException(sourceFile.FilePath.Value);
            }

            SourceFile = sourceFile;
        }

        private BlobObject(JotObject<BlobObject> jotObject, SourceFile sourceFile)
        {
            ObjectId = jotObject.Hash;
            Bytes = jotObject.Bytes;
            SourceFile = sourceFile;
        }

        public static BlobObject CreateInstanceFromSource(SourceFile sourceFile)
           => new(sourceFile);

        public static BlobObject CreateInstanceFromObject(JotObject<BlobObject> jotObject, SourceFile sourceFile)
            => new(jotObject, sourceFile);

        private SourceFile SourceFile { get; }
       
        private ObjectId? ObjectId { get; set; } = null;

        public ObjectId GetObjectId()
        {
            ObjectId ??= ObjectId.CreateInstance(GetBytes());

            return ObjectId;
        }

        public SourceFile GetSourceFile() => SourceFile;

        public FilePath SourceFilePath => SourceFile.FilePath;

        private byte[]? Bytes { get; set; } = null;

        public byte[] GetBytes()
        {
            if (Bytes == null)
            {
                var bytes = SourceFile.GetBytes();

                Bytes = bytes.AddObjectType(Type);
            }

            return Bytes;
        }

        public FilePath Persist(IFileSystem fileSystem)
        {
            var objectId = GetObjectId();

            var fileInfo = fileSystem.FileInfo.New(objectId);

            if (!fileInfo.Exists)
            {
                var bytes = GetBytes();

                using var fs = fileInfo.Create();

                fs.Write(bytes, 0, bytes.Length);
            }

            return new FilePath(fileInfo.FullName);
        }

        public FilePath Restore()
        {
            var bytes = GetBytes().StripType();

            SourceFile.Write(bytes);

            return SourceFile.FilePath;
        }
    }

    public class SourceFile : FileBase
    {
        public SourceFile(IFileInfo fileInfo) : base(fileInfo) { }
    }

    public class TrackedFile : FileBase
    {
        public TrackedFile(IFileInfo fileInfo) : base(fileInfo) { }

        public static TrackedFile Create(IFileSystem fileSystem, ObjectId hash, byte[] bytes)
        {
            var fileInfo = fileSystem.FileInfo.New(hash);

            if (!fileInfo.Exists)
            {
                using var fs = fileInfo.Create();

                fs.Write(bytes, 0, bytes.Length);
            }

            return new TrackedFile(fileInfo);
        }
    }

    public abstract class FileBase
    {
        protected FileBase(IFileInfo fileInfo)
        {
            FileInfo = fileInfo;
            FileSystem = fileInfo.FileSystem;
        }

        private IFileInfo FileInfo { get; }

        private IFileSystem FileSystem { get; }

        public bool Exists => FileInfo.Exists;

        public FilePath FilePath => new (FileInfo.FullName);

        public byte[] GetBytes()
        {
            if (!Exists)
            {
                throw new FileNotFoundException(nameof(FileInfo.FullName));
            }

            using var fs = FileInfo.OpenRead();

            using var binaryReader = new BinaryReader(fs);

            return binaryReader.ReadBytes((int)fs.Length);
        }

        public void Write(byte[] bytes)
        {
            if (!FileInfo.Directory!.Exists)
            {
                FileInfo.Directory.Create();
            }

            using var fs = FileInfo.Create();

            fs.Write(bytes, 0, bytes.Length);
        }

        public void Delete()
        {
            FileInfo.Delete();
        }
    }
}