//using System.CommandLine;
//using Jot.Features.Checkout;

//namespace Jot.Commands
//{

//    public class CheckoutCommandDescriptor : ICommandDescriptor
//    {
//        private readonly CheckoutHandler _checkoutHandler;

//        public CheckoutCommandDescriptor(CheckoutHandler checkoutHandler)
//        {
//            _checkoutHandler = checkoutHandler;
//        }

//        public Command GetCommand()
//        {
//            var commitArgument = new Argument<string>(
//                           name: "commit",
//                           description: "Commit hash.")
//            { Arity = ArgumentArity.ExactlyOne };

//            var command = new Command("checkout", "Checkout commit");

//            command.AddArgument(commitArgument);

//            command.SetHandler((context) =>
//            {
//                var commitHash = context.ParseResult.GetValueForArgument(commitArgument);

//                _ = _checkoutHandler.Execute(new CheckoutParams(commitHash));
//            });

//            return command;
//        }
//    }
//}
