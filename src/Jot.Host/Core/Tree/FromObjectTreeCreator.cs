using Jot.Core.Blob;
using Jot.Extensions;
using Jot.Functions;
using System.Diagnostics.Contracts;
using System.IO.Abstractions;

namespace Jot.Core.Tree
{
    public class TreeObjectRetreiver
    {
        private readonly IFileSystem _fileSystem;

        public TreeObjectRetreiver(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public TreeObject Get(ObjectId objectId)
        {
            var fileInfo = _fileSystem.FileInfo.New(objectId);

            var jotTreeObject = JotObject.CreateInstanceFromObject<TreeObject>(fileInfo);

            var rootPath = _fileSystem.GetCurrentDirectoryInfo();

            var rootTree = CreateInstance(rootPath, jotTreeObject);

            return rootTree;
        }

        private TreeObject CreateInstance(IDirectoryInfo directory, JotObject<TreeObject> treeJotObject)
        {
            var rows = TreeObjectRowConstructor.Create(treeJotObject);

            var blobObjects = new List<BlobObject>();

            foreach (var row in rows.OfType<TreeObjectRow<BlobObject>>())
            {
                var blobFileInfo = _fileSystem.FileInfo.New(row.ObjectId);

                var blobSourceFileInfo = _fileSystem.FileInfo.New(row.SourcePath.Value);

                var blobJotObject = JotObject.CreateInstanceFromObject<BlobObject>(blobFileInfo);

                var blobObject = BlobObject.CreateInstanceFromObject(blobJotObject, new SourceFile(blobSourceFileInfo));

                blobObjects.Add(blobObject);
            }

            var branches = new List<TreeObject>();

            foreach (var row in rows.OfType<TreeObjectRow<TreeObject>>())
            {
                var treeFileInfo = _fileSystem.FileInfo.New(row.ObjectId);

                var branchJotObject = JotObject.CreateInstanceFromObject<TreeObject>(treeFileInfo);

                var branchDirectory = _fileSystem.DirectoryInfo.New(row.SourcePath.Value);

                var branch = CreateInstance(branchDirectory, branchJotObject);

                branches.Add(branch);
            }

            return new TreeObject(directory, blobObjects!, branches, treeJotObject.Bytes);
        }

    }
}