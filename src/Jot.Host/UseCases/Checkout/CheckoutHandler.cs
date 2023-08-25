using Jot.Core.Commit;
using Jot.Functions;
using System.IO.Abstractions;

namespace Jot.Features.Checkout
{
    public class CheckoutResult
    {
        public CheckoutResult(CheckoutStatus status)
        {
            Status = status;
        }

        public CheckoutStatus Status { get; }

        public enum CheckoutStatus
        {
            CommitNotFound,
            Success,
            NoChanges
        }
    }

    public class CheckoutHandler
    {
        private readonly IFileSystem _fileSystem;
        private readonly HeadFileManager _headManager;

        public CheckoutHandler(IFileSystem fileSystem)
        {
            _headManager = new HeadFileManager(fileSystem);
            _fileSystem = fileSystem;
        }

        public CheckoutResult Execute(CheckoutParams checkoutParams)
        {
            var commitId = new ObjectId(checkoutParams.CommitHash);

            var currentHead = _headManager.Get();

            if (commitId.Equals(currentHead.ObjectId))
            {
                //Todo no changes
            }

            var commitFileInfo = _fileSystem.FileInfo.New(commitId);

            if (!commitFileInfo.Exists)
            {
                return new CheckoutResult(CheckoutResult.CheckoutStatus.CommitNotFound);
            }

            var commitRetreiver = new CommitObjectRetreiver(_fileSystem);

            var commit = commitRetreiver.Get(commitId);

            //var commit = CommitObject.CreateInstanceFromObject(commitFileInfo);

            if (commit == null)
            {
                return new CheckoutResult(CheckoutResult.CheckoutStatus.CommitNotFound);
            }

            //TreeObject.CreateInstanceFromObject(commit.TreeId).Restore();

            //var _ = _headManager.Set(new SetHeadParams(commitId));

            return new CheckoutResult(CheckoutResult.CheckoutStatus.Success);
        }
    }

    public class CheckoutParams
    {
        public CheckoutParams(string commitHash)
        {
            CommitHash = commitHash;
        }

        public string CommitHash { get; }
    }
}