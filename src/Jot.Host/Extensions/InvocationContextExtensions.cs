using System.CommandLine.Invocation;
using System.CommandLine;

namespace Jot.Extensions
{
    public static class InvocationContextExtensions
    {
        public static void SetConsole(this InvocationContext context, IConsole console)
        {
            context.Console = console;
        }
    }
}
