using Jot.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Jot.Extensions
{
    public static class CommandLineBuilderExtensions
    {
        // This overload allows you to conditionally register services based on the command line invocation context
        // in order to improve startup time when you have a lot of services to register.
        public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<InvocationContext, ServiceCollection> configureServices)
        {
            return builder.AddMiddleware(async (context, next) =>
            {
                // Register our services in the modern Microsoft dependency injection container
                var services = new ServiceCollection();
                configureServices(context, services);
                var uniqueServiceTypes = new HashSet<Type>(services.Select(x => x.ServiceType));

                services.TryAddSingleton(context.Console);

                await using var serviceProvider = services.BuildServiceProvider();

                // System.CommandLine's service provider is a "fake" implementation that relies on a dictionary of factories,
                // but we can still make sure here that "true" dependency-injected services are available from "context.BindingContext".
                // https://github.com/dotnet/command-line-api/blob/2.0.0-beta4.22272.1/src/System.CommandLine/Invocation/ServiceProvider.cs
                context.BindingContext.AddService<IServiceProvider>(_ => serviceProvider);

                foreach (var serviceType in uniqueServiceTypes)
                {
                    context.BindingContext.AddService(serviceType, _ => serviceProvider.GetRequiredService(serviceType));

                    // Enable support for "context.BindingContext.GetServices<>()" as in the modern dependency injection
                    var enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(serviceType);
                    context.BindingContext.AddService(enumerableServiceType, _ => serviceProvider.GetServices(serviceType));
                }

                await next(context);
            });
        }

        public static CommandLineBuilder UseRequireInitialization(this CommandLineBuilder builder)
        {
            return builder.AddMiddleware(async (context, next) =>
            {
                if (context.ParseResult.CommandResult.Command is not InitCommand)
                {
                    var fs = context.BindingContext.GetRequiredService<IFileSystem>();

                    var exists = fs.File.ConfigExists<ProjectOptions>();

                    if (!exists)
                    {
                        context.Console.Error.WriteLine("run > \"jot init\" to initialize a new project");
                        context.ExitCode = 1;
                        return;
                    }
                }

                await next(context);
            }, (MiddlewareOrder)100);
        }
    }
}
