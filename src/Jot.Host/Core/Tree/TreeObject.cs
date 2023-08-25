using Jot.Attributes;
using Jot.Core.Blob;
using Jot.Extensions;
using Jot.Functions;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text;

namespace Jot.Core.Tree
{
    [JotObjectType(Type)]
    public class TreeObject : IEquatable<TreeObject>
    {
        public const string Type = "tree";

        public TreeObject(IDirectoryInfo sourceDirectory, IEnumerable<BlobObject> blobObjects, IEnumerable<TreeObject> treeBranches, byte[]? bytes = null)
        {
            SourceDirectory = sourceDirectory;
            BlobObjects = blobObjects;
            TreeBranches = treeBranches;
            Bytes = bytes;
        }

        public TreeObject(IDirectoryInfo sourceDirectory, IEnumerable<BlobObject> blobObjects, byte[]? bytes = null)
        {
            SourceDirectory = sourceDirectory;
            BlobObjects = blobObjects;
            TreeBranches = Array.Empty<TreeObject>();
            Bytes = bytes;
        }

        private IDirectoryInfo SourceDirectory { get; }

        public IEnumerable<BlobObject> BlobObjects { get; }

        public IEnumerable<TreeObject> TreeBranches { get; }

        private byte[]? Bytes { get; set; }

        public byte[] GetBytes()
        {
            if (Bytes == null)
            {
                var rows = TreeObjectRowConstructor.Create(this)
                    .Select(e => e.ToString())
                    .OrderByDescending(e => e)
                    .ToArray();

                var stringValue = string.Join(Environment.NewLine, rows);

                Bytes = Encoding.UTF8.GetBytes(stringValue).AddObjectType(Type);
            }

            return Bytes;
        }

        private ObjectId? Hash { get; set; }

        public ObjectId GetObjectId()
        {
            Hash ??= ObjectId.CreateInstance(GetBytes());

            return Hash;
        }

        public IEnumerable<TreeObject> GetAllBranches()
        {
            var result = new List<TreeObject>() { this };

            if (!TreeBranches.Any())
            {
                return result;
            }

            foreach (TreeObject jotTree in TreeBranches)
            {
                result.AddRange(jotTree.GetAllBranches());
            }

            return result;
        }

        public IEnumerable<BlobObject> GetAllBlobObjects()
        {
            var result = BlobObjects.ToList();

            foreach (TreeObject branch in TreeBranches)
            {
                result.AddRange(branch.GetAllBlobObjects());
            }

            return result;
        }

        public IDirectoryInfo GetSourceDirectory() => SourceDirectory;

        public FilePath GetSourceDirectoryPath() => new (SourceDirectory.FullName);

        public override string ToString() => Encoding.UTF8.GetString(GetBytes());
        
        public FilePath Persist(IFileSystem fileSystem)
        {
            var fileInfo = fileSystem.FileInfo.New(GetObjectId());

            using var fs = fileInfo.Create();

            var bytes = GetBytes();

            fs.Write(bytes, 0, bytes.Length);

            return new FilePath(fileInfo.FullName);
        }

        public bool Equals(TreeObject? other)
        {
            if (other == null) return false;

            var objectId = GetObjectId();

            var otherObjectId = other.GetObjectId();

            return objectId.Equals(otherObjectId);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not TreeObject) return false;

            return Equals(obj as TreeObject);
        }

        public override int GetHashCode()
        {
            var objectId = GetObjectId();

            return objectId.GetHashCode();
        }
    }
}