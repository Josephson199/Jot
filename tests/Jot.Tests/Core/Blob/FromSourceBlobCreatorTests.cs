using Jot.Core.Blob;
using System.IO;
using System.IO.Abstractions.TestingHelpers;

namespace Jot.Tests.Core.Blob
{
    [TestFixture]
    public class FromSourceBlobCreatorTests
    {
        [Test]
        public void CreateInstanceShouldReturnBlobObject()
        {
            //var file = new MockFileData("foo\nbar\r\nbar");

            //var filePath = new FilePath("foo.txt");

            //var files = new Dictionary<string, MockFileData>
            //{
            //    { filePath.Value, file },
            //    { ".jot\\objects", new MockDirectoryData() }
            //};

            //var fileSystem = new MockFileSystem(files, Environment.CurrentDirectory);

            //var sut = new FromSourceBlobCreator(new MockFileSystem(files));

            //BlobObject blobObject = sut.CreateInstance(fileSystem.FileInfo.New("foo.txt"));

            //var actual = blobObject.GetSourceBytes();

            //var expected = files[filePath.Value].Contents;

            //CollectionAssert.AreEqual(expected, actual);

            //blobObject.Persist(fileSystem);

            //var baz = fileSystem.FileInfo.New(filePath.Value);

            //var foo = new FromObjectBlobCreator(fileSystem);

            //var bar = foo.CreateInstance(blobObject.GetHash(), filePath);
        }
    }
}
