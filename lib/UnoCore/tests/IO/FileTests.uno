using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;
using Uno.Text;
using Uno.Time;

namespace UnoCore_Uno_IO
{
    public class FileTests : IDisposable
    {
        const string _testDataDirectoryName = "DirectoryTest";
        const string _testStringData = "string data";
        readonly string[] _testStringArrayData = new string[] { "string line 1", "", "string line 2" };
        readonly byte[] _testBytesData = new byte[] { 1, 2, 1, 2, 1, 2 };

        public FileTests()
        {
            SetCurrentSystemDataDirectory();
            DeleteTestDataDirectory();
            Directory.CreateDirectory(_testDataDirectoryName);
            Directory.SetCurrentDirectory(_testDataDirectoryName);
        }

        //File.OpenWrite and File.OpenRead tested at BinaryWriterTest and BinaryReaderTest

        [Test]
        public void DeleteTest()
        {
            CreateFakeEmptyFile("file.tst");
            File.Delete("file.tst");
            Assert.AreEqual(false, File.Exists("file.tst"));
        }

        [Test]
        public void MoveTest()
        {
            CreateFakeEmptyFile("file.tst");
            File.Move("file.tst", "file_new.tst");
            Assert.AreEqual(false, File.Exists("file.tst"));
            Assert.AreEqual(true, File.Exists("file_new.tst"));

            Directory.CreateDirectory("NewDirectoryTest");
            File.Move("file_new.tst", "NewDirectoryTest/file_new.tst");
            Assert.AreEqual(false, File.Exists("file_new.tst"));
            Assert.AreEqual(true, File.Exists("NewDirectoryTest/file_new.tst"));
        }

        [Test]
        public void Copy()
        {
            CreateFakeEmptyFile("file.tst");
            File.Copy("file.tst", "file_new.tst");
            Assert.AreEqual(true, File.Exists("file.tst"));
            Assert.AreEqual(true, File.Exists("file_new.tst"));

            File.Copy("file_new.tst", "file.tst", true);
            Assert.AreEqual(true, File.Exists("file.tst"));
            Assert.AreEqual(true, File.Exists("file_new.tst"));

            File.Delete("file.tst");
            File.Delete("file_new.tst");
            Assert.AreEqual(false, File.Exists("file.tst"));
            Assert.AreEqual(false, File.Exists("file_new.tst"));
        }

        [Test]
        public void AppendAllLinesToNonExistedFileTest()
        {
            File.AppendAllLines("file.tst", new string[] { "Line1", "Line2" });
            Assert.AreEqual(true, File.Exists("file.tst"));
            var fileData = File.ReadAllText("file.tst");
            Assert.AreEqual("Line1" + Environment.NewLine + "Line2" + Environment.NewLine, fileData);
        }

        [Test]
        public void AppendAllLinesToExistedFileTest()
        {
            File.WriteAllText("file.tst", "Some base text");
            File.AppendAllLines("file.tst", new string[] { "Line1", "Line2" });
            var fileData = File.ReadAllText("file.tst");
            Assert.AreEqual("Some base textLine1" + Environment.NewLine + "Line2" + Environment.NewLine, fileData);
        }

        [Test]
        public void AppendAllTextToNonExistedFileTest()
        {
            File.AppendAllText("file.tst", "Some more text");
            Assert.AreEqual(true, File.Exists("file.tst"));
            var fileData = File.ReadAllText("file.tst");
            Assert.AreEqual("Some more text", fileData);
        }

        [Test]
        public void AppendAllTextToExistedFileTest()
        {
            File.WriteAllText("file.tst", "Some base text");
            File.AppendAllText("file.tst", " Some more text");
            var fileData = File.ReadAllText("file.tst");
            Assert.AreEqual("Some base text Some more text", fileData);
        }

        [Test]
        public void WriteReadAllTextTest()
        {
            File.WriteAllText("file.tst", _testStringData);
            Assert.AreEqual(true, File.Exists("file.tst"));
            var fileData = File.ReadAllText("file.tst");
            Assert.AreEqual(_testStringData, fileData);
        }

        [Test]
        public void WriteReadAllLines()
        {
            File.WriteAllLines("file.tst", _testStringArrayData);
            Assert.AreEqual(true, File.Exists("file.tst"));
            var fileData = File.ReadAllLines("file.tst");
            Assert.AreEqual(_testStringArrayData, fileData);
        }

        [Test]
        public void ReadAllLines()
        {
            File.WriteAllText("file.tst", "string line 1\r\r\nstring line 2");
            Assert.AreEqual(true, File.Exists("file.tst"));
            var fileData = File.ReadAllLines("file.tst");
            Assert.AreEqual(_testStringArrayData, fileData);
        }

        [Test]
        public void ReadAllLines_2()
        {
            File.WriteAllText("file.tst", "string line 1\n\r\nstring line 2");
            Assert.AreEqual(true, File.Exists("file.tst"));
            var fileData = File.ReadAllLines("file.tst");
            Assert.AreEqual(_testStringArrayData, fileData);
        }

        [Test]
        public void WriteReadAllBytesTest()
        {
            File.WriteAllBytes("file.tst", _testBytesData);
            Assert.AreEqual(true, File.Exists("file.tst"));
            var fileData = File.ReadAllBytes("file.tst");
            Assert.AreEqual(_testBytesData, fileData);
        }

        [Test]
        public void CreateNewMode()
        {
            var stream = File.Open("file.tst", FileMode.CreateNew);
            var writer = new BinaryWriter(stream);
            writer.Write(_testBytesData);
            writer.Dispose();

            //check that file was created
            var readBytes = File.ReadAllBytes("file.tst");
            Assert.AreEqual(_testBytesData, readBytes);
        }

        [Test]
        public void CreateMode()
        {
            //create new file and write default bytes
            var stream = File.Open("file.tst", FileMode.Create);
            var writer = new BinaryWriter(stream);
            writer.Write(_testBytesData);
            writer.Dispose();

            //check that file was created
            var readBytes = File.ReadAllBytes("file.tst");
            Assert.AreEqual(_testBytesData, readBytes);

            //reopen and write some other bytes
            stream = File.Open("file.tst", FileMode.Create);
            writer = new BinaryWriter(stream);
            writer.Write(new byte [] { 12, 13 });
            writer.Dispose();

            readBytes = File.ReadAllBytes("file.tst");
            Assert.AreEqual(new byte [] { 12, 13 }, readBytes);
        }

        [Test]
        public void OpenMode()
        {
            CreateFakeEmptyFile("file.tst");

            //open empty file and write default bytes
            var stream = File.Open("file.tst", FileMode.Open);
            var writer = new BinaryWriter(stream);
            writer.Write(_testBytesData);
            writer.Dispose();

            //replace bytes
            stream = File.Open("file.tst", FileMode.Open);
            writer = new BinaryWriter(stream);
            var reader = new BinaryReader(stream);
            reader.ReadBytes(4);
            writer.Write(new byte [] { 12, 13 });
            reader.Dispose();

            //add bytes
            stream = File.Open("file.tst", FileMode.Open);
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
            reader.ReadBytes(6);
            writer.Write(new byte [] { 22, 23 });
            reader.Dispose();

            //reopen file and make sure that all bytes were written correctly
            stream = File.Open("file.tst", FileMode.Open);
            reader = new BinaryReader(stream);
            var readBytes = reader.ReadBytes(8);
            Assert.AreEqual(new byte[] { 1, 2, 1, 2, 12, 13, 22, 23 }, readBytes);

            reader.Dispose();
        }

        [Test]
        public void OpenOrCreateMode()
        {
            //create new file and write default bytes
            var stream = File.Open("file.tst", FileMode.OpenOrCreate);
            var writer = new BinaryWriter(stream);
            writer.Write(_testBytesData);
            writer.Dispose();

            //replace bytes
            stream = File.Open("file.tst", FileMode.OpenOrCreate);
            writer = new BinaryWriter(stream);
            var reader = new BinaryReader(stream);
            reader.ReadBytes(4);
            writer.Write(new byte [] { 12, 13 });
            reader.Dispose();

            //add bytes
            stream = File.Open("file.tst", FileMode.OpenOrCreate);
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);
            reader.ReadBytes(6);
            writer.Write(new byte [] { 22, 23 });
            reader.Dispose();

            //reopen file and make sure that all bytes were written correctly
            stream = File.Open("file.tst", FileMode.OpenOrCreate);
            reader = new BinaryReader(stream);
            var readBytes = reader.ReadBytes(8);
            Assert.AreEqual(new byte[] { 1, 2, 1, 2, 12, 13, 22, 23 }, readBytes);

            reader.Dispose();
        }

        [Test]
        public void TruncateMode()
        {
            //create new file and write default bytes
            File.WriteAllBytes("file.tst", _testBytesData);

            //truncate file and write some other bytes
            var stream = File.Open("file.tst", FileMode.Truncate);
            var writer = new BinaryWriter(stream);
            writer.Write(new byte [] { 12, 13 });
            writer.Dispose();

            var readBytes = File.ReadAllBytes("file.tst");
            Assert.AreEqual(new byte [] { 12, 13 }, readBytes);
        }

        [Test]
        public void AppendMode()
        {
            //create new file and write default bytes
            File.WriteAllBytes("file.tst", new byte [] { 1, 2 });

            //append some other bytes
            var stream = File.Open("file.tst", FileMode.Append);
            var writer = new BinaryWriter(stream);
            writer.Write(new byte [] { 12, 13 });
            writer.Dispose();

            var readBytes = File.ReadAllBytes("file.tst");
            Assert.AreEqual(new byte [] { 1, 2, 12, 13 }, readBytes);
        }

        FileMode _specificFileMode;

        [Test]
        public void OpenModeForNonExistingFile()
        {
            _specificFileMode = FileMode.Open;
            Assert.Throws(SpecificModeForNonExistingFileFunc);
        }

        [Test]
        public void TruncateModeForNonExistingFile()
        {
            _specificFileMode = FileMode.Truncate;
            Assert.Throws(SpecificModeForNonExistingFileFunc);
        }

        void SpecificModeForNonExistingFileFunc()
        {
            File.Open("file.tst", _specificFileMode);
        }

        [Test]
        public void CreateNewModeForExistingFile()
        {
            Assert.Throws(CreateNewModeForExistingFileFunc);
        }

        void CreateNewModeForExistingFileFunc()
        {
            CreateFakeEmptyFile("file.tst");
            File.Open("file.tst", FileMode.CreateNew);
        }

        [Test]
        public void ReadForAppendMode()
        {
            _specificFileMode = FileMode.Append;
            Assert.Throws(ReadForSpecificFileModeFunc);
        }

        [Test]
        public void NewDirectoryInfoForExistingDirectoryHasCorrectValues()
        {
            var minTimestamp = ZonedDateTime.Now.WithZone(DateTimeZone.Utc) - Duration.FromSeconds(2);
            var dirName = CreateTestDir();
            var info = new DirectoryInfo(dirName);
            Assert.IsTrue(info.LastWriteTimeUtc > minTimestamp);
            Assert.IsTrue(info.LastAccessTimeUtc > minTimestamp);
            Assert.IsTrue(info.Exists);
            Assert.AreEqual(info.FullName, Path.GetFullPath(dirName));
            Assert.AreEqual(FileAttributes.Directory, info.Attributes);
        }

        [Test]
        public void NewDirectoryInfoForExistingHasExistsSetToFalse()
        {
            var minTimestamp = ZonedDateTime.Now.WithZone(DateTimeZone.Utc) - Duration.FromSeconds(2);
            var dirName = "non-existing-dir-" + ZonedDateTime.Now.WithZone(DateTimeZone.Utc).ToInstant().Ticks;
            var info = new DirectoryInfo(dirName);
            Assert.IsFalse(info.Exists);
        }

        [Test]
        public void NewDirectoryInfoForFileHasExistsSetToFalse()
        {
            var dirName = CreateTestFile();
            var info = new DirectoryInfo(dirName);
            Assert.IsFalse(info.Exists);
        }

        [Test]
        public void NewFileInfoForDirectoryHasExistsSetToFalse()
        {
            var dirName = CreateTestDir();
            var info = new FileInfo(dirName);
            Assert.IsFalse(info.Exists);
        }

        [Test]
        public void NewFileInfoForExistingFileHasCorrectValues()
        {
            var minTimestamp = ZonedDateTime.Now.WithZone(DateTimeZone.Utc) - Duration.FromSeconds(2);
            var fileName = CreateTestFile();
            var info = new FileInfo(fileName);
            Assert.AreEqual(info.Length, 4);
            Assert.IsTrue(info.LastWriteTimeUtc > minTimestamp);
            Assert.IsTrue(info.LastAccessTimeUtc > minTimestamp);
            Assert.IsTrue(info.Exists);
            Assert.AreEqual(info.FullName, Path.GetFullPath(fileName));
        }

        [Test]
        public void NewFileInfoForNonExistingHasExistsSetToFalse()
        {
            var info = new FileInfo("this-file-should-not-exist-123456789.tst");
            Assert.IsFalse(info.Exists);
        }

        static string CreateTestDir()
        {
            var dirName = "testdir-" + ZonedDateTime.Now.WithZone(DateTimeZone.Utc).ToInstant().Ticks;
            Directory.CreateDirectory(dirName);
            return dirName;
        }

        static string CreateTestFile()
        {
            var fileName = "testfile-" + ZonedDateTime.Now.WithZone(DateTimeZone.Utc).ToInstant().Ticks + ".txt";
            File.WriteAllText(fileName, "fuse");
            return fileName;
        }

        void ReadForSpecificFileModeFunc()
        {
            File.WriteAllBytes("file.tst", _testBytesData);
            var stream = File.Open("file.tst", _specificFileMode);
            try
            {
                var reader = new BinaryReader(stream);
                reader.ReadBytes(6);
            }
            catch (Exception ex)
            {
                stream.Close();
                throw ex;
            }
        }

        //helpers
        void SetCurrentSystemDataDirectory()
        {
            Directory.SetCurrentDirectory(Directory.GetUserDirectory(UserDirectory.Data));
        }

        void DeleteTestDataDirectory()
        {
            if (Directory.Exists(_testDataDirectoryName))
            {
                Directory.Delete(_testDataDirectoryName, true);
            }
        }

        void CreateFakeEmptyFile(string fileName)
        {
            var s = File.OpenWrite(fileName);
            s.Dispose();
        }

        //dispose
        public void Dispose()
        {
            DeleteTestDataDirectory();
        }
    }
}
