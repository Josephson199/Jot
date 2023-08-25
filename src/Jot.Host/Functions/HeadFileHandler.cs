using System.IO.Abstractions;

namespace Jot.Functions
{
    public class SetHeadRequest
    {
        public SetHeadRequest(ObjectId commitHash)
        {
            CommitId = commitHash;
        }

        public ObjectId CommitId { get; }
    }

    public class SetHeadResult
    {
        public SetHeadResult()
        {
            
        }
    }

    public abstract class HeadResult
    {
        public HeadResult(ObjectId objectId)
        {
                
        }

        public abstract ObjectId CommitId { get; }
    }

    

    public class GetHeadResult
    {
        public GetHeadResult(ObjectId objectId)
        {
            ObjectId = objectId;
        }

        public ObjectId ObjectId { get; }
    }
    
    public class HeadFileManager
    {
        private readonly IFileSystem _fileSystem;

        public HeadFileManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        private static string HeadPath => Path.Combine(JotPaths.JotBasePath, "head");

        public SetHeadResult Set(SetHeadRequest input)
        {
            _fileSystem.File.WriteAllText(HeadPath, input.CommitId.Value);

            return new SetHeadResult();
        }

        public GetHeadResult Get()
        {
            if (!_fileSystem.File.Exists(HeadPath))
            {
                return new GetHeadResult(ObjectId.Null);
            }

            var commitId = _fileSystem.File.ReadAllText(HeadPath);

            return new GetHeadResult(new ObjectId(commitId));
        }
    }

    public class JotContext
    {
        public static IFileSystem FileSystem { get; private set; } = new FileSystem();

        public static void SetOutput(TextWriter textWriter)
        {
            Console.SetOut(textWriter);
            Console.SetError(textWriter);
        }

        public static void UseFileSystem(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        public static IDirectoryInfo CurrentDirectoryInfo => FileSystem.DirectoryInfo.New(Environment.CurrentDirectory);
    }
}
