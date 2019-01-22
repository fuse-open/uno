using System;
using System.IO;
using Uno.Diagnostics;
using Uno.IO;

namespace Stuff
{
    public static class Disk
    {
        public static void CreateDirectory(string path)
        {
            var dirExists = PlatformDetection.IsWindows ? LongPathDisk.DirectoryExists(path) : Directory.Exists(path);
            if (path.Length > 1 && !dirExists)
            {
                CreateDirectory(Path.GetDirectoryName(path));
                Log.Event(IOEvent.MkDir, path);

                var isHidden = Path.GetFileName(path).StartsWith(".");
                if (PlatformDetection.IsWindows)
                {
                    LongPathDisk.CreateDirectory(path);
                    if (isHidden)
                        LongPathDisk.SetFileAttributes(path, FileAttributes.Hidden);
                }
                else
                {
                    var di = Directory.CreateDirectory(path);

                    // Hide directory when starts with a dot
                    if (isHidden)
                        di.Attributes |= FileAttributes.Hidden;
                }
            }
        }

        public static void DeleteFile(string name)
        {
            var fileExists = PlatformDetection.IsWindows ? LongPathDisk.FileExists(name) : File.Exists(name);
            if (!fileExists)
                return;

            try
            {
                Log.Event(IOEvent.Rm, name);
                if (PlatformDetection.IsWindows)
                    LongPathDisk.DeleteFile(name);
                else
                    File.Delete(name);
            }
            catch (Exception e)
            {
                Log.Warning("Failed to delete " + name.ToRelativePath() + ": " + e.Message);
            }
        }

        public static void DeleteDirectory(string name)
        {
            var directoryExists = PlatformDetection.IsWindows ? LongPathDisk.DirectoryExists(name) : Directory.Exists(name);
            if (!directoryExists)
                return;

            try
            {
                Log.Event(IOEvent.RmDir, name);

                if (PlatformDetection.IsWindows)
                    LongPathDisk.DeleteDirectory(name, true);
                else
                    Directory.Delete(name, true);
            }
            catch (Exception e)
            {
                Log.Warning("Failed to delete " + name.ToRelativePath() + ": " + e.Message);
            }
        }

        /// <summary>
        /// Set the last write time for all files in 'dir' to current time.
        /// NOTE: Does not support long paths on Windows!
        /// </summary>
        public static void TouchAllFiles(string dir)
        {
            foreach (var f in Directory.EnumerateDirectories(dir))
                TouchAllFiles(f);
            foreach (var f in Directory.EnumerateFiles(dir))
                TouchFile(f);
        }

        public static void TouchFile(string name)
        {
            try
            {
                File.SetLastWriteTimeUtc(name, DateTime.UtcNow);
            }
            catch (IOException)
            {
                // IOException usually means another process is also writing to the file,
                // which is what we want so we don't need to do anything here.
            }
            catch (Exception e)
            {
                Log.Warning("Failed to touch " + name.ToRelativePath() + ": " + e.Message);
            }
        }
    }
}