using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uno.Configuration.Format;
using Uno;
using Uno.Diagnostics;
using Uno.IO;

namespace Stuff.Core
{
    public class Installer
    {
        public static bool Install(string filename, StuffFlags flags = 0, IEnumerable<string> optionalDefines = null)
        {
            try
            {
                return new Installer(filename, flags, optionalDefines).Install();
            }
            finally
            {
                DownloadCache.AutoCollect();
            }
        }

        public static bool InstallAll(IEnumerable<string> files, StuffFlags flags = 0, IEnumerable<string> optionalDefines = null)
        {
            try
            {
                var tasks = new List<Task>();
                var retval = true;

                foreach (var file in files)
                    tasks.Add(Task.Factory.StartNew(
                        () =>
                        {
                            try
                            {
                                if (!Install(file, flags, optionalDefines))
                                    retval = false;
                            }
                            catch (Exception e)
                            {
                                Log.Error(file.ToRelativePath() + ": " + e.Message);
                                retval = false;
                            }
                        }));

                Task.WaitAll(tasks.ToArray());
                return retval;
            }
            finally
            {
                DownloadCache.AutoCollect();
            }
        }

        public static bool IsUpToDate(string filename, StuffFlags flags = 0, IEnumerable<string> optionalDefines = null)
        {
            try
            {
                var parentDir = Path.GetDirectoryName(filename);

                foreach (var item in StuffObject.Load(filename, flags, optionalDefines))
                {
                    var itemKey = item.Key.Replace('/', Path.DirectorySeparatorChar);
                    var itemFile = Path.Combine(parentDir, ".uno", "stuff", itemKey);
                    var targetDir = Path.Combine(parentDir, itemKey);
                    if (!IsItemUpToDate(targetDir, itemFile, item.Value?.ToString(), flags))
                        return false;
                }

                return true;
            }
            finally
            {
                DownloadCache.AutoCollect();
            }
        }

        public static void CleanAll(IEnumerable<string> files)
        {
            try
            {
                foreach (var file in files)
                {
                    try
                    {
                        var parentDirectory = Path.GetDirectoryName(file);
                        Disk.DeleteDirectory(Path.Combine(parentDirectory, ".uno", "stuff"));

                        foreach (var item in StuffObject.Load(file, StuffFlags.AcceptAll))
                            Disk.DeleteDirectory(
                                Path.Combine(
                                    parentDirectory,
                                    item.Key.Replace('/', Path.DirectorySeparatorChar)));
                    }
                    catch (Exception e)
                    {
                        Log.Error(file.ToRelativePath() + ": " + e.Message);
                    }
                }
            }
            finally
            {
                DownloadCache.AutoCollect();
            }
        }

        readonly string _stuffFile;
        readonly string _parentDirectory;
        readonly StuffFlags _flags;
        readonly IEnumerable<string> _optionalVars;
        readonly List<Task> _tasks = new List<Task>();
        bool _retval = true;

        Installer(string filename, StuffFlags flags, IEnumerable<string> optionalDefines)
        {
            _stuffFile = Path.GetFullPath(filename);
            _parentDirectory = Path.GetDirectoryName(_stuffFile);
            _flags = flags;
            _optionalVars = optionalDefines;
        }

        bool Install()
        {
            try
            {
                foreach (var item in StuffObject.Load(_stuffFile, _flags, _optionalVars))
                    _tasks.Add(Task.Factory.StartNew(
                        () =>
                        {
                            try
                            {
                                InstallItem(item);
                            }
                            catch (Exception e)
                            {
                                Log.Error(_stuffFile.ToRelativePath() + " (" + item.Key + "): " + e.Message);
                                _retval = false;
                            }
                        }));
            }
            catch (Exception e)
            {
                throw new Exception(_stuffFile.ToRelativePath() + ": " + e.Message, e);
            }

            Task.WaitAll(_tasks.ToArray());
            return _retval;
        }

        void InstallItem(KeyValuePair<string, object> item)
        {
            var itemKey = item.Key.Replace('/', Path.DirectorySeparatorChar);
            var components = itemKey.Split(Path.DirectorySeparatorChar);

            if (components.Contains(".") || components.Contains(".."))
                throw new FormatException(item.Key + ": '.' or '..' are not valid in directory names");

            var itemFile = Path.Combine(_parentDirectory, ".uno", "stuff", itemKey);
            var targetDir = Path.Combine(_parentDirectory, itemKey);
            var itemValue = item.Value?.ToString();

            if (string.IsNullOrEmpty(itemValue) ||
                IsItemUpToDate(targetDir, itemFile, itemValue, _flags))
                return;

            using (new FileLock(itemFile, DownloadCache.GetFileName(itemValue)))
            {
                if (IsItemUpToDate(targetDir, itemFile, itemValue, _flags))
                    return;

                for (int tries = 0;; tries++)
                {
                    Log.Verbose("Extracting " + targetDir.ToRelativePath().Quote());

                    // Support local files (e.g. from .STUFF-UPLOAD files)
                    var localFile = Path.Combine(_parentDirectory, itemValue);
                    var isLocal = File.Exists(localFile);
                    var file = isLocal
                        ? localFile
                        : DownloadCache.GetFile(itemValue);

                    Disk.DeleteDirectory(targetDir);
                    Disk.CreateDirectory(targetDir);

                    try
                    {
                        if (PlatformDetection.IsWindows)
                            LongPathZipFile.ExtractToDirectory(file, targetDir);
                        else
                        {
                            // Use system tar/unzip to preserve file permissions and links
                            // (ZipFile doesn't handle this)
                            if (Path.GetFileName(file).ToUpper().EndsWith(".TAR.GZ"))
                                Shell.Untar(file, targetDir);
                            else
                                Shell.Unzip(file, targetDir);

                            // Make sure files extracted are writable so we can touch them
                            Shell.Chmod("+w", targetDir);
                            Disk.TouchAllFiles(targetDir);
                        }

                        Disk.CreateDirectory(Path.GetDirectoryName(itemFile));
                        File.WriteAllText(itemFile, itemValue);
                        break;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Don't try more if we get this
                        throw;
                    }
                    catch
                    {
                        // Delete any installed files
                        Disk.DeleteFile(itemFile);
                        Disk.DeleteDirectory(targetDir);

                        if (isLocal)
                            throw;

                        // Delete the cached download too
                        Disk.DeleteFile(file);

                        // Redownload, and try just one more time to be sure
                        // This might fix the problem if the cached file was corrupt
                        if (tries > 0)
                            throw;

                        Thread.Sleep(150);
                    }
                }
            }
        }

        static bool IsItemUpToDate(string targetDir, string itemFile, string itemValue, StuffFlags flags)
        {
            if (!(flags.HasFlag(StuffFlags.Force) || !Directory.Exists(targetDir) ||
                    !File.Exists(itemFile) || File.ReadAllText(itemFile) != itemValue
                ) ||
                File.Exists(Path.Combine(targetDir, ".stuffignore")) ||
                Directory.Exists(Path.Combine(targetDir, ".git")) ||
                File.Exists(Path.Combine(targetDir, ".git")))
            {
                DownloadCache.UpdateTimestamp(itemValue);
                return true;
            }

            return false;
        }
    }
}
