using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

namespace Jot.Commands
{
    public interface ICommandDescriptor
    {
        Command GetCommand(IServiceProvider provider);

        void RegisterDependencies(IServiceCollection services);
    }
}
