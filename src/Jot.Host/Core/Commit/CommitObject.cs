using Jot.Attributes;
using Jot.Extensions;
using System;
using System.IO.Abstractions;
using System.Text;

namespace Jot.Core.Commit
{
    public class CommitObjectRetreiver
    {
        private readonly IFileSystem _fileSystem;

        public CommitObjectRetreiver(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public CommitObject Get(ObjectId objectId)
        {
            var jotObject = JotObject.CreateInstanceFromObject<CommitObject>(objectId, _fileSystem);

            var bytes = jotObject.Bytes.StripType();

            var commitContent = Encoding.UTF8.GetString(bytes);

            var commitLinesArray = commitContent.Split(Environment.NewLine);

            ObjectId treeId = new string(commitLinesArray[0].Skip("tree ".Length).ToArray());

            ObjectId parentCommitId = new string(commitLinesArray[1].Skip("parent ".Length).ToArray());

            //ObjectId? parentCommitId = null;

            //if (!string.IsNullOrWhiteSpace(parentCommitIdString))
            //{
            //    parentCommitId = new ObjectId(parentCommitIdString);
            //}

            var author = new string(commitLinesArray[2].Skip("author ".Length).ToArray());

            var timeString = new string(commitLinesArray[3].Skip("time ".Length).ToArray());

            var dateTime = DateTime.Parse(timeString).ToUniversalTime();

            // delimeter
            _ = commitLinesArray[4];

            var message = commitLinesArray[5];

            var commitParams = new CommitParams
            {
                ObjectId = objectId,
                Author = author,
                Bytes = bytes,
                TreeId = treeId,
                ParentCommitId = parentCommitId,
                Message = message,
                DateTime = dateTime
            };

            return new CommitObject(commitParams);
        }
    }

    public class CommitObjectCreator
    {
        public CommitObject Create(CreateCommitParams @params)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"tree {@params.TreeId}");
            sb.AppendLine($"parent {@params.ParentCommitId}");
            sb.AppendLine($"author {@params.Author}");
            sb.AppendLine($"time {@params.DateTime:O}");
            sb.AppendLine();
            sb.Append(@params.Message);

            var stringValue = sb.ToString();

            var bytes = Encoding.UTF8.GetBytes(stringValue).AddObjectType(CommitObject.Type);

            var objectId = ObjectId.CreateInstance(bytes);

            var commitParams = new CommitParams
            {
                ObjectId = objectId,
                Author = @params.Author,
                Bytes = bytes,
                TreeId = @params.TreeId,
                ParentCommitId = @params.ParentCommitId,
                Message = @params.Message,
                DateTime = @params.DateTime
            };

            return new CommitObject(commitParams);
        }
    }

    //public class FromObjectCommitCreator
    //{
    //    private readonly IFileSystem _fileSystem;

    //    public FromObjectCommitCreator(IFileSystem fileSystem)
    //    {
    //        _fileSystem = fileSystem;
    //    }

    //    public CommitObject CreateInstance(ObjectId objectId)
    //    {
    //        var jotObject = JotObject.CreateInstanceFromObject<CommitObject>(objectId, _fileSystem);

    //        return new CommitObject(jotObject);
    //    }
    //}

    [JotObjectType(Type)]
    public class CommitObject
    {
        public const string Type = "commit";

        ////Todo refacator long constructor
        //public CommitObject(JotObject<CommitObject> jotObject)
        //{
        //    Bytes = jotObject.Bytes;

        //    ObjectId = jotObject.Hash;

        //    var commitContent = Encoding.UTF8.GetString(jotObject.Bytes.StripType());

        //    var commitLinesArray = commitContent.Split(Environment.NewLine);

        //    TreeId = new ObjectId(new string(commitLinesArray[0].Skip("tree ".Length).ToArray()));

        //    var parentCommitIdString = new string(commitLinesArray[1].Skip("parent ".Length).ToArray());

        //    ObjectId? parentCommitId = null;

        //    if (!string.IsNullOrWhiteSpace(parentCommitIdString))
        //    {
        //        parentCommitId = new ObjectId(parentCommitIdString);
        //    }

        //    ParentCommitId = parentCommitId;

        //    Author = new string(commitLinesArray[2].Skip("author ".Length).ToArray());

        //    var timeString = new string(commitLinesArray[3].Skip("time ".Length).ToArray());

        //    DateTime = DateTime.Parse(timeString).ToUniversalTime();

        //    // delimeter
        //    _ = commitLinesArray[4];

        //    Message = commitLinesArray[5];
        //}

        //public CommitObject(CreateNewCommitRequest commitInput)
        //{
        //    TreeId = commitInput.TreeId;
        //    Author = commitInput.Author;
        //    DateTime = commitInput.DateTime;
        //    Message = commitInput.Message;
        //    ParentCommitId = commitInput.ParentCommitId;
        //    Bytes = commitInput.GetBytes();
        //    ObjectId = commitInput.GetObjectId();
        //}

        public CommitObject(CommitParams @params)
        {
            TreeId = @params.TreeId;
            Author = @params.Author;
            DateTime = @params.DateTime;
            Message = @params.Message;
            ParentCommitId = @params.ParentCommitId;
            Bytes = @params.Bytes;
            ObjectId = @params.ObjectId;
        }

        public ObjectId TreeId { get; }
        public string Author { get; }
        public DateTime DateTime { get; }
        public string? Message { get; }
        public ObjectId ParentCommitId { get; }
        public ObjectId ObjectId { get; }
        private byte[] Bytes { get; set; }

        //public static CommitObject CreateInstanceFromObject(ObjectId objectId, IFileSystem fileSystem)
        //{
        //    var jotObject = JotObject.CreateInstanceFromObject<CommitObject>(objectId, fileSystem);

        //    return new CommitObject(jotObject);
        //}

        //public static CommitObject CreateInstanceFromObject(IFileInfo fileInfo)
        //{
        //    var jotObject = JotObject.CreateInstanceFromObject<CommitObject>(fileInfo);

        //    return new CommitObject(jotObject);
        //}

        //public static CommitObject CreateInstanceFromObject(ObjectId objectId)
        //{
        //    return new CommitObjectRetreiver().Get(objectId);
        //    var jotObject = JotObject.CreateInstanceFromObject<CommitObject>(fileInfo);

        //    return new CommitObject(jotObject);
        //}

        //public IReadOnlyList<CommitObject> ResolveParents(IFileSystem fileSystem)
        //    => new CommitParentResolver(fileSystem).Resolve(this);

        //public ObjectId GetObjectId() => ObjectId;

        public IFileInfo Persist(IFileSystem fileSystem)
        {
            var fileInfo = fileSystem.FileInfo.New(ObjectId);

            using var fs = fileInfo.Create();

            fs.Write(Bytes, 0, Bytes.Length);

            return fileInfo;
        }
    }

    public record CommitParams
    {
        public required ObjectId TreeId { get; init; }
        public required string Author { get; init; }
        public required DateTime DateTime { get; init; }
        public string? Message { get; init; }
        public ObjectId ParentCommitId { get; init; } = ObjectId.Null;
        public required byte[] Bytes { get; init; }
        public required ObjectId ObjectId { get; init; }
    }

    public record CreateCommitParams
    {
        public required ObjectId TreeId { get; init; }
        public required string Author { get; init; }
        public DateTime DateTime { get; } = Clock.Default.Now.UtcDateTime;
        public string? Message { get; init; }
        public ObjectId ParentCommitId { get; init; } = ObjectId.Null;
    }

    public record CreateNewCommitRequest
    {
        public required ObjectId TreeId { get; init; }
        public required string Author { get; init; }
        public DateTime DateTime { get; } = Clock.Default.Now.UtcDateTime;
        public string? Message { get; init; }
        public ObjectId? ParentCommitId { get; init; }
        
        private byte[]? Bytes { get; set; }
        public byte[] GetBytes()
        {
            if (Bytes != null)
            {
                return Bytes;
            }

            var sb = new StringBuilder();

            sb.AppendLine($"tree {TreeId}");
            sb.AppendLine($"parent {ParentCommitId}");
            sb.AppendLine($"author {Author}");
            sb.AppendLine($"time {DateTime:O}");
            sb.AppendLine();
            sb.Append(Message);

            var stringValue = sb.ToString();

            Bytes = Encoding.UTF8.GetBytes(stringValue).AddObjectType(CommitObject.Type);

            return Bytes;
        }

        private ObjectId? ObjectId { get; set; }
        public ObjectId GetObjectId()
        {
            if (ObjectId != null)
            {
                return ObjectId;
            }

            ObjectId = ObjectId.CreateInstance(GetBytes());

            return ObjectId;
        }
    }
}
