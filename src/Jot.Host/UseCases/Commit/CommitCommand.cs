using Jot.Commands;
using Jot.Core.Commit;
using Jot.Core.Tree;
using Jot.Extensions;
using Jot.Functions;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using static Jot.UseCases.Commit.CommitCommand;

namespace Jot.UseCases.Commit
{
    public sealed class CommitCommand : Command<CommitOptions, CommitHandler>
    {
        public CommitCommand() : base(name: "commit", description: "Commit files")
        {
            var messageOption = new Option<string?>(
                name: "--message",
                description: "A message to store with the commit.")
            {
                Arity = ArgumentArity.ZeroOrOne,
                IsRequired = false
            };

            messageOption.AddAlias("-m");

            AddOption(messageOption);
        }

        public class CommitOptions
        {
            // Automatic binding with System.CommandLine.NamingConventionBinder
            public string? Message { get; set; }
        }

        public class CommitHandler : ICommandOptionsHandler<CommitOptions>
        {
            private readonly IFileSystem _fileSystem;
            private readonly IConsole _console;
            private readonly ProjectOptions _projectOptions;

            public CommitHandler(IFileSystem fileSystem, IConsole console)
            {
                _fileSystem = fileSystem;
                _console = console;
                _projectOptions = fileSystem.File.GetOptions<ProjectOptions>();

                TreeObjectPersistor = new TreeObjectPersistor(fileSystem);
                TreeObjectPersistor.SourceFilePersisted += (sender, input) =>
                {
                    _console.WriteLine(input.GetRelativePath());
                };

                TreeObjectCreator = new TreeObjectCreator(fileSystem);
                HeadFileHandler = new HeadFileHandler(fileSystem);
                CommitObjectCreator = new CommitObjectCreator();

                CommitValidationChain = new NoChangesToCommit(fileSystem)
                {
                    Message = "No changes detected"
                };
            }

            private TreeObjectPersistor TreeObjectPersistor { get; }
            private TreeObjectCreator TreeObjectCreator { get; }
            private HeadFileHandler HeadFileHandler { get; }
            private CommitObjectCreator CommitObjectCreator { get; }
            private ICommitValidation CommitValidationChain { get; }

            public Task<int> HandleAsync(CommitOptions options, CancellationToken cancellationToken)
            {
                var treeObject = TreeObjectCreator.Create();

                var currentHead = HeadFileHandler.Get().ObjectId;

                var validationResult = CommitValidationChain.Validate(treeObject, currentHead);

                if (validationResult.HasFailed)
                {
                    _console.Error.WriteLine(validationResult.FailedValidation!.Message);

                    return Task.FromResult(ExitCodes.Failure);
                }

                //TODO BEGIN TRANSACTION

                TreeObjectPersistor.Persist(treeObject);

                var commitObject = CommitObjectCreator.Create(new CreateCommitParams
                {
                    TreeId = treeObject.GetObjectId(),
                    Author = _projectOptions.Author,
                    Message = options.Message,
                    ParentCommitId = currentHead
                });

                commitObject.Persist(_fileSystem);

                HeadFileHandler.Set(new SetHeadRequest(commitObject.ObjectId));

                //TODO END TRANSACTION

                _console.Write(commitObject.ObjectId.Value);

                return Task.FromResult(ExitCodes.Success);
            }
        }
    }

    public interface ICommitValidation
    {
        CommitValidationResult Validate(TreeObject treeObject, ObjectId currentHead);

        string Message { get; }
    }

    public record CommitValidationResult
    {
        private CommitValidationResult()
        {
        }

        private CommitValidationResult(ICommitValidation commitValidation)
        {
            FailedValidation = commitValidation;
        }

        public ICommitValidation? FailedValidation { get; }

        public bool HasFailed => FailedValidation != null;

        public static CommitValidationResult Failed(ICommitValidation commitValidation)
            => new(commitValidation);

        public static CommitValidationResult Success()
            => new();
    }

    public class NoChangesToCommit : ICommitValidation
    {
        public NoChangesToCommit(IFileSystem fileSystem, ICommitValidation? next = null)
        {
            CommitObjectRetreiver = new CommitObjectRetreiver(fileSystem);
            TreeObjectRetreiver = new TreeObjectRetreiver(fileSystem);
            Next = next;
        }

        public string Message { get; set; } = string.Empty; 

        private ICommitValidation? Next { get; }

        private CommitObjectRetreiver CommitObjectRetreiver { get; }

        private TreeObjectRetreiver TreeObjectRetreiver { get; }

        public CommitValidationResult Validate(TreeObject treeObject, ObjectId currentHead)
        {
            if (currentHead is NullObjectId)
            {
                return CommitValidationResult.Success();
            }

            var currentCommit = CommitObjectRetreiver.Get(currentHead);

            var currentTreeObject = TreeObjectRetreiver.Get(currentCommit.TreeId);

            if (currentTreeObject.Equals(treeObject))
            {
                return CommitValidationResult.Failed(this);
            }

            if (Next is not null)
            {
                return Next.Validate(treeObject, currentHead);
            }

            return CommitValidationResult.Success();
        }
    }
}
