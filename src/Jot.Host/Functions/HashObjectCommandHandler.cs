//namespace Jot.Handlers
//{
//    public class HashObjectCommandResult : CommandResult
//    {

//    }

//    public class HashObjectCommandHandler : ICommandCreator
//    {
//        public bool CanHandle(string[] args) => args[0].Equals("hash-object", StringComparison.OrdinalIgnoreCase);

//        public CommandResult[] Execute(string[] args, IEnumerable<ICommandCreator> handlers)
//        {
//            var param = new HashObjectParams(args);

//            if (!File.Exists(param.SourcePath))
//            {
//                Console.WriteLine("File not found");

//                return new[] { new HashObjectCommandResult() };
//            }

//            var sourceFileLines = File.ReadAllLines(param.SourcePath);

//            if (!sourceFileLines.Any())
//            {
//                Console.WriteLine("Ignored empty file");

//                return new[] { new HashObjectCommandResult() };
//            }

//            var jotBlob = new JotBlob(sourceFileLines, param.SourcePath);

//            File.WriteAllLines(DirectoryUtils.GetObjectPath(jotBlob.GetHash()), jotBlob.FileLines);

//            Console.WriteLine($"Added file {param.SourcePath}");

//            return new[] { new HashObjectCommandResult() };
//        }
//    }

//    internal class HashObjectParams
//    {
//        public HashObjectParams(string[] args)
//        {
//            if (!args[0].Equals("hash-object", StringComparison.OrdinalIgnoreCase))
//            {
//                throw new ArgumentException("Expected hash-object as first argument");
//            }

//            var filePath = args[1];

//            if (!Path.IsPathFullyQualified(filePath))
//            {
//                filePath = Path.Combine(Environment.CurrentDirectory, filePath);
//            }

//            if (!File.Exists(filePath))
//            {
//                throw new ArgumentException(nameof(filePath));
//            }

//            if (args.Length == 3)
//            {
//                Type = args[2];
//            }

//            SourcePath = filePath;
//        }

//        public string SourcePath { get; }

//        public string? Type { get; }
//    }
//}