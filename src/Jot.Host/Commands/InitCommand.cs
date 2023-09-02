using Jot.Extensions;
using System.CommandLine;
using System.IO.Abstractions;
using System.Reflection;
using static Jot.Commands.InitCommand;

namespace Jot.Commands
{
    public sealed class InitCommand : Command<InitOptions, InitHandler>
    {
        public InitCommand() : base(name: "init", description: "Initialize your project")
        {
        }

        public class InitOptions
        {
        }

        public class InitHandler : ICommandOptionsHandler<InitOptions>
        {
            private readonly IFileSystem _fileSystem;
            private readonly IConsole _console;

            public InitHandler(IFileSystem fileSystem, IConsole console)
            {
                _fileSystem = fileSystem;
                _console = console;
            }

            public Task<int> HandleAsync(InitOptions options, CancellationToken cancellationToken)
            {
                var exists = _fileSystem.File.OptionsExists<ProjectOptions>();

                if (exists)
                {
                    _console.WriteLine("Project already initialized");

                    var existingOptions = _fileSystem.File.GetOptions<ProjectOptions>();

                    _console.WriteLine($"{nameof(existingOptions.Version)}:\t{existingOptions.Version}");
                    _console.WriteLine($"{nameof(existingOptions.Author)}:\t{existingOptions.Author}");

                    _console.WriteLine(nameof(existingOptions.IngoredDirectories) + ":");

                    foreach (var hidden in existingOptions.IngoredDirectories.Select((value, idx) => (value, idx)))
                    {
                        _console.WriteLine($"\t{hidden.value}");
                    }


                    return Task.FromResult(0);
                }

                var projectOptions = new ProjectOptions
                {
                    Author = Environment.MachineName,
                    Version = Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "",
                    IngoredDirectories = new[]
                    {
                        Path.Combine(Environment.CurrentDirectory, ".git"),
                        Path.Combine(Environment.CurrentDirectory, ".jot"),
                        Path.Combine(Environment.CurrentDirectory, "bin"),
                        Path.Combine(Environment.CurrentDirectory, "obj")
                    }
                };

                _fileSystem.File.WriteOptions(projectOptions);

                return Task.FromResult(0);
            }
        }
    }

}
