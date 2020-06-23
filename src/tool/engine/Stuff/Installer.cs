using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uno.Configuration.Format;
using Uno.Diagnostics;
using Uno.IO;
using Uno.Logging;

namespace Uno.Build.Stuff
{
    public class Installer : LogObject
    {
        public static bool Install(Log log, string filename, StuffFlags flags = 0, IEnumerable<string> optionalDefines = null)
        {
            try
            {
                return new Installer(log, filename, flags, optionalDefines).Install();
            }
            finally
            {
                DownloadCache.AutoCollect(log);
            }
        }

        public static bool InstallAll(Log log, IEnumerable<string> files, StuffFlags flags = 0, IEnumerable<string> optionalDefines = null)
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
                                if (!Install(log, file, flags, optionalDefines))
                                    retval = false;
                            }
                            catch (Exception e)
                            {
                                log.Error(file.ToRelativePath() + ": " + e.Message);
                                retval = false;
                            }
                        }));

                Task.WaitAll(tasks.ToArray());
                return retval;
            }
            finally
            {
                DownloadCache.AutoCollect(log);
            }
        }

        public static bool IsUpToDate(Log log, string filename, StuffFlags flags = 0, IEnumerable<string> optionalDefines = null)
        {
            try
            {
                var parentDir = Path.GetDirectoryName(filename);

                foreach (var item in StuffObject.Load(filename, flags, optionalDefines))
                {
                    var itemKey = item.Key.Replace('/', Path.DirectorySeparatorChar);
                    var itemFile = Path.Combine(parentDir, ".uno", "stuff", itemKey);
                    var targetDir = Path.Combine(parentDir, itemKey);
                    if (!IsItemUpToDate(log, targetDir, itemFile, item.Value?.ToString(), flags))
                        return false;
                }

                return true;
            }
            finally
            {
                DownloadCache.AutoCollect(log);
            }
        }

        public static void CleanAll(Log log, IEnumerable<string> files)
        {
            try
            {
                foreach (var file in files)
                {
                    try
                    {
                        var parentDirectory = Path.GetDirectoryName(file);
                        Disk.DeleteDirectory(log, Path.Combine(parentDirectory, ".uno", "stuff"));

                        foreach (var item in StuffObject.Load(file, StuffFlags.AcceptAll))
                            Disk.DeleteDirectory(log,
                                Path.Combine(
                                    parentDirectory,
                                    item.Key.Replace('/', Path.DirectorySeparatorChar)));
                    }
                    catch (Exception e)
                    {
                        log.Error(file.ToRelativePath() + ": " + e.Message);
                    }
                }
            }
            finally
            {
                DownloadCache.AutoCollect(log);
            }
        }

        readonly string _stuffFile;
        readonly string _parentDirectory;
        readonly StuffFlags _flags;
        readonly IEnumerable<string> _optionalVars;
        readonly List<Task> _tasks = new List<Task>();
        bool _retval = true;

        Installer(Log log, string filename, StuffFlags flags, IEnumerable<string> optionalDefines)
            : base(log)
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
                IsItemUpToDate(Log, targetDir, itemFile, itemValue, _flags))
                return;

            using (new FileLock(Log, itemFile, DownloadCache.GetFileName(itemValue)))
            {
                if (IsItemUpToDate(Log, targetDir, itemFile, itemValue, _flags))
                    return;

                for (int tries = 0;; tries++)
                {
                    Log.Verbose("Extracting " + targetDir.ToRelativePath().Quote(), ConsoleColor.DarkGray);

                    // Support local files (e.g. from .STUFF-UPLOAD files)
                    var localFile = Path.Combine(_parentDirectory, itemValue);
                    var isLocal = File.Exists(localFile);
                    var file = isLocal
                        ? localFile
                        : DownloadCache.GetFile(Log, itemValue);

                    Disk.DeleteDirectory(Log, targetDir);
                    Disk.CreateDirectory(Log, targetDir);

                    try
                    {
                        if (PlatformDetection.IsWindows)
                            LongPathZipFile.ExtractToDirectory(file, targetDir);
                        else
                        {
                            // Use system tar/unzip to preserve file permissions and links
                            // (ZipFile doesn't handle this)
                            if (Path.GetFileName(file).ToUpper().EndsWith(".TAR.GZ", ".TGZ"))
                                Shell.Untar(Log, file, targetDir);
                            else
                                Shell.Unzip(Log, file, targetDir);

                            // Make sure files extracted are writable so we can touch them
                            Shell.Chmod(Log, "+w", targetDir);
                            Disk.TouchAllFiles(Log, targetDir);
                        }

                        Disk.CreateDirectory(Log, Path.GetDirectoryName(itemFile));
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
                        Disk.DeleteFile(Log, itemFile);
                        Disk.DeleteDirectory(Log, targetDir);

                        if (isLocal)
                            throw;

                        // Delete the cached download too
                        Disk.DeleteFile(Log, file);

                        // Redownload, and try just one more time to be sure
                        // This might fix the problem if the cached file was corrupt
                        if (tries > 0)
                            throw;

                        Thread.Sleep(150);
                    }
                }
            }
        }

        static bool IsItemUpToDate(Log log, string targetDir, string itemFile, string itemValue, StuffFlags flags)
        {
            if (!(flags.HasFlag(StuffFlags.Force) || !Directory.Exists(targetDir) ||
                    !File.Exists(itemFile) || File.ReadAllText(itemFile) != itemValue
                ) ||
                File.Exists(Path.Combine(targetDir, ".stuffignore")) ||
                Directory.Exists(Path.Combine(targetDir, ".git")) ||
                File.Exists(Path.Combine(targetDir, ".git")))
            {
                DownloadCache.UpdateTimestamp(log, itemValue);
                return true;
            }

            return false;
        }
    }
}
