using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Jot.Features.Commit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

namespace Jot.Commands
{
    public class JotContextOptions
    {
      
        public JotContextOptions(IFileSystem fileSystem, IConsole console, IConfigurationBuilder configurationBuilder)
        {
            FileSystem = fileSystem;
            Console = console;
            ConfigurationBuilder = configurationBuilder;
        }

        public IFileSystem FileSystem { get; set; }

        public IConsole Console { get; set; }

        public IConfigurationBuilder ConfigurationBuilder { get; set; }

        public IConfigurationRoot ConfigurationRoot => ConfigurationBuilder.Build();
    }

    internal sealed class DefaultCommands
    {
        internal static Command GetInitCommand(IServiceProvider provider)
        {
            var command = new Command("init", "Initialize current directory");

            command.SetHandler((context) =>
            {
                var options = provider.GetRequiredService<JotContextOptions>();

                var fs = options.FileSystem;

                var appConfig = new ProjectOptions
                {
                    Version = "0",
                    Author = "Test",
                    IngoredDirectories = new[]
                    {
                        Path.Combine(Environment.CurrentDirectory, ".git"),
                        Path.Combine(Environment.CurrentDirectory, ".jot"),
                        Path.Combine(Environment.CurrentDirectory, "bin"),
                        Path.Combine(Environment.CurrentDirectory, "obj")
                    }
                };

                if (!fs.Directory.Exists(Path.GetDirectoryName(JotPaths.ProjectOptionsFilePath)))
                {
                    fs.Directory.CreateDirectory(Path.GetDirectoryName(JotPaths.ProjectOptionsFilePath)!);
                }

                fs.File.WriteAllText(JotPaths.ProjectOptionsFilePath, JsonSerializer.Serialize(appConfig, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));

                fs.Directory.CreateDirectory(JotPaths.ObjectsDirPath);
            });

            return command;
        }
    }

    internal sealed class DefaultMiddlewares
    {
        public static InvocationMiddleware EnsureInit(IServiceProvider provider)
        {
            var options = provider.GetRequiredService<JotContextOptions>();

            async Task EnsureInit(InvocationContext context, Func<InvocationContext, Task> next)
            {
                var config = options.ConfigurationRoot.Get<ProjectOptions>();

                var commandName = context.ParseResult.CommandResult.Command.Name;

                if (config is null && !commandName.Equals("init", StringComparison.OrdinalIgnoreCase))
                {
                    context.Console.Error.WriteLine("Run >jot init");
                    return;
                }

                await next(context);
            }

            return EnsureInit;
        }
    }


    
}
