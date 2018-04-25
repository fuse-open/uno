using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Stuff.Format;

namespace Stuff.Core
{
    public class Packer
    {
        readonly Dictionary<string, string> _result = new Dictionary<string, string>();

        public string Name;
        public string Condition;
        public string Suffix;
        public string OutputDirectory;
        public string InstallDirectory;
        public bool Modular;

        public void Pack(string inputDirectory)
        {
            if (string.IsNullOrEmpty(inputDirectory) || !Directory.Exists(inputDirectory))
                throw new ArgumentException("Invalid directory: " + inputDirectory);

            inputDirectory = Path.GetFullPath(inputDirectory);
            OutputDirectory = !string.IsNullOrEmpty(OutputDirectory)
                ? Path.GetFullPath(OutputDirectory)
                : inputDirectory;

            Disk.CreateDirectory(OutputDirectory);

            if (!Modular)
                Pack(Name, Name, inputDirectory);
            else
            {
                foreach (var f in Directory.EnumerateFiles(inputDirectory, "*.stuff-pack", SearchOption.AllDirectories))
                {
                    var package = Path.GetFileNameWithoutExtension(f);
                    var dir = Path.GetDirectoryName(f);

                    if (dir == inputDirectory)
                        throw new ArgumentException("Invalid .STUFF-PACK found in root directory");

                    File.Delete(f);
                    Pack(dir.Substring(inputDirectory.Length + 1), package, dir);
                }

                if (_result.Count == 0)
                    throw new ArgumentException("No .STUFF-PACK file(s) found in " + inputDirectory.Relative().Quote());
            }

            var uploadFile = Path.Combine(OutputDirectory, Name + ".stuff-upload");
            Log.Event(IOEvent.Write, uploadFile);

            using (var w = new StreamWriter(uploadFile))
                w.WriteConditional(Condition, _result.StringifyStuff());
        }

        void Pack(string key, string package, string directory)
        {
            if (!string.IsNullOrEmpty(InstallDirectory))
                key = Path.Combine(InstallDirectory, key);

            var name = package + Suffix + ".zip";
            _result.Add(key.Replace('\\', '/'), name);

            var zipFile = Path.Combine(OutputDirectory, name);

            if (File.Exists(zipFile))
                File.Delete(zipFile);

            Log.WriteLine(ConsoleColor.Cyan, "Creating {0}", zipFile.Relative().Quote());
            CreateFromDirectory(directory, zipFile);
        }

        void CreateFromDirectory(string directory, string zipFile)
        {
            if (PlatformDetection.IsWindows)
                ZipFile.CreateFromDirectory(directory, zipFile);
            else
            {
                // Find duplicate files and replace with symlinks to save space
                ReplaceDuplicateFilesWithSymlinks(directory);
                // Use system zip command to preserve file permissions and links
                Shell.Zip(directory, zipFile);
            }
        }

        public static void ReplaceDuplicateFilesWithSymlinks(string dir)
        {
            ReplaceDuplicateFilesWithSymlinks(dir, Directory.GetCurrentDirectory());
        }

        static void ReplaceDuplicateFilesWithSymlinks(string dir, string pwd, int depth = 0)
        {
            Directory.SetCurrentDirectory(dir);

            try
            {
                foreach (var e in Directory.EnumerateFileSystemEntries("."))
                {
                    var f = Path.GetFileName(e);

                    if (Directory.Exists(f))
                        ReplaceDuplicateFilesWithSymlinks(f, pwd, depth + 1);
                    else if (depth > 0)
                    {
                        var p = f;
                        var fi = new FileInfo(f);

                        for (int i = 0; i < depth; i++)
                        {
                            p = Path.Combine("..", p);
                            var pi = new FileInfo(p);

                            if (pi.Exists && pi.Length == fi.Length &&
                                !Shell.Readlink(p) && CompareFiles(pi, fi))
                            {
                                Log.Event(IOEvent.Symlink, Path.GetFullPath(f).Relative(pwd) + e + " -> " + p);
                                Shell.Symlink(p, f);
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(pwd);
            }
        }

        static bool CompareFiles(FileInfo a, FileInfo b)
        {
            using (var aStream = a.OpenRead())
            using (var bStream = b.OpenRead())
            {
                // 512KB
                const int bufSize = 512 * 1024;
                var aBuf = new byte[bufSize];
                var bBuf = new byte[bufSize];

                for (; ;)
                {
                    var aBytes = aStream.Read(aBuf, 0, bufSize);
                    var bBytes = bStream.Read(bBuf, 0, bufSize);

                    if (aBytes != bBytes)
                        return false;
                    if (aBytes == 0)
                        return true;

                    for (int i = 0; i < aBytes; i++)
                        if (aBuf[i] != bBuf[i])
                            return false;
                }
            }
        }
    }
}
