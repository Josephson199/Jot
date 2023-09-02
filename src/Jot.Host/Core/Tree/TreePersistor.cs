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

        public delegate void SourceFilePersistedEventHandler(object sender, FilePath sourceFilePath);

        public event SourceFilePersistedEventHandler? SourceFilePersisted;

        //Todo
        //Enrich output
        public TreePersistResult Persist(TreeObject treeObject)
        {
            var treeBranches = treeObject.GetAllBranches();

            foreach (var branch in treeBranches)
            {
                foreach (var blobObject in branch.BlobObjects)
                {
                    blobObject.Persist(_fileSystem);

                    SourceFilePersisted?.Invoke(this, blobObject.SourceFilePath);
                }

                branch.Persist(_fileSystem);
            }

            return new TreePersistResult();
        }
    }

    public class TreePersistResult
    {
        public TreePersistResult()
        {
        }
    }
}