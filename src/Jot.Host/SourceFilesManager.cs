using Jot.Core.Blob;
using Jot.Extensions;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.Serialization;

namespace Jot
{
    public class SourceFilesManagers
    {
        private readonly IFileSystem _fileSystem;
        private readonly ProjectOptions _projectOptions;

        public SourceFilesManagers(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _projectOptions = fileSystem.File.GetOptions<ProjectOptions>()!;
        }

        public IEnumerable<IDirectoryInfo> GetSubdirectories(IDirectoryInfo? directory = null)
        {
            directory ??= _fileSystem.DirectoryInfo.New(Environment.CurrentDirectory);

            var directories = directory
                .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                .Where(directory => !_projectOptions.IngoredDirectories.Any(hiddenDirectory => directory.FullName.StartsWith(hiddenDirectory)))
                .ToArray();

            return directories;
        }

        public IEnumerable<SourceFile> GetFiles(SearchOption searchOption, IDirectoryInfo? directory = null)
        {
            directory ??= _fileSystem.DirectoryInfo.New(Environment.CurrentDirectory);

            var files = directory
                .EnumerateFiles("*", searchOption)
                .Where(file => !_projectOptions.IngoredDirectories.Any(hiddenDirectory => file.DirectoryName!.StartsWith(hiddenDirectory)))
                .Select(file => new SourceFile(file))
                .ToArray();

            return files;
        }

        public void Clear()
        {
            var topFiles = GetFiles(SearchOption.TopDirectoryOnly);

            var count = 0;

            foreach (var file in topFiles)
            {
                if (count > 100)
                {
                    // Remove once confidence is increased
                    throw new SafeGuardException("Stopped deleting files at threshold count:'100'");
                }

                file.Delete();

                count++;
            }

            var subdirectories = GetSubdirectories();

            foreach (var subdirectory in subdirectories.Select(e => e.FullName))
            {
                if (!subdirectory.StartsWith("C:\\temp"))
                {
                    // Remove once confidence is increased
                    throw new SafeGuardException("Stopped deleting folder contents since we are outside of C:\\temp");
                }

                _fileSystem.Directory.Delete(subdirectory, recursive: true);
            }
        }
    }

    [Serializable]
    internal class SafeGuardException : Exception
    {
        public SafeGuardException()
        {
        }

        public SafeGuardException(string? message) : base(message)
        {
        }

        public SafeGuardException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SafeGuardException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}