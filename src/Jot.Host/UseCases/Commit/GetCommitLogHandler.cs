using Jot.Commands;
using Jot.Core.Commit;
using Jot.Extensions;
using Jot.Functions;
using System.CommandLine;
using System.IO.Abstractions;

namespace Jot.Features.Commit
{
    public class GetCommitLogHandler
    {
        private readonly HeadFileHandler _headContext;
        private readonly IFileSystem _fileSystem;

        public GetCommitLogHandler(IFileSystem fileSystem)
        {
            _headContext = new HeadFileHandler(fileSystem);
            _fileSystem = fileSystem;
        }

        public LogCommandResult Execute(LogParams logParams)
        {
            var commitHash = logParams.CommitHash;

            if (string.IsNullOrWhiteSpace(commitHash))
            {
                GetHeadResult getHeadResult = _headContext.Get();

                if (getHeadResult.ObjectId == null)
                {
                    return LogCommandResult.Empty;
                }

                commitHash = getHeadResult.ObjectId.Value;
            }

            var commitId = new ObjectId(commitHash);

            var fileInfo = _fileSystem.FileInfo.New(commitId);

            if (fileInfo.Exists)
            {
                return LogCommandResult.Empty;
            }

            var commitRetreiver = new CommitObjectRetreiver(_fileSystem);

            var commit = commitRetreiver.Get(commitId);

            //var commit = CommitObject.CreateInstanceFromObject(fileInfo);

            if (commit == null)
            {
                return LogCommandResult.Empty;
            }

            var result = new List<CommitObject>() { commit };

            var commitResolver = new CommitParentResolver(_fileSystem);

            result.AddRange(commitResolver.Resolve(commit));

            return new LogCommandResult(result.Select(LogEntry.CreateInstance));
        }
    }

    public class LogParams
    {
        public LogParams(string? commitHash)
        {
            CommitHash = commitHash;
        }

        public string? CommitHash { get; }
    }

    public class LogCommandResult
    {
        public LogCommandResult(IEnumerable<LogEntry> commits)
        {
            LogEntries = commits.ToArray();
        }

        public IReadOnlyList<LogEntry> LogEntries { get; } = Array.Empty<LogEntry>();

        public static LogCommandResult Empty => new(Array.Empty<LogEntry>());
    }

    public class LogEntry
    {
        public LogEntry(ObjectId hash, string author, DateTime dateTime, string? message)
        {
            Author = author;
            DateTime = dateTime;
            Message = message;
            Hash = hash;
        }

        public ObjectId Hash { get; }
        public DateTime DateTime { get; }
        public string? Message { get; }
        public string Author { get; }

        public static LogEntry CreateInstance(CommitObject commit)
        {
            return new LogEntry(commit.ObjectId, commit.Author, commit.DateTime, commit.Message);
        }
    }
}
