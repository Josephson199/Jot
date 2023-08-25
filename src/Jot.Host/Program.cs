using Jot.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.IO.Abstractions;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Jot.Extensions;

namespace Jot
{
    public partial class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Environment.CurrentDirectory = @"C:\temp\testFolder";

            var rootCommand = CommandFactory.CreateRoot();

            var builder = new CommandLineBuilder(rootCommand)
                  .UseDefaults()
                  .UseDependencyInjection((ctx, services) =>
                  {
                      services.AddSingleton<IFileSystem>(new FileSystem());
                  })
                  .UseRequireInitialization();

            args.ToList().ForEach(Console.WriteLine);

            return await builder.Build().InvokeAsync(new[] {"init"});
        }
    }
}