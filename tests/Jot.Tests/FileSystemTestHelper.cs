using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;

namespace Jot.Tests
{
    public class FileSystemTestHelper
    {
        public static MockFileSystem CreateInitialized()
        {
            var projectOptions = new ProjectOptions
            {
                Author = "Test",
                Version = "0.0.1-alpha",
                IngoredDirectories = new[]
                {
                   Path.Combine(Environment.CurrentDirectory, ".jot"),
                }
            };

            var files = new Dictionary<string, MockFileData>
            {
                { ".jot\\objects", new MockDirectoryData() },
                { ".jot\\.config", new MockFileData(JsonSerializer.Serialize(projectOptions)) },
            };

            var fileSystem = new MockFileSystem(files, Environment.CurrentDirectory);

            return fileSystem;
        }
    }
}
