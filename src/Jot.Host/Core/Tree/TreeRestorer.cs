using Jot.Functions;
using System.IO.Abstractions;

namespace Jot.Core.Tree
{
    public class TreeRestorer
    {
        private readonly SourceFilesManagers _sourceFiles;

        public TreeRestorer(IFileSystem fileSystem)
        {
            _sourceFiles = new SourceFilesManagers(fileSystem);
        }

        public void Restore(TreeObject tree)
        {
            //Move files to temp folder and when restore is complete, clear temp.
            _sourceFiles.Clear();

            var blobObjects = tree.GetAllBlobObjects();

            foreach (var blobObject in blobObjects)
            {
                blobObject.Restore();
            }
        }
    }
}
