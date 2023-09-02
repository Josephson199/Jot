using System.CommandLine.Invocation;
using System.CommandLine;
using Jot.Commands;

namespace Jot.Extensions
{
    public static class InvocationContextExtensions
    {
        public static void SetConsole(this InvocationContext context, IConsole console)
        {
            context.Console = console;
        }

        public static bool IsCommand<T>(this InvocationContext context)
        {
            return context.ParseResult.CommandResult.Command is T;
        }
    }
}
