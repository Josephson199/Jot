//using Jot.Core.Blob;
//using System.IO.Abstractions;
//using System.Text;

//namespace Jot.Core.Commit
//{
//    public class FromObjectCommitCreator
//    {
//        private readonly IFileSystem _fileSystem;

//        public FromObjectCommitCreator(IFileSystem fileSystem)
//        {
//            _fileSystem = fileSystem;
//        }

//        public CommitObject CreateInstance(Hash hash)
//        {
//            var jotObject = JotObject.CreateInstanceFromObject<CommitObject>(hash, _fileSystem);

//            var commitContent = Encoding.UTF8.GetString(jotObject.Content);

//            var commitLinesArray = commitContent.Split(Environment.NewLine);

//            var treeHash = new string(commitLinesArray[0].Skip("tree ".Length).ToArray());

//            var parentCommitHashString = new string(commitLinesArray[1].Skip("parent ".Length).ToArray());

//            Hash? parentCommitHash = null;

//            if (!string.IsNullOrWhiteSpace(parentCommitHashString))
//            {
//                parentCommitHash = new Hash(parentCommitHashString);
//            }

//            var author = new string(commitLinesArray[2].Skip("author ".Length).ToArray());

//            var timeString = new string(commitLinesArray[3].Skip("time ".Length).ToArray());

//            var time = DateTime.Parse(timeString).ToUniversalTime();

//            // delimeter
//            _ = commitLinesArray[4];

//            var message = commitLinesArray[5];

//            return new CommitObject(new Hash(treeHash), author, time, message, parentCommitHash);
//        }
//    }
//}
