using System;
using System.IO;
using Uno.Diagnostics;
using Uno.IO;
using Uno.Logging;

namespace Uno.Build.Stuff
{
    public static class Disk
    {
        public static void CreateDirectory(Log log, string path)
        {
            var dirExists = PlatformDetection.IsWindows ? LongPathDisk.DirectoryExists(path) : Directory.Exists(path);
            if (path.Length > 1 && !dirExists)
            {
                CreateDirectory(log, Path.GetDirectoryName(path));
                log.Event(IOEvent.MkDir, path);

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

        public static void DeleteFile(Log log, string name)
        {
            var fileExists = PlatformDetection.IsWindows ? LongPathDisk.FileExists(name) : File.Exists(name);
            if (!fileExists)
                return;

            try
            {
                log.Event(IOEvent.Rm, name);
                if (PlatformDetection.IsWindows)
                    LongPathDisk.DeleteFile(name);
                else
                    File.Delete(name);
            }
            catch (Exception e)
            {
                log.Warning("Failed to delete " + name.ToRelativePath() + ": " + e.Message);
            }
        }

        public static void DeleteDirectory(Log log, string name)
        {
            var directoryExists = PlatformDetection.IsWindows ? LongPathDisk.DirectoryExists(name) : Directory.Exists(name);
            if (!directoryExists)
                return;

            try
            {
                log.Event(IOEvent.RmDir, name);

                if (PlatformDetection.IsWindows)
                    LongPathDisk.DeleteDirectory(name, true);
                else
                    Directory.Delete(name, true);
            }
            catch (Exception e)
            {
                log.Warning("Failed to delete " + name.ToRelativePath() + ": " + e.Message);
            }
        }

        /// <summary>
        /// Set the last write time for all files in 'dir' to current time.
        /// NOTE: Does not support long paths on Windows!
        /// </summary>
        public static void TouchAllFiles(Log log, string dir)
        {
            foreach (var f in Directory.EnumerateDirectories(dir))
                TouchAllFiles(log, f);
            foreach (var f in Directory.EnumerateFiles(dir))
                TouchFile(log, f);
        }

        public static void TouchFile(Log log, string name)
        {
            try
            {
                if (!File.Exists(name))
                    File.Create(name);
                else
                    File.SetLastWriteTimeUtc(name, DateTime.UtcNow);
            }
            catch (IOException)
            {
                // IOException usually means another process is also writing to the file,
                // which is what we want so we don't need to do anything here.
            }
            catch (Exception e)
            {
                log.Warning("Failed to touch " + name.ToRelativePath() + ": " + e.Message);
            }
        }
    }
}