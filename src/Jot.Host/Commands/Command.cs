using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;

namespace Jot.Commands
{
    public abstract class Command<TOptions, TOptionsHandler> : Command
        where TOptions : class
        where TOptionsHandler : class, ICommandOptionsHandler<TOptions>
    {
        protected Command(string name, string description)
            : base(name, description)
        {
            this.Handler = CommandHandler.Create<TOptions, IServiceProvider, CancellationToken>(HandleOptions);
        }

        private static async Task<int> HandleOptions(TOptions options, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            // True dependency injection happening here
            var handler = ActivatorUtilities.CreateInstance<TOptionsHandler>(serviceProvider);
            return await handler.HandleAsync(options, cancellationToken);
        }
    }

    public interface ICommandOptionsHandler<in TOptions>
    {
        Task<int> HandleAsync(TOptions options, CancellationToken cancellationToken);
    }
}
