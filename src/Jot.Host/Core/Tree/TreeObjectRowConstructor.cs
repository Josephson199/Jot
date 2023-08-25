using Jot.Core.Blob;
using Jot.Extensions;
using System.IO.Abstractions;
using System.Text;

namespace Jot.Core.Tree
{
    public class TreeObjectRowConstructor
    {
        public static TreeObjectRow[] Create(JotObject<TreeObject> jotTreeObject)
        {
            var stringLines = Encoding.UTF8.GetString(jotTreeObject.Bytes.StripType()).Split(Environment.NewLine);

            var foo = stringLines
                .Select(fileLine =>
                {
                    var fileLineParts = fileLine.Split(" ");
                    var type = fileLineParts[0];
                    var hash = new ObjectId(fileLineParts[1]);
                    var sourcePath = new FilePath(fileLineParts[2]);

                    TreeObjectRow row = type switch
                    {
                        BlobObject.Type => new TreeObjectRow<BlobObject>(type, hash, sourcePath),
                        TreeObject.Type => new TreeObjectRow<TreeObject>(type, hash, sourcePath),
                        _ => throw new NotImplementedException(),
                    };

                    return row;
                }).ToArray();

            return foo;
        } 

        public static TreeObjectRow[] Create(TreeObject treeObject)
        {
            var rows = new List<TreeObjectRow>();

            foreach (var blobObject in treeObject.BlobObjects)
            {
                rows.Add(new TreeObjectRow<BlobObject>(BlobObject.Type, blobObject.GetObjectId(), blobObject.SourceFilePath));
            }

            foreach (var branch in treeObject.TreeBranches)
            {
                var branchHash = branch.GetObjectId();

                var branchSourceDirectory = branch.GetSourceDirectoryPath();

                var branchRow = new TreeObjectRow<TreeObject>(TreeObject.Type, branchHash, branchSourceDirectory);

                rows.Add(branchRow);
            }

            return rows.ToArray();
        }
    }

    public abstract class TreeObjectRow { }

    public class TreeObjectRow<T> : TreeObjectRow
    {
        public TreeObjectRow(string type, ObjectId hash, FilePath sourcePath)
        {
            Type = type;
            ObjectId = hash;
            SourcePath = sourcePath;
        }

        public string Type { get; }
        public ObjectId ObjectId { get; }
        public FilePath SourcePath { get; }
        public override string ToString()
        {
            return $"{Type} {ObjectId.Value} {SourcePath}";
        }
    }
}