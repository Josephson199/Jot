using Jot.Extensions;
using System.Diagnostics;
using System.IO.Abstractions;

namespace Jot.Core.Commit
{
    public class CommitParentResolver
    {
        private readonly IFileSystem _fileSystem;

        public CommitParentResolver(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            CommitObjectRetreiver = new CommitObjectRetreiver(fileSystem);
        }

        private CommitObjectRetreiver CommitObjectRetreiver { get; }

        public IReadOnlyList<CommitObject> Resolve(CommitObject commitObject)
        {
            var result = new List<CommitObject>();

            if (commitObject.ParentCommitId is NullObjectId)
            {
                return result;
            }

            var parentCommitId = commitObject.ParentCommitId;

            //TODO set upper bound
            while (parentCommitId is not NullObjectId)
            {
                var parentCommitFileInfo = _fileSystem.FileInfo.New(parentCommitId);

                Debug.Assert(!parentCommitFileInfo.Exists);

                if (!parentCommitFileInfo.Exists)
                {
                    //Todo, something is corrupted.
                }

                var parent = CommitObjectRetreiver.Get(parentCommitId);

                result.Add(parent);

                parentCommitId = parent.ParentCommitId;
            }

            return result;
        }
    }
}
