using Uno;
using Uno.Collections;
using Uno.Testing;
using Uno.IO;
using Uno.Text;

namespace UnoCore_Uno_IO
{
    public class PathTests
    {
        [Test]
        public void PathRootedTest()
        {
            var fileName = "C:\\mydir\\myfile.ext";
            var uncPath = "\\\\myPc\\mydir\\myfile";
            var relativePath = "mydir\\sudir\\";

            var unixPath = "/home/mydir/myfile";
            var unixRelativePath = "./inthisdir";

            if defined(HOST_WIN32)
            {
                Assert.IsTrue(Path.IsPathRooted(fileName));
                Assert.IsTrue(Path.IsPathRooted(uncPath));
                Assert.IsTrue(Path.IsPathRooted("\\"));
            }
            else
            {
                Assert.IsFalse(Path.IsPathRooted(fileName));
                Assert.IsFalse(Path.IsPathRooted(uncPath));
                Assert.IsFalse(Path.IsPathRooted("\\"));
            }

            Assert.IsTrue(Path.IsPathRooted(unixPath));
            Assert.IsFalse(Path.IsPathRooted(relativePath));
            Assert.IsFalse(Path.IsPathRooted(unixRelativePath));
        }

        [Test]
        public void HasExtensionTest()
        {
            var fileName1 = "myfile.jpg";
            var fileName2 = "C:\\mydir\\myfile.jpg";
            var fileNameWithoutExt1 = "myfile";
            var fileNameWithoutExt2 = "C:\\mydir\\myfile";

            Assert.IsTrue(Path.HasExtension(fileName1));
            Assert.IsTrue(Path.HasExtension(fileName2));
            Assert.IsFalse(Path.HasExtension(fileNameWithoutExt1));
            Assert.IsFalse(Path.HasExtension(fileNameWithoutExt2));

            if defined(HOST_WIN32)
            {
                Assert.IsFalse(Path.HasExtension("foo.bar\\baz"));
                Assert.IsFalse(Path.HasExtension(".:baz"));
            }
            else
            {
                Assert.IsTrue(Path.HasExtension("foo.bar\\baz"));
                Assert.IsTrue(Path.HasExtension(".:baz"));
            }
        }

        [Test]
        public void GetFullPathTest()
        {
            var fullPath = Path.GetFullPath("TestData\\testfile.bin");
            Assert.AreEqual(true, Path.IsPathRooted(fullPath));
            Assert.AreEqual(true, fullPath.IndexOf("TestData") > 0);
        }

        [Test]
        public void GetExtensionTest()
        {
            var fileName1 = "myfile.jpg";
            var fileName2 = "myfile.tst.jpg";
            var fileName3 = "myfile";
            var fileName4 = "test.folder\\myfile.png";
            var fileName5 = "./test.folder/myfile.png";

            Assert.AreEqual(".jpg", Path.GetExtension(fileName1));
            Assert.AreEqual(".jpg", Path.GetExtension(fileName2));
            Assert.IsTrue(string.IsNullOrEmpty(Path.GetExtension(fileName3)));
            Assert.AreEqual(".png", Path.GetExtension(fileName4));
            Assert.AreEqual(".png", Path.GetExtension(fileName5));

            if defined(HOST_WIN32)
            {
                Assert.AreEqual("", Path.GetExtension("foo.bar\\baz"));
                Assert.AreEqual("", Path.GetExtension(".:baz"));
            }
            else
            {
                Assert.AreEqual(".bar\\baz", Path.GetExtension("foo.bar\\baz"));
                Assert.AreEqual(".:baz", Path.GetExtension(".:baz"));
            }
        }

        [Test]
        public void GetFileNameTest()
        {
            var fileName1 = Path.Combine("C:\\mydir", "myfile.ext");
            var fileName2 = "myfile.jpg";
            var fileName3 = "/home/mydir/myscript.sh";
            var folderName = Path.Combine("mydir", "sudir") + Path.DirectorySeparatorChar;

            Assert.AreEqual("myfile.ext", Path.GetFileName(fileName1));
            Assert.AreEqual("myfile.jpg", Path.GetFileName(fileName2));
            Assert.AreEqual("myscript.sh", Path.GetFileName(fileName3));
            Assert.AreEqual("", Path.GetFileName(folderName));

            if defined(HOST_WIN32)
            {
                Assert.AreEqual("", Path.GetFileName("foo\\"));
                Assert.AreEqual("bar", Path.GetFileName("foo\\bar"));
            }
            else
            {
                Assert.AreEqual("foo\\", Path.GetFileName("foo\\"));
                Assert.AreEqual("foo\\bar", Path.GetFileName("foo\\bar"));
            }
        }

        [Test]
        public void GetFileNameWithoutExtensionTest()
        {
            var fileName1 = Path.Combine("C:\\mydir", "myfile.ext");
            var fileName2 = "myfile.jpg";
            var fileName3 = "/home/mydir/myscript.sh";
            var folderName = Path.Combine("mydir", "sudir") + Path.DirectorySeparatorChar;

            Assert.AreEqual("myfile", Path.GetFileNameWithoutExtension(fileName1));
            Assert.AreEqual("myfile", Path.GetFileNameWithoutExtension(fileName2));
            Assert.AreEqual("myscript", Path.GetFileNameWithoutExtension(fileName3));
            Assert.AreEqual("", Path.GetFileNameWithoutExtension(folderName));

            if defined(HOST_WIN32)
            {
                Assert.AreEqual("", Path.GetFileNameWithoutExtension("foo\\"));
                Assert.AreEqual("bar.baz", Path.GetFileName("foo\\bar.baz"));
            }
            else
            {
                Assert.AreEqual("foo\\", Path.GetFileNameWithoutExtension("foo\\"));
                Assert.AreEqual("foo\\bar.baz", Path.GetFileName("foo\\bar.baz"));
            }
        }

        [Test]
        public void GetDirectoryNameTest()
        {
            var folderName1 = Path.Combine("C:\\mydir", "myfile.ext");
            var folderName2 = "C:\\mydir" + Path.DirectorySeparatorChar;

            Assert.AreEqual("C:\\mydir", Path.GetDirectoryName(folderName1));
            Assert.AreEqual("C:\\mydir", Path.GetDirectoryName(folderName2));
            Assert.AreEqual("Foo" + Path.DirectorySeparatorChar + "Bar", Path.GetDirectoryName("Foo" + Path.AltDirectorySeparatorChar + Path.DirectorySeparatorChar + "Bar/Baz"));
            Assert.AreEqual(Path.Combine("Foo", "Bar", "..", "Baz"), Path.GetDirectoryName("Foo/Bar/../Baz/test.txt"));
            Assert.AreEqual(null, Path.GetDirectoryName("/"));
            Assert.AreEqual("", Path.GetDirectoryName("A"));
            Assert.AreEqual("C", Path.GetDirectoryName("C/"));
            Assert.AreEqual(Path.DirectorySeparatorChar.ToString(), Path.GetDirectoryName("/Foo"));

            if defined(HOST_WIN32)
            {
                Assert.AreEqual("C:\\mydir", Path.GetDirectoryName("C:\\mydir\\myfile.ext"));
                Assert.AreEqual(null, Path.GetDirectoryName("C" + Path.VolumeSeparatorChar + "/"));
            }
            else
            {
                Assert.AreEqual("", Path.GetDirectoryName("C:\\mydir\\myfile.ext"));
                Assert.AreEqual("C", Path.GetDirectoryName("C" + Path.VolumeSeparatorChar + "/"));
            }

            if defined(CIL && HOST_OSX)
            {
                // This seems to be a Mono-bug
                Assert.AreEqual("/", Path.GetDirectoryName("//"));
                Assert.AreEqual("", Path.GetDirectoryName("///"));
            }
            else
            {
                Assert.AreEqual(null, Path.GetDirectoryName("//"));
                Assert.AreEqual(null, Path.GetDirectoryName("///"));
            }
        }

        [Test]
        public void PathSeparatorTest()
        {
            if defined(HOST_WIN32)
                Assert.AreEqual('\\', Path.DirectorySeparatorChar);
            else
                Assert.AreEqual('/', Path.DirectorySeparatorChar);

            Assert.AreEqual('/', Path.AltDirectorySeparatorChar);
        }

        [Test]
        public void VolumeSeparatorTest()
        {
            if defined(HOST_WIN32)
                Assert.AreEqual(':', Path.VolumeSeparatorChar);
            else
                Assert.AreEqual('/', Path.VolumeSeparatorChar);
        }

        [Test]
        public void CombineTest()
        {
            var firstPart = "C:\\mydir";
            var secondPart = "sudir\\image.png";
            Assert.AreEqual("C:\\mydir" + Path.DirectorySeparatorChar + "sudir\\image.png", Path.Combine(firstPart, secondPart));

            Assert.AreEqual("foo", Path.Combine("foo", ""));

            firstPart = "/home";
            secondPart = "mydir/script.sh";
            Assert.AreEqual("/home" + Path.DirectorySeparatorChar + "mydir/script.sh", Path.Combine(firstPart, secondPart));

            //fixed separator
            if defined(HOST_WIN32)
                Assert.AreEqual("C:\\mydir\\sudir\\image.png", Path.Combine("C:\\mydir\\", "sudir\\image.png"));
            else
                Assert.AreEqual("C:\\mydir\\/sudir\\image.png", Path.Combine("C:\\mydir\\", "sudir\\image.png"));

            //fixed separator
            firstPart = "C:\\mydir/";
            secondPart = "sudir\\image.png";
            Assert.AreEqual("C:\\mydir/sudir\\image.png", Path.Combine(firstPart, secondPart));

            firstPart = "C:\\mydir\\";
            secondPart = "sudir\\";
            var thirdPart = "image.png";
            if defined(HOST_WIN32)
                Assert.AreEqual("C:\\mydir\\sudir\\image.png", Path.Combine(new string[] { firstPart, secondPart, thirdPart }));
            else
                Assert.AreEqual("C:\\mydir\\/sudir\\/image.png", Path.Combine(new string[] { firstPart, secondPart, thirdPart }));
        }

        void Combine2Path1Null()
        {
            Path.Combine(null, "foo");
        }

        void Combine2Path2Null()
        {
            Path.Combine("foo", null);
        }


        void Combine3Path1Null()
        {
            Path.Combine(null, "foo", "bar");
        }

        void Combine3Path2Null()
        {
            Path.Combine("foo", null, "bar");
        }

        void Combine3Path3Null()
        {
            Path.Combine("foo", "bar", null);
        }


        void Combine4Path1Null()
        {
            Path.Combine(null, "foo", "bar", "baz");
        }

        void Combine4Path2Null()
        {
            Path.Combine("foo", null, "bar", "baz");
        }

        void Combine4Path3Null()
        {
            Path.Combine("foo", "bar", null, "baz");
        }

        void Combine4Path4Null()
        {
            Path.Combine("foo", "bar", "baz", null);
        }

        void CombineArrayNull()
        {
            Path.Combine(null);
        }

        void CombineArrayElementNull()
        {
            Path.Combine(new string[]{ null });
        }

        [Test]
        public void CombineArgumentNullTest()
        {
            Assert.Throws<ArgumentNullException>(Combine2Path1Null);
            Assert.Throws<ArgumentNullException>(Combine2Path2Null);

            Assert.Throws<ArgumentNullException>(Combine3Path1Null);
            Assert.Throws<ArgumentNullException>(Combine3Path2Null);
            Assert.Throws<ArgumentNullException>(Combine3Path3Null);

            Assert.Throws<ArgumentNullException>(Combine4Path1Null);
            Assert.Throws<ArgumentNullException>(Combine4Path2Null);
            Assert.Throws<ArgumentNullException>(Combine4Path3Null);
            Assert.Throws<ArgumentNullException>(Combine4Path4Null);

            Assert.Throws<ArgumentNullException>(CombineArrayNull);
            Assert.Throws<ArgumentNullException>(CombineArrayElementNull);
        }
    }
}
