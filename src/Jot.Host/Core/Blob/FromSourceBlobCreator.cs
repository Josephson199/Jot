//using System.IO.Abstractions;
//using System.Text;

//namespace Jot.Core.Blob
//{
//    public class FromSourceBlobCreator
//    {
//        public BlobObject CreateInstance(IFileInfo fileInfo)
//        {
//            using var fs = fileInfo.OpenRead();
//            using var binaryReader = new BinaryReader(fs);
//            var bytes = binaryReader.ReadBytes((int)fs.Length);

//            if (!bytes.Any())
//            {
//                throw new ArgumentException("Cannot initialize with empty source file");
//            }

//            return new BlobObject(fileInfo);
//        }
//    }
//}