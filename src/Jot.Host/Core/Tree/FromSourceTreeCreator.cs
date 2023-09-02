using Jot.Core.Blob;
using Jot.Extensions;
using Jot.Functions;
using System.IO.Abstractions;

namespace Jot.Core.Tree
{
    public class TreeObjectCreator
    {
        private readonly IFileSystem _fileSystem;

        public TreeObjectCreator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            SourceFiles = new SourceFilesManagers(fileSystem);
        }

        private SourceFilesManagers SourceFiles { get; }

        public TreeObject Create()
        {
            return CreateInstanceForDirectory(_fileSystem.GetCurrentDirectoryInfo());
        }

        private TreeObject CreateInstanceForDirectory(IDirectoryInfo directory)
        {
            var blobObjects = SourceFiles.GetFiles(SearchOption.TopDirectoryOnly, directory)
                .Select(BlobObject.CreateInstanceFromSource)
                .ToArray();

            var subdirs = SourceFiles.GetSubdirectories(directory);

            // Reached bottom
            if (!subdirs.Any())
            {
                return new TreeObject(directory, blobObjects);
            }

            var treeBranches = subdirs
                .Select(CreateInstanceForDirectory)
                .ToArray();

            var tree = new TreeObject(directory, blobObjects, treeBranches);

            return tree;
        }
    }
}