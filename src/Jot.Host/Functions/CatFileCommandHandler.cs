//namespace Jot.Handlers
//{
//    public class CatFileCommandResult : CommandResult
//    {
//        public CatFileCommandResult(string fileString)
//        {
//            FileString = fileString;
//        }

//        public string FileString { get; }
//    }

//    public class CatFileCommandHandler : ICommandCreator
//    {
//        public bool CanHandle(string[] args) => args[0].Equals("cat-file", StringComparison.OrdinalIgnoreCase);

//        public CommandResult[] Execute(string[] args, IEnumerable<ICommandCreator> handlers)
//        {
//            var param = new CatFileParams(args);

//            var fileString = File.ReadAllText(DirectoryUtils.GetObjectPath(param.Hash));

//            Console.WriteLine(fileString);

//            return new[] { new CatFileCommandResult(fileString) };
//        }
//    }

//    internal class CatFileParams
//    {
//        public CatFileParams(string[] args)
//        {
//            if (!args[0].Equals("cat-file", StringComparison.OrdinalIgnoreCase))
//            {
//                throw new ArgumentException("Expected cat-file as first argument");
//            }

//            if (args.Length < 2)
//            {
//                throw new ArgumentException("Expected atleast '2' arguments");
//            }

//            if (args.Length > 3)
//            {
//                throw new ArgumentException("Expected at most '3' arguments");
//            }

//            Hash = args[1];

//            if (args.Length >= 3)
//            {
//                Type = args[2];
//            }
//        }

//        public string Hash { get; }

//        public string? Type { get; }

//        public bool EnforceType { get; } = true;
//    }
//}
