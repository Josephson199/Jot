using Jot.Core.Blob;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO.Abstractions;

namespace Jot.Core.Tree
{
    public class TreeObjectPersistor
    {
        private readonly IFileSystem _fileSystem;

        public TreeObjectPersistor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        //Todo
        //Enrich output
        public TreePersistResult Persist(TreeObject treeObject)
        {
            var treeBranches = treeObject.GetAllBranches();

            var internalFiles = new List<FilePath>();
            var sourceFiles = new List<FilePath>();

            foreach (var branch in treeBranches)
            {
                foreach (var blobObject in branch.BlobObjects)
                {
                    sourceFiles.Add(blobObject.SourceFilePath);
                    internalFiles.Add(blobObject.Persist(_fileSystem));
                }

                internalFiles.Add(branch.Persist(_fileSystem));
            }

            return new TreePersistResult(internalFiles, sourceFiles);
        }
    }

    public class TreePersistResult
    {
        //TODO Wrap collections
        public TreePersistResult(IEnumerable<FilePath> internalFiles, IEnumerable<FilePath> sourceFiles)
        {
            InternalFiles = internalFiles.ToArray();
            SourceFiles = sourceFiles.ToArray();
        }

        public IReadOnlyList<FilePath> InternalFiles { get; }

        public IReadOnlyList<FilePath> SourceFiles { get; }
    }
}