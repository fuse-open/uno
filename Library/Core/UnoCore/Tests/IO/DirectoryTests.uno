using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;
using Uno.Text;

namespace UnoCore_Uno_IO
{
    public class DirectoryTests : IDisposable
    {
        const string _testDataDirectoryName = "DirectoryTest";
        const string _testNewDataDirectoryName = "NewDirectoryTest";

        public DirectoryTests()
        {
            SetCurrentSystemDataDirectory();
            DeleteTestDataDirectories();
        }

        [Test]
        public void CreateDirectoryTest()
        {
            Directory.CreateDirectory(_testDataDirectoryName);
            Assert.AreEqual(true, Directory.Exists(_testDataDirectoryName));
        }

        [Test]
        public void MoveDirectoryTest()
        {
            Directory.CreateDirectory(_testDataDirectoryName);
            Directory.Move(_testDataDirectoryName, _testNewDataDirectoryName);
            Assert.AreEqual(false, Directory.Exists(_testDataDirectoryName));
            Assert.AreEqual(true, Directory.Exists(_testNewDataDirectoryName));
        }

        [Test]
        public void DeleteDirectoryTest()
        {
            Directory.CreateDirectory(_testDataDirectoryName);
            Assert.AreEqual(true, Directory.Exists(_testDataDirectoryName));
            Directory.Delete(_testDataDirectoryName, true);
            Assert.AreEqual(false, Directory.Exists(_testDataDirectoryName));
        }

        [Test]
        public void GetSetCurrentDirectoryTest()
        {
            Directory.CreateDirectory(_testDataDirectoryName);
            Directory.SetCurrentDirectory(_testDataDirectoryName);
            Assert.AreEqual(true, Directory.GetCurrentDirectory().IndexOf(_testDataDirectoryName) > 0);
            Assert.AreEqual(true, Path.IsPathRooted(Directory.GetCurrentDirectory()));
        }

        [Test]
        public void EnumerateDirectoryTest()
        {
            Directory.CreateDirectory(_testDataDirectoryName);
            Directory.SetCurrentDirectory(_testDataDirectoryName);
            CreateTestDirectories(5);

            var directories = new List<string>();
            foreach(var dir in Directory.EnumerateDirectories(Directory.GetCurrentDirectory()))
            {
                directories.Add(dir);
            }
            Assert.AreEqual(5, directories.Count);
        }

        [Test]
        public void EnumerateFilesTest()
        {
            Directory.CreateDirectory(_testDataDirectoryName);
            Directory.SetCurrentDirectory(_testDataDirectoryName);
            CreateTestFiles(7);

            var files = new List<string>();
            foreach(var fl in Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
            {
                files.Add(fl);
            }
            Assert.AreEqual(7, files.Count);
        }

        [Test]
        public void EnumerateFileSystemEntriesTest()
        {
            Directory.CreateDirectory(_testDataDirectoryName);
            Directory.SetCurrentDirectory(_testDataDirectoryName);
            CreateTestFiles(7);
            CreateTestDirectories(5);

            var sysEntities = new List<string>();
            foreach(var si in Directory.EnumerateFileSystemEntries(Directory.GetCurrentDirectory()))
            {
                sysEntities.Add(si);
            }
            Assert.AreEqual(12, sysEntities.Count);
        }

        //helpers
        void SetCurrentSystemDataDirectory()
        {
            Directory.SetCurrentDirectory(Directory.GetUserDirectory(UserDirectory.Data));
        }

        void CreateTestDirectories(int number)
        {
            for (int i = 1; i <= number; i++)
            {
                Directory.CreateDirectory(i.ToString());
            }
        }

        void CreateTestFiles(int number)
        {
            for (int i = 1; i <= number; i++)
            {
                var s = File.OpenWrite(i.ToString() + ".tst");
                s.Dispose();
            }
        }

        void DeleteTestDataDirectories()
        {
            if (Directory.Exists(_testDataDirectoryName))
            {
                Directory.Delete(_testDataDirectoryName, true);
            }
            if (Directory.Exists(_testNewDataDirectoryName))
            {
                Directory.Delete(_testNewDataDirectoryName, true);
            }
        }

        //dispose
        public void Dispose()
        {
            DeleteTestDataDirectories();
        }
    }
}
