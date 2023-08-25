using Jot.Commands;
using Jot.Core.Commit;
using Jot.Core.Tree;
using Jot.Extensions;
using Jot.Features.Commit;
using Jot.Functions;
using Microsoft.Extensions.Options;
using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using static Jot.UseCases.Commit.CommitCommand;
using static Jot.UseCases.Commit.CommitCommand.CommitHandler;

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

                FullTreePersistor = new TreeObjectPersistor(fileSystem);
                FromSourceTreeCreator = new TreeObjectCreator(fileSystem);
                TreeObjectRetreiever = new TreeObjectRetreiver(fileSystem);
                HeadFileManager = new HeadFileManager(fileSystem);
                CommitObjectRetreiver = new CommitObjectRetreiver(fileSystem);
                CommitObjectCreator = new CommitObjectCreator();
                TreeFileManager = new TreeFileManager(fileSystem);
                CommitFileManager = new CommitFileManager(fileSystem);
                CommitUseCase = new CommitUseCase(fileSystem, _projectOptions);
                CommitUseCase.TreePersisted += (sender, input) => Log(input);
            }

            private void Log(TreePersistResult treePersistResult)
            {

            }

            private CommitUseCase CommitUseCase { get; }
            private TreeObjectPersistor FullTreePersistor { get; }
            private TreeObjectCreator FromSourceTreeCreator { get; }
            private TreeObjectRetreiver TreeObjectRetreiever { get; }
            private HeadFileManager HeadFileManager { get; }
            private CommitObjectRetreiver CommitObjectRetreiver { get; }
            private CommitObjectCreator CommitObjectCreator { get; }
            private TreeFileManager TreeFileManager { get; }
            private CommitFileManager CommitFileManager { get; }

            public Task<int> HandleAsync(CommitOptions options, CancellationToken cancellationToken)
            {
                var treeObject = TreeFileManager.Create();

                var currentHead = HeadFileManager.Get().ObjectId;

                if (currentHead is not NullObjectId)
                {
                    var currentCommit = CommitFileManager.Get(currentHead);

                    var currentTreeObject = TreeFileManager.Get(currentCommit.TreeId);

                    if (currentTreeObject.Equals(treeObject))
                    {
                        _console.Error.WriteLine(StaticOutputs.NoChanges);
                        return Task.FromResult(ExitCodes.Failure);
                    }
                }

                var persistResult = TreeFileManager.Persist(treeObject);

                foreach (var sourceFile in persistResult.SourceFiles)
                {
                    _console.WriteLine(sourceFile.GetRelativePath());
                }

                var commitObject = CommitFileManager.Create(new CreateCommitParams
                {
                    TreeId = treeObject.GetObjectId(),
                    Author = _projectOptions.Author,
                    Message = options.Message,
                    ParentCommitId = currentHead
                });

                commitObject.Persist(_fileSystem);

                HeadFileManager.Set(new SetHeadRequest(commitObject.ObjectId));

                _console.Write(commitObject.ObjectId.Value);

                return Task.FromResult(ExitCodes.Success);
            }

            public class StaticOutputs
            {
                public const string NoChanges = "No changes detected";
            }
        }
    }

    public class CommitUseCase
    {
        private readonly IFileSystem _fileSystem;
        private readonly ProjectOptions _projectOptions;

        public CommitUseCase(IFileSystem fileSystem, ProjectOptions projectOptions)
        {
            _fileSystem = fileSystem;
            _projectOptions = projectOptions;

            HeadFileManager = new HeadFileManager(fileSystem);
            TreeFileManager = new TreeFileManager(fileSystem);
            CommitFileManager = new CommitFileManager(fileSystem);
        }
        private HeadFileManager HeadFileManager { get; }
        private TreeFileManager TreeFileManager { get; }
        private CommitFileManager CommitFileManager { get; }

        public delegate void TreePersistedEventHandler(object sender, TreePersistResult treePersistResult);

        public event TreePersistedEventHandler TreePersisted;

        public async Task<CommitResult> Execute(CommitOptions commitOptions)
        {
            var treeObject = TreeFileManager.Create();

            var currentHead = HeadFileManager.Get().ObjectId;

            if (currentHead is not NullObjectId)
            {
                var currentCommit = CommitFileManager.Get(currentHead);

                var currentTreeObject = TreeFileManager.Get(currentCommit.TreeId);

                if (currentTreeObject.Equals(treeObject))
                {
                    //_console.Error.WriteLine(StaticOutputs.NoChanges);
                    return CommitResult.Failed;
                }
            }

            var persistResult = TreeFileManager.Persist(treeObject);

            TreePersisted?.Invoke(this, persistResult);

            //foreach (var sourceFile in persistResult.SourceFiles)
            //{
            //    _console.WriteLine(sourceFile.GetRelativePath());
            //}

            var commitObject = CommitFileManager.Create(new CreateCommitParams
            {
                TreeId = treeObject.GetObjectId(),
                Author = _projectOptions.Author,
                Message = commitOptions.Message,
                ParentCommitId = currentHead
            });

            commitObject.Persist(_fileSystem);

            HeadFileManager.Set(new SetHeadRequest(commitObject.ObjectId));

            //_console.Write(commitObject.ObjectId.Value);

            return CommitResult.Success;
        }

        public abstract record CommitResult 
        {
            public static CommitResult Failed => new CommitFailed();

            public static CommitResult Success => new CommitSuccess();

        }

        public record CommitSuccess : CommitResult
        {
        }

        public record CommitFailed : CommitResult
        {
        }
    }

    public class CommitFileManager
    {
        public CommitFileManager(IFileSystem fileSystem)
        {
            CommitObjectRetreiver = new CommitObjectRetreiver(fileSystem);
            CommitObjectCreator = new CommitObjectCreator();
        }

        private CommitObjectRetreiver CommitObjectRetreiver { get; }
        private CommitObjectCreator CommitObjectCreator { get; }

        public CommitObject Get(ObjectId objectId)
        {
            return CommitObjectRetreiver.Get(objectId);
        }

        public CommitObject Create(CreateCommitParams @params)
        {
            return CommitObjectCreator.Create(@params);
        }
    }

    public class TreeFileManager
    {
        public TreeFileManager(IFileSystem fileSystem)
        {
            TreeObjectPersistor = new TreeObjectPersistor(fileSystem);
            TreeObjectCreator = new TreeObjectCreator(fileSystem);
            TreeObjectRetreiver = new TreeObjectRetreiver(fileSystem);
        }

        private TreeObjectPersistor TreeObjectPersistor { get; }
        private TreeObjectCreator TreeObjectCreator { get; }
        private TreeObjectRetreiver TreeObjectRetreiver { get; }

        public TreeObject Create()
        {
            return TreeObjectCreator.CreateInstance();
        }

        public TreeObject Get(ObjectId objectId)
        {
            return TreeObjectRetreiver.Get(objectId);
        }

        public TreePersistResult Persist(TreeObject treeObject)
        {
            return TreeObjectPersistor.Persist(treeObject);
        }

    }
}
