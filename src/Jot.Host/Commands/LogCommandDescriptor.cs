//using System.CommandLine;
//using System.CommandLine.IO;
//using Jot.Features.Commit;

//namespace Jot.Commands
//{
//    public class LogCommandDescriptor : ICommandDescriptor
//    {
//        private readonly GetCommitLogHandler _commitLogHandler;

//        public LogCommandDescriptor(GetCommitLogHandler commitLogHandler)
//        {
//            _commitLogHandler = commitLogHandler;
//        }

//        public Command GetCommand()
//        {
//            var commitArgument = new Argument<string>(
//               name: "commit",
//               description: "Commish hash.")
//            { Arity = ArgumentArity.ZeroOrOne };

//            var command = new Command("log", "Prints the commit tree");

//            command.AddAlias("l");

//            command.AddArgument(commitArgument);

//            command.SetHandler((commitHash) =>
//            {
//                Print(_commitLogHandler.Execute(new LogParams(commitHash)));

//            }, commitArgument);

//            return command;
//        }

//        public void Print(LogCommandResult result)
//        {
//            foreach (var entry in result.LogEntries)
//            {
//                Console.WriteLine(entry.Hash);
//                Console.WriteLine(entry.Message);
//                Console.WriteLine(entry.Author);
//                Console.Write(entry.DateTime);
//            }
//        }
//    }
//}
