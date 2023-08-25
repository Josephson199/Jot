using Jot.Commands;
using System.CommandLine;
using System.IO.Abstractions.TestingHelpers;
using System.CommandLine.Parsing;
using System.CommandLine.IO;
using System.IO.Abstractions;
using System.CommandLine.Builder;
using Microsoft.Extensions.DependencyInjection;
using Jot.Extensions;
using NUnit.Framework.Internal;
using Jot.Core.Commit;
using Jot.Core;
using Jot.Tests.Extensions;

namespace Jot.Tests.Commands.Commit
{

    [TestFixture]
    public class CommitTests
    {
        static Func<DateTime> FirstOfJanuary => () => new DateTime(2000, 01, 01, 12, 00, 00, DateTimeKind.Utc);

        public CommitTests()
        {
            Clock.Set(new TestClock(FirstOfJanuary));
        }

        [Test]
        public async Task GivenAFreshProject_WhenInvokingWithCommitArgs_ThenOutputShouldContainSha256AndSourceFileReferences()
        {
            var fs = FileSystemTestHelper.CreateInitialized();

            fs.AddFile("sourceFile.txt", new MockFileData("test data"));

            var parser = new CommandLineBuilder(CommandFactory.CreateRoot())
                .UseDefaults()
                .UseDependencyInjection((ctx, services) =>
                {
                    services.AddSingleton<IFileSystem>(fs);
                })
                .UseRequireInitialization()
                .Build();

            var console = new TestConsole();

            var exitCode = await parser.InvokeAsync(new[] { "commit", "-m", "message" }, console);

            Assert.That(exitCode, Is.EqualTo(0));

            var @out = console.Out.ToArray();

            var error = console.Error.ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(error, Is.Empty);
                Assert.That(@out, Contains.Item("cd3a6c521895b7abbced59456ad9f5dab704e3638744dadca8e4805c05f34b09"));
                Assert.That(@out, Contains.Item("sourceFile.txt"));
            });
        }

        //Todo fix error when no source files are commited
        [Ignore("TODO")]
        [Test]
        public async Task GivenNoChanges_WhenInvokingWithCommitArgs_ThenOutputShouldContainError()
        {
            var fs = FileSystemTestHelper.CreateInitialized();

            //fs.AddFile("sourceFile.txt", new MockFileData("test data"));

            var parser = new CommandLineBuilder(CommandFactory.CreateRoot())
                .UseDefaults()
                .UseDependencyInjection((ctx, services) =>
                {
                    services.AddSingleton<IFileSystem>(fs);
                })
                .UseRequireInitialization()
                .Build();

            // First invocation should run successfully
            var firstExitCode = await parser.InvokeAsync(new[] { "commit", "-m", "message" });

            Assert.That(firstExitCode, Is.EqualTo(0));

            var testConsole = new TestConsole();

            // Second invocation should expect error message
            var secondExitCode = await parser.InvokeAsync(new[] { "commit", "-m", "message" }, testConsole);

            Assert.That(secondExitCode, Is.EqualTo(1));

            var error = testConsole.Error.ToArray();

            Assert.That(error, Contains.Item("No changes detected"));
        }
    }
}
